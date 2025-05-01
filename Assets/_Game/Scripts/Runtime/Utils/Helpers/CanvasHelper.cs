using UnityEngine;

namespace Game.Runtime.Utils.Helpers
{
    public static partial class Helpers
    {
        public static class CanvasHelper
        {
            public static Vector3 CanvasToWorld(RectTransform target)
            {
                var componentInParent = target.GetComponentInParent<Canvas>();
                var rectTransform = componentInParent.GetComponent<RectTransform>();

                Vector2 screenPosition = RectTransformUtility.PixelAdjustPoint(target.position, target, componentInParent);
                RectTransformUtility.ScreenPointToWorldPointInRectangle(rectTransform, screenPosition, 
                    Camera.main, out var worldPosition);
                
                return worldPosition;
            }
            
            public static bool IsOnScreen(Vector3 position)
            {
                Vector3 viewportPoint = Camera.main.WorldToViewportPoint(position);

                bool onScreen = viewportPoint.x >= 0 && viewportPoint.x <= 1
                                                     && viewportPoint.y >= 0 && viewportPoint.y <= 1
                                                     && viewportPoint.z > 0;

                return onScreen;
            }
        }
    }
}