using System.Collections.Generic;
using System.Linq;
using Game.Runtime.CMS;
using Game.Runtime.CMS.Components.Implants;
using Game.Runtime.Gameplay.Implants;
using UnityEngine;

namespace Game.Runtime.Gameplay.Inventory
{
    public class InventoryHighlighter
    {
        private readonly InventoryService _inventoryService;
        private readonly List<Vector2Int> _synergySlots = new();

        public InventoryHighlighter(InventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        public bool TryHighlightSynergySlots(ImplantBehaviour implant, InventorySlot newSlot)
        {
            _synergySlots.Clear();

            if (!_inventoryService.CanPlaceItem(implant, newSlot.GridPosition, implant.CurrentRotation))
                return false;
            
            _synergySlots.AddRange(InventoryHelper.FindSynergySlots(implant, newSlot.GridPosition));
            if (_synergySlots.Count == 0)
                return false;

            var synergyColors = CM.Get(CMs.Gameplay.Inventory)
                .GetComponent<ImplantSynergyColorsComponent>().SynergyColors;
            var targetColor = synergyColors.First(c => c.ImplantType == implant.GetImplantType()).Color;

            foreach (var slotPosition in _synergySlots)
            {
                if (_inventoryService.Grid.Slots.TryGetValue(slotPosition, out var slot))
                    slot.SetColor(targetColor);
            }

            return true;
        }
        
        public void ResetSlotsHighlight()
        {
            _synergySlots.Clear();
            UpdateSlotsHighlight();
        }
        
        public void UpdateSlotsHighlight()
        {
            foreach (var slot in _inventoryService.Grid.Slots)
                UpdateSlotHighlight(slot.Key);
        }
        
        public void UpdateSlotHighlight(Vector2Int slotPosition)
        {
            if (_synergySlots.Contains(slotPosition))
                return;

            if (_inventoryService.Grid.Slots.TryGetValue(slotPosition, out var slot))
            {
                // _inventoryService.OccupiedSlots.TryGetValue(slot.Key, out var itemOccupied) && itemOccupied.Equals(item);
                var isOccupiedSlot = _inventoryService.IsSlotOccupied(slot.GridPosition); 
                slot.SetColor(isOccupiedSlot ? slot.OccupiedColor : slot.DefaultColor);
            }
        }
    }
}