using UnityEngine.Audio;

namespace Game.Runtime.CMS.Components.Commons
{
    public class AudioSettingsComponent : CMSComponent
    {
        public AudioMixerGroup VoiceGroup;
        public AudioMixerGroup SFXGroup;
        public AudioMixerGroup MusicGroup;
    }
}