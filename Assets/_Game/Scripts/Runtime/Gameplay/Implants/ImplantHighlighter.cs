using System;
using Game.Runtime.CMS;
using Game.Runtime.CMS.Components.Commons;
using Game.Runtime.CMS.Components.Gameplay;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Runtime.Gameplay.Implants
{
    [Serializable]
    public class ImplantHighlighter
    {
        [SerializeField] private RectTransform highlightImplant;

        private ImplantBehaviour _implantBehaviour;
        public RectTransform HighlightImplant => highlightImplant;

        public void SetupHighlightImplant(CMSEntity itemModel, ImplantBehaviour implantBehaviour)
        {
            _implantBehaviour = implantBehaviour;
            var highlightImage = HighlightImplant.GetComponent<Image>();
            highlightImage.sprite = itemModel.GetComponent<SpriteComponent>().Sprite;
            HighlightImplant.sizeDelta = itemModel.GetComponent<InventoryItemComponent>().SizeDelta;
            HighlightImplant.gameObject.SetActive(false);
        }

        public void ShowHighlight(Transform inventoryRootTransform, Vector2 calculateCenterPosition)
        {
            highlightImplant.SetParent(inventoryRootTransform);
            highlightImplant.anchoredPosition = calculateCenterPosition;
            highlightImplant.localRotation = Quaternion.Euler(0, 0, ImplantHelper.PresetAngles[_implantBehaviour.CurrentRotation]);
            highlightImplant.gameObject.SetActive(true);
        }
        
        public void HideHighlightImplant()
        {
            highlightImplant.gameObject.SetActive(false);
            highlightImplant.SetParent(_implantBehaviour.transform);
            highlightImplant.localRotation = Quaternion.identity;
        }
    }
}