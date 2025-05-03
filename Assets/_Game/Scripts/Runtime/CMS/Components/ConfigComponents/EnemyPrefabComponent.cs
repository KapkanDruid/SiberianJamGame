using Game.Runtime.Gameplay.Enemy;
using UnityEngine;

namespace Game.Runtime.CMS.Components.Configs
{
    public class EnemyPrefabComponent : CMSComponent
    {
        [SerializeField] private EnemyView _enemyView;
        public EnemyView EnemyView => _enemyView;
    }
}
