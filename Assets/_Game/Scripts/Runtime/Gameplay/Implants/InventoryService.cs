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
using Game.Runtime.Utils;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Game.Runtime.Gameplay.Implants
{
    public class InventoryService : IService, IInitializable, IDisposable
    {
        private readonly Dictionary<Vector2Int, InventorySlot> _slots = new();
        private readonly Dictionary<Vector2Int, ImplantBehaviour> _occupiedSlots = new();
        private readonly Dictionary<ImplantBehaviour, List<Vector2Int>> _itemPositions = new();

        private Vector2Int _gridSize;
        private int _cellSize;

        private InventoryView _inventoryView;
        public event Action OnImplpantPlaced;
        public bool IsBlocked { get; private set; }

        public void Initialize()
        {
            _inventoryView = SL.Get<HUDService>().Behaviour.InventoryView;
            SL.Get<BattleController>().OnTurnEnded += OnTurnEnded;

            CreateGrid();
        }

        private void OnTurnEnded()
        {
            foreach (var item in _itemPositions)
            {
                Object.Destroy(item.Key.gameObject);
            }

            _occupiedSlots.Clear();
            _itemPositions.Clear();

            UpdateAllSlotVisual();

            IsBlocked = false;
        }

        public WarriorTurnData CalculateTurnData()
        {
            IsBlocked = true;
            
            float health = 0f;
            float damage = 0f;
            float armor = 0f;

            var checkedPairs = new HashSet<(ImplantBehaviour, ImplantBehaviour)>(new ImplantsPairComparer());
            Vector2Int[] neighborOffsets = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

            foreach (var itemPair in _itemPositions)
            {
                var item = itemPair.Key;
                var implantType = item.GetImplantType();

                if (item.Model.Is<HealthImplantComponent>(out var healthImplant))
                    health += healthImplant.Health;
                else if (item.Model.Is<DamageImplantComponent>(out var damageImplant))
                    damage += damageImplant.Damage;
                else if (item.Model.Is<ArmorImplantComponent>(out var armorImplant))
                    armor += armorImplant.Armor;

                foreach (var itemPosition in itemPair.Value)
                {
                    if (IsPositionBlocked(item, itemPosition))
                        continue;

                    foreach (var offset in neighborOffsets)
                    {
                        var neighborPos = itemPosition + offset;

                        if (!_occupiedSlots.TryGetValue(neighborPos, out var neighborItem) || neighborItem.Equals(item))
                            continue;

                        if (IsPositionBlocked(neighborItem, neighborPos))
                            continue;

                        if (neighborItem.GetImplantType() == implantType &&
                            checkedPairs.Add((item, neighborItem)))
                        {
                            neighborItem.PingPongScale();
                            item.PingPongScale();
                            
                            switch (implantType)
                            {
                                case ImplantBehaviour.ImplantType.Health:
                                    health += 10;
                                    break;
                                case ImplantBehaviour.ImplantType.Damage:
                                    damage += 10;
                                    break;
                                case ImplantBehaviour.ImplantType.Armor:
                                    armor += 10;
                                    break;
                            }
                        }
                    }
                }
                
                item.PlayParticle();
            }

            return new WarriorTurnData(health, damage, armor);
        }

        private bool IsPositionBlocked(ImplantBehaviour implant, Vector2Int position)
        {
            if (!implant.Model.Is<BrokenImplantCellsComponent>(out var blockComponent))
                return false;

            var rotatedCell = GetRotatedSlots(blockComponent.BrokenCells.ToList(), implant.CurrentRotation);
            var globalBlockCell = rotatedCell.Select(x => x + implant.CenterSlotPosition);
            
            return globalBlockCell.Contains(position);
        }

        public bool TryPlaceItem(ImplantBehaviour item, Vector2Int gridPosition)
        {
            if (SL.Get<BattleController>().IsTurnStarted) return false;
            if (!CanPlaceItem(item, gridPosition)) return false;

            if (_itemPositions.TryGetValue(item, out var oldPositions))
            {
                foreach (var pos in oldPositions)
                {
                    _occupiedSlots.Remove(pos);
                    UpdateSlotVisual(pos);
                }
            }

            var rotatedSlots = GetRotatedSlots(item.SlotPositions, item.CurrentRotation);
            var newPositions = rotatedSlots.Select(slot => slot + gridPosition).ToList();

            foreach (var pos in newPositions)
            {
                _occupiedSlots[pos] = item;
                UpdateSlotVisual(pos);
            }

            _itemPositions[item] = newPositions;

            OnImplpantPlaced?.Invoke();
            return true;
        }

        public bool HasItem(ImplantBehaviour item)
        {
            return _itemPositions.GetValueOrDefault(item) != default;
        }

        private bool IsSlotOccupied(Vector2Int slot)
        {
            return _occupiedSlots.ContainsKey(slot);
        }

        public void SetItemPosition(InventorySlot slot, ImplantBehaviour item)
        {
            var itemCenterPosition = ImplantHelper.CalculateCenterPosition(slot, item);
            _inventoryView.SetItemInInventory(item, itemCenterPosition);
        }

        public void RemoveItem(ImplantBehaviour item)
        {
            if (_itemPositions.TryGetValue(item, out var positions))
            {
                foreach (var pos in positions)
                {
                    _occupiedSlots.Remove(pos);
                    UpdateSlotVisual(pos);
                }

                _itemPositions.Remove(item);
            }
        }

        public void UpdateSlotHighlight(ImplantBehaviour item, Vector2Int gridPosition, Color color)
        {
            var rotatedSlots = GetRotatedSlots(item.SlotPositions, item.CurrentRotation);
            var newPositions = rotatedSlots.Select(slot => slot + gridPosition).ToList();

            foreach (var pos in newPositions)
            {
                if (_slots.TryGetValue(pos, out var slot))
                {
                    if (CanPlaceItem(item, gridPosition))
                        slot.SetColor(color);
                }
            }
        }

        public void ResetSlotHighlight(ImplantBehaviour item)
        {
            foreach (var slot in _slots)
            {
                if (!slot.Value.Occupied ||
                    _occupiedSlots.TryGetValue(slot.Key, out var itemOccupied) && itemOccupied.Equals(item))
                    slot.Value.SetColor(Color.white);
            }
        }

        private void CreateGrid()
        {
            var defaultInventoryGrid = CM.Get(CMs.Gameplay.Inventory).GetComponent<InventoryComponent>();
            var currentGrid = defaultInventoryGrid.Grids.Last(grid => grid.RequiredLevel <= SL.Get<GameStateHolder>().CurrentData.Level).Grid;
            _gridSize = currentGrid.GridSize;
            _cellSize = defaultInventoryGrid.CellSize;

            _inventoryView.ResizeInventoryView(_gridSize, _cellSize);

            foreach (var slotPos in currentGrid.GridPattern)
            {
                var slotObj = new GameObject($"InventorySlot_{slotPos.x}_{slotPos.y}");
                var slot = slotObj.AddComponent<InventorySlot>();

                _inventoryView.SetupInventorySlot(slotObj, slotPos, _cellSize);

                slot.Initialize(slotPos);
                _slots[slotPos] = slot;
            }
        }

        public bool CanPlaceItem(ImplantBehaviour item, Vector2Int gridPosition)
        {
            var rotatedSlots = GetRotatedSlots(item.SlotPositions, item.CurrentRotation);
            var targetSlots = rotatedSlots.Select(slot => slot + gridPosition).ToList();

            foreach (var slot in targetSlots)
            {
                if (!IsSlotExist(slot) ||
                    (_occupiedSlots.TryGetValue(slot, out var itemOccupied) && !itemOccupied.Equals(item)))
                {
                    return false;
                }
            }

            return true;
        }

        private bool IsSlotExist(Vector2Int slotPosition)
        {
            return _slots.ContainsKey(slotPosition);
        }

        private void UpdateSlotVisual(Vector2Int slotPosition)
        {
            if (_slots.TryGetValue(slotPosition, out var slot))
            {
                slot.SetOccupied(IsSlotOccupied(slotPosition));
            }
        }

        public void UpdateAllSlotVisual()
        {
            foreach (var slot in _slots)
            {
                slot.Value.SetOccupied(IsSlotOccupied(slot.Key));
            }
        }

        private List<Vector2Int> GetRotatedSlots(List<Vector2Int> slots, int currentRotation)
        {
            for (int i = 0; i < currentRotation; i++)
            {
                slots = RotateSlotsClockwise(slots);
            }

            return slots;
        }

        private List<Vector2Int> RotateSlotsClockwise(List<Vector2Int> slots)
        {
            return slots.Select(slot => new Vector2Int(slot.y, -slot.x)).ToList();
        }

        public void Dispose()
        {
            SL.Get<BattleController>().OnTurnEnded -= OnTurnEnded;
        }
    }
}