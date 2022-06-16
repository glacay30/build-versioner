// Copyright (c) 2022 Gabriel Lacayo
// See https://mit-license.org/
// Contact: lacayo@alumni.usc.edu

using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace BuildVersioner
{
    internal sealed class BVSystemCommands
    {
#if UNITY_EDITOR
        internal static bool Editor_SetP4Username(string username)
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

        internal static string Editor_GetP4Username()
        {
            string command = Editor_GetP4Path() + " set -q P4USER";
            using Process cmd = Editor_OpenTerminalWithCommand(command);
            string stdErr = cmd.StandardError.ReadToEnd();
            string stdOut = cmd.StandardOutput.ReadToEnd();

            if (!string.IsNullOrEmpty(stdErr))
            {
                UnityEngine.Debug.LogError("Detected error: " + stdErr);
                return "";
            }

            if (string.IsNullOrEmpty(stdOut))
            {
                return "(None set)";
            }

            // should expect something like:
            // P4USER=username
            // and we want to extract 'username'

            stdOut = stdOut.Trim();
            int start = stdOut.IndexOf('=') + 1;
            string username = stdOut.Substring(start);
            return username;
        }

        internal static void Editor_SavePropertiesToFile()
        {
            using var file = File.CreateText(Editor_GetFilePath());
            foreach (var prop in typeof(BVSingleton).GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                file.WriteLine(prop.Name + ": " + prop.GetValue(BVSingleton.Instance));
            }
        }

        internal static string Editor_GetFilePath()
        {
            string userSettingsPath = Path.Combine(Application.dataPath, "../UserSettings");
            string fileName = "BVBuildVersion.asset";
            string filePath = Path.Combine(userSettingsPath, fileName);
            return filePath;
        }

        internal static string Editor_GetChangelistNumberFromCommand()
        {
            string currentPerforceWorkspace = BVSingleton.Instance.P4Workspace;
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
                    UnityEngine.Debug.LogError(stdErr + " (This usually means your username is mistyped or not set—check in Window > Build Versioner > Get Username.)");
                }
                else if (stdErr.Contains("Invalid changelist"))
                {
                    UnityEngine.Debug.LogError(stdErr + " (This usually means your workspace is misspelled—check in Window > Build Versioner > Workspace.)");
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

        internal static string Editor_ReadPropertyFromFile(string propertyName)
        {
            if (File.Exists(Editor_GetFilePath()))
            {
                var foundProperty = File.ReadAllText(Editor_GetFilePath()).Split('\r', '\n').FirstOrDefault(str =>
                {
                    if (string.IsNullOrEmpty(str))
                        return default;

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

        internal static Process Editor_OpenTerminalWithCommand(string command)
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

        internal static string Editor_GetP4Path()
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