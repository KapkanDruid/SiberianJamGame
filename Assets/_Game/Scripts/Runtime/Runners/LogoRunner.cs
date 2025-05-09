using Cysharp.Threading.Tasks;
using Game.Runtime.CMS;
using Game.Runtime.Services;
using Game.Runtime.Services.Audio;
using Game.Runtime.Services.Camera;
using Game.Runtime.Utils.Consts;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Runtime.Runners
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
            ServiceLocator.Get<CameraService>().RegisterCamera(logoCamera);
        }

        private async UniTask ShowLogo()
        {
            ServiceLocator.Get<AudioService>().Play(CMs.Audio.AmbientTest);
            ServiceLocator.Get<AudioService>().Play(CMs.Audio.SFX.SFXTest);
            await SceneManager.LoadSceneAsync(Const.ScenesConst.GameReleaseScene);
        }
    }
}