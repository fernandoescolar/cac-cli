using System;

namespace Cac.Exceptions
{
    public class CanNotDownloadPackageException : Exception
    {
        private const string CanNotDownloadPackageMessage = "Can not download package";
        public CanNotDownloadPackageException(string packageName, string version): this(packageName, version, default)
        {
        }

        public CanNotDownloadPackageException(string packageName, string version, Exception innerException) : base($"{CanNotDownloadPackageMessage}: '{packageName} {version}'", innerException)
        {
            PackageName = packageName;
            Version = version;
        }


        public string PackageName { get; }

        public string Version { get; }
    }
}
