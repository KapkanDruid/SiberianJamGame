using System.Collections.Generic;
using UnityEngine;

namespace Game.Runtime.CMS.Components.Configs
{
    public class EnemyPrefabs : CMSComponent
    {
        [SerializeField] private List<CMSPrefab> _enemyConfigs;
        public List<CMSPrefab> EnemyConfigPrefabs => _enemyConfigs; 
    }
}
