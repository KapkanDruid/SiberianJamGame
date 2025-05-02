using Game.Runtime.CMS;
using UnityEngine;

namespace Game.Runtime.Gameplay.HUD
{
    public class TestImplantBehaviour : InventoryItem
    {
        [SerializeField] private CMSPrefab ImplantModel;

        private void Start()
        {
            SetupItem(CM.Get(ImplantModel.EntityId));
        }
    }
}