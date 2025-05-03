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

        [SerializeField] private Image _rightImage;
        [SerializeField] private Image _leftImage;

        [SerializeField] private Image _background;
        public Image Background => _background;

        public enum PositionType
        {
            Right,
            Left
        }

        private bool _isSkipped;
        private bool _isPrintEnded;

        private Color _panelColor;

        public Image DialogPanel => _dialogPanel; 
        public TextMeshProUGUI Name => _name; 
        public TMPWriter DialogWriter => _dialogWriter; 
        public TextMeshProUGUI DialogText => _dialogText;

        public bool IsPrintEnded { get => _isPrintEnded; set => _isPrintEnded = value; }
        public bool IsSkipped { get => _isSkipped; set => _isSkipped = value; }
        public Color PanelColor { get => _panelColor; set => _panelColor = value; }

        public void Initialize()
        {
            gameObject.SetActive(false);
            _leftImage.gameObject.SetActive(false);
            _rightImage.gameObject.SetActive(false);
            Background.gameObject.SetActive(false);
            _panelColor = _dialogPanel.color;

            SL.Get<InputService>().OnDialogSkip += OnSkipText;
        }

        public async UniTask PrintText(string text, float speed)
        {
            _dialogText.text = text;
            _dialogWriter.DefaultDelays.delay = speed;
            _dialogWriter.StartWriter();

            _dialogWriter.OnFinishWriter.AddListener( x => _isPrintEnded = true);

            await UniTask.WaitUntil(() => _isPrintEnded && _isSkipped);

            _isSkipped = false;
            _isPrintEnded = false;
        }

        public async UniTask ShowIcon(PositionType positionType, Sprite sprite, Vector2 offset, float duration)
        {
            Image image = null;

            if (positionType == PositionType.Left)
                image = _leftImage;
            else if (positionType == PositionType.Right)
                image = _rightImage;

            image.sprite = sprite;
            image.rectTransform.anchoredPosition = offset;
            image.SetNativeSize();
            image.rectTransform.localScale = Vector3.one * 0.6f;

            var color = image.color;
            color.a = 0;
            image.color = color;
            image.gameObject.SetActive(true);

            await image.DOFade(1, duration).AsyncWaitForCompletion();
        }

        public void HideAll(float duration, Action onCompleted)
        {
            var textColor = _dialogText.color;
            var nameColor = _name.color;

            DOTween.Sequence()
                .Append(_dialogPanel.DOFade(0, duration))
                .Join(_rightImage.DOFade(0,duration))
                .Join(_leftImage.DOFade(0, duration))
                .Join(_name.DOFade(0, duration))
                .Join(_dialogText.DOFade(0, duration))
                .OnComplete(() => 
                {
                    onCompleted?.Invoke();
                    gameObject.SetActive(false);
                    _leftImage.gameObject.SetActive(false);
                    _rightImage.gameObject.SetActive(false);

                    _dialogText.color = textColor;
                    _dialogText.text = " ";

                    _name.color = nameColor;
                    _name.text = " ";
                });
        }

        public void HideIcon(PositionType positionType, float duration)
        {
            Image image = null;

            if (positionType == PositionType.Left)
                image = _leftImage;
            else if (positionType == PositionType.Right)
                image = _rightImage;

            image.DOFade(0, duration);
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
