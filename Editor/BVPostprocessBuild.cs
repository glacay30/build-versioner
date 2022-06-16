// Copyright (c) 2022 Gabriel Lacayo
// See https://mit-license.org/
// Contact: lacayo@alumni.usc.edu

using UnityEngine;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace BuildVersioner
{
    internal sealed class BVPostproccessBuild : IPostprocessBuildWithReport
    {
        public int callbackOrder => 0;

        public void OnPostprocessBuild(BuildReport report)
        {
            if (AssetDatabase.IsValidFolder(BVFolderAndFileNames.BVTempDirectory))
            {
                if (!AssetDatabase.DeleteAsset(BVFolderAndFileNames.BVTempDirectory))
                {
                    Debug.LogError("Could not delete: " + BVFolderAndFileNames.BVTempDirectory);
                }
            }
            else
            {
                Debug.LogError("Could not find: " + "Assets/BVTemp");
            }
        }
    }
}
