using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Game.Runtime.CMS;
using Game.Runtime.CMS.Components.Commons;
using Game.Runtime.CMS.Components.Gameplay;
using Game.Runtime.CMS.Components.Implants;
using Game.Runtime.Gameplay.HUD;
using Game.Runtime.Gameplay.Level;
using Game.Runtime.Services;
using Game.Runtime.Services.Save;
using Game.Runtime.Utils.Extensions;
using UnityEngine;

namespace Game.Runtime.Gameplay.Implants.Services
{
    public class LootService : IService
    {
        private readonly ImplantsPoolConfig _config;
        private readonly List<CMSEntity> _allImplants;
        private readonly List<ImplantBehaviour> _lootImplants = new();
        
        private int needChoice;

        public LootService()
        {
            _config = CM.Get(CMs.Gameplay.ImplantsPoolConfig).GetComponent<ImplantsPoolConfig>();
            _allImplants = CM.GetAll<InventoryItemComponent>();
        }

        public void GenerateLoot()
        {
            ServiceLocator.Get<HUDService>().EndTurnButton.gameObject.SetActive(false);
            ServiceLocator.Get<HUDService>().InventoryView.SetActive(false);
            ServiceLocator.Get<HUDService>().LootHolder.SetActive(true);

            var availableImplants = new List<CMSEntity>();
            
            foreach (var implant in _allImplants)
            {
                if (implant.GetComponent<ImplantLevelRequiredComponent>().RequiredLevelIndex <= ServiceLocator.Get<GameStateHolder>().CurrentData.Level)
                {
                    if (implant.GetComponent<ImplantLevelRequiredComponent>().RequiredLevelIndex == 0 && ServiceLocator.Get<GameStateHolder>().CurrentData.Level > 0)
                        continue;
                    
                    availableImplants.Add(implant);
                }
            }

            availableImplants.Shuffle();
            
            for (int i = 0; i < _config.BaseLootCount; i++)
            {
                var implantPrefab = CM.Get(CMs.Gameplay.Implants.BaseImplantBehaviour).GetComponent<PrefabComponent>().Prefab;
                var implantBehaviour = Object.Instantiate(implantPrefab).GetComponent<ImplantBehaviour>();
                
                implantBehaviour.SetupItem(availableImplants.GetRandom(), ServiceLocator.Get<HUDService>().GetComponent<RectTransform>());
                implantBehaviour.CanInteract = true;
                
                _lootImplants.Add(implantBehaviour);
            }
            
            ServiceLocator.Get<HUDService>().LootHolder.SetImplants(_lootImplants, true).Forget();
            needChoice = _config.BaseChoiceCount;
        }

        public void Choice(string implantId)
        {
            if (needChoice <= 0) return;
            
            ServiceLocator.Get<GameStateHolder>().CurrentData.ImplantsPool.Add(implantId);
            
            _lootImplants.RemoveAll(implant => implant.Model.EntityId == implantId);
            ServiceLocator.Get<HUDService>().LootHolder.SetImplants(_lootImplants, true).Forget();

            needChoice--;

            if (needChoice == 0) ServiceLocator.Get<BattleController>().EndGameAsync().Forget();
        }
    }
}