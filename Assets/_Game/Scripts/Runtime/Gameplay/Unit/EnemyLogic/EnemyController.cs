using Cysharp.Threading.Tasks;
using Game.Runtime.CMS.Components.Configs;
using Game.Runtime.Gameplay.Warrior;
using Game.Runtime.Services;
using UnityEngine;

namespace Game.Runtime.Gameplay.Enemy
{
    public class EnemyController : IEnemy
    {
        private EnemyView _view;
        private EnemyConfig _config;

        private float _health;
        private float _damage;

        public float CurrentHealth => _health;

        public EnemyController(EnemyConfig config, EnemyView enemyView)
        {
            _config = config;

            _health = _config.MaxHealth;
            _damage = _config.Damage;

            _view = GameObject.Instantiate(enemyView);

            _view.transform.position = SL.Get<BattleController>().EnemyPosition.position;
        }

        public async UniTask TakeDamage(float damage)
        {
            await _view.TakeDamageAsync(_health, _config.MaxHealth, damage);
            _health -= damage;

            if (_health <= 0)
            {
                _health = 0;
                Death();
            }
        }

        public async UniTask AttackAsync()
        {
            if (SL.Get<BattleController>().IsBattleEnded)
                return;

            await _view.AttackAsync();
            await SL.Get<WarriorController>().TakeDamage(_damage);
        }

        private void Death()
        {
            Debug.Log("Win");
            _view.Death();
            SL.Get<BattleController>().Win();
        }
    }
}
