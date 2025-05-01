using System;
using UnityEngine;

namespace Game.Runtime.Gameplay.Grid
{
    [Serializable]
    public class GridData
    {
        public float CellSize;
        public float CellScale;
        public Sprite CellSprite;
        public GridPatternData gridPatternData;
    }
}