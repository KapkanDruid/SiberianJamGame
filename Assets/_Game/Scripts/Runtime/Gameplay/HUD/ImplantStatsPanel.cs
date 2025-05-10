using System;
using DG.Tweening;
using Game.Runtime.Gameplay.Implants;
using Game.Runtime.Gameplay.Inventory;
using Game.Runtime.Services;
using TMPro;
using UnityEngine;

namespace Game.Runtime.Gameplay.HUD
{
    [Serializable]
    public class ImplantStatsPanel
    {
        [SerializeField] private ImplantStatPanel[] implantStatPanels;

        public void UpdateStats()
        {
            var inventoryStatMap = ServiceLocator.Get<InventoryService>().Stats.StatsMap;

            foreach (var statPair in inventoryStatMap)
            {
                foreach (var statPanel in implantStatPanels)
                {
                    if (statPanel.ImplantType == statPair.Key)
                    {
                        statPanel.UpdateValue(statPair.Value);
                        break;
                    }
                }
            }
        }
    }

    [Serializable]
    public class ImplantStatPanel
    {
        [SerializeField] private ImplantType implantType;
        [SerializeField] private TMP_Text valueText;
        
        public ImplantType ImplantType => implantType;
        
        private Tweener _statTween;

        public void UpdateValue(float endValue)
        {
            if (float.TryParse(valueText.text, out float startValue))
            {
                _statTween?.Kill();

                _statTween = DOTween.To(() => startValue, progressValue =>
                {
                    startValue = progressValue;
                    valueText.text = Mathf.CeilToInt(progressValue).ToString();
                }, endValue, 0.6f).OnComplete(() =>
                {
                    valueText.text = $"{Mathf.CeilToInt(endValue)}";
                    _statTween.Kill();
                });
            }
        }
    }
}