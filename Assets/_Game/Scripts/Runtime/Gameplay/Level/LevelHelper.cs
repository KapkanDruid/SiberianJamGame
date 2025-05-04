using Game.Runtime.CMS;
using Game.Runtime.CMS.Components.Level;
using Game.Runtime.Services;
using Game.Runtime.Services.Save;

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