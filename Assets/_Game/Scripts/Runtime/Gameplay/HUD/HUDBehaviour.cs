using Game.Runtime.CMS;
using UnityEngine;

namespace Game.Runtime.Gameplay.HUD
{
    public class HUDBehaviour : MonoBehaviour
    {
        [SerializeField] private RectTransform inventoryRoot;
        [SerializeField] private InventoryItem testSmallItem;
        [SerializeField] private InventoryItem testLargeItem;

        public void ResizeInventoryView(Vector2Int gridSize, int cellSize)
        {
            inventoryRoot.sizeDelta = new Vector2Int(gridSize.x * cellSize, gridSize.y * cellSize);
            inventoryRoot.anchoredPosition = Vector2.zero;
        }
        
        public void SetupInventorySlot(GameObject slotObject, Vector2Int slotPosistion, float cellSize)
        {
            slotObject.transform.SetParent(inventoryRoot.transform);
            slotObject.transform.localScale = Vector2.one;
            
            var rectTransform = slotObject.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(slotPosistion.x * cellSize, slotPosistion.y * cellSize);
            rectTransform.sizeDelta = new Vector2(cellSize, cellSize);
        }
        
        public bool IsOutsideInventory(Vector2 screenPosition)
        {
            return !RectTransformUtility.RectangleContainsScreenPoint(inventoryRoot, screenPosition);
        }
        
        public void SetItemInInventory(InventoryItem item, Vector2 calculateCenterPosition)
        {
            item.transform.SetParent(inventoryRoot.transform);
            item.GetComponent<RectTransform>().anchoredPosition = calculateCenterPosition;
        }

        public void TestItemConfigure()
        {
            var smallItemModel = CM.Get(CMs.Gameplay.Inventory.TestItem_Small);
            testSmallItem.SetupItem(smallItemModel);

            var largeItemModel = CM.Get(CMs.Gameplay.Inventory.TestItem_Large);
            testLargeItem.SetupItem(largeItemModel);
        }
    }
}