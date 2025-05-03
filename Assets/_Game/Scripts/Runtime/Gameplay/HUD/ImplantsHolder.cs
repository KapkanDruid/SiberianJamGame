using System;
using Game.Runtime.Utils.Extensions;
using UnityEngine;

namespace Game.Runtime.Gameplay.HUD
{
    [Serializable]
    public class ImplantsHolder
    {
        [SerializeField] private RectTransform holderRoot;
        
        public bool IsInsideHolder(Vector2 screenPosition)
        {
            return RectTransformUtility.RectangleContainsScreenPoint(holderRoot, screenPosition);
        }

        public Vector2 GetRandomPosition()
        {
            return holderRoot.GetRandomPositionInside();
        }
    }
}