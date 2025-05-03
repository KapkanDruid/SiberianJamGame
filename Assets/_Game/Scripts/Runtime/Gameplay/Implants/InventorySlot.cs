using Game.Runtime.CMS;
using Game.Runtime.CMS.Components.Commons;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Runtime.Gameplay.Implants
{
    [RequireComponent(typeof(Image))]
    public class InventorySlot : MonoBehaviour
    {
        private Image _backgroundImage;
        private InventoryService _inventoryService;
        private Vector2Int _gridPosition;

        public bool Occupied { get; private set; }
        public Vector2Int GridPosition => _gridPosition;

        public void Initialize( Vector2Int gridPosition)
        {
            _gridPosition = gridPosition;
            
            _backgroundImage = GetComponent<Image>();
            _backgroundImage.sprite = CM.Get(CMs.Gameplay.Inventory).GetComponent<SpriteComponent>().Sprite;
        }

        public void SetOccupied(bool isOccupied)
        {
            Occupied = isOccupied;

            var occupiedColor = Color.gray;
            occupiedColor.a = 0.5f;
            
            SetColor(isOccupied ? occupiedColor : Color.white);
        }

        public void SetColor(Color color)
        {
            _backgroundImage.color = color;
        }
    }
}