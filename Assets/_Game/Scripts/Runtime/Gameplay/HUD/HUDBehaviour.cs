using Game.Runtime.CMS;
using Game.Runtime.CMS.Components.Commons;
using Game.Runtime.CMS.Components.Gameplay;
using UnityEngine;

namespace Game.Runtime.Gameplay.HUD
{
    public class HUDBehaviour : MonoBehaviour
    {
        [SerializeField] private GameObject inventoryRoot;
        [SerializeField] private InventoryItem testSmallItem;
        [SerializeField] private InventoryItem testLargeItem;
        
        public void SetupInventorySlot(GameObject slotObj, Vector2Int slotPos, float cellSize)
        {
            slotObj.transform.SetParent(inventoryRoot.transform);
            slotObj.transform.localScale = Vector2.one;
            
            var rectTransform = slotObj.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(slotPos.x * cellSize, slotPos.y * cellSize);
            rectTransform.sizeDelta = new Vector2(cellSize, cellSize);
        }

        public void TestItemConfigure()
        {
            var smallItemModel = CM.Get(CMs.Gameplay.Inventory.TestItem_Small);
            var smallItemComponent = smallItemModel.GetComponent<InventoryItemComponent>();
            testSmallItem.Configure(smallItemComponent.Grid.GridPattern, smallItemModel.GetComponent<SpriteComponent>().Sprite, smallItemComponent.SizeDelta);

            var largeItemModel = CM.Get(CMs.Gameplay.Inventory.TestItem_Large);
            var largeItemComponent = largeItemModel.GetComponent<InventoryItemComponent>();
            testLargeItem.Configure(largeItemComponent.Grid.GridPattern, largeItemModel.GetComponent<SpriteComponent>().Sprite, largeItemComponent.SizeDelta);
        }
    }
}