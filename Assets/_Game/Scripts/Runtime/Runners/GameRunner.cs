using Cysharp.Threading.Tasks;
using Game.Runtime.CMS;
using Game.Runtime.CMS.Components.Commons;
using Game.Runtime.CMS.Components.Configs;
using Game.Runtime.Gameplay;
using Game.Runtime.Services;
using Game.Runtime.Services.Audio;
using Game.Runtime.Services.Camera;
using Game.Runtime.Services.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Runtime.Runners
{
    public class GameRunner : MonoBehaviour
    {
        [SerializeField] private Camera gameCamera;
        [SerializeField] private BattleController _battleController;
        [SerializeField] private WarriorView _warriorView;

        [Header("enemyUI")]
        [SerializeField] private RectTransform _healthParent;
        [SerializeField] private Image _healthBar;
        [SerializeField] private TextMeshProUGUI _healthText;

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
            SL.Register<WarriorController>(new WarriorController(), _gameScope);
            SL.Register<BattleController>(_battleController, _gameScope);

            var enemy = CreateEnemy();

            SL.Register<IEnemy>(enemy, _gameScope);
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

            return new EnemyController(configByIndex, new EnemyViewData(_healthParent, _healthBar, _healthText), prefabByIndex);
        }

        private void OnDestroy()
        {
            SL.DisposeScope(_gameScope);
        }
    }
}