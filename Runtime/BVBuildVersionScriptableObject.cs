using UnityEngine;

namespace BuildVersioner
{
    /// <summary>
    /// Holds version number M.m.CL to be referenced from runtime code
    /// Not typically created manually besides when building
    /// </summary>
    internal class BVBuildVersionScriptableObject : ScriptableObject
    {
        public string Value;
    }
}
