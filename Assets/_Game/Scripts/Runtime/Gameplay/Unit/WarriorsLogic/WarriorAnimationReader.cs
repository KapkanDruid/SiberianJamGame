using UnityEngine;

namespace Game.Runtime.Gameplay.Warrior
{
    public class WarriorAnimationReader : MonoBehaviour
    {
        [HideInInspector] public bool SimpleAttackPerformed;
        [HideInInspector] public bool FinishAttackPerformed;
        [HideInInspector] public bool DeathEnded;

        public void OnFinishAttack()
        {
            FinishAttackPerformed = true;
        }

        public void OnSimpleAttack()
        {
            SimpleAttackPerformed = true;
        }

        public void OnDeath()
        {
            DeathEnded = true;
        }
    }
}
