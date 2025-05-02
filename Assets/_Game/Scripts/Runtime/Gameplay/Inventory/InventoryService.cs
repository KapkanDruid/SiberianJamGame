using System.Collections.Generic;
using System.Linq;
using Game.Runtime.CMS;
using Game.Runtime.CMS.Components.Gameplay;
using Game.Runtime.Gameplay.HUD;
using Game.Runtime.Services;
using UnityEngine;

namespace Game.Runtime.Gameplay.Inventory
{
    public class InventoryService : IInitializable, IService
    {
        public int CellSize {get; private set;}

        private readonly Dictionary<Vector2Int, InventoryItem> _occupiedSlots = new();
        private readonly HashSet<Vector2Int> _availableSlots = new();
        private readonly Dictionary<InventoryItem, List<Vector2Int>> _itemPositions = new();
        private readonly Dictionary<Vector2Int, InventorySlot> _slots = new();

        public void Initialize()
        {
            var defaultInventoryGrid = CM.Get(CMs.Gameplay.Inventory.InventoryGrid).GetComponent<InventoryComponent>();
            foreach (var slotPos in defaultInventoryGrid.Grid.GridPattern)
                _availableSlots.Add(slotPos);
            
            CellSize = defaultInventoryGrid.CellSize;
            
            CreateGrid();
        }

        private void CreateGrid()
        {
            foreach (var slotPos in _availableSlots)
            {
                var slotObj = new GameObject($"InventorySlot_{slotPos.x}_{slotPos.y}");
                var slot = slotObj.AddComponent<InventorySlot>();
                
                SL.Get<HUDService>().Behaviour.SetupInventorySlot(slotObj, slotPos, CellSize);

                slot.Initialize(this, slotPos);
                _slots[slotPos] = slot;
            }
        }

        public bool CanPlaceItem(InventoryItem item, Vector2Int gridPosition)
        {
            var rotatedSlots = GetRotatedSlots(item, item.CurrentRotation);
            var targetSlots = rotatedSlots.Select(slot => slot + gridPosition).ToList();

            foreach (var slot in targetSlots)
            {
                if (!_availableSlots.Contains(slot) &&
                    (!_occupiedSlots.TryGetValue(slot, out var occupyingItem) || occupyingItem != item))
                {
                    return false;
                }
            }

            return true;
        }

        public bool TryPlaceItem(InventoryItem item, Vector2Int gridPosition)
        {
            if (!CanPlaceItem(item, gridPosition)) return false;

            if (_itemPositions.TryGetValue(item, out var oldPositions))
            {
                foreach (var pos in oldPositions)
                {
                    _occupiedSlots.Remove(pos);
                    UpdateSlotVisual(pos);
                }
            }

            var rotatedSlots = GetRotatedSlots(item, item.CurrentRotation);
            var newPositions = rotatedSlots.Select(slot => slot + gridPosition).ToList();

            foreach (var pos in newPositions)
            {
                _occupiedSlots[pos] = item;
                UpdateSlotVisual(pos);
            }

            _itemPositions[item] = newPositions;
            return true;
        }

        public void RemoveItem(InventoryItem item)
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

        public InventorySlot FindSlotAtPosition(Vector2Int position)
        {
            return _slots.GetValueOrDefault(position);
        }

        public List<Vector2Int> GetItemSlots(InventoryItem item)
        {
            return _itemPositions.TryGetValue(item, out var positions) ? positions : new List<Vector2Int>();
        }

        public bool IsSlotOccupied(Vector2Int slot)
        {
            return _occupiedSlots.ContainsKey(slot);
        }

        public bool IsSlotAvailable(Vector2Int slot)
        {
            return _availableSlots.Contains(slot);
        }

        public Vector2Int? FindFreePositionForItem(InventoryItem item)
        {
            foreach (var slot in _availableSlots)
            {
                if (CanPlaceItem(item, slot))
                {
                    return slot;
                }
            }

            return null;
        }

        public bool TryAddItemToInventory(InventoryItem item)
        {
            var freePosition = FindFreePositionForItem(item);
            if (freePosition.HasValue && _slots.TryGetValue(freePosition.Value, out var slot))
            {
                item.transform.SetParent(slot.transform);
                item.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                return TryPlaceItem(item, freePosition.Value);
            }

            return false;
        }

        public void UpdateSlotVisual(Vector2Int slotPosition)
        {
            if (_slots.TryGetValue(slotPosition, out var slot))
            {
                slot.UpdateVisual(IsSlotOccupied(slotPosition));
            }
        }

        private Vector2Int CalculateItemSize(List<Vector2Int> slots)
        {
            if (slots.Count == 0) return Vector2Int.zero;

            int minX = slots[0].x;
            int maxX = slots[0].x;
            int minY = slots[0].y;
            int maxY = slots[0].y;

            foreach (var pos in slots)
            {
                minX = Mathf.Min(minX, pos.x);
                maxX = Mathf.Max(maxX, pos.x);
                minY = Mathf.Min(minY, pos.y);
                maxY = Mathf.Max(maxY, pos.y);
            }

            return new Vector2Int(maxX - minX + 1, maxY - minY + 1);
        }

        private List<Vector2Int> GetRotatedSlots(InventoryItem item, int rotation)
        {
            var slots = new List<Vector2Int>(item.SlotPositions);

            for (int i = 0; i < rotation; i++)
            {
                slots = RotateSlotsClockwise(slots);
            }

            return slots;
        }

        private List<Vector2Int> RotateSlotsClockwise(List<Vector2Int> slots)
        {
            return slots.Select(slot => new Vector2Int(slot.y, -slot.x)).ToList();
        }
    }
}