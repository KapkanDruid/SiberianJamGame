using System.Collections.Generic;
using System.Linq;
using Game.Runtime.CMS.Components.Implants;
using UnityEngine;

namespace Game.Runtime.Gameplay.Implants
{
    public class InventoryStats
    {
        private readonly InventoryService _inventoryService;
        private readonly Vector2Int[] _neighborOffsets = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
        private readonly HashSet<(ImplantBehaviour, ImplantBehaviour)> _implantBehaviours = new(new ImplantsPairComparer());
        private readonly List<Vector2Int> _synergySlots = new();

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
        
        public List<Vector2Int> FindSynergySlots(ImplantBehaviour implant)
        {
            _synergySlots.Clear();
            
            foreach (var itemPair in _inventoryService.ItemPositions)
            {
                var item = itemPair.Key;
                if (item.Equals(implant)) continue;
                
                var implantType = item.GetImplantType();
                if (implantType != implant.GetImplantType()) 
                    continue;

                foreach (var itemPosition in itemPair.Value)
                {
                    if (IsPositionBlocked(item, itemPosition))
                        continue;

                    foreach (var offset in _neighborOffsets)
                    {
                        var neighborPos = itemPosition + offset;

                        if (_inventoryService.OccupiedSlots.TryGetValue(neighborPos, out _))
                            continue;

                        if (_inventoryService.IsSlotExist(neighborPos))
                            _synergySlots.Add(neighborPos);
                    }
                }
            }

            return _synergySlots;
        }

        public void UpdateStatsMap()
        {
            _implantBehaviours.Clear();
            
            float health = 0f;
            float damage = 0f;
            float armor = 0f;

            foreach (var itemPair in _inventoryService.ItemPositions)
            {
                var item = itemPair.Key;
                var implantType = item.GetImplantType();

                if (item.Model.Is<HealthImplantComponent>(out var healthImplant))
                    health += healthImplant.Health;
                else if (item.Model.Is<DamageImplantComponent>(out var damageImplant))
                    damage += damageImplant.Damage;
                else if (item.Model.Is<ArmorImplantComponent>(out var armorImplant))
                    armor += armorImplant.Armor;

                foreach (var itemPosition in itemPair.Value)
                {
                    if (IsPositionBlocked(item, itemPosition))
                        continue;

                    foreach (var offset in _neighborOffsets)
                    {
                        var neighborPos = itemPosition + offset;

                        if (!_inventoryService.OccupiedSlots.TryGetValue(neighborPos, out var neighborItem) || neighborItem.Equals(item))
                            continue;

                        if (IsPositionBlocked(neighborItem, neighborPos))
                            continue;

                        if (neighborItem.GetImplantType() == implantType &&
                            _implantBehaviours.Add((item, neighborItem)))
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
        }
        
        private bool IsPositionBlocked(ImplantBehaviour implant, Vector2Int position)
        {
            if (!implant.Model.Is<BrokenImplantCellsComponent>(out var blockComponent))
                return false;

            var rotatedCell = InventoryHelper.GetRotatedSlots(blockComponent.BrokenCells.ToList(), implant.CurrentRotation);
            var globalBlockCell = rotatedCell.Select(x => x + implant.CenterSlotPosition);
            
            return globalBlockCell.Contains(position);
        }
    }
}