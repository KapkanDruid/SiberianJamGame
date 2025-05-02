using Cysharp.Threading.Tasks;
using DG.Tweening;
using Game.Runtime.CMS.Components.Configs;
using Game.Runtime.Services;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Runtime.Gameplay
{
    public class EnemyController : MonoBehaviour, IEnemy
    {
        private EnemyView _view;
        private EnemyConfig _config;

        private float _health;
        private float _damage;

        public void Configurate(EnemyConfig config, EnemyViewData viewData)
        {
            _config = config;

            _health = _config.MaxHealth;
            _damage = _config.Damage;

            _view = new EnemyView(viewData);
        }

        public async UniTask TakeDamage(float damage)
        {
            await _view.TakeDamageAsync(_health, _config.MaxHealth, damage);
            _health -= damage;

            if (_health <= 0)
            {
                _health = 0;
                Death();
                return;
            }

            Debug.Log("Enemy damaged Current enemy health" + _health);
        }

        public async UniTask AttackAsync()
        {
            if (SL.Get<BattleController>().IsBattleEnded)
                return;

            await SL.Get<WarriorController>().TakeDamage(_damage);
        }

        private void Death()
        {
            Debug.Log("Win");
            SL.Get<BattleController>().Win();
        }
    }

    public class EnemyView
    {
        private RectTransform _healthParent;
        private Image _healthBar;
        private TextMeshProUGUI _healthText;

        public EnemyView(EnemyViewData data)
        {
            _healthParent = data.HealthParent;
            _healthBar = data.HealthBar;
            _healthText = data.HealthText;
        }

        public async UniTask TakeDamageAsync(float currentHealth, float maxHealth, float damage)
        {
            //playAnim 

            _healthBar.fillAmount = currentHealth / maxHealth;

            float startHp = currentHealth;
            float endHp = currentHealth - damage;

            if (endHp <= 0)
                endHp = 0;

            var sequence = DOTween.Sequence()
                .Append(_healthParent.DOScale(1.5f, 0.2f))
                .Append(_healthParent.DOShakeAnchorPos(0.5f, strength: 20))
                .Append(_healthBar.DOFillAmount(endHp / maxHealth, 0.6f))
                .Join(DOTween.To(() => startHp, x =>
                {
                    startHp = x;
                    _healthText.text = Mathf.CeilToInt(x).ToString();
                }, endHp, 0.6f))
                .AppendInterval(0.4f)
                .Append(_healthParent.DOScale(1f, 0.2f));

            await sequence.AsyncWaitForCompletion();
        }
    }

    public class EnemyViewData
    {
        public RectTransform HealthParent;
        public Image HealthBar;
        public TextMeshProUGUI HealthText;

        public EnemyViewData(RectTransform healthParent, Image healthBar, TextMeshProUGUI healthText)
        {
            HealthParent = healthParent;
            HealthBar = healthBar;
            HealthText = healthText;
        }
    }
}
