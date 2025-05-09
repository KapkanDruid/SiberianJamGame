using System;
using System.Collections.Generic;
using Game.Runtime.CMS;
using Game.Runtime.CMS.Components.Commons;
using Game.Runtime.Gameplay.HUD;
using Game.Runtime.Gameplay.Level;
using Game.Runtime.Services;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Game.Runtime.Gameplay.Implants.Services
{
    public class ImplantsHolderService : IService, IInitializable, IDisposable
    {
        private readonly List<ImplantBehaviour> _implants = new();
        private ImplantsHolder _implantsHolder;
            
        public void Initialize()
        {
            _implantsHolder = ServiceLocator.Get<HUDService>().ImplantsHolder;
            ServiceLocator.Get<BattleController>().OnTurnEnded += OnTurnEnded;
        }
        
        public void SpawnImplants()
        {
            foreach (var implantModel in ServiceLocator.Get<ImplantsPoolService>().GetImplants())
            {
                var implantPrefab = CM.Get(CMs.Gameplay.Implants.BaseImplantBehaviour).GetComponent<PrefabComponent>().Prefab;
                var implantBehaviour = Object.Instantiate(implantPrefab).GetComponent<ImplantBehaviour>();
                implantBehaviour.SetupItem(implantModel, ServiceLocator.Get<HUDService>().GetComponent<RectTransform>());
                
                _implants.Add(implantBehaviour);
                _implantsHolder.SetItemPosition(implantBehaviour, Vector2.zero);
            }
        }

        public void RemoveItem(ImplantBehaviour implantBehaviour)
        {
            _implants.Remove(implantBehaviour);
        }

        public bool HasItem(ImplantBehaviour implantBehaviour)
        {
            return _implants.Contains(implantBehaviour);
        }

        public bool TryReturnToHolder(ImplantBehaviour implantBehaviour, Vector2 position)
        {
            if (_implants.Contains(implantBehaviour)) 
                return false;
            
            if (!_implantsHolder.IsInsideHolder(position))
                return false;

            _implants.Add(implantBehaviour);

            _implantsHolder.SetItemPosition(implantBehaviour, position);
            return true;
        }
        
        private void OnTurnEnded()
        {
            foreach (var item in _implants)
                item.ReleaseImplant();
            _implants.Clear();
            
            if (!ServiceLocator.Get<BattleController>().IsBattleEnded)
                SpawnImplants();
        }
        
        public void Dispose()
        {
            ServiceLocator.Get<BattleController>().OnTurnEnded -= OnTurnEnded;
        }
    }
}