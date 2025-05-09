using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Runtime.Gameplay.Implants
{
    public static class InventoryHelper
    {
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