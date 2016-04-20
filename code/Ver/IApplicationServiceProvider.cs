namespace Ver
{
    public interface IApplicationServiceProvider
    {
        T GetService<T>()
            where T : class;
    }
}
