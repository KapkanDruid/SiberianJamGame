using UnityEngine;
using System.Collections.Generic;

namespace Game.Runtime.Gameplay.Grid
{
    [CreateAssetMenu(fileName = "GridConfig", menuName = "Gameplay/GridConfig")]
    public class GridConfig : ScriptableObject
    {
        [SerializeField] private List<Vector2Int> gridPattern;
        [SerializeField] private Vector2Int gridSize;
        
        public Vector2Int GridSize => gridSize;
        public List<Vector2Int> GridPattern => gridPattern;

        public void ClearPattern()
        {
            gridPattern = null;
        }

        public string GetPositionsString()
        {
            return string.Join(" ", gridPattern);
        }
    }
}