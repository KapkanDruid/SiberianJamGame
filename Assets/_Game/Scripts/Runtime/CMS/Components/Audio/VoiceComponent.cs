using System;
using UnityEngine;

namespace Game.Runtime.CMS.Components.Audio
{
    [Serializable]
    public class VoiceComponent : CMSComponent
    {
        public AudioClip Clip;
        [Range(0,2)] public float Volume = 1f;
    }
}