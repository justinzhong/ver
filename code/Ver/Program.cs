namespace Ver
{
    /// <summary>
    /// Usage:
    /// Increase the highest ranking version number of AssemblyVersion by 1.
    /// 
    /// Note: any lower ranked version numbers will be reset to 0.
    /// e.g. [assembly: AssemblyVersion("1.8.5.23")] => [assembly: AssemblyVersion("2.0.0.0")]
    /// 
    /// > ver +1
    /// 
    /// Increase the second ranking version number of AssemblyVersion by 1.
    /// e.g. [assembly: AssemblyVersion("1.8.5.23")] => [assembly: AssemblyVersion("1.9.0.0")]
    /// 
    /// > ver +0.1
    /// 
    /// Increase the lowest ranking version number of AssemblyVersion by 1.
    /// e.g. [assembly: AssemblyVersion("1.0.0.0")] => [assembly: AssemblyVersion("1.0.0.1")]
    /// 
    /// > ver +0.0.0.1
    /// 
    /// Increase the lowest ranking version number of AssemblyFileVersion by 1.
    /// e.g. [assembly: AssemblyFileVersion("1.0.0.0")] => [assembly: AssemblyFileVersion("1.0.0.1")]
    /// 
    /// > ver +0.0.0.1 -target=fileVersion
    /// 
    /// Increase the first ranking version number of AssemblyFileVersion by 1.
    /// e.g. [assembly: AssemblyFileVersion("1.8.5.23")] => [assembly: AssemblyFileVersion("2.0.0.0")]
    /// 
    /// > ver +1 -target=fileVersion
    /// 
    /// Specify the AssemblyInfo.cs file's location as the folder 'Properties'
    /// relative to the current executing path.
    /// e.g. [assembly: AssemblyFileVersion("1.7.0.0")] => [assembly: AssemblyFileVersion("2.0.0.0")]
    /// 
    /// > ver +1 -target=fileVersion -path=Properties\AssemblyInfo.cs
    /// 
    /// Note: by default, it is assumed that the common use of the 'ver' program
    /// is from the current csproj and when nothing is specified for the path 
    /// parameter, the value is assumed to be:
    /// Properties\AssemblyInfo.cs
    /// 
    /// Exact rules apply for subtraction, and if the number falls below 0 an 
    /// ArgumentOutOfRangeException will be thrown.
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            var serviceProvider = new ApplicationServiceProvider();
            var commandParser = serviceProvider.GetService<ICommandParser>();
            var commands = commandParser.Parse(args);

            commands.ForEach(command => command.Execute());
        }
    }
}
