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
        }

        private async UniTask ShowAsync(Action action)
        {
            await ServiceLocator.Get<DialogController>().ShowIcon(_positionType, _sprite, _offset, _duration);

            action?.Invoke();
        }
    }
}
