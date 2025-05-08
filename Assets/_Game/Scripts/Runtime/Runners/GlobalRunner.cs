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
            SL.Register<GameStateHolder>(new GameStateHolder(), _globalScope);
            SL.Register<ImplantsPoolService>(new ImplantsPoolService(), _globalScope);
            SL.Register<DialogController>(CreateDialogController(), _globalScope);
            SL.Register<Invoker>(new Invoker(), _globalScope);
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
            SL.Clear();
            _isRunning = false;
        }
    }
}