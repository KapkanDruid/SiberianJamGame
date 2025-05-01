using System.Collections.Generic;
using Game.Runtime.CMS;
using Game.Runtime.Gameplay.HUD;
using Game.Runtime.Services;
using UnityEngine;

namespace Game.Runtime.Gameplay.Unit.Brain
{
    public class BrainService : IService
    {
        public readonly BrainData BrainData;
        private readonly CMSEntity _defaultGridModel;

        public BrainService()
        {
            _defaultGridModel = CM.Get(CMs.Gameplay.DefaultGrid);
            BrainData = new BrainData();
        }

        public void SetBrainGrid(List<Vector2Int> brainGrid)
        {
            BrainData.CurrentGrid = brainGrid;
            SL.Get<HUDService>().SetBrainGrid(_defaultGridModel, brainGrid);
        }
    }
}