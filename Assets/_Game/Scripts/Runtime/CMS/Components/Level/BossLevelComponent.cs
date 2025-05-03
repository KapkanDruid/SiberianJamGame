using Game.Runtime.Gameplay.Enemy;
using UnityEngine;

namespace Game.Runtime.CMS.Components.Level
{
    public class BossLevelComponent : CMSComponent
    {
        public float FirstHitDamage;
        public float SecondHitDamage;
        public float Armor;
        public float Health;
        public float Heal;
        public BossView BossViewPrefab;
    }
}