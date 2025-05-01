using UnityEngine;

namespace Runtime._Game.Scripts.Runtime.Utils.Helpers
{
    public static partial class Helpers
    {
        public static class ColorHelper
        {
            public static Color Rainbow(float time)
            {
                float red = Mathf.Sin(time * 0.5f + 0) * 0.5f + 0.5f;
                float green = Mathf.Sin(time * 0.5f + 2 * Mathf.PI / 3) * 0.5f + 0.5f;
                float blue = Mathf.Sin(time * 0.5f + 4 * Mathf.PI / 3) * 0.5f + 0.5f;

                return new Color(red, green, blue);
            }

            public static void SetAlpha(SpriteRenderer sprite, float alpha)
            {
                var color = sprite.color;
                color.a = alpha;
                sprite.color = color;
            }
        }
    }
}