namespace Ver.Infrastructure
{
    interface IVersionUpdateCommandFactory
    {
        VersionUpdateCommandFilter Build();
    }
}
