using UnityEngine;

namespace Game.Runtime.Utils.Extensions
{
    public static class RectTransformExtensions
    {
        public static Vector2 MousePositionToRectTransform(this RectTransform rectTransform, Camera uiCamera = null)
        {
            uiCamera ??= Camera.main;
            
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, Input.mousePosition, 
                uiCamera, out var localPoint);

            return localPoint;
        }
    }
}