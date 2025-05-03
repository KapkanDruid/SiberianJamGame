using UnityEngine;

namespace Game.Runtime.Gameplay.Enemy
{
    public class EnemyAnimationReader : MonoBehaviour
    {
        [HideInInspector] public bool AttackPerformed;
        [HideInInspector] public bool IsDeath;

        public void OnSimpleAttack()
        {
            AttackPerformed = true;
        }

        public void OnDeath()
        {
            IsDeath = true;
        }
    }
}
