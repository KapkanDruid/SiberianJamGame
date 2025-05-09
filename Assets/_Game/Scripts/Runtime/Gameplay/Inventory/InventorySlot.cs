using Game.Runtime.CMS;
using Game.Runtime.CMS.Components.Commons;
using Game.Runtime.CMS.Components.Implants;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Runtime.Gameplay.Inventory
{
    [RequireComponent(typeof(Image))]
    public class InventorySlot : MonoBehaviour
    {
        public Color DefaultColor { get; private set; }
        public Color OccupiedColor { get; private set; }
            
        private Image _backgroundImage;
        private InventoryService _inventoryService;
        private Vector2Int _gridPosition;

        public Vector2Int GridPosition => _gridPosition;

        public void Initialize( Vector2Int gridPosition)
        {
            _gridPosition = gridPosition;
            
            _backgroundImage = GetComponent<Image>();
            _backgroundImage.sprite = CM.Get(CMs.Gameplay.Inventory).GetComponent<SpriteComponent>().Sprite;

            var inventorySlotColorsComponent = CM.Get(CMs.Gameplay.Inventory).GetComponent<InventorySlotColorsComponent>();
            DefaultColor = inventorySlotColorsComponent.DefaultColor;
            OccupiedColor = inventorySlotColorsComponent.OccupiedColor;
            
            SetColor(DefaultColor);
        }

        public void SetColor(Color color)
        {
            _backgroundImage.color = color;
        }
    }
}