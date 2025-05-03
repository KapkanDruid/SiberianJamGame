using Cysharp.Threading.Tasks;
using Game.Runtime.CMS;
using Game.Runtime.CMS.Components.Configs;
using Game.Runtime.Gameplay.Level;
using Game.Runtime.Gameplay.Warrior;
using Game.Runtime.Services;
using UnityEngine;

namespace Game.Runtime.Gameplay.Enemy
{
    public class EnemyController : IService, IInitializable
    {
        private readonly CMSEntity _enemyModel;
        private readonly EnemyConfig _config;

        private EnemyView _view;
        private float _health;
        private float _damage;

        public float CurrentHealth => _health;

        public EnemyController(CMSEntity enemyModel)
        {
            _enemyModel = enemyModel;
            _config = enemyModel.GetComponent<EnemyConfig>();
            
            _health = _config.MaxHealth;
            _damage = _config.Damage;
        }

        public void Initialize()
        {
            _view = Object.Instantiate(_enemyModel.GetComponent<EnemyPrefabComponent>().EnemyView);
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
