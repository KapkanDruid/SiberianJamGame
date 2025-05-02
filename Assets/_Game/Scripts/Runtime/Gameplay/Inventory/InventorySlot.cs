using Game.Runtime.CMS;
using Game.Runtime.CMS.Components.Commons;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Runtime.Gameplay.Inventory
{
    [RequireComponent(typeof(Image))]
    public class InventorySlot : MonoBehaviour
    {
        [SerializeField] private Color _normalColor = Color.white;
        [SerializeField] private Color _occupiedColor = Color.red;
        [SerializeField] private Color _hoverColor = Color.yellow;

        private Image _backgroundImage;
        private InventoryService _inventoryService;
        private Vector2Int _gridPosition;

        public Vector2Int GridPosition => _gridPosition;

        public void Initialize(InventoryService inventoryService, Vector2Int gridPosition)
        {
            _inventoryService = inventoryService;
            _gridPosition = gridPosition;
            
            _backgroundImage = GetComponent<Image>();
            _backgroundImage.sprite = CM.Get(CMs.Gameplay.Inventory.InventoryGrid).GetComponent<SpriteComponent>().Sprite;
            
            UpdateVisual(_inventoryService.IsSlotOccupied(_gridPosition));
        }

        public void SetHighlight(bool needHighlight)
        {
            _backgroundImage.color = needHighlight ? _hoverColor : _normalColor;

        }

        public void UpdateVisual(bool isOccupied)
        {
            _backgroundImage.color = isOccupied ? _occupiedColor : _normalColor;
        }
    }
}