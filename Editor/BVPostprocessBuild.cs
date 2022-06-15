using UnityEngine;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace BuildVersioner
{
    internal class BVPostproccessBuild : IPostprocessBuildWithReport
    {
        public int callbackOrder => 0;

        public void OnPostprocessBuild(BuildReport report)
        {
            Debug.Log("BV POST Process Build...");
            if (AssetDatabase.IsValidFolder("Assets/Temp"))
            {
                Debug.Log("Found: " + "Assets/Temp");
                if (!AssetDatabase.DeleteAsset("Assets/Temp"))
                {
                    Debug.LogError("Could not delete: " + "Assets/Temp");
                }
                else
                {
                    Debug.Log("Deleted: " + "Assets/Temp");
                }
            }
            else
            {
                Debug.LogError("Could not find: " + "Assets/Temp");
            }
            Debug.Log("Done! BV POST Process Build");
        }
    }
}
