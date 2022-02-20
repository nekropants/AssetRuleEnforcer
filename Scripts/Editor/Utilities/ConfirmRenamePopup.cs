namespace AssetRules
{
    using UnityEngine;
    using UnityEditor;
    using System.Collections.Generic;
    using System;

    class ConfirmRenamePopup : EditorWindow
    {
        private static List<bool> _checks = new List<bool>();
        private static List<RuleBase.RuleResult> _ruleResults = new List<RuleBase.RuleResult>();
        private static List<WarningRule.WarningResult> _warningResults = new List<WarningRule.WarningResult>();
        private static int _fixCount;
        private Vector2 scroll;

        private  GUIStyle _kLeftAligned;

        public static void Get(List<RuleBase.RuleResult> ruleResults, List<WarningRule.WarningResult> warningResults)
        {
       
            _ruleResults.AddRange(ruleResults);
            _warningResults.AddRange(warningResults);
            for (int j = 0; j < _ruleResults.Count; j++)
            {
                _checks.Add(true);
            }

            ConfirmRenamePopup window = EditorWindow.GetWindow<ConfirmRenamePopup>();
            if (window == null)
            {
                window = ScriptableObject.CreateInstance<ConfirmRenamePopup>();
                window.name = "Crime Watch";
                window.position = new Rect(Screen.width / 2, Screen.height / 2, 800, 500);
                window.ShowUtility();
            }
        }

        private void OnGUI()
        {
            
            _kLeftAligned = new GUIStyle(GUI.skin.label);
            _kLeftAligned.alignment = TextAnchor.MiddleLeft;
            GUILayout.BeginVertical();
            {

                GUILayout.BeginHorizontal("box");
                {
                    foreach (WarningRule.WarningResult warningResult in _warningResults)
                    {
                        GUILayout.Label(warningResult.message);
                    }
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal(GUILayout.Width(100));
                {
                    if (GUILayout.Button("All"))
                    {
                        for (int j = 0; j < _checks.Count; j++)
                        {
                            _checks[j] = true;
                        }
                    }

                    if (GUILayout.Button("None"))
                    {
                        for (int j = 0; j < _checks.Count; j++)
                        {
                            _checks[j] = false;
                        }
                    }

                    if (GUILayout.Button("Invert"))
                    {
                        for (int j = 0; j < _checks.Count; j++)
                        {
                            _checks[j] = !_checks[j];
                        }
                    }

                    GUILayout.FlexibleSpace();

                    GUI.color = Color.green;
                    GUILayout.Label($"{_fixCount} files fixed");
                    GUI.color = Color.white;
                }

                GUILayout.EndHorizontal();

                scroll = GUILayout.BeginScrollView(scroll);

                GUILayout.BeginHorizontal("box");
                {
                    DrawIndices();

                    DrawOldNames();
                    GUILayout.Space(40);

                    DrawCheckboxes();
                    GUILayout.Space(40);

                    DrawNewNames();
                    GUILayout.Space(40);
                    GUILayout.FlexibleSpace();

                    DrawFixButtons();
                }

                GUILayout.EndHorizontal();

                GUILayout.EndScrollView();

                GUILayout.FlexibleSpace();

                GUILayout.Space(20);

                GUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button("Reset Ignore List"))
                    {
                        AssetNamingPrefs.instance._ignoreList.Clear();
                    }

                    if (GUILayout.Button("Ignore Selected"))
                    {
                        for (int i = _checks.Count - 1; i >= 0; i--)
                        {
                            if (_checks[i])
                            {
                                Ignore(i);
                            }
                        }
                    }

                    if (GUILayout.Button("Fix Selected"))
                    {
                        bool ok = EditorUtility.DisplayDialog("Confirm Asset Rename",
                            "You will only be able to undo this using version control", "Rename", "Cancel");

                        if (ok)
                        {
                            EditorUtility.DisplayProgressBar("Renaming", "", 0);
                            for (int i = _checks.Count - 1; i >= 0; i--)
                            {
                                if (_checks[i])
                                {
                                    try
                                    {
                                        EditorUtility.DisplayProgressBar("Renaming",
                                            _ruleResults[i].oldPath + " -> " + _ruleResults[i].newPath,
                                            (_checks.Count - i) / (float)_checks.Count);
                                        
                                        Fix(i);

                                    }
                                    catch (Exception e)
                                    {
                                        Debug.Log(i);
                                        Debug.Log(e);
                                        EditorUtility.ClearProgressBar();
                                    }
                                }
                            }

                            EditorUtility.ClearProgressBar();
                        }
                    }

                    if (GUILayout.Button("Close"))
                    {
                        this.Close();
                    }
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();

        }

        void DrawIndices()
        {
            GUILayout.BeginVertical();
            {
                for (var i = 0; i < _ruleResults.Count; i++)
                {
                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.Space(5);
                        GUILayout.Label($"{i}.");
                    }
                    GUILayout.EndHorizontal();
                }
            }
            GUILayout.EndVertical();
        }

        void DrawOldNames()
        {
            GUILayout.BeginVertical();
            {
                for (var i = 0; i < _ruleResults.Count; i++)
                {
                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.Space(5);

                        Color color = GUI.color;
                        GUI.color = new Color(129 / 255f, 180 / 255f, 255 / 255f, 1);

                        if (GUILayout.Button($"{_ruleResults[i].oldPath.Replace(_ruleResults[i].commonPath, "")}",
                            _kLeftAligned))
                        {
                            EditorGUIUtility.PingObject(
                                AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(_ruleResults[i].oldPath));
                        }

                        GUI.color = color;
                    }
                    GUILayout.EndHorizontal();
                }
            }
            GUILayout.EndVertical();
        }

        void DrawNewNames()
        {
            GUILayout.BeginVertical();
            {
                for (var i = 0; i < _ruleResults.Count; i++)
                {
                    GUILayout.BeginHorizontal();
                    {
                        string outcome = _ruleResults[i].customMessage;
                        if (string.IsNullOrWhiteSpace(outcome))
                            outcome = _ruleResults[i].newPath.Replace(_ruleResults[i].commonPath, "");
                        GUILayout.Label($"{outcome}", _kLeftAligned);
                    }
                    GUILayout.EndHorizontal();
                }
            }
            GUILayout.EndVertical();
        }

        void DrawFixButtons()
        {
            GUILayout.BeginVertical();
            {
                for (var i = 0; i < _ruleResults.Count; i++)
                {
                    GUILayout.BeginHorizontal();
                    {
                        if (GUILayout.Button("Fix", GUILayout.Height(EditorGUIUtility.singleLineHeight)))
                        {
                            Fix(i);
                        }

                        if (GUILayout.Button("Ignore", GUILayout.Height(EditorGUIUtility.singleLineHeight)))
                        {
                            Ignore(i);
                        }
                    }
                    GUILayout.EndHorizontal();
                }
            }
            GUILayout.EndVertical();
        }

        private  void DrawCheckboxes()
        {
            GUILayout.BeginVertical();
            {
                for (var i = 0; i < _ruleResults.Count; i++)
                {
                    GUILayout.BeginHorizontal();
                    {
                        _checks[i] = GUILayout.Toggle(_checks[i], $" ");
                        GUILayout.Label($"{_ruleResults[i].rule.name}:", _kLeftAligned );
                    }
                    GUILayout.EndHorizontal();
                }
            }
            GUILayout.EndVertical();
        }

        private static void Ignore(int i)
        {
            AssetNamingPrefs.instance._ignoreList.Add(_ruleResults[i].oldPath);

            _checks.RemoveAt(i);
            _ruleResults.RemoveAt(i);
        }

        private void Fix(int i)
        {
            try
            {

                _ruleResults[i]?.fixAction();

                _fixCount++;
            }
            catch (Exception e)
            {
                Debug.LogException(e, this);
            }

            _checks.RemoveAt(i);
            _ruleResults.RemoveAt(i);
        }

        private void OnDestroy()
        {
            ClearLists();
        }

        private void ClearLists()
        {
            _checks.Clear();
            _ruleResults.Clear();
        }
    }
}