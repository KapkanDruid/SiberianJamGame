using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Game.Runtime.Gameplay.HUD;
using Game.Runtime.Gameplay.Implants;
using Game.Runtime.Gameplay.Level;
using Game.Runtime.Gameplay.Warrior;
using Game.Runtime.Services;
using UnityEngine;

namespace Game.Runtime.Gameplay.Inventory
{
    public class InventoryService : IService, IInitializable, IDisposable
    {
        public event Action OnImplantPlaced;

        public InventoryGrid Grid { get; }
        public InventoryStats Stats { get; }
        public InventoryHighlighter Highlighter { get; }
        
        public Dictionary<ImplantBehaviour, List<Vector2Int>> ItemPositions { get; } = new();
        public Dictionary<Vector2Int, ImplantBehaviour> OccupiedSlots { get; } = new();
        public bool IsBlocked { get; private set; }

        public InventoryService()
        {
            Grid = new InventoryGrid();
            Stats = new InventoryStats(this);
            Highlighter = new InventoryHighlighter(this);
        }

        public void Initialize()
        {
            Grid.CreateGrid();
            ServiceLocator.Get<BattleController>().OnTurnEnded += OnTurnEnded;
        }
        
        public bool HasItem(ImplantBehaviour item) => ItemPositions.GetValueOrDefault(item) != default;
        public bool IsSlotOccupied(Vector2Int slot) => OccupiedSlots.ContainsKey(slot);

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
                    Highlighter.UpdateSlotHighlight(pos);
                }
            }

            var rotatedSlots = InventoryHelper.GetRotatedSlots(item.SlotPositions, item.CurrentRotation);
            var newPositions = rotatedSlots.Select(slot => slot + gridPosition).ToList();

            foreach (var pos in newPositions)
            {
                OccupiedSlots[pos] = item;
                Highlighter.UpdateSlotHighlight(pos);
            }

            ItemPositions[item] = newPositions;
            RecalculateImplantStats();
            OnImplantPlaced?.Invoke();
            
            return true;
        }
        
        public bool CanPlaceItem(ImplantBehaviour item, Vector2Int gridPosition, int rotation)
        {
            var rotatedSlots = InventoryHelper.GetRotatedSlots(item.SlotPositions, rotation);
            var targetSlots = rotatedSlots.Select(slot => slot + gridPosition).ToList();

            foreach (var slot in targetSlots)
            {
                if (!Grid.Slots.ContainsKey(slot))
                    return false;
                
                if(OccupiedSlots.TryGetValue(slot, out var itemOccupied) && !itemOccupied.Equals(item))
                    return false;
            }

            return true;
        }

        public void SetItemPosition(InventorySlot slot, ImplantBehaviour item, bool isShadow)
        {
            var itemCenterPosition = InventoryHelper.CalculateCenterPosition(slot, item);
            ServiceLocator.Get<HUDService>().InventoryView.SetItemInInventory(item, itemCenterPosition, isShadow);
        }

        public void RemoveItem(ImplantBehaviour item)
        {
            if (ItemPositions.TryGetValue(item, out var positions))
            {
                foreach (var pos in positions)
                {
                    OccupiedSlots.Remove(pos);
                    Highlighter.UpdateSlotHighlight(pos);
                }

                ItemPositions.Remove(item);
                RecalculateImplantStats();
            }
        }

        private void RecalculateImplantStats()
        {
            Stats.RecalculateImplantStats();
        }

        private void OnTurnEnded()
        {
            foreach (var item in ItemPositions)
                item.Key.ReleaseImplant().Forget();

            OccupiedSlots.Clear();
            ItemPositions.Clear();
            Highlighter.ResetSlotsHighlight();
            RecalculateImplantStats();
            IsBlocked = false;
        }
        
        public void Dispose()
        {
            ServiceLocator.Get<BattleController>().OnTurnEnded -= OnTurnEnded;
        }
    }
}