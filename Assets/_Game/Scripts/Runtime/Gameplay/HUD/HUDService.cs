using Game.Runtime.CMS;
using Game.Runtime.CMS.Components.Commons;
using Game.Runtime.Gameplay.Grid;
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

        public void SetBrainGrid(GridData gridData)
        {
            _hudBehaviour.SetBrainGrid(gridData);
        }
    }
}