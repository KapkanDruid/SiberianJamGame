using Cysharp.Threading.Tasks;
using Game.Runtime.CMS;
using Game.Runtime.CMS.Components.Gameplay;
using Game.Runtime.Gameplay.HUD;
using Game.Runtime.Gameplay.Unit;
using Game.Runtime.Services;
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
            SL.Register<HUDService>(new HUDService(), _gameScope);
        }

        private async UniTask StartGame()
        {
            SL.InitializeScope(_gameScope);
            TestBrain();
            await SL.Get<UIFaderService>().FadeOut();
        }

        private void TestBrain()
        {
            var testBrain = CM.Get(CMs.Gameplay.TestUnit).GetComponent<UnitComponent>();
            var brainController = new BrainController();
            brainController.SetBrainGrid(testBrain.BrainGrid);
        }

        private void OnDestroy()
        {
            SL.DisposeScope(_gameScope);
        }
    }
}