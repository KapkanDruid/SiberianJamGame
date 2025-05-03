using Game.Runtime.Services;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Game.Runtime.Gameplay.HUD;

namespace Game.Runtime.Gameplay.Warrior
{
    public class WarriorView : MonoBehaviour, IService, IInitializable
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private WarriorAnimationReader _reader;

        public void Initialize()
        {
            SL.Get<HUDService>().Behaviour.WarriorUI.SetArmorIconActive(false);
        }

        public async UniTask DeathAsync()
        {
            _animator.SetTrigger("Death");

            await UniTask.WaitUntil(() => _reader.DeathEnded);

            _reader.DeathEnded = false;
        }

        public void PlayHitAnimation()
        {
            _animator.SetTrigger("Hit");
        }

        public async UniTask TakeDamageAsync(float currentHealth, float maxHealth, float damage)
        {
            SL.Get<HUDService>().Behaviour.WarriorUI.UpdateHealthBar(currentHealth, maxHealth);

            float startHp = currentHealth;
            float endHp = currentHealth - damage;

            if (endHp <= 0)
                endHp = 0;

            await SL.Get<HUDService>().Behaviour.WarriorUI.TakeDamageSequenceAsync(maxHealth, startHp, endHp);
        }

        public async UniTask AttackAsync()
        {
            _animator.SetTrigger("SimpleAttack");

            await UniTask.WaitUntil(() => _reader.SimpleAttackPerformed == true);

            _reader.SimpleAttackPerformed = false;
        }

        public async UniTask FinishAttackAsync()
        {
            _animator.SetTrigger("FinishAttack");

            await UniTask.WaitUntil(() => _reader.FinishAttackPerformed);

            _reader.FinishAttackPerformed = false;
        }
    }
}
