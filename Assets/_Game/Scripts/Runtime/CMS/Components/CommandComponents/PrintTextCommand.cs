using Cysharp.Threading.Tasks;
using Game.Runtime.Services;
using System;
using UnityEngine;

namespace Game.Runtime.CMS.Components.Commands
{
    public class PrintTextCommand : Command
    {
        [SerializeField, Range(0f, 0.5f)] private float _delay = 0.035f;
        [SerializeField] private string _name;
        [SerializeField, TextArea(5, 10)] private string _text;

        public override void Execute(Action onCompleted)
        {
            Print(onCompleted).Forget();
        }

        private async UniTask Print(Action onCompleted)
        {
            SL.Get<DialogController>().IsSkipped = false;

            SL.Get<DialogController>().Name.text = _name;
            await SL.Get<DialogController>().PrintText(_text, _delay);

            onCompleted?.Invoke();
        }
    }
}
