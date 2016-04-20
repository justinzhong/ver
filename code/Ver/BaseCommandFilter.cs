using System;
using System.Linq;

namespace Ver
{
    public abstract class BaseCommandFilter : ICommandFilter
    {
        public abstract CommandFilterModel Filter(string[] args);

        /// <summary>
        /// Identifies the argument from <paramref name="args"/> that is
        /// recongised by <paramref name="filterRule"/> and collate remaining
        /// arguments.
        /// </summary>
        /// <param name="args">Array of arguments to be filtered.</param>
        /// <param name="filterRule">The filter rule to be applied on each 
        /// argument in the array.</param>
        /// <returns>An array of arguments that were ignored by the filter rule.
        /// </returns>
        protected string[] FilterArgs(string[] args, Func<string, bool> filterRule)
        {
            var stopFilter = false;

            return args.Where(arg =>
            {
                // All subsequent arg string will be collated for the next
                // filter to scan.
                if (stopFilter) return true;

                // Check if the current arg string triggers the filter rule.
                // If this argument does not match the filter rule, collect
                // this argument and continue.
                if (!filterRule(arg)) return true;

                // Otherwise, indicate to stop filtering and skip
                // this argument as it matches the filterRule.
                stopFilter = true;

                return false;
            }).ToArray();
        }
    }
}
