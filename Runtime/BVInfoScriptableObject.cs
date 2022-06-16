using UnityEngine;

namespace BuildVersioner
{
    /// <summary>
    /// Holds version number M.m.CL to be referenced from runtime code
    /// Not typically created manually besides when building
    /// </summary>
    internal sealed class BVInfoScriptableObject : ScriptableObject
    {
        public bool Enabled;
        public string VersionMajor;
        public string VersionMinor;
        public string Changelist;
    }
}
