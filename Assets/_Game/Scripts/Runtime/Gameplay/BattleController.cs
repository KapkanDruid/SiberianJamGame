using Cysharp.Threading.Tasks;
using Game.Runtime.Services;
using System;
using Game.Runtime.Gameplay.Enemy;
using Game.Runtime.Gameplay.HUD;
using Game.Runtime.Gameplay.Inventory;
using Game.Runtime.Gameplay.Warrior;
using UnityEngine;

namespace Game.Runtime.Gameplay
{
    public class BattleController : MonoBehaviour, IInitializable, IService
    {
        [SerializeField] private Transform _enemyPosition;

        private bool _isTurnStarted;
        private bool _isBattleEnded;

        public event Func<WarriorTurnData> OnTurnStarted;
        public event Action OnTurnEnded;
        public bool IsBattleEnded => _isBattleEnded;

        public Transform EnemyPosition => _enemyPosition; 

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

            var warrior = SL.Get<WarriorController>();
            var enemy = SL.Get<EnemyController>();
            
            //TODO: Это просто тест, потом удалить
            warrior.SetTurnData( SL.Get<InventoryService>().CalculateTurnData()); //MOCK
            //warrior.SetTurnData(OnTurnStarted.Invoke()); //To replace MOCK

            var token = this.GetCancellationTokenOnDestroy();

            await warrior.AttackAsync();
            await enemy.AttackAsync();
            await warrior.HealAsync();


            OnTurnEnded?.Invoke();
            SL.Get<HUDService>().Behaviour.EndTurnButton.interactable = true;
            _isTurnStarted = false;
        }

        public void Loose()
        {
            _isBattleEnded = true;
        }

        public void Win()
        {
            _isBattleEnded = true;
        }
    }
}
