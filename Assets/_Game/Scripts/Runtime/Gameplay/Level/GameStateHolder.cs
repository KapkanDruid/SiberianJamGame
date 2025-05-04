using Game.Runtime.Services;

namespace Game.Runtime.Gameplay.Level
{
    public class GameStateHolder : IService
    {
        public int CurrentLevel;
        public float CharacterHealth;
        public float CachedHealth;
        public string DialogBlockID;
    }
}