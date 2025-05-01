#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Game.Runtime.CMS.Editor
{
    public static class CMSPrefabButton
    {
        [MenuItem("Assets/CMS/Create CMSPrefab", priority = 0)]
        private static void CreateCMSEntityPrefab()
        {
            string folderPath = "Assets";
            foreach (Object obj in Selection.GetFiltered(typeof(Object), SelectionMode.Assets))
            {
                string assetPath = AssetDatabase.GetAssetPath(obj);
                if (!string.IsNullOrEmpty(assetPath))
                {
                    if (File.Exists(assetPath))
                    {
                        folderPath = Path.GetDirectoryName(assetPath);
                        if (string.IsNullOrEmpty(folderPath))
                            folderPath = "Assets";
                    }
                    else folderPath = assetPath;
                }
                break;
            }

            GameObject newPrefab = new GameObject("CMSPrefab");
            newPrefab.AddComponent<CMSPrefab>();

            string prefabPath = AssetDatabase.GenerateUniqueAssetPath(Path.Combine(folderPath, "CMSPrefab.prefab"));
            PrefabUtility.SaveAsPrefabAsset(newPrefab, prefabPath);

            Object.DestroyImmediate(newPrefab);

            AssetDatabase.Refresh();
            Object createdPrefab = AssetDatabase.LoadAssetAtPath<Object>(prefabPath);
            Selection.activeObject = createdPrefab;
        }
    }
}
#endif