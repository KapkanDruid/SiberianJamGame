using DG.Tweening;
using Game.Runtime.CMS;
using Game.Runtime.Gameplay.Implants.Services;
using Game.Runtime.Gameplay.Inventory;
using Game.Runtime.Services;
using Game.Runtime.Services.Audio;
using UnityEngine.EventSystems;

namespace Game.Runtime.Gameplay.Implants
{
    public class ImplantPointerHandler
    {
        private readonly ImplantBehaviour _implant;

        public ImplantPointerHandler(ImplantBehaviour implant) => _implant = implant;

        public void OnPointerEnter()
        {
            if (!CanInteract()) return;
            _implant.CurrentTweenScale?.Kill();
            _implant.CurrentTweenScale = _implant.RectTransform.DOScale(_implant.OriginalScale * 1.2f, 0.2f)
                .SetEase(Ease.OutBack).OnKill(() => _implant.CurrentTweenScale = null);
        }

        public void OnPointerExit()
        {
            if (!CanInteract()) return;
            ServiceLocator.Get<AudioService>().Play(CMs.Audio.SFX.Hover);
            _implant.CurrentTweenScale?.Kill();
            _implant.CurrentTweenScale = _implant.RectTransform.DOScale(_implant.OriginalScale, 0.2f)
                .SetEase(Ease.InOutQuad).OnKill(() => _implant.CurrentTweenScale = null);
        }

        private bool CanInteract()
        {
            if (_implant == null) return false;
            if (!_implant.CanInteract) return false;
            if (ServiceLocator.Get<ImplantsGameLoop>().IsGlobalDragging) return false;
            if (ServiceLocator.Get<InventoryService>().HasItem(_implant)) return false;

            return true;
        }
    }
}