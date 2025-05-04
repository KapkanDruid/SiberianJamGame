using DG.Tweening;
using UnityEngine;

namespace Game.Runtime.Services.Camera
{
    public class CameraService : IService
    {
        public UnityEngine.Camera Camera { get; private set; }

        public void RegisterCamera(UnityEngine.Camera camera)
        {
            Camera = camera;
        }

        public void Shake(float duration, float strength)
        {
            Camera.DOShakePosition(duration, strength, 10, 45f);
        }

        public void UIShake()
        {
            Shake(0.025f, 0.2f);
        }
    }
}