using System.Collections.Generic;
using System.Linq;
using Game.Runtime.CMS.Components.Implants;
using Game.Runtime.Gameplay.Implants;
using Game.Runtime.Services;
using UnityEngine;

namespace Game.Runtime.Gameplay.Inventory
{
    public static class InventoryHelper
    {
        public static readonly Vector2Int[] NeighborOffsets = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right, Vector2Int.zero };

        public static List<Vector2Int> ConvertToInventorySlots(ImplantBehaviour implant, Vector2Int gridPosition)
        {
            var rotatedCells = GetRotatedSlots(implant.SlotPositions, implant.CurrentRotation);
            return rotatedCells.Select(x => x + gridPosition).ToList();
        }
        
        public static List<Vector2Int> FindSynergySlots(ImplantBehaviour implant, Vector2Int gridPosition)
        {
            var hasSynergy = false;
            var synergySlots = new List<Vector2Int>();
            var inventoryService = ServiceLocator.Get<InventoryService>();
            var inventorySlots = ConvertToInventorySlots(implant, gridPosition);
            
            foreach (var cell in inventorySlots)
            {
                if (IsPositionBlocked(implant, gridPosition, cell))
                    continue;

                foreach (var offset in NeighborOffsets)
                {
                    var neighborPos = cell + offset;
            
                    if (!inventoryService.OccupiedSlots.TryGetValue(neighborPos, out var neighborSlot) || 
                        neighborSlot == implant)
                        continue;

                    if (neighborSlot.GetImplantType() != implant.GetImplantType())
                        continue;

                    if (IsPositionBlocked(neighborSlot, neighborSlot.CenterSlotPosition, neighborPos))
                        continue;

                    foreach (var neighborCell in inventoryService.ItemPositions.GetValueOrDefault(neighborSlot))
                    {
                        if (!IsPositionBlocked(neighborSlot, neighborSlot.CenterSlotPosition, neighborCell))
                        {
                            synergySlots.Add(neighborCell);
                            hasSynergy = true;
                        }
                    }
                }
            }

            if (hasSynergy)
            {
                synergySlots.AddRange(inventorySlots.Where(cell => !IsPositionBlocked(implant, gridPosition, cell)));
            }
            
            return synergySlots;
        }
        
        public static Vector2 CalculateCenterPosition(InventorySlot slot, ImplantBehaviour item)
        {
            RectTransform baseSlotRect = slot.GetComponent<RectTransform>();

            Vector2 pivotOffset = new Vector2(
                (item.SlotPositions.Max(p => p.x) + item.SlotPositions.Min(p => p.x)) * 0.5f,
                (item.SlotPositions.Max(p => p.y) + item.SlotPositions.Min(p => p.y)) * 0.5f
            );

            float slotWidth = baseSlotRect.rect.width;
            float slotHeight = baseSlotRect.rect.height;
            
            return baseSlotRect.anchoredPosition + ImplantHelper.ApplyRotationToOffset(new Vector2(pivotOffset.x * slotWidth, pivotOffset.y * slotHeight), item.CurrentRotation);
        }
        
        public static bool IsPositionBlocked(ImplantBehaviour implant, Vector2Int gridPosition, Vector2Int position)
        {
            if (!implant.Model.Is<BrokenImplantCellsComponent>(out var blockComponent))
                return false;

            var rotatedCell = InventoryHelper.GetRotatedSlots(blockComponent.BrokenCells.ToList(), implant.CurrentRotation);
            var globalBlockCell = rotatedCell.Select(x => x + gridPosition);
            
            return globalBlockCell.Contains(position);
        }
        
        public static List<Vector2Int> GetRotatedSlots(List<Vector2Int> slots, int currentRotation)
        {
            for (int i = 0; i < currentRotation; i++)
            {
                slots = RotateSlotsClockwise(slots);
            }

            return slots;
        }

        private static List<Vector2Int> RotateSlotsClockwise(List<Vector2Int> slots)
        {
            return slots.Select(slot => new Vector2Int(slot.y, -slot.x)).ToList();
        }
    }
}