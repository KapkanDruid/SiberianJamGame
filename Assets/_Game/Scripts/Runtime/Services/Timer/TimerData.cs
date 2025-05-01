using System;
using System.Threading;

namespace Game.Runtime.Services.Timer
{
    public class TimerData
    {
        public float Duration;
        public Action OnComplete;
        public bool Loop;
        public CancellationTokenSource TimerTokenSource;
    }
}