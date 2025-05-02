using Game.Runtime.Gameplay;
using UnityEngine;

namespace Game.Runtime.CMS.Components.Configs
{
    public class EnemyPrefabComponent : CMSComponent
    {
        [SerializeField] private EnemyController _enemyController;
        public EnemyController EnemyController => _enemyController;
    }
}
