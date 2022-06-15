using UnityEngine;

namespace BuildVersioner
{
    internal sealed class BVSingleton
    {
        private static BVSingleton _instance;
        public static BVSingleton Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new BVSingleton();
                return _instance;
            }
        }

        public string VersionMajor { get; set; }
        public string VersionMinor { get; set; }

#if UNITY_STANDALONE
        private string _changelistNumber;
#endif

#if UNITY_EDITOR
        public string P4Workspace { get; set; }
#endif

        private BVSingleton()
        {
#if UNITY_EDITOR
            VersionMajor = BVSystemCommands.Editor_ReadPropertyFromFile(nameof(VersionMajor));
            VersionMinor = BVSystemCommands.Editor_ReadPropertyFromFile(nameof(VersionMinor));
            P4Workspace = BVSystemCommands.Editor_ReadPropertyFromFile(nameof(P4Workspace));
#elif UNITY_STANDALONE
            var infoAssets = Resources.LoadAll<BVInfoScriptableObject>(nameof(BVInfoScriptableObject));
            if (infoAssets.Length == 1)
            {
                string fullVersion = infoAssets[0].Value;
                string[] splitVersion = fullVersion.Split('.');
                VersionMajor = splitVersion[0];
                VersionMinor = splitVersion[1];
                _changelistNumber = splitVersion[2];
            }
#endif
        }

        internal string GetChangelist()
        {
            // not a property because don't want to save to editor usersettings file
#if UNITY_EDITOR
            return BVSystemCommands.Editor_GetChangelistNumberFromCommand();
#elif UNITY_STANDALONE
            return Instance._changelistNumber;
#endif
        }
    }
}
