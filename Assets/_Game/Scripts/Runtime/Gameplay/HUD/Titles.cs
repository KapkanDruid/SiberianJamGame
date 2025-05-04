using Game.Runtime.Gameplay.Level;
using Game.Runtime.Services;
using UnityEngine;
using UnityEngine.SceneManagement;
using Game.Runtime.Utils.Consts;

namespace Game.Runtime.Gameplay
{
    public class Titles : MonoBehaviour
    {
        [SerializeField] Animator _animator;
        public void Play()
        {
            _animator.SetTrigger("Play");
        }

        public void End()
        {
            SL.Get<GameStateHolder>().CurrentLevel = 0;
            SceneManager.LoadSceneAsync(Const.ScenesConst.GameReleaseScene);
        }
    }
}
