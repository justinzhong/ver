using System.Collections.Generic;

namespace Ver
{
    public interface ICommandParser
    {
        List<ICommand> Parse(string[] args);
    }
}
