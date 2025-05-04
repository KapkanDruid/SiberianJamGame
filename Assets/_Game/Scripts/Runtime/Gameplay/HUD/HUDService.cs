using Game.Runtime.CMS;
using Game.Runtime.CMS.Components.Commons;
using Game.Runtime.Services;
using Game.Runtime.Services.Camera;
using UnityEngine;

namespace Game.Runtime.Gameplay.HUD
{
    public class HUDService : IService, IInitializable
    {
        public readonly HUDBehaviour Behaviour;

        public HUDService()
        {
            var uiPrefab = CM.Get(CMs.Gameplay.HUD).GetComponent<PrefabComponent>().Prefab;
            var uiObject = Object.Instantiate(uiPrefab);
            uiObject.name = nameof(HUDService);
            
            Behaviour = uiObject.GetComponent<HUDBehaviour>();
        }

        public void Initialize()
        {
            var canvas = Behaviour.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = SL.Get<CameraService>().Camera;
        }
    }
}