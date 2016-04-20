using System;
using System.Text.RegularExpressions;

namespace Ver
{
    public class AssemblyVersionParser : IAssemblyVersionParser
    {
        private const string AssemblyVersionPattern = @"(?<Major>[0-9]+)(\.(?<Minor>[0-9]+))?(\.(?<Build>[0-9]+))?(\.(?<Revision>[0-9]+))?";

        public AssemblyVersion Parse(string text)
        {
            if (text == null) throw new ArgumentNullException(nameof(text));
            if (string.IsNullOrWhiteSpace(text)) throw new ArgumentException("String cannot be empty.", nameof(text));

            var match = Regex.Match(text, AssemblyVersionPattern);

            if (!match.Success) throw new FormatException(@"Text '{text}' does not match the version number pattern.");

            var major = match.Groups["Major"];

            if (!major.Success) throw new FormatException("Major version number must be specified.");

            return new AssemblyVersion
            {
                Major = ParseInt(major.Value).Value,
                Minor = ParseInt(match.Groups["Minor"].Value),
                Build = ParseInt(match.Groups["Build"].Value),
                Revision = ParseInt(match.Groups["Revision"].Value)
            };
        }

        private int? ParseInt(string input)
        {
            int parsedInt = 0;

            if (int.TryParse(input, out parsedInt))
            {
                return parsedInt;
            }

            return default(int?);
        }
    }
}
