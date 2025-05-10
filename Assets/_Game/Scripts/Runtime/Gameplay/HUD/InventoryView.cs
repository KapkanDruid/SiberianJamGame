using System;
using Game.Runtime.Gameplay.Implants;
using UnityEngine;

namespace Game.Runtime.Gameplay.HUD
{
    [Serializable]
    public class InventoryView
    {
        [SerializeField] private RectTransform inventoryRoot;
        [SerializeField] private RectTransform cellsRoot;
        
        public void ResizeInventoryView(Vector2Int gridSize, int cellSize, Vector2 rectPosition)
        {
            var inventoryRootSizeDelta = new Vector2(gridSize.x * cellSize, gridSize.y * cellSize);
            inventoryRoot.sizeDelta = inventoryRootSizeDelta;
            cellsRoot.sizeDelta = inventoryRootSizeDelta;
            inventoryRoot.anchoredPosition = rectPosition;
        }
        
        public void SetActive(bool active)
        {
            inventoryRoot.gameObject.SetActive(active);
        }
        
        public void SetupInventorySlot(Vector2 gridOffset, GameObject slotObject, Vector2Int slotPosition, float cellSize)
        {
            slotObject.transform.SetParent(cellsRoot.transform);
            slotObject.transform.localScale = Vector2.one;
            
            var rectTransform = slotObject.GetComponent<RectTransform>();
            
            Vector3 offsetPosition = new Vector3(
                (slotPosition.x + gridOffset.x) * cellSize,
                (slotPosition.y + gridOffset.y) * cellSize, 0);
    
            rectTransform.anchoredPosition = offsetPosition;
            rectTransform.sizeDelta = new Vector2(cellSize, cellSize);
        }
        
        public void SetItemInInventory(ImplantBehaviour item, Vector2 calculateCenterPosition, bool showHighlighter)
        {
            if (showHighlighter)
            {
                item.Highlighter.ShowHighlight(inventoryRoot.transform, calculateCenterPosition);
            }
            else
            {
                item.transform.SetParent(inventoryRoot.transform);
                item.GetComponent<RectTransform>().anchoredPosition = calculateCenterPosition;
            }
        }
    }
}