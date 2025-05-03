using Game.Runtime.Services;
using Game.Runtime.Services.Audio;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Runtime.CMS.Components.Commands
{
    public class PlaySoundCommand : Command
    {
        [SerializeField] private List<CMSPrefab> _soundConfigs;

        public override void Execute(Action onCompleted)
        {
            foreach (var config in _soundConfigs)
                SL.Get<AudioService>().Play(config.EntityId);

            onCompleted?.Invoke();
        }
    }
}
