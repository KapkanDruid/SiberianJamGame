using Game.Runtime.CMS;
using Game.Runtime.Services;
using Game.Runtime.Services.Audio;
using Game.Runtime.Services.Camera;
using Game.Runtime.Services.Input;
using Game.Runtime.Services.Save;
using Game.Runtime.Services.Timer;
using Game.Runtime.Services.UI;
using UnityEngine;

namespace Game.Runtime.Runners
{
    public class GlobalRunner : MonoBehaviour
    {
        private static bool _isRunning;
        private static readonly Scope _globalScope = Scope.Global;

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
            CM.Reload();
            RegisterServices();
            SL.InitializeScope(_globalScope);
        }

        private void RegisterServices()
        {
            SL.Register<SaveService>(new SaveService(), _globalScope);
            SL.Register<AudioService>(new AudioService(), _globalScope);
            SL.Register<TimerService>(new TimerService(), _globalScope);
            SL.Register<UITextService>(new UITextService(), _globalScope);
            SL.Register<UIFaderService>(new UIFaderService(), _globalScope);
            SL.Register<CameraService>(new CameraService(), _globalScope);
            SL.Register<InputService>(new InputService(), _globalScope);
        }

        private void OnDestroy()
        {
            SL.Clear();
            _isRunning = false;
        }
    }
}