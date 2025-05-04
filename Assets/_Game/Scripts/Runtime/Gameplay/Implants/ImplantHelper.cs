using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Runtime.Gameplay.Implants
{
    public static class ImplantHelper
    {
        public static Vector2 CalculateCenterPosition(InventorySlot slot, ImplantBehaviour item)
        {
            RectTransform baseSlotRect = slot.GetComponent<RectTransform>();

            Vector2 pivotOffset = new Vector2(
                (item.SlotPositions.Max(p => p.x) + item.SlotPositions.Min(p => p.x)) * 0.5f,
                (item.SlotPositions.Max(p => p.y) + item.SlotPositions.Min(p => p.y)) * 0.5f
            );

            float slotWidth = baseSlotRect.rect.width;
            float slotHeight = baseSlotRect.rect.height;
            
            return baseSlotRect.anchoredPosition + ApplyRotationToOffset(new Vector2(pivotOffset.x * slotWidth, pivotOffset.y * slotHeight), item.CurrentRotation);
        }
        
        public static Vector2 ApplyRotationToOffset(Vector2 offset, int rotation)
        {
            return rotation switch
            {
                1 => new Vector2(offset.y, -offset.x),
                2 => new Vector2(-offset.x, -offset.y),
                3 => new Vector2(-offset.y, offset.x),
                _ => offset
            };
        }
    }
}