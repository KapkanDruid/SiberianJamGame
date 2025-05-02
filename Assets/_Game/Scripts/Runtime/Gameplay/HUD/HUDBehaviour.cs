using System.Collections.Generic;
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
        
        public RectTransform InventoryRoot => inventoryRoot.GetComponent<RectTransform>();

        public void CalculateInventorySize(List<Vector2Int> grid, int cellSize)
        {
            int minX = int.MaxValue;
            int maxX = int.MinValue;
            int minY = int.MaxValue;
            int maxY = int.MinValue;

            foreach (var pos in grid)
            {
                minX = Mathf.Min(minX, pos.x);
                maxX = Mathf.Max(maxX, pos.x);
                minY = Mathf.Min(minY, pos.y);
                maxY = Mathf.Max(maxY, pos.y);
            }

            int width = maxX - minX + 1;
            int height = maxY - minY + 1;
            Debug.Log(width + "x" + height);
            InventoryRoot.sizeDelta = new Vector2Int(width * cellSize, height * cellSize);
            InventoryRoot.anchoredPosition = Vector2.zero;
        }
        public void SetupInventorySlot(GameObject slotObj, Vector2Int slotPos, float cellSize)
        {
            slotObj.transform.SetParent(inventoryRoot.transform);
            slotObj.transform.localScale = Vector2.one;
            slotObj.transform.position = Vector2.zero;
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

        public void SetItemInSlots(InventoryItem item, Vector2 calculateCenterPosition)
        {
            item.transform.SetParent(inventoryRoot.transform);
            item.GetComponent<RectTransform>().anchoredPosition = calculateCenterPosition;
        }
    }
}