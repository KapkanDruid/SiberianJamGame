using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Runtime.Utils.Extensions;
using Game.Runtime.Utils.Helpers;
using TMPro;

namespace Game.Runtime.Services.UI
{
    public class UITextService : IService, IDisposable
    {
        private CancellationTokenSource _disposeTokenSource = new();
        
        public async UniTask Print(TMP_Text text, string message, CancellationToken externalToken = default, string fx = "wave", string audioEntityId = "")
        {
            if (!_disposeTokenSource.IsValid()) return;

            var visibleLength = Helpers.TextHelper.GetVisibleLength(message);
            if (visibleLength != 0)
            {
                var tokenSource = CancellationTokenSource.CreateLinkedTokenSource(_disposeTokenSource.Token, externalToken);

                for (var i = 0; i < visibleLength; i++)
                {
                    await UniTask.WaitForEndOfFrame(cancellationToken: tokenSource.Token)
                        .SuppressCancellationThrow();

                    if (tokenSource.IsValid())
                    {
                        text.text = $"<link={fx}>{Helpers.TextHelper.CutSmart(message, 1 + i)}</link>";
                    }
                }
            }
        }
        
        public async UniTask UnPrint(TMP_Text text, CancellationToken externalToken = default, string fx = "wave")
        {
            if (!_disposeTokenSource.IsValid()) return;
            
            var visibleLength = Helpers.TextHelper.GetVisibleLength(text.text);
            if (visibleLength != 0)
            {
                var tokenSource = CancellationTokenSource.CreateLinkedTokenSource(_disposeTokenSource.Token, externalToken);
                
                for (var i = visibleLength - 1; i >= 0; i--)
                {
                    await UniTask.WaitForEndOfFrame(cancellationToken: tokenSource.Token)
                        .SuppressCancellationThrow();

                    if (tokenSource.IsValid())
                    {
                        var str = Helpers.TextHelper.CutSmart(text.text, i);
                        text.text = $"<link={fx}>{str}</link>";
                    }
                }

                if (text != null)
                    text.text = "";
            }
        }

        public void Dispose()
        {
            _disposeTokenSource?.Dispose();
            _disposeTokenSource = null;
        }
    }
}