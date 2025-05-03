using Cysharp.Threading.Tasks;
using Game.Runtime.Services;
using System;
using UnityEngine;

namespace Game.Runtime.CMS.Components.Commands
{
    public class IconAppearCommand : Command
    {
        [SerializeField] private DialogController.PositionType _positionType;
        [SerializeField] private Vector3 _offset;
        [SerializeField] private float _duration = 0.3f;
        [SerializeField] private Sprite _sprite;

        public override void Execute(Action onCompleted)
        {
            ShowAsync(onCompleted).Forget();

/*            if (_appearType == AppearType.Show)
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
            }*/
        }

        private async UniTask ShowAsync(Action action)
        {
            await SL.Get<DialogController>().ShowIcon(_positionType, _sprite, _offset, _duration);

            action?.Invoke();
        }
    }

    public class IconHideCommand : Command
    {
        public override void Execute(Action onCompleted)
        {
            throw new NotImplementedException();
        }
    }
}
