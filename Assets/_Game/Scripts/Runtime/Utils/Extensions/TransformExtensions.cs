using UnityEngine;

namespace Game.Runtime.Utils.Extensions
{
    public static class TransformExtensions
    {
        public static void MoveTowards(this Transform origin, Vector3 target, float speed)
        {
            origin.transform.position = Vector3.MoveTowards(origin.position, target, speed);
        }
    }
}