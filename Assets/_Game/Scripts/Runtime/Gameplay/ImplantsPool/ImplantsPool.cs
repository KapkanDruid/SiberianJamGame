using System.Collections.Generic;
using Game.Runtime.CMS;
using Game.Runtime.Services;

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
        private readonly List<ImplantPoolState> _implantPool = new();

        public void InitializePool()
        {
        }
        
        public bool HasDice(string implantId)
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