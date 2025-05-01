using UnityEngine;

namespace Runtime._Game.Scripts.Runtime.Utils.Helpers
{
    public static partial class Helpers
    {
        public static class RandomHelper
        {
            public static bool TryChance(double chance)
            {
                return Random.Range(0f, 1f) < chance;
            }
            
            public static int RandomDirection()
            {
                return TryChance(0.5f) ? -1 : 1;
            }
        }
    }
}