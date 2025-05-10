using System;
using System.Collections.Generic;
using Game.Runtime.Gameplay.Implants;

namespace Game.Runtime.Gameplay.Inventory
{
    public class InventoryItemPairComparer : IEqualityComparer<(ImplantBehaviour, ImplantBehaviour)>
    {
        public bool Equals((ImplantBehaviour, ImplantBehaviour) x, (ImplantBehaviour, ImplantBehaviour) y)
        {
            return (x.Item1 == y.Item1 && x.Item2 == y.Item2) ||
                   (x.Item1 == y.Item2 && x.Item2 == y.Item1);
        }

        public int GetHashCode((ImplantBehaviour, ImplantBehaviour) obj)
        {
            int hash1 = obj.Item1.GetHashCode();
            int hash2 = obj.Item2.GetHashCode();
            return hash1 < hash2 ? HashCode.Combine(hash1, hash2) : HashCode.Combine(hash2, hash1);
        }
    }
}