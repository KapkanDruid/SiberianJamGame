using Cysharp.Threading.Tasks;
using Game.Runtime.Gameplay.HUD;
using Game.Runtime.Services;
using UnityEngine;

namespace Game.Runtime.Gameplay.Enemy
{
    public class BossView : MonoBehaviour
    {
        [SerializeField] private EnemyAnimationReader _reader;
        [SerializeField] private Animator _animator;

        public async UniTask AttackAsync()
        {
            _animator.SetTrigger("Attack");

            await UniTask.WaitUntil(() => _reader.AttackPerformed == true);

            _reader.AttackPerformed = false;
        }

        public async UniTask TakeDamageAsync(float currentHealth, float maxHealth, float damage)
        {
            SL.Get<HUDService>().Behaviour.EnemyUI.UpdateHealthBar(currentHealth, maxHealth);

            float startHp = currentHealth;
            float endHp = currentHealth - damage;

            if (endHp <= 0)
                endHp = 0;

            await SL.Get<HUDService>().Behaviour.EnemyUI.TakeDamageSequenceAsync(maxHealth, startHp, endHp);
        }

        public void PlayHitAnimation()
        {
            _animator.SetTrigger("Hit");
        }
        public void Death()
        {
            _animator.SetTrigger("Death");
        }
    }
}
