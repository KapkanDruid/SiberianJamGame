using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Game.Runtime.CMS;
using Game.Runtime.CMS.Components.Commons;
using Game.Runtime.Gameplay.HUD;
using Game.Runtime.Gameplay.Level;
using Game.Runtime.Services;
using UnityEngine;

namespace Game.Runtime.Gameplay.Implants.Services
{
    public class ImplantsGameLoop : IService, IInitializable, IDisposable
    {
        public event Action OnNextTurnStarted;
        
        private readonly List<ImplantBehaviour> _nextImplants = new();

        public bool IsGlobalDragging;

        public void Initialize()
        {
            ServiceLocator.Get<BattleController>().OnTurnEnded += OnTurnEnded;
        }
        
        public void StartLevel()
        {
            UpdateImplants(_nextImplants.Count == 0 ? CreateImplants() : _nextImplants, true).Forget();
        }

        private async UniTask UpdateImplants(List<ImplantBehaviour> implants, bool force = false)
        {
            await ServiceLocator.Get<ImplantsHolderService>().ReleaseImplants();
            
            implants.ForEach(implant => implant.CanInteract = true);
            await ServiceLocator.Get<ImplantsHolderService>().SetImplantsAsync(implants, force);
            
            CalculateNextImplants();
            OnNextTurnStarted?.Invoke();
        }
        
        private void CalculateNextImplants()
        {
            _nextImplants.Clear();
            _nextImplants.AddRange(CreateImplants());
            _nextImplants.ForEach(implant => implant.CanInteract = false);
            ServiceLocator.Get<HUDService>().TVPanel.SetImplants(_nextImplants);
        }
        
        private List<ImplantBehaviour> CreateImplants()
        {
            var result = new List<ImplantBehaviour>();
            
            foreach (var implantModel in ServiceLocator.Get<ImplantsPoolService>().GetImplants())
            {
                var implantPrefab = CM.Get(CMs.Gameplay.Implants.BaseImplantBehaviour).GetComponent<PrefabComponent>().Prefab;
                var implantBehaviour = UnityEngine.Object.Instantiate(implantPrefab).GetComponent<ImplantBehaviour>();
                implantBehaviour.SetupItem(implantModel, ServiceLocator.Get<HUDService>().GetComponent<RectTransform>());
                
                result.Add(implantBehaviour);
            }

            return result;
        }

        private void OnTurnEnded()
        {
            if (!ServiceLocator.Get<BattleController>().IsBattleEnded)
            {
                UpdateImplants(_nextImplants).Forget();
            }
            else
            {
                ServiceLocator.Get<ImplantsHolderService>().ReleaseImplants().Forget();

                foreach (var implant in _nextImplants) 
                    implant.ReleaseImplant().Forget();
                
                _nextImplants.Clear();
            }
        }

        public void Dispose()
        {
            ServiceLocator.Get<BattleController>().OnTurnEnded -= OnTurnEnded;
        }
    }
}