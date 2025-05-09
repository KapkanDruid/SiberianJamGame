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
        private Vector2 gridOffset;
        private Vector2Int _gridSize;
        private int _cellSize;
        
        public Dictionary<Vector2Int, InventorySlot> Slots { get; } = new();
        
        public void CreateGrid()
        {
            var inventoryView = ServiceLocator.Get<HUDService>().InventoryView;
            
            var defaultInventoryGrid = CM.Get(CMs.Gameplay.Inventory).GetComponent<InventoryComponent>();
            var currentGrid = defaultInventoryGrid.Grids.Last(grid => grid.RequiredLevel <= ServiceLocator.Get<GameStateHolder>().CurrentData.Level);
            
            var gridPattern = currentGrid.Grid.GridPattern;
            _gridSize = CalculateGridSize(gridPattern);
            _cellSize = defaultInventoryGrid.CellSize;
            gridOffset = new Vector2((currentGrid.Grid.GridSize.x - _gridSize.x) /2f, 
                (currentGrid.Grid.GridSize.y - _gridSize.y) /2f);
            
            inventoryView.ResizeInventoryView(_gridSize, _cellSize, currentGrid.RectPosition);

            foreach (var slotPos in gridPattern)
            {
                var slotObj = new GameObject($"InventorySlot_{slotPos.x}_{slotPos.y}");
                var slot = slotObj.AddComponent<InventorySlot>();

                inventoryView.SetupInventorySlot(gridOffset, slotObj, slotPos, _cellSize);
                
                slot.Initialize(slotPos);
                Slots[slotPos] = slot;
            }
        }
        
        private List<Vector2Int> NormalizeSlots(List<Vector2Int> slots)
        {
            if (slots == null || slots.Count == 0)
                return new List<Vector2Int>();

            int minX = slots[0].x;
            int minY = slots[0].y;

            foreach (var slot in slots)
            {
                if (slot.x < minX) minX = slot.x;
                if (slot.y < minY) minY = slot.y;
            }

            List<Vector2Int> normalizedSlots = new List<Vector2Int>();
            foreach (var slot in slots)
                normalizedSlots.Add(new Vector2Int(slot.x - minX, slot.y - minY));

            return normalizedSlots;
        }
        
        private Vector2Int CalculateGridSize(List<Vector2Int> slots)
        {
            if (slots == null || slots.Count == 0)
                return Vector2Int.zero;

            int minX = slots[0].x;
            int maxX = slots[0].x;
            int minY = slots[0].y;
            int maxY = slots[0].y;

            foreach (var slot in slots)
            {
                if (slot.x < minX) minX = slot.x;
                if (slot.x > maxX) maxX = slot.x;
                if (slot.y < minY) minY = slot.y;
                if (slot.y > maxY) maxY = slot.y;
            }

            int width = maxX - minX + 1;
            int height = maxY - minY + 1;

            return new Vector2Int(width, height);
        }
    }
}