using Game.Runtime.CMS;
using UnityEngine;

namespace Game.Runtime.Gameplay.Test
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