using System.Threading;

namespace Game.Runtime.Utils.Extensions
{
    public static class CancellationTokenExtensions
    {
        public static bool IsValid(this CancellationTokenSource tokenSource)
        {
            return tokenSource != null && !tokenSource.IsCancellationRequested;
        }
    }
}