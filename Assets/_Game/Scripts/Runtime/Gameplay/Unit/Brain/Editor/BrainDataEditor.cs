#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Game.Runtime.Gameplay.Unit.Editor
{
    [CustomEditor(typeof(BrainPatternData))]
    public class BrainDataEditor : UnityEditor.Editor
    {
        private Dictionary<Vector2Int, bool> _togglePositions;
        private List<Vector2Int> _localGridPattern;

        private SerializedProperty _listProperty;

        private BrainPatternData _brainData;

        private GUIStyle _localPatternTextStyle;

        private int _gridHeight;
        private int _gridWidth;

        private int _maxX;
        private int _maxY;

        private bool _sizeModifiedForced;
        private bool _isChangesSaved;


        private void OnEnable()
        {
            _brainData = (BrainPatternData)target;

            _listProperty = serializedObject.FindProperty("_gridPattern");

            _localPatternTextStyle = new();

            _localPatternTextStyle.fontSize = 14;
            _localPatternTextStyle.wordWrap = true;

            _isChangesSaved = true;

            _localPatternTextStyle.normal.textColor = Color.green;

            GenerateGrid(true);
        }

        public override void OnInspectorGUI()
        {
            ShowHelpBoxes();

            SetGridSize();

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
            if (_brainData.GridPattern != null && _brainData.GridPattern.Count > 0)
            {
                GUIStyle textStyle = new();

                textStyle.normal.textColor = Color.white;
                textStyle.wordWrap = true;
                textStyle.fontSize = 14;

                EditorGUILayout.LabelField($"Saved pattern: {_brainData.GridPattern.Count} positions", textStyle);

                textStyle.fontSize = 12;
                EditorGUILayout.LabelField(_brainData.GetPositionsString(), textStyle);
            }

            if (IsCollectionsEqual(_localGridPattern, _brainData.GridPattern))
                _isChangesSaved = true;
            else
                _isChangesSaved = false;

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

                    serializedObject.ApplyModifiedProperties();
                    EditorUtility.SetDirty(_brainData);
                }

                GUI.backgroundColor = Color.white;
            }
        }

        private void ClearSavedPattern()
        {
            if (_brainData.GridPattern != null && _brainData.GridPattern.Count > 0)
            {
                GUI.backgroundColor = Color.red;

                if (GUILayout.Button("Clear saved pattern", GUILayout.Width(200), GUILayout.Height(30)))
                {
                    _brainData.ClearPattern();
                }

                GUI.backgroundColor = Color.white;
            }
        }

        private void GenerateGrid(bool firstCall)
        {
            _togglePositions = new();

            if (_brainData.GridPattern != null)
            {
                _sizeModifiedForced = false;

                foreach (var gridPattern in _brainData.GridPattern)
                {
                    int suitablePatternWith = Mathf.Abs(gridPattern.x) * 2 + 1;
                    if (suitablePatternWith > _gridWidth)
                    {
                        _gridWidth = suitablePatternWith;

                        if (!firstCall)
                            _sizeModifiedForced = true;
                    }

                    int suitablePatternHeigh = Mathf.Abs(gridPattern.y) * 2 + 1;
                    if (suitablePatternHeigh > _gridHeight)
                    {
                        _gridHeight = suitablePatternHeigh;

                        if (!firstCall)
                            _sizeModifiedForced = true;
                    }
                }
            }

            _maxX = _gridWidth / 2;
            _maxY = _gridHeight / 2;

            for (int y = -_maxY; y <= _maxY; y++)
            {
                for (int x = -_maxX; x <= _maxX; x++)
                {
                    var positionKey = new Vector2Int(x, y);
                    _togglePositions[positionKey] = false;
                }
            }

            if (_brainData.GridPattern != null)
            {
                foreach (var gridPattern in _brainData.GridPattern)
                {
                    _togglePositions[gridPattern] = true;
                }
            }
        }
        
        private void SetGridSize()
        {
            _gridHeight = EditorGUILayout.IntSlider("Height", _gridHeight, 1, 49);
            _gridWidth = EditorGUILayout.IntSlider("Width", _gridWidth, 1, 49);

            if (_gridHeight % 2 == 0)
                _gridHeight++;

            if (_gridWidth % 2 == 0)
                _gridWidth++;
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
                    if (togglePosition.Key == Vector2Int.zero)
                        continue;

                    if (togglePosition.Value)
                        _localGridPattern.Add(togglePosition.Key);
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

                _togglePositions[Vector2Int.zero] = true;
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