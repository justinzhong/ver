using System;
using System.Collections.Generic;

namespace Ver
{
    public class CommandParser : ICommandParser
    {
        private readonly ICommandFilter[] _commandFilters;

        public CommandParser(ICommandFilter[] commandFilters)
        {
            if (commandFilters == null) throw new ArgumentNullException(nameof(commandFilters));

            _commandFilters = commandFilters;
        }

        public List<ICommand> Parse(string[] args)
        {
            var commands = new List<ICommand>();
            var filteredArgs = args;

            foreach (var filter in _commandFilters)
            {
                var model = filter.Filter(filteredArgs);

                if (model == null || model.Command == null) continue;

                commands.Add(model.Command);
                filteredArgs = model.Args;
            }

            return commands;
        }
    }
}
