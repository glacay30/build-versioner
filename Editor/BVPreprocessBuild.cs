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
                AssetDatabase.SaveAssets(); // save newly created asset

                var infoAssets = Resources.LoadAll<BVInfoScriptableObject>(nameof(BVInfoScriptableObject));
                if (infoAssets.Length == 1)
                {
                    // store data into asset and force save
                    infoAssets[0].Enabled = BVSingleton.Instance.Enabled;
                    if (infoAssets[0].Enabled)
                    {
                        infoAssets[0].VersionMajor = BVInfo.VersionMajor;
                        infoAssets[0].VersionMinor = BVInfo.VersionMinor;
                        infoAssets[0].Changelist = BVInfo.Changelist;
                    }
                    EditorUtility.SetDirty(infoAssets[0]);
                    AssetDatabase.SaveAssets();
                }
                else
                {
                    Debug.LogError("Could not find a " + nameof(BVInfoScriptableObject) + " resource!");
                }
            }
        }
    }
}
