using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Runtime.CMS.Components.Audio
{
    [Serializable]
    public class SFXComponent : CMSComponent
    {
        public List<AudioClip> Clips = new();
        [Range(0, 2)] public float Volume = 1f;
    }
}