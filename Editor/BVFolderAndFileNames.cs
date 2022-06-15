using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace BuildVersioner
{
    internal class BVFolderAndFileNames
    {
        public static string BVTempDirectory => "Assets/BVTemp/";
        public static string BVTempResourcesDirectory => BVTempDirectory + "Resources/";
        public static string BVTempAssetPath => BVTempResourcesDirectory + nameof(BVInfoScriptableObject) + ".asset";

        /// <summary>
        /// Create subdirectories given a path, assuming the first dir is Assets
        /// </summary>
        /// <remarks>
        /// Example: Assets/Foo/Bar will attempt to create Foo/Bar inside of Assets/
        /// </remarks>
        /// <returns>True if successfully created all subdirectories</returns>
        public static bool CreateSubdirectoriesInAssetsFromPath(string path)
        {
            List<string> dirs = new List<string>(path.Split(new char[] { '/' }));

            if (dirs[0] == "Assets")
            {
                Debug.LogError("First dir needs to be Assets! Given path was: " + path);
                return false;
            }
            dirs.RemoveAt(0);

            if (dirs.Count == 0)
            {
                Debug.LogError("No other subdirectories given besides Assets! Given path was " + path);
                return false;
            }

            string currentPath = "Assets";
            foreach (string dir in dirs)
            {
                string newPath = currentPath + "/" + dir;
                if (!AssetDatabase.IsValidFolder(newPath))
                {
                    string createdDirGuid = AssetDatabase.CreateFolder(currentPath, dir);
                    if (string.IsNullOrEmpty(createdDirGuid))
                    {
                        Debug.LogError("Unable to create: " + newPath);
                        return false;
                    }
                    else
                    {
                        Debug.Log("Created: " + newPath);
                        currentPath = newPath;
                    }
                }
            }
            return true;
        }
    }
}
