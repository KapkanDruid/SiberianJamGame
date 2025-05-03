using System;
using System.Collections.Generic;
using Game.Runtime.CMS;
using Game.Runtime.CMS.Components.Commons;
using Game.Runtime.Gameplay.HUD;
using Game.Runtime.Gameplay.Level;
using Game.Runtime.Services;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Game.Runtime.Gameplay.Implants
{
    public class ImplantsHolderService : IService, IInitializable, IDisposable
    {
        private readonly List<ImplantBehaviour> _implants = new();
            
        public void Initialize()
        {                
            SL.Get<BattleController>().OnTurnEnded += OnTurnEnded;
        }

        public void SpawnImplants()
        {
            foreach (var implantModel in SL.Get<ImplantsPoolService>().GetImplants())
            {
                var implantPrefab = CM.Get(CMs.Gameplay.Implants.BaseImplantBehaviour).GetComponent<PrefabComponent>().Prefab;
                var implantBehaviour = Object.Instantiate(implantPrefab).GetComponent<ImplantBehaviour>();
                implantBehaviour.SetupItem(implantModel, SL.Get<HUDService>().Behaviour.GetComponent<RectTransform>());
                
                _implants.Add(implantBehaviour);
                SL.Get<HUDService>().Behaviour.ImplantsHolder.SetItemPosition(implantBehaviour, Vector2.zero);
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
            
            if (!SL.Get<HUDService>().Behaviour.ImplantsHolder.IsInsideHolder(position))
                return false;

            _implants.Add(implantBehaviour);

            SL.Get<HUDService>().Behaviour.ImplantsHolder.SetItemPosition(implantBehaviour, position);
            return true;
        }
        
        private void OnTurnEnded()
        {
            foreach (var item in _implants)
            {
                Object.Destroy(item.gameObject);
            }
            
            _implants.Clear();
            
            if (!SL.Get<BattleController>().IsBattleEnded)
                SpawnImplants();
        }
        
        public void Dispose()
        {
            SL.Get<BattleController>().OnTurnEnded -= OnTurnEnded;
        }
    }
}