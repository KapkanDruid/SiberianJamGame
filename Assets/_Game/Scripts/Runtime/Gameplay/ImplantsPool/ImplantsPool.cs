using System.Collections.Generic;
using Game.Runtime.CMS;
using Game.Runtime.CMS.Components.Gameplay;
using Game.Runtime.CMS.Components.Implants;
using Game.Runtime.Services;
using Game.Runtime.Utils.Extensions;

namespace Game.Runtime.Gameplay.ImplantsPool
{
    public class ImplantPoolState
    {
        public readonly string ImplantId;

        public ImplantPoolState(string implantId)
        {
            ImplantId = implantId;
        }
    }
    
    public class ImplantsPool : IService
    {
        private readonly ImplantsPoolConfig _config;
        private readonly List<CMSEntity> _basicImplantModels;
        private readonly List<ImplantPoolState> _implantPool;

        public ImplantsPool()
        {
            _implantPool = new List<ImplantPoolState>();
            _config = CM.Get(CMs.Gameplay.ImplantsPoolConfig).GetComponent<ImplantsPoolConfig>();
            
            _basicImplantModels = new List<CMSEntity>();
            foreach (var implant in CM.GetAll<InventoryItemComponent>())
            {
                if (implant.GetComponent<ImplantLevelRequiredComponent>().RequiredLevelIndex == 0)
                    _basicImplantModels.Add(implant);
            }
        }

        public void AddImplants(List<string> implantIds)
        {
            foreach (var implantId in implantIds)
                _implantPool.Add(new ImplantPoolState(implantId));
        }

        public List<CMSEntity> GetImplants()
        {
            var result = new List<CMSEntity>();

            for (var i = 0; i < _config.BaseDeckCount; i++)
            {
                if (_implantPool.Count == 0)
                {
                    result.Add(_basicImplantModels.GetRandom());
                }
                else
                {
                    result.Add(CM.Get(_implantPool.Pop().ImplantId));
                }
            }

            return result;
        }
        
        public bool HasImplant(string implantId)
        {
            foreach (var implant in _implantPool)
            {
                if (implant.ImplantId == implantId)
                    return true;
            }
              
            return false;
        }
    }
}