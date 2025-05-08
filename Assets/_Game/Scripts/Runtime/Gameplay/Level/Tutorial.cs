using Cysharp.Threading.Tasks;
using Game.Runtime.CMS;
using Game.Runtime.Gameplay.HUD;
using Game.Runtime.Services;
using Game.Runtime.Services.Audio;
using Game.Runtime.Gameplay.Implants;
using UnityEngine;

namespace Game.Runtime.Gameplay.Level
{
    public class Tutorial : MonoBehaviour, IService, IInitializable
    {
        public Animator Finger;
        public bool IsImplantLooted;
        public bool IsFinished;

        public void Initialize()
        {
            if (SL.Get<GameStateHolder>().CurrentData.Level == 0)
                SL.Get<Invoker>().Play(CM.Get(CMs.CommandBlocks.Tutorial.tutorial_block_1));

            Finger.gameObject.SetActive(false);
        }

        public async UniTask StartFirstStage()
        {
            await ShowImplantUse();
            await ShowUseButton();

            SL.Get<Invoker>().Play(CM.Get(CMs.CommandBlocks.Tutorial.tutorial_block_2));
        }

        public async UniTask StartSecondStage()
        {
            Finger.gameObject.SetActive(true);

            Finger.SetTrigger("TakeImplant");

            await UniTask.WaitUntil(() => IsImplantLooted);

            Finger.gameObject.SetActive(false);

            SL.Get<Invoker>().Play(CM.Get(CMs.CommandBlocks.Tutorial.tutorial_block_3));
        }

        private async UniTask ShowImplantUse()
        {
            Finger.gameObject.SetActive(true);

            Finger.SetTrigger("ImplantUse");

            bool executed = false;
            SL.Get<InventoryService>().OnImplpantPlaced += () =>
            {
                executed = true;
                SL.Get<HUDService>().Behaviour.EndTurnButtonParent.SetActive(true);
            };
            await UniTask.WaitUntil(() => executed == true);
        }

        private async UniTask ShowUseButton()
        {
            Finger.SetTrigger("ButtonUse");

            await UniTask.WaitUntil(() => SL.Get<BattleController>().IsTurnStarted);

            Finger.gameObject.SetActive(false);

            bool executed = false;
            SL.Get<BattleController>().OnTurnEnded += () => executed = true;
            await UniTask.WaitUntil(() => executed == true);

            Finger.gameObject.SetActive(false);
        }
    }
}
