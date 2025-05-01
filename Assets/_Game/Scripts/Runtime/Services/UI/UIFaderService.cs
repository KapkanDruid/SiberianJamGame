using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Runtime._Game.Scripts.Runtime.Utils.Extensions;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Runtime._Game.Scripts.Runtime.Services.UI
{
    public class UIFaderService : IService, IDisposable
    {
        private readonly float _fadeDuration = 1f;
        private readonly Image _fadeImage;
        
        private  CancellationTokenSource _disposeTokenSource = new();

        public UIFaderService()
        {
            var canvas = new GameObject(nameof(UIFaderService)).AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 9;

            _fadeImage = new GameObject("FadeImage").AddComponent<Image>();
            _fadeImage.transform.SetParent(canvas.transform, false);
            _fadeImage.rectTransform.anchoredPosition = Vector2.zero;
            _fadeImage.rectTransform.sizeDelta = new Vector2(5000, 3000);

            _fadeImage.color = new Color(0f, 0f, 0f, 1f);
        
            Object.DontDestroyOnLoad(canvas.gameObject);
        }
        
        public async UniTask FadeIn()
        {
            if (!_disposeTokenSource.IsValid()) return;

            await Fade(0f, 1f);
        }

        public async UniTask FadeOut()
        {
            if (!_disposeTokenSource.IsValid()) return;

            await Fade(1f, 0f);
        }
        
        private async UniTask Fade(float startAlpha, float endAlpha)
        {
            float elapsed = 0f;
            Color fadeColor = _fadeImage.color;

            while (elapsed < _fadeDuration && _disposeTokenSource.IsValid())
            {
                await UniTask.Yield(cancellationToken: _disposeTokenSource.Token).SuppressCancellationThrow();

                if (_disposeTokenSource.IsValid())
                {
                    elapsed += Time.deltaTime;
                    
                    fadeColor.a = Mathf.Lerp(startAlpha, endAlpha, elapsed / _fadeDuration);
                    _fadeImage.color = fadeColor;
                }
            }

            if (_fadeImage != null)
            {
                fadeColor.a = endAlpha;
                _fadeImage.color = fadeColor;
            }
        }
        
        public void Dispose()
        {
            _disposeTokenSource?.Dispose();
            _disposeTokenSource = null;
        }
    }
}