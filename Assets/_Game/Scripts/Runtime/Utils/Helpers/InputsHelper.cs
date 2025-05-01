using UnityEngine;

namespace Game.Runtime.Utils.Helpers
{
    public static partial class Helpers
    {
        public static class InputsHelper
        {
            public static Vector2 DirectionToMouse2DRaw(Vector2 from, Camera cam = null)
            {
                Vector2 mousePosition = MousePos2D(cam);
                return (mousePosition - from);
            }

            public static Vector2 DirectionToMouse2D(Vector2 from, Camera cam = null)
            {
                Vector2 mousePosition = MousePos2D(cam);
                return (mousePosition - from).normalized;
            }

            public static Vector3 MousePos2D(Camera cam = null)
            {
                cam ??= Camera.main;
                return cam.ScreenToWorldPoint(Input.mousePosition);
            }

            public static float AngleToMouse2D(Vector2 from, Camera cam = null)
            {
                return MathHelper.AngleToPos2D(from, MousePos2D(cam));
            }
            
            public static Vector3 MouseCameraOffset2D()
            {
                Vector2 mousePosition = Input.mousePosition;

                Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
                Vector2 offset = mousePosition - screenCenter;

                Vector2 normalizedOffset = new Vector2(offset.x / Screen.width, offset.y / Screen.height);

                return normalizedOffset;
            }

            public static Vector3 GetInputAxis2D()
            {
                var inputAxis2D = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
                return inputAxis2D.normalized;
            }
        }
    }
}