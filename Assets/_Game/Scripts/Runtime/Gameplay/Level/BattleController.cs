using Cysharp.Threading.Tasks;
using Game.Runtime.CMS;
using Game.Runtime.Services;
using Game.Runtime.Services.Audio;
using System;
using Game.Runtime.Gameplay.Enemy;
using Game.Runtime.Gameplay.HUD;
using Game.Runtime.Gameplay.Implants;
using Game.Runtime.Gameplay.Warrior;
using UnityEngine;
using Game.Runtime.CMS.Components.Level;
using Game.Runtime.Services.UI;
using Game.Runtime.Utils;
using UnityEngine.SceneManagement;
using Game.Runtime.Utils.Consts;

namespace Game.Runtime.Gameplay.Level
{
    public class BattleController : MonoBehaviour, IInitializable, IService
    {
        [SerializeField] private Transform _enemyPosition;

        private bool _isTurnStarted;
        private bool _isBattleEnded;
        private int _currentTurnIndex;

        public event Func<WarriorTurnData> OnTurnStarted;
        public event Action OnTurnEnded;
        public bool IsBattleEnded => _isBattleEnded;
        public bool IsTurnStarted => _isTurnStarted;

        public Transform EnemyPosition => _enemyPosition;
        public BossLevelComponent BossConfig;
        public BattleType CurrentBattleType;
        
        private LevelComponent _levelConfig;

        public enum BattleType
        {
            Common,
            Boss,
        }

        public void Initialize()
        {
            _levelConfig = LevelHelper.GetCurrentLevelModel().GetComponent<LevelComponent>();
            ServiceLocator.Get<HUDService>().EndTurnButton.onClick.AddListener(() =>
            {
                TurnAsync().Forget();
                ServiceLocator.Get<HUDService>().EndTurnButton.interactable = false;
            });
        }

        private async UniTask TurnAsync()
        {
            if (_isTurnStarted)
                return;

            _isTurnStarted = true;

            //TODO: Это просто тест, потом удалить
            ServiceLocator.Get<WarriorController>().SetTurnData( ServiceLocator.Get<InventoryService>().CalculateTurnData()); //MOCK
            //SL.Get<WarriorController>().SetTurnData(OnTurnStarted.Invoke()); //To replace MOCK

            var token = this.GetCancellationTokenOnDestroy();

            if (CurrentBattleType == BattleType.Common)
                await CommonTurn();
            else if (CurrentBattleType == BattleType.Boss)
                await BossTurn();

            OnTurnEnded?.Invoke();
            ServiceLocator.Get<HUDService>().EndTurnButton.interactable = true;
            _isTurnStarted = false;
        }

        private async UniTask CommonTurn()
        {
            var warrior = ServiceLocator.Get<WarriorController>();
            var enemy = ServiceLocator.Get<EnemyController>();

            await warrior.HealAsync();
            await warrior.AttackAsync();
            await enemy.AttackAsync();
        }

        private async UniTask BossTurn()
        {
            var warrior = ServiceLocator.Get<WarriorController>();
            var boss = ServiceLocator.Get<BossController>();

            switch (_currentTurnIndex)
            {
                case 0:
                    await warrior.HealAsync();
                    await warrior.AttackAsync();
                    await boss.Attack(BossConfig.FirstHitDamage);
                    _currentTurnIndex++;
                    break;
                case 1:
                    await warrior.HealAsync();
                    await boss.ActivateArmor(BossConfig.Armor);
                    await warrior.AttackAsync();
                    await boss.DeactivateArmor();
                    _currentTurnIndex++;
                    break;
                case 2:
                    await warrior.HealAsync();
                    await boss.Heal();
                    await warrior.AttackAsync();
                    _currentTurnIndex++;
                    break;
                case 3:
                    await warrior.HealAsync();
                    await warrior.AttackAsync();
                    await boss.Attack(BossConfig.SecondHitDamage);

                    //TODO: показывают импланты
                    
                    _currentTurnIndex++;
                    break;
            }

            if (_currentTurnIndex >= 4)
                _currentTurnIndex = 0;
        }

        public void Loose()
        {
            _isBattleEnded = true;
            ServiceLocator.Get<Invoker>().Play(CM.Get(_levelConfig.DeathDialog.EntityId));
            ServiceLocator.Get<GameStateHolder>().NeedRespawnOnCheckpoint = true;

            LogUtil.Log(nameof(BattleController), $"You loose! " +
                                                  $"Current level - {ServiceLocator.Get<GameStateHolder>().CurrentData.Level}. " +
                                                  $"Last checkpoint - {ServiceLocator.Get<GameStateHolder>().CheckpointData.Level}");
        }

        public void Win()
        {
            _isBattleEnded = true;
            ServiceLocator.Get<LootService>().GenerateLoot();
            ServiceLocator.Get<GameStateHolder>().NeedRespawnOnCheckpoint = false;

            LogUtil.Log(nameof(BattleController), "You win! " +
                                                  $"Current level - {ServiceLocator.Get<GameStateHolder>().CurrentData.Level}.");
        }

        public async UniTask EndGameAsync()
        {
            if (ServiceLocator.Get<GameStateHolder>().CurrentData.Level == 0)
            {
                ServiceLocator.Get<Tutorial>().IsImplantLooted = true;

                await UniTask.WaitUntil(() => ServiceLocator.Get<Tutorial>().IsFinished);
            }

            ServiceLocator.Get<GameStateHolder>().CurrentData.Level++;
            ServiceLocator.Get<GameStateHolder>().DialogBlockID = _levelConfig.NextSceneDialog.EntityId;
            ServiceLocator.Get<HUDService>().DisableUI.SetActive(true);
            await ServiceLocator.Get<UIFaderService>().FadeIn();
            ServiceLocator.Get<HUDService>().LootHolder.SetActive(false);
            await SceneManager.LoadSceneAsync(Const.ScenesConst.DialogReleaseScene);
        }
    }
}
