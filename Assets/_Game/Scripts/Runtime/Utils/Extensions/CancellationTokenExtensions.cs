using System.Threading;

namespace Runtime._Game.Scripts.Runtime.Utils.Extensions
{
    public static class CancellationTokenExtensions
    {
        public static bool IsValid(this CancellationTokenSource tokenSource)
        {
            return tokenSource != null && !tokenSource.IsCancellationRequested;
        }
    }
}