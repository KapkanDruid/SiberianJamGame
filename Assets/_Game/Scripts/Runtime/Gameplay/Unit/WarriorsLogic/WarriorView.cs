using Game.Runtime.Services;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using Cysharp.Threading.Tasks;

namespace Game.Runtime.Gameplay
{
    public class WarriorView : MonoBehaviour, IService
    {
        [SerializeField] private RectTransform _healthParent;
        [SerializeField] private Image _healthBar;
        [SerializeField] private TextMeshProUGUI _healthText;

        [SerializeField] private Image _armorIcon;
        [SerializeField] private TextMeshProUGUI _armorText;

        [SerializeField] private Animator _animator;
        [SerializeField] private WarriorAnimationReader _reader;

        private void Start()
        {
            _armorIcon.gameObject.SetActive(false);
        }

        public void SetStartHealth(float value)
        {
            _healthText.text = value.ToString();
        }

        public async UniTask TakeDamageAsync(float currentHealth, float maxHealth, float damage)
        {
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

        public async UniTask HealAsync(float currentHealth, float maxHealth, float heal)
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

        public async UniTask ShowArmor(float value)
        {
            _armorIcon.gameObject.SetActive(true);
            _armorText.text = value.ToString();
            _armorIcon.rectTransform.localScale = Vector3.zero;

            var sequence = DOTween.Sequence()
                .Append(_armorIcon.rectTransform.DOScale(1.5f, 0.2f))
                .AppendInterval(0.2f)
                .Append(_armorIcon.rectTransform.DOScale(1f, 0.2f));

            await sequence.AsyncWaitForCompletion();
        }

        public async UniTask HideArmor()
        {
            _armorIcon.rectTransform.localScale = Vector3.one;

            var sequence = DOTween.Sequence()
                .Append(_armorIcon.rectTransform.DOScale(0, 0.2f));

            await sequence.AsyncWaitForCompletion();
        }

        public async UniTask DecreaseArmor(float startValue, float endValue)
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

        public async UniTask BreakArmor(float startValue)
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
