using Game.Runtime.Gameplay.Grid;
using UnityEngine;

namespace Game.Runtime.CMS.Components.Gameplay
{
    public class InventoryItemComponent : CMSComponent
    {
        public Vector2 SizeDelta;
        public Vector2 Pivot;
        public GridConfig Grid;
    }
}