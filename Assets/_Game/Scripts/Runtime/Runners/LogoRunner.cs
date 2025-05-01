using Cysharp.Threading.Tasks;
using Runtime._Game.Scripts.Runtime.Services;
using Runtime._Game.Scripts.Runtime.Services.Camera;
using Runtime._Game.Scripts.Runtime.Utils.Сonstants;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Runtime._Game.Scripts.Runtime.Runners
{
    public class LogoRunner : MonoBehaviour
    {
        [SerializeField] private Camera logoCamera;
        
        private void Start()
        {
            RegisterCamera();
            ShowLogo().Forget();
        }

        private void RegisterCamera()
        {
            ServiceLocator.GetService<CameraService>().RegisterCamera(logoCamera);
        }

        private async UniTask ShowLogo()
        {
            await SceneManager.LoadSceneAsync(Const.ScenesConst.GameReleaseScene);
        }
    }
}