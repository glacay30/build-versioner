using UnityEngine;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace BuildVersioner
{
    internal class BVPreproccessBuild : IPreprocessBuildWithReport
    {
        public int callbackOrder => 0;

        public static string BVBuildAssetDirectory
        {
            get
            {
                return "Assets/Temp/Resources/";
            }
        }

        public static string BVBuildSettingsResourcePath
        {
            get
            {
                return BVBuildAssetDirectory + nameof(BVBuildVersionScriptableObject) + ".asset";
            }
        }

        public void OnPreprocessBuild(BuildReport report)
        {
            Debug.Log("BV PRE Process Build...");
            var settings = ScriptableObject.CreateInstance<BVBuildVersionScriptableObject>();
            if (!AssetDatabase.IsValidFolder(BVBuildAssetDirectory))
            {
                if (!AssetDatabase.IsValidFolder("Assets/Temp"))
                {
                    string folderTemp = AssetDatabase.CreateFolder("Assets", "Temp");
                    if (string.IsNullOrEmpty(folderTemp))
                    {
                        Debug.LogError("Unable to create: " + "Assets/Temp");
                    }
                    else
                    {
                        Debug.Log("Created: " + "Assets/Temp");
                    }
                }

                string folderTempResources = AssetDatabase.CreateFolder("Assets/Temp", "Resources");
                if (string.IsNullOrEmpty(folderTempResources))
                {
                    Debug.LogError("Unable to create: " + "Assets/Temp/Resources");
                }
                else
                {
                    Debug.Log("Created: " + "Assets/Temp/Resources");
                }
            }
            AssetDatabase.CreateAsset(settings, BVBuildSettingsResourcePath);
            Debug.Log("Created: " + BVBuildSettingsResourcePath);
            string version = BVBuildVersion.GetFullVersionFormatted();
            AssetDatabase.SaveAssets(); // must save before loading

            var allSettingSOs = Resources.LoadAll<BVBuildVersionScriptableObject>
            (
                nameof(BVBuildVersionScriptableObject)
            );

            if (allSettingSOs.Length > 0)
            {
                var mySettings = allSettingSOs[0];
                mySettings.Value = version;
                EditorUtility.SetDirty(mySettings);
                AssetDatabase.SaveAssets(); // make sure serialized
            }
            else
            {
                Debug.LogError("Could not find a " + nameof(BVBuildVersionScriptableObject) + " resource!");
            }
            Debug.Log("Done! BV PRE Process Build...");
        }
    }

}
