using System;
using UnityEngine;

namespace Game.Runtime.Gameplay.HUD
{
    [Serializable]
    public class InventoryView
    {
        [SerializeField] private RectTransform inventoryRoot;
        [SerializeField] private RectTransform cellsRoot;
        
        public void ResizeInventoryView(Vector2Int gridSize, int cellSize)
        {
            inventoryRoot.sizeDelta = new Vector2Int(gridSize.x * cellSize, gridSize.y * cellSize);
            //inventoryRoot.anchoredPosition = Vector2.zero;
        }
        
        public void SetupInventorySlot(GameObject slotObject, Vector2Int slotPosistion, float cellSize)
        {
            slotObject.transform.SetParent(cellsRoot.transform);
            slotObject.transform.localScale = Vector2.one;
            
            var rectTransform = slotObject.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(slotPosistion.x * cellSize, slotPosistion.y * cellSize);
            rectTransform.sizeDelta = new Vector2(cellSize, cellSize);
        }
        
        public void SetItemInInventory(InventoryItem item, Vector2 calculateCenterPosition)
        {
            item.transform.SetParent(inventoryRoot.transform);
            item.GetComponent<RectTransform>().anchoredPosition = calculateCenterPosition;
        }
    }
}