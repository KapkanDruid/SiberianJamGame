using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Game.Runtime.Gameplay.Implants;
using Game.Runtime.Services;
using Game.Runtime.Services.Camera;
using NUnit.Framework;
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
                ServiceLocator.Get<CameraService>().Camera.ScreenToViewportPoint(screenPosition));
        }
        
        public async UniTask SetImplants(List<ImplantBehaviour> implants, bool force = false)
        {
            if (implants == null || implants.Count == 0) 
                return;

            float totalWidth = implants.Sum(implant => implant.RectTransform.sizeDelta.x) + (implants.Count - 1) * 4f;
            float currentXPosition = -totalWidth * 0.5f;
    
            foreach (var implant in implants)
            {
                implant.SetCenterRectTransform();
                implant.transform.SetParent(holderRoot);
                implant.transform.localScale = Vector3.one * 0.7f;
        
                float itemWidth = implant.RectTransform.sizeDelta.x;
                Vector2 targetPosition = new Vector2(currentXPosition + itemWidth * 0.5f, 0f);
                currentXPosition += itemWidth + 4f;

                var moveTask = implant.RectTransform.DOAnchorPos(targetPosition, 0.3f).SetEase(Ease.Flash).Pause();
                
                if (force)
                {
                    moveTask.Play();
                    continue;
                }
        
                await moveTask.Play().ToUniTask();
            }
        }
    }
}