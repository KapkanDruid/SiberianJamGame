using System.Collections.Generic;
using UnityEngine;

namespace Game.Runtime.CMS
{
    public class CMSPrefab : MonoBehaviour
    {
        [SerializeField, HideInInspector] private string entityId;
        [SerializeReference] public List<CMSComponent> Components;

        public string EntityId => entityId;

#if UNITY_EDITOR
        public void PingEntity()
        {
            string path = UnityEditor.AssetDatabase.GetAssetPath(gameObject);

            if (path.StartsWith("Assets/_Game/Resources/CMS/") && path.EndsWith(".prefab"))
            {
                path = path.Substring("Assets/_Game/Resources/".Length);
                path = path.Substring(0, path.Length - ".prefab".Length);
            }

            entityId = path;
        }
#endif
    }
}