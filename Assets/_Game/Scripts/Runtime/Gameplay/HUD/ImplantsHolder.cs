using System;
using Game.Runtime.Gameplay.Implants;
using Game.Runtime.Services;
using Game.Runtime.Services.Camera;
using UnityEngine;

namespace Game.Runtime.Gameplay.HUD
{
    [Serializable]
    public class ImplantsHolder
    {
        [SerializeField] private RectTransform parent;
        [SerializeField] private RectTransform holderRoot;

        public void SetActive(bool active)
        {
            parent.gameObject.SetActive(active);
        }
        
        public bool IsInsideHolder(Vector2 screenPosition)
        {
            return RectTransformUtility.RectangleContainsScreenPoint(parent, 
                SL.Get<CameraService>().Camera.ScreenToViewportPoint(screenPosition));
        }

        public void SetItemPosition(ImplantBehaviour item, Vector2 position)
        {
            item.transform.SetParent(holderRoot);
            item.GetComponent<RectTransform>().anchoredPosition = position;
        }
    }
}