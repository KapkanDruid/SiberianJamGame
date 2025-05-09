using Cysharp.Threading.Tasks;
using Game.Runtime.Services;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Game.Runtime.Utils.Consts;
using Game.Runtime.Services.UI;

namespace Game.Runtime.CMS.Components.Commands
{
    public class EndDialogSceneCommand : Command
    {
        public override void Execute(Action onCompleted)
        {
            SwitchScene(onCompleted).Forget();
        }

        private async UniTask SwitchScene(Action onCompleted)
        {
            await ServiceLocator.Get<UIFaderService>().FadeIn();
            await SceneManager.LoadSceneAsync(Const.ScenesConst.GameReleaseScene);
            onCompleted?.Invoke();
        }
    }
}
