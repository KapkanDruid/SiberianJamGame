using Cysharp.Threading.Tasks;
using Game.Runtime.CMS;
using Game.Runtime.Gameplay.HUD;
using Game.Runtime.Services;
using Game.Runtime.Services.Audio;
using UnityEngine;

namespace Game.Runtime.Gameplay.Enemy
{
    public class EnemyView : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private EnemyAnimationReader _reader;
        [SerializeField] private CMSPrefab _shootEffect;
        [SerializeField] private CMSPrefab _hitEffect;
        [SerializeField] private CMSPrefab _deathEffect;

        public void Configurate(float startHealth)
        {
            SL.Get<HUDService>().Behaviour.EnemyUI.UpdateText(startHealth.ToString());
        }

        public void PlayHitAnimation()
        {
            if (_hitEffect != null)
                SL.Get<AudioService>().Play(_hitEffect.EntityId);
            _animator.SetTrigger("Hit");
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

        public async UniTask AttackAsync()
        {
            _animator.SetTrigger("Attack");
            if (_shootEffect != null)
                SL.Get<AudioService>().Play(_shootEffect.EntityId);

            await UniTask.WaitUntil(() => _reader.AttackPerformed == true);

            _reader.AttackPerformed = false;
        }

        public void Death()
        {
            _animator.SetTrigger("Death");
            if (_deathEffect != null)
                SL.Get<AudioService>().Play(_deathEffect.EntityId);
        }
    }
}
