// Copyright (c) 2022 Gabriel Lacayo
// See https://mit-license.org/
// Contact: lacayo@alumni.usc.edu

#if UNITY_STANDALONE
using UnityEngine;
#endif

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

        public bool Enabled { get; set; } = true;

        public string VersionMajor { get; set; }

        public string VersionMinor { get; set; }

#if UNITY_STANDALONE
        private string _changelist;
#endif

#if UNITY_EDITOR
        public string P4Workspace { get; set; }
#endif

        private BVSingleton()
        {
#if UNITY_EDITOR
            Enabled = bool.Parse(BVSystemCommands.Editor_ReadPropertyFromFile(nameof(Enabled)));
            if (Enabled)
            {
                VersionMajor = BVSystemCommands.Editor_ReadPropertyFromFile(nameof(VersionMajor));
                VersionMinor = BVSystemCommands.Editor_ReadPropertyFromFile(nameof(VersionMinor));
                P4Workspace = BVSystemCommands.Editor_ReadPropertyFromFile(nameof(P4Workspace));
            }
#elif UNITY_STANDALONE
            var infoAssets = Resources.LoadAll<BVInfoScriptableObject>(nameof(BVInfoScriptableObject));
            if (infoAssets.Length == 1)
            {
                Enabled = infoAssets[0].Enabled;
                if (Enabled)
                {
                    VersionMajor = infoAssets[0].VersionMajor;
                    VersionMinor = infoAssets[0].VersionMinor;
                    _changelist = infoAssets[0].Changelist;
                }
            }
#endif
        }

        internal string GetChangelist()
        {
            // not a property because don't want to save to editor usersettings file
#if UNITY_EDITOR
            return BVSystemCommands.Editor_GetChangelistNumberFromCommand();
#elif UNITY_STANDALONE
            return Instance._changelist;
#endif
        }
    }
}
