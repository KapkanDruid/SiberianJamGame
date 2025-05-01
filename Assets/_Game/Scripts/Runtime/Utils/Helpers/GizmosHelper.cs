using UnityEngine;

namespace Game.Runtime.Utils.Helpers
{
    public static partial class Helpers
    {
        public static class GizmosHelper
        {
            public static void DrawCircle(Vector3 center, float radius)
            {
                int numPoints = 100;
                float angleStep = 360.0f / numPoints;

                Vector3 prevPoint = center + new Vector3(radius, 0, 0);
                Vector3 nextPoint = Vector3.zero;

                for (int i = 1; i <= numPoints; i++)
                {
                    nextPoint.x = center.x + Mathf.Cos(Mathf.Deg2Rad * angleStep * i) * radius;
                    nextPoint.z = center.z + Mathf.Sin(Mathf.Deg2Rad * angleStep * i) * radius;

                    Gizmos.DrawLine(prevPoint, nextPoint);

                    prevPoint = nextPoint;
                }
            }

            public static void DrawSquare(Vector3 center, Vector2 size, Quaternion rotation)
            {
                Vector3 right = rotation * Vector3.right * size.x / 2f;
                Vector3 forward = rotation * Vector3.forward * size.y / 2f;

                Vector3 p1 = center + right + forward;
                Vector3 p2 = center - right + forward;
                Vector3 p3 = center - right - forward;
                Vector3 p4 = center + right - forward;

                Gizmos.DrawLine(p1, p2);
                Gizmos.DrawLine(p2, p3);
                Gizmos.DrawLine(p3, p4);
                Gizmos.DrawLine(p4, p1);
            }

            public static void DrawFan(Vector3 origin, Vector3 direction, float radius, float angle)
            {
                int segments = 100;

                Quaternion leftRotation = Quaternion.AngleAxis(-angle / 2, Vector3.up);
                Quaternion rightRotation = Quaternion.AngleAxis(angle / 2, Vector3.up);

                Vector3 leftDirection = leftRotation * direction;
                Vector3 rightDirection = rightRotation * direction;

                Gizmos.DrawRay(origin, leftDirection * radius);
                Gizmos.DrawRay(origin, rightDirection * radius);

                Vector3 previousPoint = origin + rightDirection * radius;
                for (int i = 1; i <= segments; i++)
                {
                    float rotationStep = (float)i / segments;
                    Quaternion rotation = Quaternion.Lerp(rightRotation, leftRotation, rotationStep);

                    Vector3 directionPoint = rotation * direction;
                    Vector3 point = origin + directionPoint * radius;

                    Gizmos.DrawLine(previousPoint, point);

                    previousPoint = point;
                }
            }
        }
    }
}