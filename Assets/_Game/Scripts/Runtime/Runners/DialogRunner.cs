using Cysharp.Threading.Tasks;
using Game.Runtime.CMS;
using Game.Runtime.Gameplay.Level;
using Game.Runtime.Services;
using Game.Runtime.Services.Audio;
using Game.Runtime.Services.Save;
using Game.Runtime.Services.UI;
using UnityEngine;

namespace Game.Runtime.Runners
{
    public class DialogRunner : MonoBehaviour
    {
        private void Start()
        {
            StartGame().Forget();
        }

        private async UniTask StartGame()
        {
            Configurate();
            await SL.Get<UIFaderService>().FadeOut();
        }

        private void Configurate()
        {
            var dialogPrefabID = SL.Get<GameStateHolder>().DialogBlockID;

            SL.Get<Invoker>().Play(CM.Get(dialogPrefabID));
        }
    }
}