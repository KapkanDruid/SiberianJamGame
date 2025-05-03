using Cysharp.Threading.Tasks;
using Game.Runtime.Services;

namespace Game.Runtime.Gameplay.Enemy
{
    public interface IEnemy : IService
    {
        public float CurrentHealth { get; }
        public float CurrentArmor { get; }

        public UniTask TakeDamage(float damage);
    }
}
