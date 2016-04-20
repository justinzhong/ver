using System.Text;

namespace Ver
{
    public class AssemblyVersion
    {
        public int Major { get; set; }

        public int? Minor { get; set; }

        public int? Build { get; set; }

        public int? Revision { get; set; }

        public override string ToString()
        {
            var versionBuilder = new StringBuilder();

            versionBuilder.Append(Major.ToString());

            foreach (var component in new[] { Minor, Build, Revision })
            {
                if (!component.HasValue) return versionBuilder.ToString();

                versionBuilder.Append($".{component}");
            }

            return versionBuilder.ToString();
        }
    }
}
