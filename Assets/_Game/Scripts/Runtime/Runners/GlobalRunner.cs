using Game.Runtime.CMS;
using Game.Runtime.CMS.Components.Commons;
using Game.Runtime.Gameplay.Implants;
using Game.Runtime.Gameplay.Level;
using Game.Runtime.Services;
using Game.Runtime.Services.Audio;
using Game.Runtime.Services.Camera;
using Game.Runtime.Services.Input;
using Game.Runtime.Services.Save;
using Game.Runtime.Services.Timer;
using Game.Runtime.Services.UI;
using Game.Runtime.Utils;
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
            LogUtil.Log(nameof(GlobalRunner), "ENTRY POINT!", Color.green);
            
            CM.Reload();
            RegisterServices();
            ServiceLocator.InitializeScope(_globalScope);
        }

        private void RegisterServices()
        {
            ServiceLocator.Register<SaveService>(new SaveService(), _globalScope);
            ServiceLocator.Register<AudioService>(new AudioService(), _globalScope);
            ServiceLocator.Register<TimerService>(new TimerService(), _globalScope);
            ServiceLocator.Register<UITextService>(new UITextService(), _globalScope);
            ServiceLocator.Register<UIFaderService>(new UIFaderService(), _globalScope);
            ServiceLocator.Register<CameraService>(new CameraService(), _globalScope);
            ServiceLocator.Register<InputService>(new InputService(), _globalScope);
            ServiceLocator.Register<GameStateHolder>(new GameStateHolder(), _globalScope);
            ServiceLocator.Register<ImplantsPoolService>(new ImplantsPoolService(), _globalScope);
            ServiceLocator.Register<DialogController>(CreateDialogController(), _globalScope);
            ServiceLocator.Register<Invoker>(new Invoker(), _globalScope);
        }

        private DialogController CreateDialogController()
        {
            var prefab = CM.Get(CMs.Configs.DialogReference).GetComponent<PrefabComponent>().Prefab;
            var DialogyObject = Instantiate(prefab);
            DontDestroyOnLoad(DialogyObject);
            
            return DialogyObject.GetComponentInChildren<DialogController>();
        }

        private void OnDestroy()
        {
            ServiceLocator.Clear();
            _isRunning = false;
        }
    }
}