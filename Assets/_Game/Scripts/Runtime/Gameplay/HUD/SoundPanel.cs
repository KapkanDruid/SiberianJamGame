using Game.Runtime.Services;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

namespace Game.Runtime.Gameplay
{
    public class SoundPanel : MonoBehaviour, IService
    {
        [SerializeField] private Slider _voice;
        [SerializeField] private Slider _music;
        [SerializeField] private Slider _effects;

        [SerializeField] private AudioMixer _audioMixer;

        private static float _musicValue = 1;
        private static float _effectsValue = 1;
        private static float _voiceValue = 1;

        private void Start()
        {
            _music.value = _musicValue;
            _effects.value = _effectsValue;
            _voice.value = _voiceValue;

            _voice.onValueChanged.AddListener((x) => SetVoiceVolume(x));
            _music.onValueChanged.AddListener((x) => SetMusicVolume(x));
            _effects.onValueChanged.AddListener((x) => SetEffectsVolume(x));
        }

        public void SetVoiceVolume(float value)
        {
            _voiceValue = value;
            _audioMixer.SetFloat("VoiceValue", Mathf.Log10(value) * 20);
        }

        public void SetEffectsVolume(float value)
        {
            _effectsValue = value;
            _audioMixer.SetFloat("SFXValue", Mathf.Log10(value) * 20);
        }

        public void SetMusicVolume(float value)
        {
            _musicValue = value;
            _audioMixer.SetFloat("MusicValue", Mathf.Log10(value) * 20);
        }
    }
}
