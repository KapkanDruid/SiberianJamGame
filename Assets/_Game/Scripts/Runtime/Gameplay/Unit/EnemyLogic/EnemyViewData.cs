using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Runtime.Gameplay
{
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
