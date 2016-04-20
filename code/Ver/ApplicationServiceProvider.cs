using System;
using System.Collections.Generic;
using Ver.Infrastructure;

namespace Ver
{
    public class ApplicationServiceProvider : IApplicationServiceProvider
    {
        private readonly Dictionary<Type, Func<object>> _services;

        public ApplicationServiceProvider()
        {
            _services = new Dictionary<Type, Func<object>>();
            _services.Add(typeof(IVersionUpdateCommandFactory), () => new VersionUpdateCommandFactory());

            SetupCommandParser();
        }

        private void SetupCommandParser()
        {
            // Register all command filters.
            //
            // Note that commands are parsed and therefore executed in this
            // order.
            Func<ICommandParser> commandParser = () => new CommandParser(new[]
            {
                GetService<IVersionUpdateCommandFactory>().Build()
            });

            _services.Add(typeof(ICommandParser), commandParser);
        }

        public T GetService<T>()
            where T : class
        {
            return (T)_services[typeof(T)]();
        }
    }
}
