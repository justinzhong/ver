using System;
using System.IO;

namespace Ver.Infrastructure
{
    sealed class VersionUpdateCommandFactory : IVersionUpdateCommandFactory
    {
        public VersionUpdateCommandFilter Build()
        {
            Func<string, string> fileReader = filePath => File.ReadAllText(Path.GetFullPath(filePath));

            return new VersionUpdateCommandFilter(
                new AssemblyVersionParser(),
                (filePath) => new AssemblyVersionReader(filePath, fileReader, new AssemblyVersionParser()),
                (filePath) => new AssemblyVersionWriter(filePath, fileReader, (path, fileContent) => File.WriteAllText(path, fileContent)));
        }
    }
}
