using Cysharp.Threading.Tasks;
using DG.Tweening;
using Game.Runtime.Services.Input;
using System;
using TMPEffects.Components;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Runtime.Services
{
    public class DialogController : MonoBehaviour, IService, IInitializable, IDisposable
    {
        [SerializeField] private TextMeshProUGUI _dialogText;
        [SerializeField] private TMPWriter _dialogWriter;

        [SerializeField] private TextMeshProUGUI _name;
        [SerializeField] private Image _dialogPanel;
        [SerializeField] private Image _iconImage;

        private bool _isSkipped;
        private bool _isPrintEnded;

        public Image IconImage => _iconImage; 
        public Image DialogPanel => _dialogPanel; 
        public TextMeshProUGUI Name => _name; 
        public TMPWriter DialogWriter => _dialogWriter; 
        public TextMeshProUGUI DialogText => _dialogText;

        public bool IsPrintEnded { get => _isPrintEnded; set => _isPrintEnded = value; }
        public bool IsSkipped { get => _isSkipped; set => _isSkipped = value; }

        public void Initialize()
        {
            gameObject.SetActive(false);
            _iconImage.gameObject.SetActive(false);

            SL.Get<InputService>().OnDialogSkip += OnSkipText;
        }

        public async UniTask PrintText(string text, float speed)
        {
            _dialogText.text = text;
            _dialogWriter.DefaultDelays.delay = speed;
            _dialogWriter.StartWriter();

            _dialogWriter.OnFinishWriter.AddListener( x => _isPrintEnded = true);

            await UniTask.WaitUntil(() => _isPrintEnded && _isSkipped);

            Debug.Log($"Await dialog ended _isPrintEnded {_isPrintEnded}  _isSkipped  {_isSkipped}");

            _isSkipped = false;
            _isPrintEnded = false;
        }

        public void HideAll(float duration, Action onCompleted)
        {
            DOTween.Sequence()
                .Append(_dialogPanel.DOFade(0, duration))
                .Join(_iconImage.DOFade(0,duration))
                .Join(_name.DOFade(0, duration))
                .Join(_dialogText.DOFade(0, duration))
                .OnComplete(() => 
                {
                    onCompleted?.Invoke();
                    gameObject.SetActive(false);
                });
        }

        private void OnSkipText()
        {
            if (!gameObject.activeInHierarchy)
                return;

            if (!_isPrintEnded)
            {
                _dialogWriter.SkipWriter();
                return;
            }

            _isSkipped = true;
        }

        public void Dispose()
        {
            if (SL.Get<InputService>() == null)
                return;

            SL.Get<InputService>().OnDialogSkip -= OnSkipText;
        }
    }
}
