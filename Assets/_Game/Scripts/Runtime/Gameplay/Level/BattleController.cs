using Cysharp.Threading.Tasks;
using Game.Runtime.CMS;
using Game.Runtime.Services;
using Game.Runtime.Services.Audio;
using System;
using Game.Runtime.Gameplay.Enemy;
using Game.Runtime.Gameplay.HUD;
using Game.Runtime.Gameplay.Implants;
using Game.Runtime.Gameplay.Warrior;
using Game.Runtime.Services.Save;
using UnityEngine;
using Game.Runtime.CMS.Components.Level;
using Game.Runtime.Services.UI;
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
        public LevelComponent LevelConfig;
        public BossLevelComponent BossConfig;
        public BattleType CurrentBattleType;

        public enum BattleType
        {
            Common,
            Boss,
        }

        public void Initialize()
        {
            SL.Get<HUDService>().Behaviour.EndTurnButton.onClick.AddListener(() =>
            {
                TurnAsync().Forget();
                SL.Get<HUDService>().Behaviour.EndTurnButton.interactable = false;
            });
        }

        private async UniTask TurnAsync()
        {
            if (_isTurnStarted)
                return;

            _isTurnStarted = true;

            //TODO: Это просто тест, потом удалить
            SL.Get<WarriorController>().SetTurnData( SL.Get<InventoryService>().CalculateTurnData()); //MOCK
            //SL.Get<WarriorController>().SetTurnData(OnTurnStarted.Invoke()); //To replace MOCK

            var token = this.GetCancellationTokenOnDestroy();

            if (CurrentBattleType == BattleType.Common)
                await CommonTurn();
            else if (CurrentBattleType == BattleType.Boss)
                await BossTurn();

            OnTurnEnded?.Invoke();
            SL.Get<HUDService>().Behaviour.EndTurnButton.interactable = true;
            _isTurnStarted = false;
        }

        private async UniTask CommonTurn()
        {
            var warrior = SL.Get<WarriorController>();
            var enemy = SL.Get<EnemyController>();

            await warrior.HealAsync();
            await warrior.AttackAsync();
            await enemy.AttackAsync();
        }

        private async UniTask BossTurn()
        {
            var warrior = SL.Get<WarriorController>();
            var boss = SL.Get<BossController>();

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

            SL.Get<Invoker>().Play(CM.Get(LevelConfig.DeathDialog.EntityId));
        }

        public void Win()
        {
            _isBattleEnded = true;
            Debug.Log($"[BattleController] You win! {SL.Get<GameStateHolder>().CurrentLevel}");
            SL.Get<LootService>().GenerateLoot();
        }

        public async UniTask EndGameAsync()
        {
            SL.Get<SaveService>().SaveData.LevelIndex++;
            SL.Get<SaveService>().SaveData.DialogBlockID = LevelConfig.NextSceneDialog.EntityId;
            SL.Get<SaveService>().Save();
            SL.Get<HUDService>().Behaviour.DisableUI.SetActive(true);
            await SL.Get<UIFaderService>().FadeIn();
            SL.Get<HUDService>().Behaviour.LootHolder.SetActive(false);
            await SceneManager.LoadSceneAsync(Const.ScenesConst.DialogReleaseScene);
        }
    }
}
