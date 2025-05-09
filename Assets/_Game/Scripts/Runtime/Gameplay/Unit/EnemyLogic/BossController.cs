using Cysharp.Threading.Tasks;
using Game.Runtime.Gameplay.HUD;
using Game.Runtime.Gameplay.Level;
using Game.Runtime.Gameplay.Warrior;
using Game.Runtime.Services;
using UnityEngine;

namespace Game.Runtime.Gameplay.Enemy
{
    public class BossController : IEnemy, IInitializable
    {
        private BossView _view;

        private float _currentHealth;
        private float _armor;
        private float _maxHealth;//SETTTT!!!
        private float _heal;

        public float CurrentHealth => _currentHealth;
        public float CurrentArmor => _armor;

        public BossController(float maxHealth, float heal, BossView viewPrefab)
        {
            _maxHealth = maxHealth;
            _currentHealth = _maxHealth;
            _heal = heal;

            ServiceLocator.Get<HUDService>().EnemyUI.UpdateArmorIcon(_armor);
            _view = Object.Instantiate(viewPrefab);
        }

        public void Initialize()
        {
            _view.Configurate(_maxHealth);
            ServiceLocator.Get<HUDService>().EnemyUI.UpdateHealthBar(CurrentHealth, _maxHealth);
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

            await _view.TakeDamageAsync(_currentHealth, _maxHealth, damage);
            _currentHealth -= damage;

            if (_currentHealth <= 0)
            {
                _currentHealth = 0;
                Death();
            }
        }
        private void Death()
        {
            Debug.Log("Win");
            _view.Death();
            ServiceLocator.Get<BattleController>().Win();
        }

        public async UniTask Attack(float damage)
        {
            if (ServiceLocator.Get<BattleController>().IsBattleEnded)
                return;

            await _view.AttackAsync();
            await ServiceLocator.Get<WarriorController>().TakeDamage(damage);
        }

        public async UniTask ActivateArmor(float armor)
        {
            _armor = armor;
            await ServiceLocator.Get<HUDService>().EnemyUI.ShowArmorSequenceAsync(armor);
        }

        public async UniTask DeactivateArmor()
        {
            _armor = 0;
            await ServiceLocator.Get<HUDService>().EnemyUI.HideArmorSequenceAsync();
        }

        public async UniTask Heal()
        {
            if (ServiceLocator.Get<BattleController>().IsBattleEnded)
                return;

            if (_heal <= 0)
                return;

            if (_currentHealth >= _maxHealth)
                return;

            await ServiceLocator.Get<HUDService>().EnemyUI.HealSequenceAsync(_currentHealth, _maxHealth, _heal);

            _currentHealth += _heal;

            if (_currentHealth >= _maxHealth)
                _currentHealth = _maxHealth;
        }
    }
}
