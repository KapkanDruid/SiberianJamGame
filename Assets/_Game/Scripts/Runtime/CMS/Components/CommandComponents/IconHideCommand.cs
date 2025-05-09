using Game.Runtime.Services;
using System;
using UnityEngine;

namespace Game.Runtime.CMS.Components.Commands
{
    public class IconHideCommand : Command
    {
        [SerializeField] private DialogController.PositionType _positionType;
        [SerializeField] private float _duration = 0.3f;
        public override void Execute(Action onCompleted)
        {
            ServiceLocator.Get<DialogController>().HideIcon(_positionType, _duration);
            onCompleted?.Invoke();
        }
    }
}
