using UnityEngine;

namespace Game.Runtime.CMS.Components.Configs
{
    public class EnemyConfig : CMSComponent
    {
        [SerializeField] private float _maxHealth;
        [SerializeField] private float _damage;
        [SerializeField] private float _armor;

        public float MaxHealth => _maxHealth;
        public float Damage => _damage;
        public float Armor => _armor; 
    }
}
