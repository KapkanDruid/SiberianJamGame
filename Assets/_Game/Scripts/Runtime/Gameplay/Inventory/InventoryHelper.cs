using System.Linq;
using UnityEngine;

namespace Game.Runtime.Gameplay.Inventory
{
    public static partial class Helpers
    {
        public static class InventoryHelper
        {
            public static Vector2 CalculateCenterPosition(InventorySlot slot, InventoryItem item)
            {
                RectTransform baseSlotRect = slot.GetComponent<RectTransform>();
    
                Vector2 pivotOffset = new Vector2(
                    (item.SlotPositions.Max(p => p.x) + item.SlotPositions.Min(p => p.x)) * 0.5f,
                    (item.SlotPositions.Max(p => p.y) + item.SlotPositions.Min(p => p.y)) * 0.5f
                );
    
                float slotWidth = baseSlotRect.rect.width;
                float slotHeight = baseSlotRect.rect.height;
                
                Vector2 itemOffset;
                switch (item.CurrentRotation)
                {
                    case 1:
                    {
                        itemOffset = new Vector2(pivotOffset.y * slotHeight, -pivotOffset.x * slotWidth);
                        break;
                    }
                    case 2:
                    {
                        itemOffset = new Vector2(-pivotOffset.x * slotWidth, -pivotOffset.y * slotHeight);
                        break;
                    }
                    case 3:
                    {
                        itemOffset = new Vector2(-pivotOffset.y * slotHeight, pivotOffset.x * slotWidth);
                        break;
                    }
                    default:
                    {
                        itemOffset = new Vector2(pivotOffset.x * slotWidth, pivotOffset.y * slotHeight);
                        break;
                    }
                }

                return baseSlotRect.anchoredPosition + itemOffset;
            }
        }
    }
}