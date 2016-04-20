namespace Ver
{
    public class VersionUpdateModel
    {
        public AssemblyVersion VersionUpdate { get; set; }

        public bool Increment { get; set; }

        public bool IsFileVersion { get; set; }

        public string AssemblyInfoPath { get; set; }
    }
}
