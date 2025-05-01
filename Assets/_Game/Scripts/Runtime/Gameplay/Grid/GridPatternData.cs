using UnityEngine;
using System.Collections.Generic;

namespace Game.Runtime.Gameplay.Grid
{
    [CreateAssetMenu(fileName = "GridPattern", menuName = "Gameplay/GridPattern")]
    public class GridPatternData : ScriptableObject
    {
        [SerializeField] private List<Vector2Int> _gridPattern;
        public List<Vector2Int> GridPattern => _gridPattern;

        public void ClearPattern()
        {
            _gridPattern = null;
        }

        public string GetPositionsString()
        {
            return string.Join(" ", _gridPattern);
        }
    }
}