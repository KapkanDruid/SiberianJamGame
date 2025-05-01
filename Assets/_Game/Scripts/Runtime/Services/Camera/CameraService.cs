using DG.Tweening;
using UnityEngine;

namespace Game.Runtime.Services.Camera
{
    public class CameraService : IService
    {
        private UnityEngine.Camera _camera;

        public void RegisterCamera(UnityEngine.Camera camera)
        {
            _camera = camera;
            Debug.Log($"[Service Locator] Set camera: {_camera}");
        }

        public void Shake(float duration, float strength)
        {
            _camera.DOShakePosition(duration, strength, 10, 45f);
        }

        public void UIShake()
        {
            Shake(0.025f, 0.2f);
        }
    }
}