using UnityEngine;

namespace Game.Runtime.Utils.Extensions
{
    public static class VectorExtensions
    {
        public static Vector2 XZ(this Vector3 vec3D)
        {
            return new Vector2(vec3D.x, vec3D.z);
        }

        public static Vector2 XY(this Vector3 vec3D)
        {
            return new Vector2(vec3D.x, vec3D.y);
        }
    }
}