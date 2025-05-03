using Cysharp.Threading.Tasks;
using DG.Tweening;
using Game.Runtime.Services;
using System;
using UnityEngine;

namespace Game.Runtime.CMS.Components.Commands
{
    public abstract class Command : CMSComponent
    {
        public abstract void Execute(Action onCompleted);
    }

    public class TestCommand : Command
    {
        public override void Execute(Action onCompleted)
        {
            Debug.Log("Test Command executed");
        }
    }

    public class PrintTextCommand : Command
    {
        [SerializeField] private string _name;
        [SerializeField, TextArea(5, 10)] private string _text;

        public override void Execute(Action onCompleted)
        {
            Print(onCompleted).Forget();
        }

        private async UniTask Print(Action onCompleted)
        {
            SL.Get<DialogController>().Name.text = _name;
            await SL.Get<DialogController>().PrintText(_text);

            onCompleted?.Invoke();
        }
    }

    public class DialogAppearCommand : Command
    {
        [SerializeField] private AppearType _appearType;
        [SerializeField] private float _duration = 0.3f;
        public override void Execute(Action onCompleted)
        {
            var controller = SL.Get<DialogController>();
            if (_appearType == AppearType.Show)
            {
                controller.gameObject.SetActive(true);
                controller.DialogPanel.DOFade(1, _duration).OnComplete(() => onCompleted?.Invoke());
            }
            if (_appearType == AppearType.Hide)
            {
                controller.gameObject.SetActive(false);
                controller.DialogPanel.DOFade(0, _duration).OnComplete(() => onCompleted?.Invoke());
            }
        }
    }

    public class IconAppearCommand : Command
    {
        [SerializeField] private AppearType _appearType;
        [SerializeField] private float _duration = 0.3f;
        public override void Execute(Action onCompleted)
        {
            if (_appearType == AppearType.Show)
                SL.Get<DialogController>().IconImage.DOFade(1, _duration).OnComplete(() => onCompleted?.Invoke());
            if (_appearType == AppearType.Hide)
                SL.Get<DialogController>().IconImage.DOFade(0, _duration).OnComplete(() => onCompleted?.Invoke());
        }
    }

    public enum AppearType
    {
        None,
        Show,
        Hide,
    }
}
