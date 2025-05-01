using System.Collections.Generic;
using UnityEngine;

namespace Game.Runtime.Utils.Helpers
{
    public static class ColliderHelper
    {
        public static int BySphereNonAlloc(Collider[] colliders,
            Vector3 position, float radius, LayerMask colliderMask = default) => 
            Physics.OverlapSphereNonAlloc(position, radius, colliders, colliderMask);
        
        public static int ByBoxNonAlloc(Collider[] colliders,
            Vector3 position, Vector2 size, Quaternion rotation, LayerMask colliderMask = default) => 
            Physics.OverlapBoxNonAlloc(position, new Vector3(size.x / 2, 0, size.y / 2), 
                colliders, rotation, colliderMask);
        
        public static Collider ByDistanceNonAlloc(Collider[] colliders, 
            Vector3 position, float radius, LayerMask colliderMask = default)
        {
            float minDistanceSqr = float.MaxValue;
            Collider nearestEnemy = null;
    
            var colliderCount = BySphereNonAlloc(colliders, position, radius, colliderMask);
            
            for (int i = 0; i < colliderCount; i++)
            {
                var collider = colliders[i];
                float distanceToEnemySqr = (position - collider.transform.position).sqrMagnitude;
                if (distanceToEnemySqr < minDistanceSqr)
                {
                    minDistanceSqr = distanceToEnemySqr;
                    nearestEnemy = collider;
                }
            }
    
            return nearestEnemy;
        }
        
        public static IEnumerable<Collider> ByFunNonAlloc(Collider[] colliders, 
            Vector3 origin, Vector3 direction, float radius, float angle, LayerMask colliderMask = default)
        {
            var colliderCount = BySphereNonAlloc(colliders, origin, radius, colliderMask);
            
            for (int i = 0; i < colliderCount; i++)
            {
                var collider = colliders[i];
                Vector3 directionToEnemy = (collider.transform.position - origin).normalized;

                if (Vector3.Angle(direction, directionToEnemy) <= angle / 2)
                {
                    yield return collider;
                }
            }
        }
    }
}