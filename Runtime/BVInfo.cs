
namespace BuildVersioner
{
    using static BVSingleton;

    public sealed class BVInfo
    {
        public static string VersionMajor => Instance.VersionMajor;
        public static string VersionMinor => Instance.VersionMinor;
        public static string Changelist => Instance.GetChangelist();

        public static string GetVersionFormatted()
        {
            string major = Instance.VersionMajor;
            if (string.IsNullOrEmpty(major))
                major = "#";

            string minor = Instance.VersionMinor;
            if (string.IsNullOrEmpty(minor))
                minor = "#";

            string changelist = Instance.GetChangelist();
            if (string.IsNullOrEmpty(changelist))
                changelist = "######";

            return major + "." + minor + "." + changelist;
        }
    }
}
