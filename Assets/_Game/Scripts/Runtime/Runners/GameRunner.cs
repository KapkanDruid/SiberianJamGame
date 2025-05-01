using Cysharp.Threading.Tasks;
using Game.Runtime.CMS;
using Game.Runtime.Services;
using Game.Runtime.Services.Audio;
using Game.Runtime.Services.Camera;
using Game.Runtime.Services.UI;
using UnityEngine;

namespace Game.Runtime.Runners
{
    public class GameRunner : MonoBehaviour
    {
        [SerializeField] private Camera gameCamera;
        
        private readonly Scope _gameScope = Scope.Game;
        
        private void Start()
        {
            RegisterCamera();
            RegisterServices();
            StartGame().Forget();
        }

        private void RegisterCamera()
        {
            SL.Get<CameraService>().RegisterCamera(gameCamera);
        }

        private void RegisterServices()
        {
            
        }

        private async UniTask StartGame()
        {
            SL.InitializeScope(_gameScope);
            await SL.Get<UIFaderService>().FadeOut();
        }

        private void OnDestroy()
        {
            SL.DisposeScope(_gameScope);
        }
    }
}