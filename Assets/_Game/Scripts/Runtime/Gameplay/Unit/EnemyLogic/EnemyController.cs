using Cysharp.Threading.Tasks;
using Game.Runtime.CMS;
using Game.Runtime.CMS.Components.Configs;
using Game.Runtime.Gameplay.HUD;
using Game.Runtime.Gameplay.Level;
using Game.Runtime.Gameplay.Warrior;
using Game.Runtime.Services;
using UnityEngine;

namespace Game.Runtime.Gameplay.Enemy
{
    public class EnemyController : IEnemy, IInitializable
    {
        private readonly CMSEntity _enemyModel;
        private readonly EnemyConfig _config;

        private EnemyView _view;

        private float _armor;
        private float _health;
        private float _damage;

        public float CurrentHealth => _health;
        public float CurrentArmor => _armor;


        public EnemyController(CMSEntity enemyModel)
        {
            _enemyModel = enemyModel;
            _config = enemyModel.GetComponent<EnemyConfig>();

            _health = _config.MaxHealth;
            _damage = _config.Damage;
            _armor = _config.Armor;

            ServiceLocator.Get<HUDService>().EnemyUI.UpdateArmorIcon(_armor);
            _view = Object.Instantiate(_enemyModel.GetComponent<EnemyPrefabComponent>().EnemyView);
        }

        public void Initialize()
        {
            _view.Configurate(_config.MaxHealth);
            ServiceLocator.Get<HUDService>().EnemyUI.UpdateHealthBar(CurrentHealth, _config.MaxHealth);
            _view.transform.position = ServiceLocator.Get<BattleController>().EnemyPosition.position;
        }

        public async UniTask TakeDamage(float damage)
        {
            _view.PlayHitAnimation();
            if (_armor > damage)
            {
                await ServiceLocator.Get<HUDService>().EnemyUI.DecreaseArmorSequenceAsync(_armor, _armor - damage);
                _armor -= damage;
                return;
            }
            else if (_armor <= damage && _armor > 0)
            {
                await ServiceLocator.Get<HUDService>().EnemyUI.BreakArmorSequenceAsync(_armor);

                damage -= _armor;
                _armor = 0;
            }

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
            if (ServiceLocator.Get<BattleController>().IsBattleEnded)
                return;

            await _view.AttackAsync();
            await ServiceLocator.Get<WarriorController>().TakeDamage(_damage);
        }

        private void Death()
        {
            _view.Death();
            ServiceLocator.Get<BattleController>().Win();
        }
    }
}
