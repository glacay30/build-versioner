using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace BuildVersioner
{
    public class BVBuildVersion
    {
        private static BVBuildVersion _instance;
        public static BVBuildVersion Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new BVBuildVersion();
                return _instance;
            }
        }

        public string VersionMajor { get; set; }
        public string VersionMinor { get; set; }

#if UNITY_EDITOR
        public string P4Workspace { get; set; }
#elif UNITY_STANDALONE
    private string _changelistNumber;
#endif

        public static string GetFullVersionFormatted()
        {
            string major = Instance.VersionMajor;
            if (string.IsNullOrEmpty(major))
                major = "#";

            string minor = Instance.VersionMinor;
            if (string.IsNullOrEmpty(minor))
                minor = "#";

            string changelist = GetChangelistNumber();
            if (string.IsNullOrEmpty(changelist))
                changelist = "######";

            return major + "." + minor + "." + changelist;
        }

        public static string GetChangelistNumber()
        {
#if UNITY_EDITOR
            return Editor_GetChangelistNumberFromCommand();
#elif UNITY_STANDALONE
        return Instance._changelistNumber;
#endif
        }

        private BVBuildVersion()
        {
#if UNITY_EDITOR
            VersionMajor = Editor_ReadPropertyFromFile(nameof(VersionMajor));
            VersionMinor = Editor_ReadPropertyFromFile(nameof(VersionMinor));
            P4Workspace = Editor_ReadPropertyFromFile(nameof(P4Workspace));
#elif UNITY_STANDALONE
        var allSettingSOs = Resources.LoadAll<BVBuildVersionScriptableObject>
        (
            nameof(BVBuildVersionScriptableObject)
        );

        if (allSettingSOs.Length > 0)
        {
            var settings = allSettingSOs.First();
            string fullVersion = settings.Value;
            string[] splitVersion = fullVersion.Split('.');
            VersionMajor = splitVersion[0];
            VersionMinor = splitVersion[1];
            _changelistNumber = splitVersion[2];
        }
#endif
        }

#if UNITY_EDITOR
        public static bool Editor_SetP4Username(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                UnityEngine.Debug.LogError("No username to set!");
                return false;
            }

            string command = Editor_GetP4Path() + " set P4USER=" + username;
            using Process cmd = Editor_OpenTerminalWithCommand(command);
            string stdErr = cmd.StandardError.ReadToEnd();
            string stdOut = cmd.StandardOutput.ReadToEnd();

            if (!string.IsNullOrEmpty(stdErr))
            {
                UnityEngine.Debug.LogError("Detected error: " + stdErr);
                return false;
            }
            else if (!string.IsNullOrEmpty(stdOut))
            {
                UnityEngine.Debug.LogError("Unexpected output: " + stdOut);
                return false;
            }

            return true;
        }

        public static void Editor_SaveToFile()
        {
            using var file = File.CreateText(Editor_GetFilePath());
            foreach (var prop in typeof(BVBuildVersion).GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                file.WriteLine(prop.Name + ": " + prop.GetValue(Instance));
            }
        }

        private static string Editor_GetFilePath()
        {
            string userSettingsPath = Path.Combine(Application.dataPath, "../UserSettings");
            string fileName = "BVBuildVersion.asset";
            string filepath = Path.Combine(userSettingsPath, fileName);
            return filepath;
        }

        public static string Editor_GetChangelistNumberFromCommand()
        {
            string currentPerforceWorkspace = Instance.P4Workspace;
            if (string.IsNullOrEmpty(currentPerforceWorkspace))
            {
                UnityEngine.Debug.LogError("No workspace set! Set one at Window > Build Versioner > Workspace");
                return "";
            }

            string command = Editor_GetP4Path() + " changes -m 1 @" + currentPerforceWorkspace;
            using Process cmd = Editor_OpenTerminalWithCommand(command);
            string stdOut = cmd.StandardOutput.ReadToEnd();
            string stdErr = cmd.StandardError.ReadToEnd();

            if (!string.IsNullOrEmpty(stdErr))
            {
                if (stdErr.Contains("has not been enabled by 'p4 protect'."))
                {
                    UnityEngine.Debug.LogError(stdErr + " (This usually means the username set is invalid—make sure to check for typos when setting your p4 username.)");
                }
                else
                {
                    UnityEngine.Debug.LogError(stdErr);
                }
                return "";
            }
            else if (string.IsNullOrEmpty(stdOut))
            {
                UnityEngine.Debug.LogError("No std out!");
                return "";
            }

            string[] outputSplit = stdOut.Split(' ');
            string number = outputSplit[1];

            if (string.IsNullOrEmpty(number))
            {
                UnityEngine.Debug.LogError("Could not parse changelist from: " + stdOut);
                return "";
            }

            return number;
        }

        private static string Editor_ReadPropertyFromFile(string propertyName)
        {
            if (File.Exists(Editor_GetFilePath()))
            {
                var foundProperty = File.ReadAllText(Editor_GetFilePath()).Split('\r', '\n').FirstOrDefault(str =>
                {
                    if (string.IsNullOrEmpty(str))
                        return false;

                    string[] propAndValue = str.Split(new char[] { ':' }, 2);
                    return propAndValue[0].Equals(propertyName);
                });

                if (string.IsNullOrEmpty(foundProperty))
                    return default;

                string[] propAndValue = foundProperty.Split(new char[] { ':' }, 2);
                string value = propAndValue[1];
                value = value.Trim();
                return value;
            }

            return default;
        }

        private static Process Editor_OpenTerminalWithCommand(string command)
        {
            var startInfo = new ProcessStartInfo
            {
#if UNITY_EDITOR_WIN
                FileName = "cmd.exe",
                Arguments = "/C " + command,
#elif UNITY_EDITOR_OSX
            FileName = "/bin/bash",
            Arguments = "-c \"" + command + "\"",
#endif
                WindowStyle = ProcessWindowStyle.Hidden,
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
            };
            return Process.Start(startInfo);
        }

        private static string Editor_GetP4Path()
        {
#if UNITY_EDITOR_WIN
            return "p4";
#elif UNITY_EDITOR_OSX
            return "/usr/local/bin/p4";
#endif
        }
#endif
    }
}
