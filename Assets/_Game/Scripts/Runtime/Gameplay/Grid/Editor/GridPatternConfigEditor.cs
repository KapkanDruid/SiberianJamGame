#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Game.Runtime.Gameplay.Grid.Editor
{
    [CustomEditor(typeof(GridConfig))]
    public class GridDataEditor : UnityEditor.Editor
    {
        private GridConfig _gridConfig;

        private Dictionary<Vector2Int, bool> _togglePositions;
        private List<Vector2Int> _localGridPattern;
        private Vector2Int _localGridSize;

        private SerializedProperty _listProperty;
        private SerializedProperty _sizeProperty;

        private GUIStyle _localPatternTextStyle;

        private int _maxX;
        private int _maxY;

        private bool _sizeModifiedForced;
        private bool _isChangesSaved;


        private void OnEnable()
        {
            _gridConfig = (GridConfig)target;

            _listProperty = serializedObject.FindProperty("gridPattern");
            _sizeProperty = serializedObject.FindProperty("gridSize");

            _localPatternTextStyle = new()
            {
                fontSize = 14,
                wordWrap = true,
                normal =
                {
                    textColor = Color.green
                }
            };

            _isChangesSaved = true;
            GenerateGrid(true);
        }

        public override void OnInspectorGUI()
        {
            ShowHelpBoxes();
            SetGridSize();
            
            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Generate Grid", GUILayout.Width(100), GUILayout.Height(25)))
                GenerateGrid(false);

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            ClearSavedPattern();
            SaveCurrentPattern();

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            ShowPatternText();

            GUILayout.Space(10);

            DrawGrid();

            GUILayout.Space(10);
        }

        private void ShowPatternText()
        {
            if (_gridConfig.GridPattern != null && _gridConfig.GridPattern.Count > 0)
            {
                GUIStyle textStyle = new()
                {
                    normal =
                    {
                        textColor = Color.white
                    },
                    wordWrap = true,
                    fontSize = 14
                };

                EditorGUILayout.LabelField($"Saved pattern: {_gridConfig.GridPattern.Count} positions", textStyle);

                textStyle.fontSize = 12;
                EditorGUILayout.LabelField(_gridConfig.GetPositionsString(), textStyle);
            }

            _isChangesSaved = IsCollectionsEqual(_localGridPattern, _gridConfig.GridPattern);
            _localPatternTextStyle.normal.textColor = _isChangesSaved ? Color.green : Color.yellow;

            GUILayout.Space(4);

            if (_localGridPattern != null && _localGridPattern.Count > 0)
            {
                _localPatternTextStyle.fontSize = 14;
                EditorGUILayout.LabelField($"Local pattern: {_localGridPattern.Count} positions", _localPatternTextStyle);

                _localPatternTextStyle.fontSize = 12;
                EditorGUILayout.LabelField(GetPositionsString(), _localPatternTextStyle);
            }
        }

        private void SaveCurrentPattern()
        {
            if (_localGridPattern != null && _localGridPattern.Count > 0)
            {
                GUI.backgroundColor = _isChangesSaved ? Color.white : Color.yellow;

                if (GUILayout.Button("Save current pattern", GUILayout.Width(200), GUILayout.Height(30)))
                {
                    _listProperty.ClearArray();

                    for (int i = 0; i < _localGridPattern.Count; i++)
                    {
                        _listProperty.InsertArrayElementAtIndex(i);
                        _listProperty.GetArrayElementAtIndex(i).vector2IntValue = _localGridPattern[i];
                    }
                    
                    _sizeProperty.vector2IntValue =_localGridSize;

                    serializedObject.ApplyModifiedProperties();
                    EditorUtility.SetDirty(_gridConfig);
                }

                GUI.backgroundColor = Color.white;
            }
        }

        private void ClearSavedPattern()
        {
            if (_gridConfig.GridPattern != null && _gridConfig.GridPattern.Count > 0)
            {
                GUI.backgroundColor = Color.red;

                if (GUILayout.Button("Clear saved pattern", GUILayout.Width(200), GUILayout.Height(30)))
                {
                    _gridConfig.ClearPattern();
                }

                GUI.backgroundColor = Color.white;
            }
        }

        private void GenerateGrid(bool firstCall)
        {
            _togglePositions = new();

            if (_gridConfig.GridPattern != null)
            {
                _sizeModifiedForced = false;

                foreach (var gridPattern in _gridConfig.GridPattern)
                {
                    int suitablePatternWith = Mathf.Abs(gridPattern.x) * 2 + 1;
                    if (suitablePatternWith > _localGridSize.x)
                    {
                        _localGridSize = new Vector2Int(suitablePatternWith, _localGridSize.y);

                        if (!firstCall)
                            _sizeModifiedForced = true;
                    }

                    int suitablePatternHeight = Mathf.Abs(gridPattern.y) * 2 + 1;
                    if (suitablePatternHeight > _localGridSize.y)
                    {
                        _localGridSize = new Vector2Int(_localGridSize.x, suitablePatternHeight);

                        if (!firstCall)
                            _sizeModifiedForced = true;
                    }
                }
            }

            _maxX = _localGridSize.x / 2;
            _maxY = _localGridSize.y / 2;

            for (int y = -_maxY; y <= _maxY; y++)
            {
                for (int x = -_maxX; x <= _maxX; x++)
                {
                    var positionKey = new Vector2Int(x, y);
                    _togglePositions[positionKey] = false;
                }
            }

            if (_gridConfig.GridPattern != null)
            {
                foreach (var gridPattern in _gridConfig.GridPattern)
                {
                    _togglePositions[gridPattern] = true;
                }
            }
        }
        
        private void SetGridSize()
        {
            _localGridSize = EditorGUILayout.Vector2IntField("Grid Size", _localGridSize);
        }
        
        private void ShowHelpBoxes()
        {
            if (_sizeModifiedForced)
            {
                EditorGUILayout.HelpBox("The grid size is changed forcibly because the saved pattern does not fit into the specified grid size", MessageType.Warning);
            }
            if (!_isChangesSaved)
            {
                EditorGUILayout.HelpBox("Changes made are not saved!", MessageType.Warning);
            }
        }
        
        private void DrawGrid()
        {
            if (_togglePositions != null)
            {
                _localGridPattern = new();

                foreach (var togglePosition in _togglePositions)
                {
                    if (togglePosition.Value)
                    {
                        _localGridPattern.Add(togglePosition.Key);
                    }
                }
            }

            if (_togglePositions != null)
            {
                int width = _maxX;
                int height = _maxY;

                for (int y = -height; y <= height; y++)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();

                    for (int x = -width; x <= width; x++)
                    {
                        var currentPosition = new Vector2Int(x, -y);

                        GUI.backgroundColor = _togglePositions[currentPosition] ? Color.green : Color.white;

                        if (GUILayout.Button($"{x},{-y}", GUILayout.Width(40), GUILayout.Height(40)))
                        {
                            _togglePositions[currentPosition] = !_togglePositions[currentPosition];
                        }
                    }

                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();
                }
            }
        }
        
        private string GetPositionsString()
        {
            return string.Join(" ", _localGridPattern);
        }
        
        private bool IsCollectionsEqual(List<Vector2Int> firstCollection, List<Vector2Int> secondCollection)
        {
            if (firstCollection == null || secondCollection == null)
                return false;

            if (firstCollection.Count != secondCollection.Count)
                return false;

            var firstSortedCollection = firstCollection.OrderBy(v => v.x).ThenBy(v => v.y).ToList();
            var secondSortedCollection = secondCollection.OrderBy(v => v.x).ThenBy(v => v.y).ToList();

            return firstSortedCollection.SequenceEqual(secondSortedCollection);
        }
    }
}
#endif