using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Game.Runtime.CMS;
using Game.Runtime.CMS.Components.Audio;
using Game.Runtime.CMS.Components.Configs;
using Game.Runtime.CMS.Components.Level;
using Game.Runtime.Gameplay.Enemy;
using Game.Runtime.Gameplay.HUD;
using Game.Runtime.Gameplay.Implants;
using Game.Runtime.Gameplay.Level;
using Game.Runtime.Gameplay.Warrior;
using Game.Runtime.Services;
using Game.Runtime.Services.Audio;
using Game.Runtime.Services.Camera;
using Game.Runtime.Services.Save;
using Game.Runtime.Services.UI;
using Game.Runtime.Utils;
using UnityEngine;

namespace Game.Runtime.Runners
{
    public class GameRunner : MonoBehaviour
    {
        [SerializeField] private Camera gameCamera;
        [SerializeField] private BattleController _battleController;
        [SerializeField] private WarriorView _warriorView;
        [SerializeField] private SpriteRenderer _backgroundRenderer;
        [SerializeField] private HUDService _hudService;
        [SerializeField] private Tutorial _tutorial;
        [SerializeField] private int _debugLevelIndex = -1;

        private bool _isDebug;
        
        private readonly Scope _gameScope = Scope.Game;

        private void Start()
        {
            RegisterCamera();
            RegisterServices();
            StartGame().Forget();
        }

        private void RegisterCamera()
        {
            ServiceLocator.Get<CameraService>().RegisterCamera(gameCamera);
        }

        private void RegisterServices()
        {
            ServiceLocator.Register<HUDService>(_hudService, _gameScope);
            ServiceLocator.Register<ImplantsHolderService>(new ImplantsHolderService(), _gameScope);
            ServiceLocator.Register<InventoryService>(new InventoryService(), _gameScope);
            ServiceLocator.Register<LootService>(new LootService(), _gameScope);
            ServiceLocator.Register<WarriorController>(new WarriorController(), _gameScope);
            ServiceLocator.Register<BattleController>(_battleController, _gameScope);
            ServiceLocator.Register<Tutorial>(_tutorial, _gameScope);

            ConfigureLevel();
        }
        
        private async UniTask StartGame()
        {
            ServiceLocator.InitializeScope(_gameScope);
            ServiceLocator.Get<HUDService>().DisableUI.SetActive(true);
            ServiceLocator.Get<HUDService>().EndTurnButtonParent.SetActive(ServiceLocator.Get<GameStateHolder>().CurrentData.Level > 0);
            ServiceLocator.Get<HUDService>().InventoryView.SetActive(true);
            ServiceLocator.Get<DialogController>().Background.gameObject.SetActive(false);
            ServiceLocator.Get<ImplantsHolderService>().SpawnImplants();
            ServiceLocator.Get<HUDService>().DisableUI.SetActive(false);
            await ServiceLocator.Get<UIFaderService>().FadeOut();
        }

        private void ConfigureLevel()
        {
            var currentLevelIndex = GetCurrentLevelIndex();
            var levelModel = LevelHelper.GetLevelModel(currentLevelIndex);
            var levelComponent = levelModel.GetComponent<LevelComponent>();

            RegisterCharacter(levelModel, currentLevelIndex);
            RegisterEnemy(levelModel, levelComponent);
            ConfigureImplants(levelModel);
            ConfigureLevelEnvironment(levelModel, levelComponent);
            RegisterLevelCheckpoint(levelModel, currentLevelIndex);

            LogUtil.Log(nameof(GameRunner), $"Level {currentLevelIndex} loaded!");
        }
        
        private int GetCurrentLevelIndex()
        {
            if (ServiceLocator.Get<GameStateHolder>().NeedRespawnOnCheckpoint)
                ServiceLocator.Get<GameStateHolder>().LoadCheckpoint();
            
            _isDebug = _debugLevelIndex >= 0;
            if (_isDebug)
            {
                ServiceLocator.Get<GameStateHolder>().CurrentData.Level = _debugLevelIndex;
                return _debugLevelIndex;
            }

            return ServiceLocator.Get<GameStateHolder>().CurrentData.Level;
        }

