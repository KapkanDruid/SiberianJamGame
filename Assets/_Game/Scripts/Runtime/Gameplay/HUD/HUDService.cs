using System.Collections.Generic;
using Game.Runtime.CMS;
using Game.Runtime.CMS.Components.Commons;
using Game.Runtime.Services;
using UnityEngine;

namespace Game.Runtime.Gameplay.HUD
{
    public class HUDService : IService
    {
        private readonly HUDBehaviour _hudBehaviour;

        public HUDService()
        {
            var uiPrefab = CM.Get(CMs.Gameplay.HUD).GetComponent<PrefabComponent>().Prefab;
            var uiObject = Object.Instantiate(uiPrefab);
            uiObject.name = nameof(HUDService);
            
            _hudBehaviour = uiObject.GetComponent<HUDBehaviour>();
        }

        public void SetBrainGrid(CMSEntity defaultGridModel, List<Vector2Int> brainGrid)
        {
            _hudBehaviour.SetBrainGrid(defaultGridModel, brainGrid);
        }
    }
}