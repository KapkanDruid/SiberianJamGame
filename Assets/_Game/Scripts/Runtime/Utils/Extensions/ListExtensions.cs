using System.Collections.Generic;

namespace Game.Runtime.Utils.Extensions
{
    public static class ListExtensions
    {
        public static T GetRandom<T>(this IList<T> list, bool ignoreEmpty = true)
        {
            if (list == null || list.Count == 0)
            {
                if (!ignoreEmpty)
                    UnityEngine.Debug.LogError("The list cannot be null or empty.");
            
                return default;
            }

            int index = UnityEngine.Random.Range(0, list.Count);
            return list[index];
        }
    
        public static void Shuffle<T>(this IList<T> list)
        {
            System.Random rng = new System.Random();
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                (list[k], list[n]) = (list[n], list[k]);
            }
        }

        public static T Pop<T>(this IList<T> list)
        {
            if (list.Count == 0)
                throw new System.InvalidOperationException("Cannot pop from an empty list.");

            int lastIndex = list.Count - 1;
            T item = list[lastIndex];
            list.RemoveAt(lastIndex);
            return item;
        }
    }
}