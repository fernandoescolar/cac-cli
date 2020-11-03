using Cac.Output;
using NuGet.Common;
using NuGet.Frameworks;
using NuGet.Packaging;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace Cac.Plugins
{
    public class PackageManager
    {
        private static readonly List<string> SupportedFrameworks = new List<string>(new[]{
            "netcoreapp3.1",
            "netstandard2.1",
            "netstandard2.0",
            "netstandard1.6",
            "netstandard1.5",
            "netstandard1.4",
            "netstandard1.3",
            "netstandard1.2",
            "netstandard1.1",
            "netstandard1.0"
        });

        private readonly string _packagesFolder;
        private readonly IOutput _output;
        private readonly ILogger _logger;
        private readonly SourceCacheContext _cache;
        private readonly SourceRepository _repository;
        private List<string> _loadedDependencies;

        public PackageManager(string packagesFolder, IOutput output)
        {
            _packagesFolder = packagesFolder;
            _output = output;
            _logger = NullLogger.Instance;
            _cache = new SourceCacheContext();
            _repository = Repository.Factory.GetCoreV3("https://api.nuget.org/v3/index.json");
        }

        public async Task<string> InstallAsync(string packageId, string version, CancellationToken cancellationToken = default)
        {
            _loadedDependencies = new List<string>();
            var destinationFolderPath = Path.Combine(_packagesFolder, packageId, version);
            var destinationFilePath = Path.Combine(destinationFolderPath, $"{packageId}.dll");
            if (Directory.Exists(destinationFolderPath) && File.Exists(destinationFilePath))
            {
                return destinationFilePath;
            }

            await InstallAsync(destinationFolderPath, packageId, version, SupportedFrameworks.First(), cancellationToken);

            var extensibilityDll = Path.Combine(destinationFolderPath, "Cac.Extensibility.dll");
            if (File.Exists(extensibilityDll))
            {
                File.Delete(extensibilityDll);
            }

            return destinationFilePath;
        }

        private async Task InstallAsync(string destinationFolderPath, string packageId, string version, string maxFramework, CancellationToken cancellationToken)
        {
            var resource = await _repository.GetResourceAsync<FindPackageByIdResource>();
            var versions = await resource.GetAllVersionsAsync(packageId, _cache, _logger, cancellationToken);
            var packageVersion = versions.FirstOrDefault(x => x.ToString().Equals(version, StringComparison.InvariantCultureIgnoreCase));
            await DownloadPackageAsync(destinationFolderPath, packageId, resource, packageVersion, maxFramework, cancellationToken);
        }

        private async Task InstallAsync(string destinationFolderPath, string packageId, VersionRange versionRange, string maxFramework, CancellationToken cancellationToken)
        {
            var resource = await _repository.GetResourceAsync<FindPackageByIdResource>();
            var versions = await resource.GetAllVersionsAsync(packageId, _cache, _logger, cancellationToken);
            var packageVersion = versionRange.FindBestMatch(versions);
            await DownloadPackageAsync(destinationFolderPath, packageId, resource, packageVersion, maxFramework, cancellationToken);
        }

        private async Task InstallDependenciesAsync(string destinationFolderPath, PackageArchiveReader packageReader, string framework, CancellationToken cancellationToken)
        {
            var nuspecReader = await packageReader.GetNuspecReaderAsync(cancellationToken);
            var dependencies = nuspecReader.GetDependencyGroups()
                                           .Where(x => x.TargetFramework.AllFrameworkVersions || IsEqual(x.TargetFramework, framework))
                                           .SelectMany(x => x.Packages);

            _output.BeginSection();
            foreach (var dependency in dependencies)
            {
                if (_loadedDependencies.Contains(dependency.Id)) continue;

                _loadedDependencies.Add(dependency.Id);

                if (dependency.Id == "Cac.Extensibility") continue;              
                await InstallAsync(destinationFolderPath, dependency.Id, dependency.VersionRange, framework, cancellationToken);
            }
            _output.EndSection();
        }

        private async Task DownloadPackageAsync(string destinationFolderPath, string packageId, FindPackageByIdResource resource, NuGetVersion packageVersion, string maxFramework, CancellationToken cancellationToken)
        {
            using (var packageStream = new MemoryStream())
            {
                await resource.CopyNupkgToStreamAsync(packageId, packageVersion, packageStream, _cache, _logger, cancellationToken);
                using (var packageReader = new PackageArchiveReader(packageStream))
                {
                    _output.Verbose.Write($"{packageId} {packageVersion.Version}");
                    if (AssemblyIsLoaded(packageId))
                    {
                        _output.Verbose.WriteLine(" (Skipped)");
                        return;
                    }

                    _output.Verbose.WriteLine(string.Empty);

                    var (os, arch) = GetOsAndArchitecture();
                    var framework = await SelectFrameworkAsync(packageReader, maxFramework, cancellationToken);
                    AddRuntimeNativeFiles(destinationFolderPath, packageReader, os, arch);

                    if (string.IsNullOrEmpty(framework)) return;
                    if (framework == "netstandard1.0") return;

                    AddRuntimeFrameworkFiles(destinationFolderPath, packageReader, os, framework);
                    AddLibraryFiles(destinationFolderPath, packageReader, framework);

                    await InstallDependenciesAsync(destinationFolderPath, packageReader, framework, cancellationToken);
                }
            }
        }

        private void AddLibraryFiles(string destinationFolderPath, PackageArchiveReader packageReader, string framework)
        {
            var files = packageReader.GetFiles()
                                     .Where(f => f.StartsWith($"lib/{framework}/", StringComparison.InvariantCultureIgnoreCase));

            foreach (var file in files)
            {
                var path = Path.Combine(destinationFolderPath, Path.GetFileName(file));
                if (File.Exists(path)) continue;
                packageReader.ExtractFile(file, path, _logger);
            }
        }

        private void AddRuntimeFrameworkFiles(string destinationFolderPath, PackageArchiveReader packageReader, string os, string framework)
        {
            var runtimeFiles = packageReader.GetFiles()
                                            .Where(f => f.StartsWith($"runtimes/{os}/lib/{framework}", StringComparison.InvariantCultureIgnoreCase));

            foreach (var file in runtimeFiles)
            {
                var path = Path.Combine(destinationFolderPath, Path.GetFileName(file));
                packageReader.ExtractFile(file, path, _logger);
            }
        }

        private void AddRuntimeNativeFiles(string destinationFolderPath, PackageArchiveReader packageReader, string os, string arch)
        {
            var nativeFiles = packageReader.GetFiles()
                                           .Where(f => f.StartsWith($"runtimes/{os}-{arch}/native", StringComparison.InvariantCultureIgnoreCase));

            foreach (var file in nativeFiles)
            {
                var path = Path.Combine(destinationFolderPath, Path.GetFileName(file));
                if (File.Exists(path)) File.Delete(path);
                packageReader.ExtractFile(file, path, _logger);
            }
        }

        private async Task<string> SelectFrameworkAsync(PackageArchiveReader packageReader, string maxFramework, CancellationToken cancellationToken)
        {
            var frameworks = await packageReader.GetSupportedFrameworksAsync(cancellationToken);
            for (var index = SupportedFrameworks.IndexOf(maxFramework); index < SupportedFrameworks.Count; index++)
            {
                var framework = SupportedFrameworks[index];
                var fw = frameworks.FirstOrDefault(x => x.AllFrameworkVersions || IsEqual(x, framework));
                if (fw != null)
                    return framework;
            }

            return string.Empty;
        }

        private bool IsEqual(NuGetFramework fw, string framework)
        {
            var frameworkName = "." + framework.Substring(0, framework.Length - 3);
            var frameworkVersion = new Version(framework.Substring(framework.Length - 3) + ".0.0");

            return fw.Framework.Equals(frameworkName, StringComparison.InvariantCultureIgnoreCase)
                   && fw.Version == frameworkVersion;
        }

        private static (string os, string arch) GetOsAndArchitecture()
        {
            var os = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "win"
                   : RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? "unix"
                   : RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? "osx"
                   : "unknown";
            var arch = RuntimeInformation.OSArchitecture.ToString().ToLowerInvariant();
            return (os, arch);
        }

        private static bool AssemblyIsLoaded(string packageId)
        {
            try
            {
                var a = Assembly.Load(packageId);
                return a != null;
            }
            catch
            {
            }

            return false;
        }
    }
}
