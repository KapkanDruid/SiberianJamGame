using System;
using Game.Runtime.Gameplay.Implants;
using Game.Runtime.Utils.Extensions;
using UnityEngine;

namespace Game.Runtime.Gameplay.HUD
{
    [Serializable]
    public class ImplantsHolder
    {
        [SerializeField] private RectTransform parent;
        [SerializeField] private RectTransform holderRoot;
        
        public bool IsInsideHolder(Vector2 screenPosition)
        {
            return RectTransformUtility.RectangleContainsScreenPoint(parent, screenPosition);
        }

        public void SetItemPosition(ImplantBehaviour item, Vector2 position)
        {
            item.transform.SetParent(holderRoot);
            item.GetComponent<RectTransform>().anchoredPosition = position;
        }

        public Vector2 GetRandomPosition()
        {
            return holderRoot.GetRandomPositionInside();
        }
    }
}