using Cysharp.Threading.Tasks;
using Game.Runtime.Services;

namespace Game.Runtime.Gameplay
{
    public interface IEnemy : IService
    {
        public UniTask TakeDamage(float damage);
    }
}
