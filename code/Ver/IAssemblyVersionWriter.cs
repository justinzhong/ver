using System;

namespace Ver
{
    public interface IAssemblyVersionWriter : IDisposable
    {
        void WriteAssemblyVersion(string version, bool isFileVersion = false);
    }
}
