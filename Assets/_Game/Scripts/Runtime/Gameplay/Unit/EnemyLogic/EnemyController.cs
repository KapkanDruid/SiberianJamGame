using Cysharp.Threading.Tasks;
using Game.Runtime.CMS.Components.Configs;
using Game.Runtime.Services;
using UnityEngine;

namespace Game.Runtime.Gameplay
{
    public class EnemyController : MonoBehaviour, IEnemy
    {
        private EnemyView _view;
        private EnemyConfig _config;

        private float _health;
        private float _damage;

        public float CurrentHealth => _health;

        public void Configurate(EnemyConfig config, EnemyViewData viewData)
        {
            _config = config;

            _health = _config.MaxHealth;
            _damage = _config.Damage;

            _view = new EnemyView(viewData);
        }

        public async UniTask TakeDamage(float damage)
        {
            await _view.TakeDamageAsync(_health, _config.MaxHealth, damage);
            _health -= damage;

            if (_health <= 0)
            {
                _health = 0;
                Death();
                return;
            }
        }

        public async UniTask AttackAsync()
        {
            if (SL.Get<BattleController>().IsBattleEnded)
                return;

            await SL.Get<WarriorController>().TakeDamage(_damage);
        }

        private void Death()
        {
            Debug.Log("Win");
            SL.Get<BattleController>().Win();
        }
    }
}
