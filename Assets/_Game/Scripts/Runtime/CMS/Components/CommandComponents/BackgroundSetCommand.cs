using Game.Runtime.Services;
using System;
using UnityEngine;

namespace Game.Runtime.CMS.Components.Commands
{
    public class BackgroundSetCommand : Command
    {
        public Sprite BackgroundSprite;

        public override void Execute(Action onCompleted)
        {
            var background = ServiceLocator.Get<DialogController>().Background;
            background.sprite = BackgroundSprite;
            background.gameObject.SetActive(true);
            onCompleted?.Invoke();
        }
    }
}
