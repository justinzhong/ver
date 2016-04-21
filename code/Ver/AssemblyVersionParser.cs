using System;
using System.Text.RegularExpressions;

namespace Ver
{
    public class AssemblyVersionParser : IAssemblyVersionParser
    {
        private const string AssemblyVersionPattern = @"^(?<Abbreviation>(?<Number>[0-9]+)(?<Identifier>[Mmbr]))$|(?<Major>[0-9]+)(\.(?<Minor>[0-9]+))?(\.(?<Build>[0-9]+))?(\.(?<Revision>[0-9]+))?";

        public AssemblyVersion Parse(string text)
        {
            if (text == null) throw new ArgumentNullException(nameof(text));
            if (string.IsNullOrWhiteSpace(text)) throw new ArgumentException("String cannot be empty.", nameof(text));

            var match = Regex.Match(text, AssemblyVersionPattern);

            if (!match.Success) throw new FormatException(@"Text '{text}' does not match the version number pattern.");

            if (match.Groups["Abbreviation"].Success) return ParseAbbreviation(match);

            return ParseVersionNumber(match);
        }

        private AssemblyVersion ParseAbbreviation(Match abbreviation)
        {
            if (!abbreviation.Groups["Number"].Success) throw new ApplicationException("Number missing, expected an integer.");
            if (!abbreviation.Groups["Identifier"].Success) throw new ApplicationException("Identifier missing, expected value is: M, m, b, r.");

            var identifier = abbreviation.Groups["Identifier"].Value;
            var number = ParseInt(abbreviation.Groups["Number"].Value);

            switch (identifier)
            {
                case "M":
                    return new AssemblyVersion { Major = number.Value };
                case "m":
                    return new AssemblyVersion { Minor = number };
                case "b":
                    return new AssemblyVersion { Build = number };
                case "r":
                    return new AssemblyVersion { Revision = number };
                default:
                    throw new ApplicationException("Unexpcted identifier value, expected value is: M, m, b, r.");
            }
        }

        private AssemblyVersion ParseVersionNumber(Match versionNumber)
        {
            var major = versionNumber.Groups["Major"];

            if (!major.Success) throw new FormatException("Major version number must be specified.");

            return new AssemblyVersion
            {
                Major = ParseInt(major.Value).Value,
                Minor = ParseInt(versionNumber.Groups["Minor"].Value),
                Build = ParseInt(versionNumber.Groups["Build"].Value),
                Revision = ParseInt(versionNumber.Groups["Revision"].Value)
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
