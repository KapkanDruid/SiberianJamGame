using System.Collections.Generic;
using UnityEngine;

namespace Game.Runtime.Utils.Helpers
{
    public static partial class Helpers
    {
        public static class TransformHelper
        {
            public static Quaternion RandomRotation2D()
            {
                return Quaternion.Euler(0, 0, Random.Range(0, 360f));
            }

            public static bool IsColliding(Vector3 transformPosition, Vector3 position, float f)
            {
                return Vector2.Distance(transformPosition, position) < f;
            }

            public static T IsColliding<T>(Vector3 transformPosition, List<T> any, float f) where T : Component
            {
                foreach (var e in any)
                    if (Vector2.Distance(transformPosition, e.transform.position) < f)
                        return e;
                return default;
            }

            public static (Transform, float) FindClosest<T>(List<T> transforms, Vector2 pos) where T : Component
            {
                Transform closestTransform = null;
                float minDistance = float.MaxValue;

                foreach (T t in transforms)
                {
                    float distance = Vector2.Distance(t.transform.position, pos);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        closestTransform = t.transform;
                    }
                }

                return (closestTransform, minDistance);
            }
        }
    }
}