using System.Collections.Generic;
using Game.Runtime.CMS;
using Game.Runtime.CMS.Components.Commons;
using Game.Runtime.CMS.Components.Gameplay;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Runtime.Gameplay.HUD
{
    public class HUDBehaviour : MonoBehaviour
    {
        [SerializeField] private GameObject brainRoot;
        
        public void SetBrainGrid(CMSEntity defaultGridModel, List<Vector2Int> brainGrid)
        {
            var cellSize = defaultGridModel.GetComponent<CellSizeComponent>().CellSize;
            var cellSprite = defaultGridModel.GetComponent<SpriteComponent>().Sprite;
            
            foreach (var cell in brainGrid)
            {
                GameObject cellUI = new GameObject("Cell");
                cellUI.transform.SetParent(brainRoot.transform);
                
                Image cellImage = cellUI.AddComponent<Image>();
                cellImage.sprite = cellSprite;
        
                RectTransform rectTransform = cellUI.GetComponent<RectTransform>();
                
                rectTransform.sizeDelta = Vector2.one * cellSize;
                rectTransform.anchoredPosition = new Vector2(cell.x * cellSize, cell.y * cellSize);
            }
        }
    }
}