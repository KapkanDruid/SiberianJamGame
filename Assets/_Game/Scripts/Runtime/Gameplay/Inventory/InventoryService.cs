using System.Collections.Generic;
using System.Linq;
using Game.Runtime.CMS;
using Game.Runtime.CMS.Components.Gameplay;
using Game.Runtime.Gameplay.HUD;
using Game.Runtime.Services;
using UnityEngine;

namespace Game.Runtime.Gameplay.Inventory
{
    public class InventoryService : IService, IInitializable
    {
        private readonly Dictionary<Vector2Int, InventorySlot> _slots = new();
        private readonly Dictionary<Vector2Int, InventoryItem> _occupiedSlots = new();
        private readonly Dictionary<InventoryItem, List<Vector2Int>> _itemPositions = new();
        
        private Vector2Int _gridSize;
        private int _cellSize;
        
        private InventoryView _inventoryView;

        public void Initialize()
        {
            _inventoryView = SL.Get<HUDService>().Behaviour.InventoryView;
            CreateGrid();
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

            var rotatedSlots = GetRotatedSlots(item);
            var newPositions = rotatedSlots.Select(slot => slot + gridPosition).ToList();

            foreach (var pos in newPositions)
            {
                _occupiedSlots[pos] = item;
                UpdateSlotVisual(pos);
            }

            _itemPositions[item] = newPositions;
                
            return true;
        }
        
        public bool NeedRemoveItem(InventoryItem item, Vector2 screenPosition)
        {
            return _itemPositions.GetValueOrDefault(item) != default && 
                   _inventoryView.IsOutsideInventory(screenPosition);
        }
        
        private bool IsSlotOccupied(Vector2Int slot)
        {
            return _occupiedSlots.ContainsKey(slot);
        }
        
        public void SetItemPosition(InventorySlot slot, InventoryItem item)
        {
            var itemCenterPosition = Helpers.InventoryHelper.CalculateCenterPosition(slot, item);
            _inventoryView.SetItemInInventory(item, itemCenterPosition);
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
        
        public void UpdateSlotHighlight(InventoryItem item, Vector2Int gridPosition, Color color)
        {
            var rotatedSlots = GetRotatedSlots(item);
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
        
        public void ResetSlotHighlight(InventoryItem item)
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
            var defaultInventoryGrid = CM.Get(CMs.Gameplay.Inventory.InventoryGrid).GetComponent<InventoryComponent>();
            _gridSize = defaultInventoryGrid.Grid.GridSize;
            _cellSize = defaultInventoryGrid.CellSize;
            
            _inventoryView.ResizeInventoryView(_gridSize, _cellSize);
            
            foreach (var slotPos in defaultInventoryGrid.Grid.GridPattern)
            {
                var slotObj = new GameObject($"InventorySlot_{slotPos.x}_{slotPos.y}");
                var slot = slotObj.AddComponent<InventorySlot>();
                
                _inventoryView.SetupInventorySlot(slotObj, slotPos, _cellSize);

                slot.Initialize(slotPos);
                _slots[slotPos] = slot;
            }
        }

        private bool CanPlaceItem(InventoryItem item, Vector2Int gridPosition)
        {
            var rotatedSlots = GetRotatedSlots(item);
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

        private List<Vector2Int> GetRotatedSlots(InventoryItem item)
        {
            var slots = new List<Vector2Int>(item.SlotPositions);

            for (int i = 0; i < item.CurrentRotation; i++)
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