using UnityEngine;

namespace Game.Runtime.Gameplay.Implants
{
    public class InventoryHighlighter
    {
        private readonly InventoryService _inventoryService;

        public InventoryHighlighter(InventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        public void HighlightSynergySlots(InventorySlot slot, Color color)
        {
            slot.SetColor(color);
        }
        
        public void ResetSlotHighlight(ImplantBehaviour item)
        {
            foreach (var slot in _inventoryService.Slots)
            {
                if (_inventoryService.SynergySlots.Contains(slot.Key))
                    return;
                
                if (!slot.Value.Occupied ||
                    _inventoryService.OccupiedSlots.TryGetValue(slot.Key, out var itemOccupied) &&
                    itemOccupied.Equals(item))
                {
                    slot.Value.SetColor(slot.Value.DefaultColor);
                }
                else
                {
                    slot.Value.SetColor(slot.Value.OccupiedColor);
                }
            }
        }
        
        public void UpdateSlotVisual(Vector2Int slotPosition)
        {
            if (_inventoryService.SynergySlots.Contains(slotPosition))
                return;
            
            if (_inventoryService.Slots.TryGetValue(slotPosition, out var slot))
            {
                slot.SetOccupied(_inventoryService.IsSlotOccupied(slotPosition));
            }
        }

        public void UpdateAllSlotVisual()
        {
            foreach (var slot in _inventoryService.Slots)
            {
                if (_inventoryService.SynergySlots.Contains(slot.Key))
                    return;

                slot.Value.SetOccupied(_inventoryService.IsSlotOccupied(slot.Key));
            }
        }

    }
}