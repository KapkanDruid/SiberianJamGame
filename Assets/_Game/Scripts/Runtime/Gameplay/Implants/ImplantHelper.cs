using UnityEngine;

namespace Game.Runtime.Gameplay.Implants
{
    public static class ImplantHelper
    {
        public static readonly float[] PresetAngles = { 0f, -90f, -180f, -270f };
        
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