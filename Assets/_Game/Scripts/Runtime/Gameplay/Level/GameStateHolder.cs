using System.Collections.Generic;
using Game.Runtime.Services;

namespace Game.Runtime.Gameplay.Level
{
    public class GameStateHolder : IService
    {
        public readonly GameData CurrentData = new();
        public readonly GameData CheckpointData = new();
        public bool NeedRespawnOnCheckpoint;
        public string DialogBlockID;

        public void LoadCheckpoint()
        {
            CurrentData.Level = CheckpointData.Level;
            CurrentData.CharacterHealth = CheckpointData.CharacterHealth;
            CurrentData.ImplantsPool.Clear();
            CurrentData.ImplantsPool.AddRange(CheckpointData.ImplantsPool);
        }
    }

    public class GameData
    {
        public int Level;
        public float CharacterHealth;
        public List<string> ImplantsPool = new();
    }
}