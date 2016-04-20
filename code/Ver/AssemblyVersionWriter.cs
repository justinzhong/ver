using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Ver
{
    public sealed class AssemblyVersionWriter : IAssemblyVersionWriter
    {
        //[assembly: AssemblyVersion("1.0.0.0")]
        //[assembly: AssemblyFileVersion("1.0.0.0")]
        private const string VersionSignatureTemplate = @"^(?<FrontSignature>\[assembly: {VersionKind})(?<LeftPadding>[^""]+"")(?<AssemblyVersion>([^""]+))(?<RightPadding>.+)$";

        private static readonly string AssemblyVersionPattern = VersionSignatureTemplate.Replace("{VersionKind}", "AssemblyVersion");
        private static readonly string AssemblyFileVersionPattern = VersionSignatureTemplate.Replace("{VersionKind}", "AssemblyFileVersion");

        private bool _disposed;
        private readonly string _filePath;
        private readonly Func<string, string> _fileReader;
        private readonly Action<string, string> _fileWriter;
        private readonly List<Func<string, string>> _writeBuffer;

        public AssemblyVersionWriter(string filePath, Func<string, string> fileReader, Action<string, string> fileWriter)
        {
            if (filePath == null) throw new ArgumentNullException(nameof(filePath));
            if (string.IsNullOrWhiteSpace(filePath)) throw new ArgumentException("String cannot be empty.", nameof(filePath));
            if (fileReader == null) throw new ArgumentNullException(nameof(fileReader));
            if (fileWriter == null) throw new ArgumentNullException(nameof(fileWriter));

            _filePath = filePath;
            _fileReader = fileReader;
            _fileWriter = fileWriter;
            _writeBuffer = new List<Func<string, string>>();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void WriteAssemblyVersion(string version, bool isFileVersion = false)
        {
            if (isFileVersion)
            {
                _writeBuffer.Add(fileContent => UpdateAssemblyVersion(version, fileContent, AssemblyFileVersionPattern));
            }
            else
            {
                _writeBuffer.Add(fileContent => UpdateAssemblyVersion(version, fileContent, AssemblyVersionPattern));
            }
        }

        private void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                Flush();
                _disposed = true;
            }
        }

        private void Flush()
        {
            if (_writeBuffer.Count == 0) return;

            var fileContent = _fileReader(_filePath);

            _writeBuffer.ForEach(buffer => fileContent = buffer(fileContent));
            _fileWriter(_filePath, fileContent);
        }

        private string UpdateAssemblyVersion(string version, string fileContent, string versionPattern)
        {
            return Regex.Replace(fileContent, versionPattern, "${FrontSignature}${LeftPadding}" + version + "${RightPadding}", RegexOptions.Multiline);
        }
    }
}
