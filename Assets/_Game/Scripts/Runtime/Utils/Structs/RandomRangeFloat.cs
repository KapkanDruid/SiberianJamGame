using System;
using UnityEngine;

namespace Game.Runtime.Utils.Structs
{
    [Serializable]
    public class RandomRangeFloat
    {
        [SerializeField] private float minValue;
        [SerializeField] private float maxValue;

        public float Random()
        {
            return UnityEngine.Random.Range(minValue, maxValue);
        }

        public int IntRandom()
        {
            return (int)Random();
        }
    }
}