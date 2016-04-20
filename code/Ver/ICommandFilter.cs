namespace Ver
{
    public interface ICommandFilter
    {
        CommandFilterModel Filter(string[] args);
    }
}
