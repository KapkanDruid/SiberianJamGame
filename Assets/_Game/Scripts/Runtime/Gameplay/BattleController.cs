using Cysharp.Threading.Tasks;
using Game.Runtime.Services;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Runtime.Gameplay
{
    public class BattleController : MonoBehaviour, IInitializable, IService
    {
        [SerializeField] private Button _endTurnButton;
        [SerializeField] private Transform _enemyPosition;

        private bool _isTurnStarted;
        private bool _isBattleEnded;

        public event Func<WarriorTurnData> OnTurnStarted;
        public event Action OnTurnEnded;
        public bool IsBattleEnded => _isBattleEnded;

        public void Initialize()
        {
            _endTurnButton.onClick.AddListener(() =>
            {
                TurnAsync().Forget();
                _endTurnButton.interactable = false;
            });

            SL.Get<EnemyController>().transform.position = _enemyPosition.transform.position;
        }

        private async UniTask TurnAsync()
        {
            if (_isTurnStarted)
                return;

            _isTurnStarted = true;

            var warrior = SL.Get<WarriorController>();
            var enemy = SL.Get<EnemyController>();

            warrior.SetTurnData(new WarriorTurnData(5, 8, 3)); //MOCK
            //warrior.SetTurnData(OnTurnStarted.Invoke()); //To replace MOCK


            var token = this.GetCancellationTokenOnDestroy();

            await warrior.AttackAsync();
            await enemy.AttackAsync();
            await warrior.HealAsync();


            OnTurnEnded?.Invoke();
            _endTurnButton.interactable = true;
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
