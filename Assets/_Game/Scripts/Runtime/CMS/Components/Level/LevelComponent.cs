using UnityEngine;

namespace Game.Runtime.CMS.Components.Level
{
    public class LevelComponent : CMSComponent
    {
        public int LevelIndex;
        public Sprite BackgroundSprite;
        public CMSPrefab EnemyPrefab;
        public CMSPrefab DeathDialog;
        public CMSPrefab NextSceneDialog;
    }
}