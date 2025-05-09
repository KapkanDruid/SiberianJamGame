using DG.Tweening;
using Game.Runtime.CMS;
using Game.Runtime.Services;
using Game.Runtime.Services.Audio;
using UnityEngine.EventSystems;

namespace Game.Runtime.Gameplay.Implants
{
    public class ImplantPointerHandler
    {
        private readonly ImplantBehaviour _implant;

        public ImplantPointerHandler(ImplantBehaviour implant) => _implant = implant;

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (_implant == null) return;
            if (_implant.IsDragging) return;
            if (ServiceLocator.Get<InventoryService>().HasItem(_implant)) return;
            
            _implant.CurrentTweenScale?.Kill();
            _implant.CurrentTweenScale = _implant.RectTransform.DOScale(_implant.OriginalScale * 1.2f, 0.2f)
                .SetEase(Ease.OutBack).OnKill(() => _implant.CurrentTweenScale = null);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (_implant == null) return;
            if (_implant.IsDragging) return;
            if (ServiceLocator.Get<InventoryService>().HasItem(_implant)) return;
            
            ServiceLocator.Get<AudioService>().Play(CMs.Audio.SFX.Hover);

            _implant.CurrentTweenScale?.Kill();
            _implant.CurrentTweenScale = _implant.RectTransform.DOScale(_implant.OriginalScale, 0.2f)
                .SetEase(Ease.InOutQuad).OnKill(() => _implant.CurrentTweenScale = null);
        }
    }
}