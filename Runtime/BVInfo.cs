// Copyright (c) 2022 Gabriel Lacayo
// See https://mit-license.org/
// Contact: lacayo@alumni.usc.edu

namespace BuildVersioner
{
    using static BVSingleton;

    public sealed class BVInfo
    {
        public static string VersionMajor => Instance.Enabled ? Instance.VersionMajor : "";
        public static string VersionMinor => Instance.Enabled ? Instance.VersionMinor : "";
        public static string Changelist => Instance.Enabled ? Instance.GetChangelist() : "";

        public static string GetVersionFormatted()
        {
            string major = VersionMajor;
            if (string.IsNullOrEmpty(major))
                major = "#";

            string minor = VersionMinor;
            if (string.IsNullOrEmpty(minor))
                minor = "#";

            string changelist = Changelist;
            if (string.IsNullOrEmpty(changelist))
                changelist = "######";

            return major + "." + minor + "." + changelist;
        }
    }
}
