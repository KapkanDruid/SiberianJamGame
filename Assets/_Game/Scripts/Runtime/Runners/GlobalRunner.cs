using Runtime._Game.Scripts.Runtime.Services;
using Runtime._Game.Scripts.Runtime.Services.Camera;
using Runtime._Game.Scripts.Runtime.Services.Input;
using Runtime._Game.Scripts.Runtime.Services.Save;
using Runtime._Game.Scripts.Runtime.Services.Timer;
using Runtime._Game.Scripts.Runtime.Services.UI;
using UnityEngine;

namespace Runtime._Game.Scripts.Runtime.Runners
{
    public class GlobalRunner : MonoBehaviour
    {
        private static bool _isRunning;
        private static readonly ServiceScope _globalScope = ServiceScope.Global;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void AutoInstantiate()
        {
            if (!_isRunning)
            {
                GameObject servicedMain = new GameObject(nameof(GlobalRunner));
                servicedMain.AddComponent<GlobalRunner>();
                
                DontDestroyOnLoad(servicedMain);
                _isRunning = true;
            }
        }
        
        private void Awake()
        {
            Debug.Log("[GlobalRunner] Entry point!");
            RegisterServices();
            ServiceLocator.InitializeServices(_globalScope);
        }

        private void RegisterServices()
        {
            ServiceLocator.RegisterService<SaveService>(new SaveService(), _globalScope);
            ServiceLocator.RegisterService<TimerService>(new TimerService(), _globalScope);
            ServiceLocator.RegisterService<UITextService>(new UITextService(), _globalScope);
            ServiceLocator.RegisterService<UIFaderService>(new UIFaderService(), _globalScope);
            ServiceLocator.RegisterService<CameraService>(new CameraService(), _globalScope);
            ServiceLocator.RegisterService<InputService>(new InputService(), _globalScope);
        }

        private void OnDestroy()
        {
            ServiceLocator.Clear();
            _isRunning = false;
        }
    }
}