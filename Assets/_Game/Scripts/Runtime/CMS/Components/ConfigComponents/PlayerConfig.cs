using UnityEngine;

namespace Game.Runtime.CMS.Components.Configs
{
    public class PlayerConfig : CMSComponent
    {
        [SerializeField] private float _maxHealth;

        public float MaxHealth => _maxHealth;
    }
}
