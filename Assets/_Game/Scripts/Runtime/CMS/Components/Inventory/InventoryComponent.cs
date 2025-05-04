using System;
using Game.Runtime.Gameplay.Grid;

namespace Game.Runtime.CMS.Components.Gameplay
{
    public class InventoryComponent : CMSComponent
    {
        public int CellSize;
        public GridByLevel[] Grids;
    }

    [Serializable]
    public class GridByLevel
    {
        public int RequiredLevel;
        public GridConfig Grid;
    }
}