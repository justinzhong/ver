using System;

namespace Ver
{
    public class VersionUpdateCommandFilter : BaseCommandFilter
    {
        private readonly IAssemblyVersionParser _assemblyVersionParser;
        private readonly Func<string, IAssemblyVersionReader> _assemblyVersionReaderFactory;
        private readonly Func<string, IAssemblyVersionWriter> _assemblyVersionWriterFactory;

        public VersionUpdateCommandFilter(IAssemblyVersionParser assemblyVersionParser,
            Func<string, IAssemblyVersionReader> assemblyVersionReaderFactory, 
            Func<string, IAssemblyVersionWriter> assemblyVersionWriterFactory)
        {
            if (assemblyVersionParser == null) throw new ArgumentNullException(nameof(assemblyVersionParser));
            if (assemblyVersionReaderFactory == null) throw new ArgumentNullException(nameof(assemblyVersionReaderFactory));
            if (assemblyVersionWriterFactory == null) throw new ArgumentNullException(nameof(assemblyVersionWriterFactory));

            _assemblyVersionParser = assemblyVersionParser;
            _assemblyVersionReaderFactory = assemblyVersionReaderFactory;
            _assemblyVersionWriterFactory = assemblyVersionWriterFactory;
        }

        public override CommandFilterModel Filter(string[] args)
        {
            // Argument does not contain any information that can be used
            // to construct VersionUpdateCommand.
            if (args == null || args.Length == 0) return null;

            var filteredArgs = args;
            var versionUpdateModel = new VersionUpdateModel();

            // Extract the version update value, e.g. 0.1
            // TODO: Use observer (event) based trigger to notify information capture.
            filteredArgs = FilterArgs(filteredArgs, arg =>
            {
                if (arg.StartsWith("+") || arg.StartsWith("-"))
                {
                    versionUpdateModel.Increment = arg.StartsWith("+");
                    versionUpdateModel.VersionUpdate = _assemblyVersionParser.Parse(arg.Substring(1));
                    return true;
                }

                return false;
            });

            // Extract the target value, e.g. fileVersion
            filteredArgs = FilterArgs(filteredArgs, arg =>
            {
                if (arg.StartsWith("-f"))
                {
                    versionUpdateModel.IsFileVersion = true;
                    return true;
                }

                return false;
            });

            // Extract the path value, e.g. Properties\AssemblyInfo.cs
            filteredArgs = FilterArgs(filteredArgs, arg =>
            {
                if (arg.StartsWith("-path="))
                {
                    versionUpdateModel.AssemblyInfoPath = arg.Substring("-path=".Length);
                    return false;
                }

                return false;
            });

            return new CommandFilterModel
            {
                Command = new VersionUpdateCommand(versionUpdateModel, _assemblyVersionReaderFactory, _assemblyVersionWriterFactory),
                Args = filteredArgs
            };
        }
    }
}
