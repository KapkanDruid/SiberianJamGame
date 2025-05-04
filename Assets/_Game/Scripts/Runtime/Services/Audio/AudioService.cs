using Game.Runtime.CMS;
using Game.Runtime.CMS.Components.Audio;
using Game.Runtime.CMS.Components.Commons;
using Game.Runtime.Utils.Extensions;
using UnityEngine;

namespace Game.Runtime.Services.Audio
{
    public class AudioService : IService
    {
        private bool _muteSFX;
        private bool _muteAmbient;
        private bool _muteMusic;
        private bool _muteVoice;

        private float _sfxVolume = 1.0f;
        private float _ambientVolume = 1.0f;
        private float _musicVolume = 1.0f;
        private float _voiceVolume = 1.0f;

        private readonly AudioSource _ambientSource;
        private readonly AudioSource _musicSource;
        private readonly AudioSource _sfxSource;
        private readonly AudioSource _voiceSource;

        public AudioService()
        {
            var audioObject = new GameObject(nameof(AudioService));
            _ambientSource = audioObject.AddComponent<AudioSource>();
            _musicSource = audioObject.AddComponent<AudioSource>();
            _sfxSource = audioObject.AddComponent<AudioSource>();
            _voiceSource = audioObject.AddComponent<AudioSource>();

            var mixerReference = CM.Get(CMs.Audio.AudioControl).GetComponent<AudioSettingsComponent>();

            _musicSource.outputAudioMixerGroup = mixerReference.MusicGroup;
            _sfxSource.outputAudioMixerGroup = mixerReference.SFXGroup;
            _voiceSource.outputAudioMixerGroup = mixerReference.VoiceGroup;

            Object.DontDestroyOnLoad(audioObject);
        }

        public void Play(string entityId)
        {
            Play(CM.Get(entityId));
        }
        
        public void Play(CMSEntity entity)
        {
            if (entity.Is(out SFXComponent sfxComponent))
                PlaySFX(sfxComponent);
            else if (entity.Is(out AmbientComponent ambientComponent))
                PlayAmbient(ambientComponent);
            else if (entity.Is(out MusicComponent musicComponent))
                PlayMusic(musicComponent);
            else if (entity.Is(out VoiceComponent voiceComponent))
                PlayVoice(voiceComponent);
        }
        
        public void SetVolume(AudioType type, float volume)
        {
            switch (type)
            {
                case AudioType.SFX:
                    _sfxVolume = volume;
                    _sfxSource.volume = volume;
                    break;
                case AudioType.Ambient:
                    _ambientVolume = volume;
                    _ambientSource.volume = _ambientVolume;
                    break;
                case AudioType.Music:
                    _musicVolume = volume;
                    _musicSource.volume = volume;
                    break;
            }
        }

        public void Mute(AudioType type, bool mute)
        {
            switch (type)
            {
                case AudioType.SFX:
                    _muteSFX = mute;
                    break;
                case AudioType.Ambient:
                    _muteAmbient = mute;
                    _ambientSource.enabled = !mute;
                    break;
                case AudioType.Music:
                    _muteMusic = mute;
                    _musicSource.enabled = !mute;
                    break;
            }
        }

        private void PlaySFX(SFXComponent sfxComponent)
        {
            if (_muteSFX) return;
            
            var clip = sfxComponent.Clips.GetRandom(ignoreEmpty: true);
            _sfxSource.PlayOneShot(clip, _sfxVolume * sfxComponent.Volume);
        }

        private void PlayAmbient(AmbientComponent ambient)
        {
            if (_muteAmbient) return;
            
            _ambientSource.clip = ambient.Clip;
            _ambientSource.loop = true;
            _ambientSource.volume = _ambientVolume;
            
            _ambientSource.Play();
        }

        private void PlayMusic(MusicComponent music)
        {
            if (_muteMusic) return;
            
            _musicSource.clip = music.Clip;
            _musicSource.loop = true;
            _musicSource.volume = _musicVolume;
            
            _musicSource.Play();
        }

        private void PlayVoice(VoiceComponent voice)
        {
            if (_muteVoice) return;

            _voiceSource.clip = voice.Clip;
            _voiceSource.loop = false;
            _voiceSource.volume = _voiceVolume * voice.Volume;

            _voiceSource.Play();
        }
    }
}