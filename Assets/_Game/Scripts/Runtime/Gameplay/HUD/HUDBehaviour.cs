using UnityEngine;
using UnityEngine.UI;

namespace Game.Runtime.Gameplay.HUD
{
    public class HUDBehaviour : MonoBehaviour
    {
        [SerializeField] private InventoryView inventoryView;
        [SerializeField] private WarriorUI warriorUI;
        [SerializeField] private EnemyUI enemyUI;
        [SerializeField] private Button endTurnButton;
        
        public InventoryView InventoryView => inventoryView;
        public WarriorUI WarriorUI => warriorUI;
        public EnemyUI EnemyUI => enemyUI;
        public Button EndTurnButton => endTurnButton;
    }
}