using Game.Runtime.Gameplay;
using UnityEngine;

namespace Game.Runtime.CMS.Components.Configs
{
    public class EnemyPrefabComponent : CMSComponent
    {
        [SerializeField] private EnemyView _enemyView;
        public EnemyView EnemyView => _enemyView;
    }
}
