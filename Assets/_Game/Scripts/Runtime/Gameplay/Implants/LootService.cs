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

namespace Game.Runtime.Gameplay.Implants
{
    public class LootService : IService
    {
        private readonly ImplantsPoolConfig _config;
        private readonly List<CMSEntity> _allImplants;

        private int needChoice;

        public LootService()
        {
            _config = CM.Get(CMs.Gameplay.ImplantsPoolConfig).GetComponent<ImplantsPoolConfig>();
            _allImplants = CM.GetAll<InventoryItemComponent>();
        }

        public void GenerateLoot()
        {
            SL.Get<HUDService>().Behaviour.EndTurnButton.gameObject.SetActive(false);
            SL.Get<HUDService>().Behaviour.InventoryView.SetActive(false);
            SL.Get<HUDService>().Behaviour.LootHolder.SetActive(true);

            var availableImplants = new List<CMSEntity>();
            
            foreach (var implant in _allImplants)
            {
                if (implant.GetComponent<ImplantLevelRequiredComponent>().RequiredLevelIndex <= SL.Get<GameStateHolder>().CurrentLevel)
                {
                    if (implant.GetComponent<ImplantLevelRequiredComponent>().RequiredLevelIndex == 0 && SL.Get<GameStateHolder>().CurrentLevel > 0)
                        continue;
                    
                    availableImplants.Add(implant);
                }
            }

            availableImplants.Shuffle();

            for (int i = 0; i < _config.BaseLootCount; i++)
            {
                var implantPrefab = CM.Get(CMs.Gameplay.Implants.BaseImplantBehaviour).GetComponent<PrefabComponent>().Prefab;
                var implantBehaviour = Object.Instantiate(implantPrefab).GetComponent<ImplantBehaviour>();
                implantBehaviour.SetupItem(availableImplants.GetRandom(), SL.Get<HUDService>().Behaviour.GetComponent<Canvas>());
            
                SL.Get<HUDService>().Behaviour.LootHolder.SetItemPosition(implantBehaviour, Vector2.zero);
            }

            needChoice = _config.BaseChoiceCount;
        }

        public void Choice(string implantId)
        {
            if (needChoice <= 0) return;
            
            SL.Get<ImplantsPoolService>().AddImplant(implantId);
            needChoice--;

            if (needChoice == 0)
            {
                SL.Get<BattleController>().EndGameAsync().Forget();
            }
        }
    }
}