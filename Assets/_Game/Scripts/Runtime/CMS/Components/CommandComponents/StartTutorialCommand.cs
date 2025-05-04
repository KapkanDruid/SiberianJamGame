using Cysharp.Threading.Tasks;
using Game.Runtime.Gameplay.Level;
using Game.Runtime.Services;
using System;

namespace Game.Runtime.CMS.Components.Commands
{
    public class StartTutorialCommand : Command
    {
        public int TutorialStage;

        public override void Execute(Action onCompleted)
        {
            if (TutorialStage == 1)
                SL.Get<Tutorial>().StartFirstStage().Forget();
            else if (TutorialStage == 2)
                SL.Get<Tutorial>().StartSecondStage().Forget();
            else if (TutorialStage == 3)
                SL.Get<Tutorial>().IsFinished = true;

            onCompleted?.Invoke();
        }
    }
}
