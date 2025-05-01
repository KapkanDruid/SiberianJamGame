using Cysharp.Threading.Tasks;
using Runtime._Game.Scripts.Runtime.Services;
using Runtime._Game.Scripts.Runtime.Services.Camera;
using Runtime._Game.Scripts.Runtime.Services.UI;
using UnityEngine;

namespace Runtime._Game.Scripts.Runtime.Runners
{
    public class GameRunner : MonoBehaviour
    {
        [SerializeField] private Camera gameCamera;
        
        private readonly ServiceScope _gameScope = ServiceScope.Game;
        
        private void Start()
        {
            RegisterCamera();
            RegisterServices();
            StartGame().Forget();
        }

        private void RegisterCamera()
        {
            ServiceLocator.GetService<CameraService>().RegisterCamera(gameCamera);
        }

        private void RegisterServices()
        {
            
        }

        private async UniTask StartGame()
        {
            ServiceLocator.InitializeServices(_gameScope);
            await ServiceLocator.GetService<UIFaderService>().FadeOut();
        }

        private void OnDestroy()
        {
            ServiceLocator.DisposeScope(_gameScope);
        }
    }
}