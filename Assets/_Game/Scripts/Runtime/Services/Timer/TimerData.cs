using System;
using System.Threading;

namespace Runtime._Game.Scripts.Runtime.Services.Timer
{
    public class TimerData
    {
        public float Duration;
        public Action OnComplete;
        public bool Loop;
        public CancellationTokenSource TimerTokenSource;
    }
}