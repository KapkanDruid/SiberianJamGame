using Game.Runtime.CMS;
using Game.Runtime.CMS.Components.Level;

namespace Game.Runtime.Gameplay.Level
{
    public static class LevelHelper
    {
        public static CMSEntity GetLevelModel(int levelIndex)
        {
            var allLevels = CM.GetAll<LevelComponent>();
            return allLevels.Find(level => level.GetComponent<LevelComponent>().LevelIndex == levelIndex);
        }
    }
}