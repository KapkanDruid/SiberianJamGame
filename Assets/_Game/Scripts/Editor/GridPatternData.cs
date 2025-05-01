using UnityEngine;
using System.Collections.Generic;

namespace Game.Editor
{
    [CreateAssetMenu(fileName = "GridPattern", menuName = "_Game/GridPattern")]
    public class GridPatternData : ScriptableObject
    {
        [SerializeField] private List<Vector2Int> _gridPattern;
        public List<Vector2Int> GridPattern => _gridPattern;

        public void SetPattern(List<Vector2Int> positions)
        {
            var center = Vector2Int.zero;

            if (positions.Contains(center))
                positions.Remove(center);

            _gridPattern = positions;
        }

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