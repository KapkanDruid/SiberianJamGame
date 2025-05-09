﻿using DG.Tweening;
using Game.Runtime.Services;
using System;
using UnityEngine;

namespace Game.Runtime.CMS.Components.Commands
{
    public class DialogAppearCommand : Command
    {
        [SerializeField] private AppearType _appearType;
        [SerializeField] private float _duration = 0.3f;
        public override void Execute(Action onCompleted)
        {
            var controller = ServiceLocator.Get<DialogController>();
            if (_appearType == AppearType.Show)
            {
                controller.gameObject.SetActive(true);
                controller.DialogPanel.DOFade(controller.PanelColor.a, _duration).OnComplete(() => onCompleted?.Invoke());
            }
            if (_appearType == AppearType.Hide)
            {
                controller.HideAll(_duration, onCompleted);
            }
        }
    }
}
