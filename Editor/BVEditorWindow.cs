// Copyright (c) 2022 Gabriel Lacayo
// See https://mit-license.org/
// Contact: lacayo@alumni.usc.edu

using System.Collections;
using UnityEngine;
using UnityEditor;
using Unity.EditorCoroutines.Editor;

namespace BuildVersioner
{
    internal sealed class BVEditorWindow : EditorWindow
    {
        private string _changelistButtonOutput;
        private string _usernameSet;
        private string _usernameSetStatusMessage;
        private string _usernameGet;

        public override void SaveChanges()
        {
            BVSystemCommands.Editor_SavePropertiesToFile();
            base.SaveChanges();
        }

        [MenuItem("Window/Build Versioner")]
        private static void Init()
        {
            var window = (BVEditorWindow)GetWindow(typeof(BVEditorWindow), false, "BV Settings");
            window.minSize = new Vector2(400.0f, 200.0f);
            window.Show();
        }

        private void Awake()
        {
            _changelistButtonOutput = BVSingleton.Instance.GetChangelist();
        }

        private void OnGUI()
        {
            float BUTTON_WIDTH = 175.0f;
            float SECTION_VERTICAL_SPACING = 10.0f;

            GUILayout.BeginHorizontal();
            BVSingleton.Instance.Enabled = GUILayout.Toggle(BVSingleton.Instance.Enabled, "Enabled");
            GUILayout.EndHorizontal();

            if (!BVSingleton.Instance.Enabled)
            {
                SaveChanges();
                return;
            }

            GUILayout.Label("Version Settings", EditorStyles.boldLabel);
            {
                BVSingleton.Instance.VersionMajor = EditorGUILayout.TextField("Version Major", BVSingleton.Instance.VersionMajor);
                BVSingleton.Instance.VersionMinor = EditorGUILayout.TextField("Version Minor", BVSingleton.Instance.VersionMinor);
                BVSingleton.Instance.P4Workspace = EditorGUILayout.TextField("Workspace", BVSingleton.Instance.P4Workspace);

                {
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("Get Changelist Number", GUILayout.Width(BUTTON_WIDTH)))
                    {
                        _changelistButtonOutput = BVSingleton.Instance.GetChangelist();
                        if (_changelistButtonOutput.Length == 0)
                            _changelistButtonOutput = "None";
                    }
                    EditorGUI.BeginDisabledGroup(true);
                    GUILayout.TextField(_changelistButtonOutput);
                    EditorGUI.EndDisabledGroup();
                    GUILayout.EndHorizontal();
                }
            }


            GUILayout.Space(SECTION_VERTICAL_SPACING);

            GUILayout.Label("Set P4 Variables", EditorStyles.boldLabel);
            {
                {
                    GUILayout.BeginHorizontal();
                    bool shouldSetUsername = GUILayout.Button("Set Username", GUILayout.Width(BUTTON_WIDTH));
                    _usernameSet = GUILayout.TextField(_usernameSet);
                    if (shouldSetUsername)
                    {
                        bool successfullySet = BVSystemCommands.Editor_SetP4Username(_usernameSet);
                        string statusMessage = successfullySet ?
                            "Username set to " + _usernameSet :
                            "Failed to set username! Check unity console for errors.";
                        EditorCoroutineUtility.StartCoroutine(ShowSetP4UsernameStatusMessage(statusMessage), this);
                    }
                    GUILayout.EndHorizontal();
                    if (!string.IsNullOrEmpty(_usernameSetStatusMessage))
                    {
                        GUILayout.Label(_usernameSetStatusMessage);
                    }
                }
                {
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("Get Username", GUILayout.Width(BUTTON_WIDTH)))
                    {
                        _usernameGet = BVSystemCommands.Editor_GetP4Username();
                        EditorCoroutineUtility.StartCoroutine(ClearGetUsernameField(), this);
                    }

                    EditorGUI.BeginDisabledGroup(true);
                    GUILayout.TextField(_usernameGet);
                    EditorGUI.EndDisabledGroup();
                    GUILayout.EndHorizontal();
                }
            }

            SaveChanges();
        }

        private IEnumerator ShowSetP4UsernameStatusMessage(string message)
        {
            _usernameSetStatusMessage = message;
            yield return new EditorWaitForSeconds(2.0f);
            _usernameSetStatusMessage = "";
        }

        private IEnumerator ClearGetUsernameField()
        {
            yield return new EditorWaitForSeconds(2.0f);
            _usernameGet = "";
        }
    }
}
