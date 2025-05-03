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

        [SerializeField] private Image _armorIcon;
        [SerializeField] private TMP_Text _armorText;

        public void UpdateHealthBar(float currentHealth, float maxHealth)
        {
            _healthBar.fillAmount = currentHealth / maxHealth;
        }

        public void UpdateText(string text)
        {
            _healthText.text = text;
        }

        public void UpdateArmorIcon(float armor)
        {
            if (armor <= 0)
            {
                _armorIcon.gameObject.SetActive(false);
            }

            _armorText.text = armor.ToString();
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
        public async UniTask HealSequenceAsync(float currentHealth, float maxHealth, float heal)
        {
            float startHp = currentHealth;
            float endHp = currentHealth + heal;

            if (endHp >= maxHealth)
                endHp = maxHealth;

            var sequence = DOTween.Sequence()
                .Append(_healthParent.DOScale(1.5f, 0.2f))
                .AppendInterval(0.2f)
                .Append(_healthBar.DOFillAmount(endHp / maxHealth, 0.6f))
                .Join(DOTween.To(() => startHp, x =>
                {
                    startHp = x;
                    _healthText.text = Mathf.CeilToInt(x).ToString();
                }, endHp, 0.6f))
                .AppendInterval(0.2f)
                .Append(_healthParent.DOScale(1f, 0.2f));

            await sequence.AsyncWaitForCompletion();
        }

        public async UniTask ShowArmorSequenceAsync(float value)
        {
            _armorIcon.gameObject.SetActive(true);
            _armorText.text = $"{value}";
            _armorIcon.rectTransform.localScale = Vector3.zero;

            var sequence = DOTween.Sequence()
                .Append(_armorIcon.rectTransform.DOScale(1.5f, 0.2f))
                .AppendInterval(0.2f)
                .Append(_armorIcon.rectTransform.DOScale(1f, 0.2f));

            await sequence.AsyncWaitForCompletion();
        }

        public async UniTask HideArmorSequenceAsync()
        {
            _armorIcon.rectTransform.localScale = Vector3.one;

            var sequence = DOTween.Sequence()
                .Append(_armorIcon.rectTransform.DOScale(0, 0.2f));

            await sequence.AsyncWaitForCompletion();
        }

        public async UniTask DecreaseArmorSequenceAsync(float startValue, float endValue)
        {
            var sequence = DOTween.Sequence()
                .Append(_armorIcon.rectTransform.DOScale(1.5f, 0.2f))
                .AppendInterval(0.2f)
                .Append(DOTween.To(() => startValue, x =>
                {
                    startValue = x;
                    _armorText.text = Mathf.CeilToInt(x).ToString();
                }, endValue, 0.6f).SetEase(Ease.Linear))
                .AppendInterval(0.2f)
                .Append(_armorIcon.rectTransform.DOScale(1f, 0.2f));

            await sequence.AsyncWaitForCompletion();
        }

        public async UniTask BreakArmorSequenceAsync(float startValue)
        {
            var sequence = DOTween.Sequence()
                .Append(_armorIcon.rectTransform.DOScale(1.5f, 0.2f))
                .AppendInterval(0.2f)
                .Append(DOTween.To(() => startValue, x =>
                {
                    startValue = x;
                    _armorText.text = Mathf.CeilToInt(x).ToString();
                }, 0, 0.4f).SetEase(Ease.Linear))
                .AppendInterval(0.2f)
                .Append(_armorIcon.rectTransform.DOShakeAnchorPos(0.5f, strength: 20))
                .Append(_armorIcon.rectTransform.DOScale(0f, 0.2f))
                .OnComplete(() => _armorIcon.gameObject.SetActive(false));

            await sequence.AsyncWaitForCompletion();
        }
    }
}