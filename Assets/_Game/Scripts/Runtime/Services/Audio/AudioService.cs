using Game.Runtime.CMS;
using Game.Runtime.CMS.Components.Audio;
using Game.Runtime.Utils.Extensions;
using UnityEngine;

namespace Game.Runtime.Services.Audio
{
    public class AudioService : IService
    {
        private bool _muteSFX;
        private bool _muteAmbient;
        private bool _muteMusic;

        private float _sfxVolume = 1.0f;
        private float _ambientVolume = 1.0f;
        private float _musicVolume = 1.0f;

        private readonly AudioSource _ambientSource;
        private readonly AudioSource _musicSource;
        private readonly AudioSource _sfxSource;
        
        public AudioService()
        {
            var audioObject = new GameObject(nameof(AudioService));
            _ambientSource = audioObject.AddComponent<AudioSource>();
            _musicSource = audioObject.AddComponent<AudioSource>();
            _sfxSource = audioObject.AddComponent<AudioSource>();

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
    }
}