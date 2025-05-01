#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Editor._Game.Scripts.Editor
{
    public class PlayerPrefsCleanerTool
    {
        [MenuItem("Tools/Clear All PlayerPrefs")]
        public static void ClearAllPlayerPrefs()
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
                
            Debug.Log("PlayerPrefs have been completely cleared!");
        }
    }
}
#endif