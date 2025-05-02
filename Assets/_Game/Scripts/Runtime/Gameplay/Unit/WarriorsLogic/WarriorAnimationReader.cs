using UnityEngine;

namespace Game.Runtime.Gameplay
{
    public class WarriorAnimationReader : MonoBehaviour
    {
        [HideInInspector] public bool SimpleAttackPerformed;
        [HideInInspector] public bool FinishAttackPerformed;

        public void OnFinishAttack()
        {
            FinishAttackPerformed = true;
        }

        public void OnSimpleAttack()
        {
            SimpleAttackPerformed = true;
        }
    }
}
