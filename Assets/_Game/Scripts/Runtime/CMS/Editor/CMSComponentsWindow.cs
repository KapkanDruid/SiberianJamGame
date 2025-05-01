#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Game.Runtime.CMS.Editor
{
    public class CMSComponentsWindow : EditorWindow
    {
        private class ComponentFolder
        {
            public string DisplayName;
            public readonly Dictionary<string, ComponentFolder> Subfolders = new();
            public readonly List<Type> Components = new();
        }

        private string _search = "";
        private Vector2 _scrollPos;

        private CMSPrefabEditor _editor;

        private readonly ComponentFolder _rootFolder = new();
        private readonly Dictionary<string, bool> _folderExpansionStates = new();

        public void Initialize(CMSPrefabEditor editor)
        {
            _editor = editor;

            var componentTypes = TypeCache.GetTypesDerivedFrom<CMSComponent>()
                .Where(t => !t.IsAbstract && !t.IsInterface)
                .OrderBy(t => t.Name);

            foreach (var type in componentTypes)
            {
                string folder = type.Namespace ?? "Other";
                AddComponentToFolder(type, folder.Replace(".", "/"));
            }
        }

        private void AddComponentToFolder(Type type, string relativePath)
        {
            ComponentFolder currentFolder = _rootFolder;

            if (!string.IsNullOrEmpty(relativePath))
            {
                string folderName = relativePath.Split('/').Last();

                if (!currentFolder.Subfolders.ContainsKey(folderName))
                {
                    currentFolder.Subfolders[folderName] = new ComponentFolder
                    {
                        DisplayName = folderName
                    };
                }

                currentFolder = currentFolder.Subfolders[folderName];
            }

            currentFolder.Components.Add(type);
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

            string newSearch = EditorGUILayout.TextField(_search, EditorStyles.toolbarSearchField);
            if (newSearch != _search)
            {
                _search = newSearch;
                Repaint();
            }

            EditorGUILayout.EndHorizontal();

            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);

            if (string.IsNullOrEmpty(_search))
            {
                DrawFolder(_rootFolder, 0);
            }
            else
            {
                var allComponents = GetAllComponents(_rootFolder)
                    .Where(t => t.Name.IndexOf(_search, StringComparison.OrdinalIgnoreCase) >= 0)
                    .OrderBy(t => t.Name);

                foreach (var type in allComponents)
                {
                    if (GUILayout.Button(type.Name, EditorStyles.miniButton,
                            GUILayout.Height(20), GUILayout.ExpandWidth(true)))
                    {
                        _editor.AddComponent(type);
                        Close();
                    }
                }
            }

            EditorGUILayout.EndScrollView();
        }

        private IEnumerable<Type> GetAllComponents(ComponentFolder folder)
        {
            foreach (var component in folder.Components)
            {
                yield return component;
            }

            foreach (var subfolder in folder.Subfolders.Values)
            {
                foreach (var component in GetAllComponents(subfolder))
                {
                    yield return component;
                }
            }
        }

        private void DrawFolder(ComponentFolder folder, int indentLevel)
        {
            foreach (var subfolder in folder.Subfolders.OrderBy(f => f.Key))
            {
                string folderKey = subfolder.Value.DisplayName;
                _folderExpansionStates.TryAdd(folderKey, false);

                EditorGUILayout.BeginHorizontal();

                bool isExpanded = _folderExpansionStates[folderKey];
                isExpanded = EditorGUILayout.Foldout(isExpanded, subfolder.Value.DisplayName, true);

                EditorGUILayout.EndHorizontal();

                _folderExpansionStates[folderKey] = isExpanded;

                if (isExpanded)
                {
                    DrawFolder(subfolder.Value, indentLevel + 1);
                }
            }

            foreach (var type in folder.Components.OrderBy(t => t.Name))
            {
                EditorGUILayout.BeginHorizontal();

                GUILayout.Space(indentLevel * 8);

                if (GUILayout.Button(type.Name, EditorStyles.miniButton,
                        GUILayout.Height(20), GUILayout.ExpandWidth(true)))
                {
                    _editor.AddComponent(type);
                    Close();
                }

                EditorGUILayout.EndHorizontal();
            }
        }
    }
}
#endif