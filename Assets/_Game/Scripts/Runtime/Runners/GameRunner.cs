using Cysharp.Threading.Tasks;
using Game.Runtime.CMS;
using Game.Runtime.CMS.Components.Level;
using Game.Runtime.Gameplay.Enemy;
using Game.Runtime.Gameplay.HUD;
using Game.Runtime.Gameplay.ImplantsPool;
using Game.Runtime.Gameplay.Inventory;
using Game.Runtime.Gameplay.Level;
using Game.Runtime.Gameplay.Warrior;
using Game.Runtime.Services;
using Game.Runtime.Services.Camera;
using Game.Runtime.Services.Save;
using Game.Runtime.Services.UI;
using UnityEngine;

namespace Game.Runtime.Runners
{
    public class GameRunner : MonoBehaviour
    {
        [SerializeField] private Camera gameCamera;
        [SerializeField] private BattleController _battleController;
        [SerializeField] private WarriorView _warriorView;
        [SerializeField] private SpriteRenderer _backgroundRenderer;

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
            SL.Register<ImplantsPool>(new ImplantsPool(), _gameScope);
            SL.Register<InventoryService>(new InventoryService(), _gameScope);
            SL.Register<WarriorController>(new WarriorController(), _gameScope);
            SL.Register<BattleController>(_battleController, _gameScope);
            
            ConfigureLevel();
        }

        private async UniTask StartGame()
        {
            SL.InitializeScope(_gameScope);
            await SL.Get<UIFaderService>().FadeOut();
        }

        private void ConfigureLevel()
        {
            var currentLevelIndex = SL.Get<SaveService>().SaveData.LevelIndex;
            var levelModel = LevelHelper.GetLevelModel(currentLevelIndex);

            if (levelModel == null)
            {
                Debug.LogError($"Level {currentLevelIndex} not found");
                return;
            }
            
            var levelComponent = levelModel.GetComponent<LevelComponent>();
            _backgroundRenderer.sprite = levelComponent.BackgroundSprite;

            SL.Register<EnemyController>( new EnemyController(CM.Get(levelComponent.EnemyPrefab.EntityId)), _gameScope);
            SL.Register<WarriorView>(_warriorView, _gameScope);
            
            Debug.Log($"[GameRunner] Level loaded!");
        }
        
        private void OnDestroy()
        {
            SL.DisposeScope(_gameScope);
        }
    }
}