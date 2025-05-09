﻿using System;
using System.Collections.Generic;
using Game.Runtime.CMS;
using Game.Runtime.CMS.Components.Gameplay;
using Game.Runtime.CMS.Components.Implants;
using Game.Runtime.Gameplay.Level;
using Game.Runtime.Services;
using Game.Runtime.Utils.Extensions;

namespace Game.Runtime.Gameplay.Implants.Services
{
    public class ImplantsPoolService : IService
    {
        private readonly ImplantsPoolConfig _config;
        private readonly List<CMSEntity> _basicImplantModels;
        private readonly List<string> _dynamicImplantPool;

        public ImplantsPoolService()
        {
            _dynamicImplantPool = new List<string>();
            _config = CM.Get(CMs.Gameplay.ImplantsPoolConfig).GetComponent<ImplantsPoolConfig>();
            _basicImplantModels = new List<CMSEntity>();
            foreach (var implant in CM.GetAll<InventoryItemComponent>())
            {
                if (implant.GetComponent<ImplantLevelRequiredComponent>().RequiredLevelIndex == 0)
                    _basicImplantModels.Add(implant);
            }
        }
        
        public void CachePool()
        {
            _dynamicImplantPool.Clear();
            _dynamicImplantPool.AddRange(ServiceLocator.Get<GameStateHolder>().CurrentData.ImplantsPool);
            _dynamicImplantPool.Shuffle();
        }

        public List<CMSEntity> GetImplants()
        {
            var result = new List<CMSEntity>();

            for (var i = 0; i < _config.BaseDeckCount; i++)
            {
                if (_dynamicImplantPool.Count == 0)
                {
                    result.Add(CM.Get(_basicImplantModels.GetRandom().EntityId));
                }
                else
                {
                    var implant = _dynamicImplantPool.GetRandom();
                    result.Add(CM.Get(implant));
                    _dynamicImplantPool.Remove(implant);
                }
            }

            return result;
        }
    }
}