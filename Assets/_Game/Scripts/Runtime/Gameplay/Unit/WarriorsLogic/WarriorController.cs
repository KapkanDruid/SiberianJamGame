using Cysharp.Threading.Tasks;
using Game.Runtime.CMS.Components.Configs;
using Game.Runtime.CMS;
using Game.Runtime.Gameplay.Enemy;
using Game.Runtime.Gameplay.HUD;
using Game.Runtime.Gameplay.Level;
using Game.Runtime.Services;

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
        }   

        public void Initialize()
        {
            ServiceLocator.Get<BattleController>().OnTurnEnded += () => ServiceLocator.Get<HUDService>().WarriorUI.HideArmorSequenceAsync().Forget();

            _currentHealth = ServiceLocator.Get<GameStateHolder>().CurrentData.CharacterHealth;
            
            ServiceLocator.Get<HUDService>().WarriorUI.UpdateHealthBar(_currentHealth, _maxHealth);
            ServiceLocator.Get<HUDService>().WarriorUI.SetStartHealth(_currentHealth);
        }

        public void SetTurnData(WarriorTurnData turnData)
        {
            _heal = turnData.Heal;
            _armor = turnData.Armor;
            _damage = turnData.Damage;

            if (_armor > 0)
                ServiceLocator.Get<HUDService>().WarriorUI.ShowArmorSequenceAsync(_armor).Forget();
        }

        public async UniTask TakeDamage(float damage)
        {
            ServiceLocator.Get<WarriorView>().PlayHitAnimation();
            if (_armor > damage)
            {
                await ServiceLocator.Get<HUDService>().WarriorUI.DecreaseArmorSequenceAsync(_armor, _armor - damage);
                _armor -= damage;
                return;
            }
            else if (_armor <= damage && _armor > 0)
            {
                await ServiceLocator.Get<HUDService>().WarriorUI.BreakArmorSequenceAsync(_armor);

                damage -= _armor;
                _armor = 0;
            }

            await ServiceLocator.Get<WarriorView>().TakeDamageAsync(_currentHealth, _maxHealth, damage);
            _currentHealth -= damage;

            if (_currentHealth <= 0)
            {
                _currentHealth = 0;
                Death().Forget();
            }
            
            ServiceLocator.Get<GameStateHolder>().CurrentData.CharacterHealth = _currentHealth;
        }

        public async UniTask AttackAsync()
        {
            if (_damage <= 0)
                return;

            if (ServiceLocator.Get<BattleController>().IsBattleEnded)
                return;

            if (ServiceLocator.Get<IEnemy>().CurrentHealth + ServiceLocator.Get<IEnemy>().CurrentArmor > _damage)
                await ServiceLocator.Get<WarriorView>().AttackAsync();
            else
                await ServiceLocator.Get<WarriorView>().FinishAttackAsync();

            await ServiceLocator.Get<IEnemy>().TakeDamage(_damage);
        }

        public async UniTask HealAsync()
        {
            if (ServiceLocator.Get<BattleController>().IsBattleEnded)
                return;

            if (_heal <= 0)
                return;

            if (_currentHealth >= _maxHealth)
                return;

            await ServiceLocator.Get<HUDService>().WarriorUI.HealSequenceAsync(_currentHealth, _maxHealth, _heal);

            _currentHealth += _heal;

            if (_currentHealth >= _maxHealth)
                _currentHealth = _maxHealth;

            ServiceLocator.Get<GameStateHolder>().CurrentData.CharacterHealth = _currentHealth;
        }

        private async UniTask Death()
        {
            ServiceLocator.Get<BattleController>().Loose();
            await ServiceLocator.Get<WarriorView>().DeathAsync();
        }
    }
}
