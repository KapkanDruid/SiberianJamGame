using System.Collections.Generic;
using System.Linq;
using Game.Runtime.CMS;
using Game.Runtime.CMS.Components.Gameplay;
using Game.Runtime.Gameplay.HUD;
using Game.Runtime.Gameplay.Level;
using Game.Runtime.Services;
using UnityEngine;

namespace Game.Runtime.Gameplay.Inventory
{
    public class InventoryGrid
    {
        private Vector2Int _gridSize;
        private int _cellSize;
        
        public Dictionary<Vector2Int, InventorySlot> Slots { get; } = new();
        
        public void CreateGrid()
        {
            var inventoryView = ServiceLocator.Get<HUDService>().InventoryView;
            
            var defaultInventoryGrid = CM.Get(CMs.Gameplay.Inventory).GetComponent<InventoryComponent>();
            var currentGrid = defaultInventoryGrid.Grids.Last(grid => grid.RequiredLevel <= ServiceLocator.Get<GameStateHolder>().CurrentData.Level).Grid;
            
            _gridSize = currentGrid.GridSize;
            _cellSize = defaultInventoryGrid.CellSize;

            inventoryView.ResizeInventoryView(_gridSize, _cellSize);

            foreach (var slotPos in currentGrid.GridPattern)
            {
                var slotObj = new GameObject($"InventorySlot_{slotPos.x}_{slotPos.y}");
                var slot = slotObj.AddComponent<InventorySlot>();

                inventoryView.SetupInventorySlot(slotObj, slotPos, _cellSize);

                slot.Initialize(slotPos);
                Slots[slotPos] = slot;
            }
        }
    }
}