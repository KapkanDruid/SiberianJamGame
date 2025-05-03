using Cysharp.Threading.Tasks;
using Game.Runtime.CMS.Components.Configs;
using Game.Runtime.CMS;
using Game.Runtime.Gameplay.Enemy;
using Game.Runtime.Gameplay.HUD;
using Game.Runtime.Gameplay.Level;
using Game.Runtime.Services;
using UnityEngine;

namespace Game.Runtime.Gameplay.Warrior
{
    public class WarriorController : IService, IInitializable
    {
        private float _maxHealth;
        private float _currentHealth;

        private float _heal;
        private float _armor;
        private float _damage;

        public WarriorController()
        {
            var entity = CM.Get(CMs.Configs.PlayerConfig);
            var config = entity.GetComponent<PlayerConfig>();

            _maxHealth = config.MaxHealth;
            _currentHealth = _maxHealth;
        }   

        public void Initialize()
        {
            SL.Get<BattleController>().OnTurnEnded += () => SL.Get<HUDService>().Behaviour.WarriorUI.HideArmorSequenceAsync().Forget();
            SL.Get<HUDService>().Behaviour.WarriorUI.SetStartHealth(_maxHealth);
        }

        public void SetTurnData(WarriorTurnData turnData)
        {
            _heal = turnData.Heal;
            _armor = turnData.Armor;
            _damage = turnData.Damage;

            if (_armor > 0)
                SL.Get<HUDService>().Behaviour.WarriorUI.ShowArmorSequenceAsync(_armor).Forget();
        }

        public async UniTask TakeDamage(float damage)
        {
            SL.Get<WarriorView>().PlayHitAnimation();
            if (_armor > damage)
            {
                await SL.Get<HUDService>().Behaviour.WarriorUI.DecreaseArmorSequenceAsync(_armor, _armor - damage);
                _armor -= damage;
                return;
            }
            else if (_armor <= damage && _armor > 0)
            {
                await SL.Get<HUDService>().Behaviour.WarriorUI.BreakArmorSequenceAsync(_armor);

                damage -= _armor;
                _armor = 0;
            }

            await SL.Get<WarriorView>().TakeDamageAsync(_currentHealth, _maxHealth, damage);
            _currentHealth -= damage;

            if (_currentHealth <= 0)
            {
                _currentHealth = 0;
                Death().Forget();
                return;
            }
        }

        public async UniTask AttackAsync()
        {
            if (_damage <= 0)
                return;

            if (SL.Get<BattleController>().IsBattleEnded)
                return;

            if (SL.Get<EnemyController>().CurrentHealth > _damage)
                await SL.Get<WarriorView>().AttackAsync();
            else
                await SL.Get<WarriorView>().FinishAttackAsync();

            await SL.Get<EnemyController>().TakeDamage(_damage);
        }

        public async UniTask HealAsync()
        {
            if (SL.Get<BattleController>().IsBattleEnded)
                return;

            if (_heal <= 0)
                return;

            if (_currentHealth >= _maxHealth)
                return;

            await SL.Get<HUDService>().Behaviour.WarriorUI.HealSequenceAsync(_currentHealth, _maxHealth, _heal);

            _currentHealth += _heal;

            if (_currentHealth >= _maxHealth)
                _currentHealth = _maxHealth;
        }

        private async UniTask Death()
        {
            Debug.Log("Loose");
            await SL.Get<WarriorView>().DeathAsync();
            SL.Get<BattleController>().Loose();
        }
    }
}
