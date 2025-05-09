using System;
using System.Collections.Generic;
using Game.Runtime.CMS.Components.Implants;
using Game.Runtime.Gameplay.Implants;

namespace Game.Runtime.Gameplay.Inventory
{
    public class InventoryStats
    {
        public event Action OnImplantStatsRecalculated;

        private readonly InventoryService _inventoryService;
        private readonly HashSet<(ImplantBehaviour, ImplantBehaviour)> _implantBehaviours = new(new InventoryItemPairComparer());

        public Dictionary<ImplantType, float> StatsMap { get; } = new()
        {
            { ImplantType.Health, 0 },
            { ImplantType.Armor, 0 },
            { ImplantType.Damage, 0 }
        };

        public InventoryStats(InventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        public void RecalculateImplantStats()
        {
            _implantBehaviours.Clear();
            
            float health = 0f;
            float damage = 0f;
            float armor = 0f;

            foreach (var itemPair in _inventoryService.ItemPositions)
            {
                var item = itemPair.Key;

                if (item.Model.Is<HealthImplantComponent>(out var healthImplant))
                    health += healthImplant.Health;
                else if (item.Model.Is<DamageImplantComponent>(out var damageImplant))
                    damage += damageImplant.Damage;
                else if (item.Model.Is<ArmorImplantComponent>(out var armorImplant))
                    armor += armorImplant.Armor;

                foreach (var itemPosition in itemPair.Value)
                {
                    if (InventoryHelper.IsPositionBlocked(item, item.CenterSlotPosition, itemPosition))
                        continue;

                    foreach (var offset in InventoryHelper.NeighborOffsets)
                    {
                        var neighborPos = itemPosition + offset;

                        if (!_inventoryService.OccupiedSlots.TryGetValue(neighborPos, out var neighborItem) || neighborItem.Equals(item))
                            continue;

                        if (InventoryHelper.IsPositionBlocked(neighborItem, neighborItem.CenterSlotPosition, neighborPos))
                            continue;
                        
                        var implantType = item.GetImplantType();
                        if (neighborItem.GetImplantType() == implantType && _implantBehaviours.Add((item, neighborItem)))
                        {
                            switch (implantType)
                            {
                                case ImplantType.Health:
                                    health += 10;
                                    break;
                                case ImplantType.Damage:
                                    damage += 10;
                                    break;
                                case ImplantType.Armor:
                                    armor += 10;
                                    break;
                            }
                        }
                    }
                }
            }

            StatsMap[ImplantType.Health] = health;
            StatsMap[ImplantType.Damage] = damage;
            StatsMap[ImplantType.Armor] = armor;
            
            OnImplantStatsRecalculated?.Invoke();
        }
    }
}