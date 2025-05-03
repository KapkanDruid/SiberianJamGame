using UnityEngine;

namespace Game.Runtime.Gameplay.Enemy
{
    public class EnemyAnimationReader : MonoBehaviour
    {
        [HideInInspector] public bool AttackPerformed;

        public void OnSimpleAttack()
        {
            AttackPerformed = true;
        }
    }
}
