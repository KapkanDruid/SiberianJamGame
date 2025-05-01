using Game.Runtime.Gameplay.Grid;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Runtime.Gameplay.HUD
{
    public class HUDBehaviour : MonoBehaviour
    {
        [SerializeField] private GameObject brainRoot;
        
        public void SetBrainGrid(GridData gridData)
        {
            foreach (var cell in gridData.gridPatternData.GridPattern)
            {
                GameObject cellUI = new GameObject("Cell");
                cellUI.transform.SetParent(brainRoot.transform);
                
                Image cellImage = cellUI.AddComponent<Image>();
                cellImage.sprite = gridData.CellSprite;
        
                RectTransform rectTransform = cellUI.GetComponent<RectTransform>();
                rectTransform.sizeDelta = Vector2.one * gridData.CellSize;
                rectTransform.anchoredPosition = new Vector2(cell.x * gridData.CellSize, cell.y * gridData.CellSize);
            }
        }
    }
}