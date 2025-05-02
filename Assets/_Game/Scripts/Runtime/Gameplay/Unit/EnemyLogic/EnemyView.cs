using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Runtime.Gameplay
{
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
}
