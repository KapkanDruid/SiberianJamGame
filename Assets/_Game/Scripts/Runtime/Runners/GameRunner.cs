using Cysharp.Threading.Tasks;
using Game.Runtime.CMS;
using Game.Runtime.CMS.Components.Configs;
using Game.Runtime.Gameplay;
using Game.Runtime.Gameplay.Enemy;
using Game.Runtime.Gameplay.HUD;
using Game.Runtime.Gameplay.ImplantsPool;
using Game.Runtime.Gameplay.Inventory;
using Game.Runtime.Gameplay.Warrior;
using Game.Runtime.Services;
using Game.Runtime.Services.Camera;
using Game.Runtime.Services.UI;
using UnityEngine;

namespace Game.Runtime.Runners
{
    public class GameRunner : MonoBehaviour
    {
        [SerializeField] private Camera gameCamera;
        [SerializeField] private BattleController _battleController;
        [SerializeField] private WarriorView _warriorView;

        private int _currentLevelIndex;

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

            var enemy = CreateEnemy();

            SL.Register<EnemyController>(enemy, _gameScope);
            SL.Register<WarriorView>(_warriorView, _gameScope);
        }

        private async UniTask StartGame()
        {
            SL.InitializeScope(_gameScope);
            await SL.Get<UIFaderService>().FadeOut();
        }

        private EnemyController CreateEnemy()
        {
            var listConfigEntity = CM.Get(CMs.Configs.EnemiesByLevel);
            var listConfig = listConfigEntity.GetComponent<EnemyPrefabs>().EnemyConfigPrefabs;

            var entityByIndex = CM.Get(listConfig[_currentLevelIndex].EntityId);

            var prefabByIndex = entityByIndex.GetComponent<EnemyPrefabComponent>().EnemyView;
            var configByIndex = entityByIndex.GetComponent<EnemyConfig>();

            return new EnemyController(configByIndex, prefabByIndex);
        }

        private void OnDestroy()
        {
            SL.DisposeScope(_gameScope);
        }
    }
}