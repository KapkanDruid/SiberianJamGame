using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Game.Runtime.CMS
{
    public class CMSPrefab : MonoBehaviour
    {
        [SerializeField, HideInInspector] private string entityId;
        [SerializeReference] public List<CMSComponent> Components;

        public string EntityId => entityId;

#if UNITY_EDITOR
        public void PingEntity(bool showDebug = true)
        {
            if (showDebug)
            {
                var stringBuilder = new StringBuilder();
                stringBuilder.Append($"[{nameof(CMSPrefab)}] ");
                if (Components == null)
                {
                    stringBuilder.Append("Entity doesnt has components");
                }
                else
                {
                    for (int i = 0; i < Components.Count; i++)
                    {
                        stringBuilder.Append($"{i}.{Components[i].GetType().Name}");
                        stringBuilder.Append(i < Components.Count - 1 ? ", " : ".");
                    }
                }
            
                Debug.Log(stringBuilder.ToString());
            }
            
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