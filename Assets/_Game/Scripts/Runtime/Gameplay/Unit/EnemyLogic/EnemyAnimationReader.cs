using UnityEngine;

namespace Game.Runtime.Gameplay
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