        private void RegisterCharacter(CMSEntity levelModel, int currentLevelIndex)
        {
            if (levelModel.Is<SetCharacterHealthComponent>(out var healthComponent))
            {
                ServiceLocator.Get<GameStateHolder>().CurrentData.CharacterHealth = healthComponent.Health;
            }
            else if (currentLevelIndex == 0 || _isDebug)
            {
                var playerConfig = CM.Get(CMs.Configs.PlayerConfig).GetComponent<PlayerConfig>();
                ServiceLocator.Get<GameStateHolder>().CurrentData.CharacterHealth = playerConfig.MaxHealth;
            }
            
            ServiceLocator.Register<WarriorView>(_warriorView, _gameScope);
        }

        private void RegisterEnemy(CMSEntity levelModel, LevelComponent levelComponent)
        {
            if (levelModel.Is<BossLevelComponent>(out var bossComponent))
            {
                var boss = new BossController(bossComponent.Health, bossComponent.Heal, bossComponent.BossViewPrefab);
                ServiceLocator.Register<IEnemy>(boss, _gameScope);
                ServiceLocator.Register<BossController>(boss, _gameScope);
                _battleController.CurrentBattleType = BattleController.BattleType.Boss;
                _battleController.BossConfig = bossComponent;
            }
            else
            {
                var enemy = new EnemyController(CM.Get(levelComponent.EnemyPrefab.EntityId));

                ServiceLocator.Register<IEnemy>(enemy, _gameScope);
                ServiceLocator.Register<EnemyController>(enemy, _gameScope);
                _battleController.CurrentBattleType = BattleController.BattleType.Common;
            }
        }

        private void ConfigureImplants(CMSEntity levelModel)
        {
            if (levelModel.Is<AddImplantsToPoolAtStartComponent>(out var component))
            {
                foreach (var implant in component.ImplantsPrefabs)
                    ServiceLocator.Get<GameStateHolder>().CurrentData.ImplantsPool.Add(implant.EntityId);
            }

            ServiceLocator.Get<ImplantsPoolService>().CachePool();
        }

        private void ConfigureLevelEnvironment(CMSEntity levelModel, LevelComponent levelComponent)
        {
            ServiceLocator.Get<AudioService>().Play(levelModel);
            _backgroundRenderer.sprite = levelComponent.BackgroundSprite;
            if (levelModel.Is<LevelParticleComponent>(out var particleComponent))
                Instantiate(particleComponent.Particle);
        }
        
        private void RegisterLevelCheckpoint(CMSEntity levelModel, int currentLevelIndex)
        {
            var checkPointData = ServiceLocator.Get<GameStateHolder>().CheckpointData;
            if (checkPointData.Level >= currentLevelIndex)
                return;
            
            if (levelModel.Is<LevelCheckpointComponent>(out _))
            {
                var gameStateHolder = ServiceLocator.Get<GameStateHolder>();
                var playerConfig = CM.Get(CMs.Configs.PlayerConfig).GetComponent<PlayerConfig>();

                checkPointData.Level = currentLevelIndex;
                checkPointData.CharacterHealth =
                    gameStateHolder.CurrentData.CharacterHealth >= playerConfig.MaxHealth / 2
                        ? gameStateHolder.CurrentData.CharacterHealth
                        : playerConfig.MaxHealth / 2;
                checkPointData.ImplantsPool.Clear(); 
                checkPointData.ImplantsPool.AddRange(gameStateHolder.CurrentData.ImplantsPool);
                
                LogUtil.Log(nameof(GameRunner), $"Level checkpoint - {gameStateHolder.CheckpointData.Level}! " +
                                                $"Character health - {gameStateHolder.CheckpointData.CharacterHealth}, " +
                                                $"Count static implant pool - {gameStateHolder.CheckpointData.ImplantsPool.Count}");
            }
        }

        private void OnDestroy()
        {
            ServiceLocator.DisposeScope(_gameScope);
        }
    }
}