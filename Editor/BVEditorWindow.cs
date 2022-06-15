using System.Collections;
using UnityEngine;
using UnityEditor;
using Unity.EditorCoroutines.Editor;

namespace BuildVersioner
{
    internal class BVEditorWindow : EditorWindow
    {
        private string _changelistButtonOutput;
        private string _usernameSet;
        private string _usernameSetStatusMessage;

        [MenuItem("Window/Build Versioner")]
        static private void Init()
        {
            var window = (BVEditorWindow)GetWindow(typeof(BVEditorWindow), false, "BV Build Settings");
            window.minSize = new Vector2(400.0f, 200.0f);
            window.Show();
        }

        private void Awake()
        {
            _changelistButtonOutput = BVBuildVersion.GetChangelistNumber();
        }

        private void OnGUI()
        {
            float BUTTON_WIDTH = 175.0f;
            float SECTION_VERTICAL_SPACING = 10.0f;

            GUILayout.Label("Version Settings", EditorStyles.boldLabel);
            BVBuildVersion.Instance.VersionMajor = EditorGUILayout.TextField("Version Major", BVBuildVersion.Instance.VersionMajor);
            BVBuildVersion.Instance.VersionMinor = EditorGUILayout.TextField("Version Minor", BVBuildVersion.Instance.VersionMinor);
            BVBuildVersion.Instance.P4Workspace = EditorGUILayout.TextField("Workspace", BVBuildVersion.Instance.P4Workspace);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Get Changelist Number", GUILayout.Width(BUTTON_WIDTH)))
            {
                _changelistButtonOutput = BVBuildVersion.GetChangelistNumber();
                if (_changelistButtonOutput.Length == 0)
                    _changelistButtonOutput = "None";
            }
            EditorGUI.BeginDisabledGroup(true);
            GUILayout.TextField(_changelistButtonOutput);
            EditorGUI.EndDisabledGroup();
            GUILayout.EndHorizontal();

            GUILayout.Space(SECTION_VERTICAL_SPACING);

            GUILayout.Label("Set P4 Variables", EditorStyles.boldLabel);
            GUILayout.BeginHorizontal();
            bool shouldSetUsername = GUILayout.Button("Set Username", GUILayout.Width(BUTTON_WIDTH));
            _usernameSet = GUILayout.TextField(_usernameSet);
            if (shouldSetUsername)
            {
                bool successfullySet = BVBuildVersion.Editor_SetP4Username(_usernameSet);
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

            SaveChanges();
        }

        public override void SaveChanges()
        {
            BVBuildVersion.Editor_SaveToFile();
            base.SaveChanges();
        }

        private IEnumerator ShowSetP4UsernameStatusMessage(string message)
        {
            _usernameSetStatusMessage = message;
            yield return new EditorWaitForSeconds(2.0f);
            _usernameSetStatusMessage = "";
        }
    }

}
