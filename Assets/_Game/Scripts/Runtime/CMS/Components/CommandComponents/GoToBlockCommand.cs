using Game.Runtime.Services;
using Game.Runtime.Services.Audio;
using System;
using UnityEngine;

namespace Game.Runtime.CMS.Components.Commands
{
    public class GoToBlockCommand : Command
    {
        [SerializeField] private CMSPrefab _blockPrefab;
        public override void Execute(Action onCompleted)
        {
            ServiceLocator.Get<Invoker>().Play(CM.Get(_blockPrefab.EntityId));
            onCompleted?.Invoke();
        }
    }
}
