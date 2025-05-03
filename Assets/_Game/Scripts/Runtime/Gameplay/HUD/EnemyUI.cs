using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Runtime.Gameplay.HUD
{
    [Serializable]
    public class EnemyUI
    {
        [SerializeField] private RectTransform _healthParent;
        [SerializeField] private Image _healthBar;
        [SerializeField] private TMP_Text _healthText;

        public void UpdateHealthBar(float currentHealth, float maxHealth)
        {
            _healthBar.fillAmount = currentHealth / maxHealth;
        }
        
        public async UniTask TakeDamageSequenceAsync(float maxHealth, float startHp, float endHp)
        {
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