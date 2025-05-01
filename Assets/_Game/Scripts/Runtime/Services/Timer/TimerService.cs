using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Runtime.Utils.Extensions;

namespace Game.Runtime.Services.Timer
{
    public class TimerService : IService, IDisposable
    {
        private readonly List<TimerData> _activeTimers = new();
        
        private CancellationTokenSource _disposeTokenSource = new();

        public TimerData StartTimer(float duration, Action onComplete = null, bool loop = false)
        {
            if (!_disposeTokenSource.IsValid()) return null;

            var timerData = new TimerData()
            {
                Duration = duration,
                OnComplete = onComplete,
                Loop = loop,
                TimerTokenSource = CancellationTokenSource.CreateLinkedTokenSource(_disposeTokenSource.Token)
            };
            
            _activeTimers.Add(timerData);
            RunTimerAsync(timerData).Forget();

            return timerData;
        }

        public async UniTask WaitTimerAsync(float duration)
        {
            if (!_disposeTokenSource.IsValid()) return;
            
            var timerToken = CancellationTokenSource.CreateLinkedTokenSource(_disposeTokenSource.Token);
            await UniTask.Delay(TimeSpan.FromSeconds(duration), DelayType.DeltaTime, PlayerLoopTiming.Update, timerToken.Token);
        }

        public void StopTimer(TimerData timerData)
        {
            timerData.TimerTokenSource?.Dispose();
            timerData.TimerTokenSource = null;
            _activeTimers.Remove(timerData);
        }

        private async UniTaskVoid RunTimerAsync(TimerData timerData)
        {
            while (timerData.TimerTokenSource.IsValid())
            {
                await UniTask.Delay(TimeSpan.FromSeconds(timerData.Duration), 
                    DelayType.DeltaTime, PlayerLoopTiming.Update, timerData.TimerTokenSource.Token).SuppressCancellationThrow();

                if (timerData.TimerTokenSource.IsValid())
                {
                    timerData.OnComplete?.Invoke();
                    if (!timerData.Loop) break;
                }
            }

            StopTimer(timerData);
        }

        public void Dispose()
        {
            _disposeTokenSource?.Dispose();
            _disposeTokenSource = null;

            foreach (var timerData in _activeTimers)
            {
                timerData.TimerTokenSource?.Dispose();
                timerData.TimerTokenSource = null;
            }
            _activeTimers.Clear();
        }
    }
}