using System;
using System.Linq;

namespace Ver
{
    public class VersionUpdateCommand : ICommand
    {
        private const string DefaultAssemblyInfoPath = @"Properties\AssemblyInfo.cs";
        private readonly VersionUpdateModel _versionUpdateModel;
        private readonly Func<string, IAssemblyVersionReader> _assemblyVersionReaderFactory;
        private readonly Func<string, IAssemblyVersionWriter> _assemblyVersionWriterFactory;

        public VersionUpdateCommand(VersionUpdateModel versionUpdateModel,
            Func<string, IAssemblyVersionReader> assemblyVersionReaderFactory,
            Func<string, IAssemblyVersionWriter> assemblyVersionWriterFactory)
        {
            if (versionUpdateModel == null) throw new ArgumentNullException(nameof(versionUpdateModel));
            if (string.IsNullOrEmpty(versionUpdateModel.AssemblyInfoPath)) versionUpdateModel.AssemblyInfoPath = DefaultAssemblyInfoPath;

            _versionUpdateModel = versionUpdateModel;
            _assemblyVersionReaderFactory = assemblyVersionReaderFactory;
            _assemblyVersionWriterFactory = assemblyVersionWriterFactory;
        }

        public void Execute()
        {
            var versionGroupModel = _assemblyVersionReaderFactory(_versionUpdateModel.AssemblyInfoPath).Read();
            var updatedVersion = UpdateVersion(_versionUpdateModel.IsFileVersion ? versionGroupModel.AssemblyFileVersion : versionGroupModel.AssemblyVersion);

            using (var writer = _assemblyVersionWriterFactory(_versionUpdateModel.AssemblyInfoPath))
            {
                writer.WriteAssemblyVersion(updatedVersion.ToString(), _versionUpdateModel.IsFileVersion);
            }
        }

        private AssemblyVersion UpdateVersion(AssemblyVersion assemblyVersion)
        {
            var versionUpdate = _versionUpdateModel.VersionUpdate;
            var updatedVersion = new AssemblyVersion
            {
                Major = assemblyVersion.Major,
                Minor = assemblyVersion.Minor,
                Build = assemblyVersion.Build,
                Revision = assemblyVersion.Revision
            };

            if (versionUpdate.Major != 0)
            {
                updatedVersion.Major += _versionUpdateModel.Increment ? versionUpdate.Major : -versionUpdate.Major;
                ResetVersionComponents(updatedVersion, 3);
            }

            if (versionUpdate.Minor.HasValue && versionUpdate.Minor != 0)
            {
                updatedVersion.Minor += _versionUpdateModel.Increment ? versionUpdate.Minor : -versionUpdate.Minor;
                ResetVersionComponents(updatedVersion, 2);
            }

            if (versionUpdate.Build.HasValue && versionUpdate.Build != 0)
            {
                updatedVersion.Build += _versionUpdateModel.Increment ? versionUpdate.Build : -versionUpdate.Build;
                ResetVersionComponents(updatedVersion, 1);
            }

            if (versionUpdate.Revision.HasValue && versionUpdate.Revision != 0)
            {
                updatedVersion.Revision += _versionUpdateModel.Increment ? versionUpdate.Revision : -versionUpdate.Revision;
            }

            ValidateVersionNumbers(updatedVersion);

            return updatedVersion;
        }

        private void ResetVersionComponents(AssemblyVersion version, int components)
        {
            if (components > 4) throw new ArgumentException($"AssemblyVersion has a maximum of four components, but received argument to reset {components}.", nameof(components));

            var resetComponents = new Action[] { () => version.Major = 0, () => version.Minor = 0, () => version.Build = 0, () => version.Revision = 0 };
            var applicableComponents = resetComponents.Skip(4 - components);

            foreach (var component in applicableComponents)
            {
                component();
            }
        }

        private void ValidateVersionNumbers(AssemblyVersion updatedVersion)
        {
            if (updatedVersion.Major < 0 ||
                (updatedVersion.Minor.HasValue && updatedVersion.Minor < 0) ||
                (updatedVersion.Build.HasValue && updatedVersion.Build < 0) ||
                (updatedVersion.Revision.HasValue && updatedVersion.Revision < 0))
            {
                throw new ApplicationException($"The update results in a version with one or more negative component: {updatedVersion}");
            }
        }
    }
}
