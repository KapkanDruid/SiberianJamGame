using Cysharp.Threading.Tasks;
using Game.Runtime.Gameplay.HUD;
using Game.Runtime.Services;
using UnityEngine;

namespace Game.Runtime.Gameplay.Enemy
{
    public class EnemyView : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private EnemyAnimationReader _reader;

        public void Configurate(float startHealth)
        {
            SL.Get<HUDService>().Behaviour.EnemyUI.UpdateText(startHealth.ToString());
        }

        public async UniTask TakeDamageAsync(float currentHealth, float maxHealth, float damage)
        {
            _animator.SetTrigger("Hit");

            SL.Get<HUDService>().Behaviour.EnemyUI.UpdateHealthBar(currentHealth, maxHealth);

            float startHp = currentHealth;
            float endHp = currentHealth - damage;

            if (endHp <= 0)
                endHp = 0;
            
            await SL.Get<HUDService>().Behaviour.EnemyUI.TakeDamageSequenceAsync(maxHealth, startHp, endHp);
        }

        public async UniTask AttackAsync()
        {
            _animator.SetTrigger("Attack");

            await UniTask.WaitUntil(() => _reader.AttackPerformed == true);

            _reader.AttackPerformed = false;
        }

        public void Death()
        {
            _animator.SetTrigger("Death");
        }
    }
}
