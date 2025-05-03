using Cysharp.Threading.Tasks;
using System;

namespace Game.Runtime.CMS.Components.Commands
{
    public class WaitSecondsCommand : Command
    {
        public float Seconds;

        public override void Execute(Action onCompleted)
        {
            Wait(onCompleted).Forget();
        }

        private async UniTask Wait(Action onCompleted)
        {
            await UniTask.WaitForSeconds(Seconds);

            onCompleted?.Invoke();
        }
    }
}
