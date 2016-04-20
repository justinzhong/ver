using System;
using System.Text.RegularExpressions;

namespace Ver
{
    public class AssemblyVersionReader : IAssemblyVersionReader
    {
        //[assembly: AssemblyVersion("1.0.0.0")]
        //[assembly: AssemblyFileVersion("1.0.0.0")]
        private const string VersionSignatureTemplate = @"^(?<FrontSignature>[assembly: {VersionKind})([^""]+"")(?<AssemblyVersion>([^""]+))(.+)$";

        private static readonly string AssemblyVersionPattern = VersionSignatureTemplate.Replace("{VersionKind}", "AssemblyVersion");
        private static readonly string AssemblyFileVersionPattern = VersionSignatureTemplate.Replace("{VersionKind}", "AssemblyFileVersion");

        private readonly string _filePath;
        private readonly Func<string, string> _fileReader;
        private readonly IAssemblyVersionParser _assemblyVersionParser;

        public AssemblyVersionReader(string filePath, Func<string, string> fileReader, IAssemblyVersionParser assemblyVersionParser)
        {
            if (filePath == null) throw new ArgumentNullException(nameof(filePath));
            if (string.IsNullOrWhiteSpace(filePath)) throw new ArgumentException("String cannot be empty or whitespace.", nameof(filePath));
            if (fileReader == null) throw new ArgumentNullException(nameof(fileReader));
            if (assemblyVersionParser == null) throw new ArgumentNullException(nameof(assemblyVersionParser));

            _filePath = filePath;
            _fileReader = fileReader;
            _assemblyVersionParser = assemblyVersionParser;
        }

        public AssemblyVersionGroupModel Read()
        {
            var fileContent = _fileReader(_filePath);

            return new AssemblyVersionGroupModel
            {
                AssemblyVersion = ParseVersion(fileContent, AssemblyVersionPattern),
                AssemblyFileVersion = ParseVersion(fileContent, AssemblyFileVersionPattern)
            };
        }

        private AssemblyVersion ParseVersion(string content, string pattern)
        {
            var match = Regex.Match(content, pattern, RegexOptions.Multiline);

            if (match == null || !match.Success) throw new ApplicationException($"Unable to match the assembly version details with the pattern {pattern}");

            return _assemblyVersionParser.Parse(match.Groups["AssemblyVersion"].Value);
        }
    }
}
