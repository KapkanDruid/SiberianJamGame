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
        
        public static Vector2 GetRandomPositionInside(this RectTransform rectTransform)
        {
            Vector2 size = rectTransform.rect.size;
            Vector2 scaledSize = new Vector2(
                size.x * rectTransform.lossyScale.x,
                size.y * rectTransform.lossyScale.y
            );

            Vector2 pivotOffset = new Vector2(
                scaledSize.x * rectTransform.pivot.x,
                scaledSize.y * rectTransform.pivot.y
            );

            float randomX = Random.Range(-pivotOffset.x, scaledSize.x - pivotOffset.x);
            float randomY = Random.Range(-pivotOffset.y, scaledSize.y - pivotOffset.y);

            Vector2 localRandomPosition = new Vector2(randomX, randomY);
            Vector2 worldRandomPosition = rectTransform.TransformPoint(localRandomPosition);

            return worldRandomPosition;
        }
    }
}