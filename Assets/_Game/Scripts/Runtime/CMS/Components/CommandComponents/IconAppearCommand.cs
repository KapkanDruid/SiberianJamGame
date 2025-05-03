using DG.Tweening;
using Game.Runtime.Services;
using System;
using UnityEngine;

namespace Game.Runtime.CMS.Components.Commands
{
    public class IconAppearCommand : Command
    {
        [SerializeField] private AppearType _appearType;
        [SerializeField] private float _duration = 0.3f;
        public override void Execute(Action onCompleted)
        {
            var controller = SL.Get<DialogController>();
            if (_appearType == AppearType.Show)
            {
                var color = controller.IconImage.color;
                color.a = 0;
                controller.IconImage.color = color;
                controller.IconImage.gameObject.SetActive(true);
                controller.IconImage.DOFade(1, _duration).OnComplete(() => onCompleted?.Invoke());

            }
            if (_appearType == AppearType.Hide)
            {
                controller.IconImage.DOFade(0, _duration).OnComplete(() =>
                {
                    onCompleted?.Invoke();
                    controller.IconImage.gameObject.SetActive(false);
                });
            }
        }
    }
}
