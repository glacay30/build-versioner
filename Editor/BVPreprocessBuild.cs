using UnityEngine;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace BuildVersioner
{
    internal sealed class BVPreproccessBuild : IPreprocessBuildWithReport
    {
        public int callbackOrder => 0;

        public void OnPreprocessBuild(BuildReport report)
        {
            if (BVFolderAndFileNames.CreateSubdirectoriesInAssetsFromPath(BVFolderAndFileNames.BVTempResourcesDirectory))
            {
                var settings = ScriptableObject.CreateInstance<BVInfoScriptableObject>();
                AssetDatabase.CreateAsset(settings, BVFolderAndFileNames.BVTempAssetPath);
                Debug.Log("Created: " + BVFolderAndFileNames.BVTempAssetPath);
                AssetDatabase.SaveAssets(); // save newly created asset

                var infoAssets = Resources.LoadAll<BVInfoScriptableObject>(nameof(BVInfoScriptableObject));
                if (infoAssets.Length == 1)
                {
                    infoAssets[0].Value = BVInfo.GetVersionFormatted(); // store data into asset
                    EditorUtility.SetDirty(infoAssets[0]);
                    AssetDatabase.SaveAssets(); // force save again
                }
                else
                {
                    Debug.LogError("Could not find a " + nameof(BVInfoScriptableObject) + " resource!");
                }
            }
        }
    }
}
