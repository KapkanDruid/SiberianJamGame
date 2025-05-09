using Game.Runtime.Services;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Game.Runtime.Gameplay.HUD;
using Game.Runtime.CMS;
using Game.Runtime.Services.Audio;

namespace Game.Runtime.Gameplay.Warrior
{
    public class WarriorView : MonoBehaviour, IService, IInitializable
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private WarriorAnimationReader _reader;
        [SerializeField] private Transform _shootPoint;
        [SerializeField] private ParticleSystem _singleShootPrefab;
        [SerializeField] private ParticleSystem _doubleShootPrefab;
        [SerializeField] private CMSPrefab _shootEffect;
        [SerializeField] private CMSPrefab _superShootEffect;
        [SerializeField] private CMSPrefab _hitEffect;
        [SerializeField] private CMSPrefab _deathEffect;

        private ParticleSystem _singleShoot;
        private ParticleSystem _doubleShoot;

        public void Initialize()
        {
            ServiceLocator.Get<HUDService>().WarriorUI.SetArmorIconActive(false);
            _singleShoot = Instantiate(_singleShootPrefab, _shootPoint.position, Quaternion.identity);
            _doubleShoot = Instantiate(_doubleShootPrefab, _shootPoint.position, Quaternion.identity);
        }

        public async UniTask DeathAsync()
        {
            _animator.SetTrigger("Death");
            if (_deathEffect != null)
                ServiceLocator.Get<AudioService>().Play(_hitEffect.EntityId);

            await UniTask.WaitUntil(() => _reader.DeathEnded);

            _reader.DeathEnded = false;
        }

        public void PlayHitAnimation()
        {
            _animator.SetTrigger("Hit");
            if (_hitEffect != null)
                ServiceLocator.Get<AudioService>().Play(_hitEffect.EntityId);
        }

        public async UniTask TakeDamageAsync(float currentHealth, float maxHealth, float damage)
        {
            ServiceLocator.Get<HUDService>().WarriorUI.UpdateHealthBar(currentHealth, maxHealth);

            float startHp = currentHealth;
            float endHp = currentHealth - damage;

            if (endHp <= 0)
                endHp = 0;

            await ServiceLocator.Get<HUDService>().WarriorUI.TakeDamageSequenceAsync(maxHealth, startHp, endHp);
        }

        public async UniTask AttackAsync()
        {
            _animator.SetTrigger("SimpleAttack");
            if (_shootEffect != null)
                ServiceLocator.Get<AudioService>().Play(_shootEffect.EntityId);

            await UniTask.WaitUntil(() => _reader.SimpleAttackPerformed == true);

            _singleShoot.Play();

            _reader.SimpleAttackPerformed = false;
        }

/*        private void Update()
        {
            if (_singleShoot != null && _singleShoot.gameObject.activeInHierarchy && !_singleShoot.isPlaying)
            {
                _singleShoot.gameObject.SetActive(false);
            }
            if (_doubleShoot != null && _doubleShoot.gameObject.activeInHierarchy && !_doubleShoot.isPlaying)
            {
                _doubleShoot.gameObject.SetActive(false);
            }
        }*/

        public async UniTask FinishAttackAsync()
        {
            _animator.SetTrigger("FinishAttack");
            if (_superShootEffect != null)
                ServiceLocator.Get<AudioService>().Play(_superShootEffect.EntityId);

            await UniTask.WaitUntil(() => _reader.FinishAttackPerformed);

            _doubleShoot.Play();

            _reader.FinishAttackPerformed = false;
        }
    }
}
