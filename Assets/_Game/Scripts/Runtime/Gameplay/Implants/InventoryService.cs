using System;
using System.Collections.Generic;
using System.Linq;
using Game.Runtime.CMS;
using Game.Runtime.CMS.Components.Gameplay;
using Game.Runtime.CMS.Components.Implants;
using Game.Runtime.Gameplay.HUD;
using Game.Runtime.Gameplay.Level;
using Game.Runtime.Gameplay.Warrior;
using Game.Runtime.Services;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Game.Runtime.Gameplay.Implants
{
    public class InventoryService : IService, IInitializable, IDisposable
    {
        public event Action OnImplantPlaced;

        private Vector2Int _gridSize;
        private int _cellSize;
        private InventoryView _inventoryView;
        
        public InventoryStats Stats { get; }
        public InventoryHighlighter Highlighter { get; }
        
        public Dictionary<Vector2Int, InventorySlot> Slots { get; } = new();
        public List<Vector2Int> SynergySlots { get; } = new();
        public Dictionary<ImplantBehaviour, List<Vector2Int>> ItemPositions { get; } = new();
        public Dictionary<Vector2Int, ImplantBehaviour> OccupiedSlots { get; } = new();
        public bool IsBlocked { get; private set; }

        public InventoryService()
        {
            Stats = new InventoryStats(this);
            Highlighter = new InventoryHighlighter(this);
        }

        public void Initialize()
        {
            _inventoryView = ServiceLocator.Get<HUDService>().InventoryView;
            ServiceLocator.Get<BattleController>().OnTurnEnded += OnTurnEnded;

            CreateGrid();
        }
        
        public bool HasItem(ImplantBehaviour item) => ItemPositions.GetValueOrDefault(item) != default;
        public bool IsSlotExist(Vector2Int slotPosition) => Slots.ContainsKey(slotPosition);
        public bool IsSlotOccupied(Vector2Int slot) => OccupiedSlots.ContainsKey(slot);

        public void HighlightSynergySlots(ImplantBehaviour implantBehaviour)
        {
            SynergySlots.Clear();
            SynergySlots.AddRange(Stats.FindSynergySlots(implantBehaviour));

            var synergyColors = CM.Get(CMs.Gameplay.ImplantSynergyColors).GetComponent<ImplantSynergyColorsComponent>().SynergyColors;
            var targetSynergyColor = synergyColors.First(synergy => synergy.ImplantType == implantBehaviour.GetImplantType()).Color;

            foreach (var slotPosition in SynergySlots)
            {
                if (Slots.TryGetValue(slotPosition, out var slot))
                    Highlighter.HighlightSynergySlots(slot, targetSynergyColor);
            }
        }

        public WarriorTurnData CalculateTurnData()
        {
            IsBlocked = true;

            foreach (var itemPair in ItemPositions)
                itemPair.Key.PlayParticle();

            return new WarriorTurnData(Stats.StatsMap[ImplantType.Health],
                Stats.StatsMap[ImplantType.Damage],
                Stats.StatsMap[ImplantType.Armor]);
        }
        
        public bool TryPlaceItem(ImplantBehaviour item, Vector2Int gridPosition)
        {
            if (ServiceLocator.Get<BattleController>().IsTurnStarted) return false;
            if (!CanPlaceItem(item, gridPosition, item.CurrentRotation)) return false;

            if (ItemPositions.TryGetValue(item, out var oldPositions))
            {
                foreach (var pos in oldPositions)
                {
                    OccupiedSlots.Remove(pos);
                    Highlighter.UpdateSlotVisual(pos);
                }
            }

            var rotatedSlots = InventoryHelper.GetRotatedSlots(item.SlotPositions, item.CurrentRotation);
            var newPositions = rotatedSlots.Select(slot => slot + gridPosition).ToList();

            foreach (var pos in newPositions)
            {
                OccupiedSlots[pos] = item;
                Highlighter.UpdateSlotVisual(pos);
            }

            ItemPositions[item] = newPositions;
            UpdateStatsMap();
            OnImplantPlaced?.Invoke();
            
            return true;
        }
        
        public bool CanPlaceItem(ImplantBehaviour item, Vector2Int gridPosition, int rotation)
        {
            var rotatedSlots = InventoryHelper.GetRotatedSlots(item.SlotPositions, rotation);
            var targetSlots = rotatedSlots.Select(slot => slot + gridPosition).ToList();

            foreach (var slot in targetSlots)
            {
                if (!IsSlotExist(slot) ||
                    (OccupiedSlots.TryGetValue(slot, out var itemOccupied) && !itemOccupied.Equals(item)))
                {
                    return false;
                }
            }

            return true;
        }

        public void SetItemPosition(InventorySlot slot, ImplantBehaviour item, bool isShadow)
        {
            var itemCenterPosition = ImplantHelper.CalculateCenterPosition(slot, item);
            _inventoryView.SetItemInInventory(item, itemCenterPosition, isShadow);
        }

        public void RemoveItem(ImplantBehaviour item)
        {
            if (ItemPositions.TryGetValue(item, out var positions))
            {
                foreach (var pos in positions)
                {
                    OccupiedSlots.Remove(pos);
                    Highlighter.UpdateSlotVisual(pos);
                }

                ItemPositions.Remove(item);
                UpdateStatsMap();
            }
        }

        private void CreateGrid()
        {
            var defaultInventoryGrid = CM.Get(CMs.Gameplay.Inventory).GetComponent<InventoryComponent>();
            var currentGrid = defaultInventoryGrid.Grids
                .Last(grid => grid.RequiredLevel <= ServiceLocator.Get<GameStateHolder>().CurrentData.Level).Grid;
            _gridSize = currentGrid.GridSize;
            _cellSize = defaultInventoryGrid.CellSize;

            _inventoryView.ResizeInventoryView(_gridSize, _cellSize);

            foreach (var slotPos in currentGrid.GridPattern)
            {
                var slotObj = new GameObject($"InventorySlot_{slotPos.x}_{slotPos.y}");
                var slot = slotObj.AddComponent<InventorySlot>();

                _inventoryView.SetupInventorySlot(slotObj, slotPos, _cellSize);

                slot.Initialize(slotPos);
                Slots[slotPos] = slot;
            }
        }
        
        private void UpdateStatsMap()
        {
            Stats.UpdateStatsMap();

            ServiceLocator.Get<HUDService>().StatsPanel
                .UpdateStat(ImplantType.Health, Stats.StatsMap[ImplantType.Health]);
            ServiceLocator.Get<HUDService>().StatsPanel
                .UpdateStat(ImplantType.Damage, Stats.StatsMap[ImplantType.Damage]);
            ServiceLocator.Get<HUDService>().StatsPanel
                .UpdateStat(ImplantType.Armor, Stats.StatsMap[ImplantType.Armor]);
        }

        private void OnTurnEnded()
        {
            foreach (var item in ItemPositions)
            {
                Object.Destroy(item.Key.HighlightImplant.gameObject);
                Object.Destroy(item.Key.gameObject);
            }

            OccupiedSlots.Clear();
            ItemPositions.Clear();
            SynergySlots.Clear();
            
            Highlighter.UpdateAllSlotVisual();
            UpdateStatsMap();
            IsBlocked = false;
        }
        
        public void Dispose()
        {
            ServiceLocator.Get<BattleController>().OnTurnEnded -= OnTurnEnded;
        }
    }
}