using System;

using SsisUnitBase.Enums;

namespace SsisUnitBase.EventArgs
{
    public class CommandFailedEventArgs : System.EventArgs
    {
        public CommandFailedEventArgs(DateTime failedExecutionTime, string commandName, string parentName, string package, string results, CommandParentType commandParentType)
        {
            CommandName = commandName;
            FailedExecutionTime = failedExecutionTime;
            ParentName = parentName;
            Package = package;
            Results = results;
            CommandParentType = commandParentType;
        }

        public CommandFailedEventArgs(DateTime failedExecutionTime, string commandName, string parentName, string package, string results, CommandResultsBase commandResults, CommandParentType commandParentType)
            : this(failedExecutionTime, commandName, parentName, package, results, commandParentType)
        {
            CommandResults = commandResults;
        }

        public string CommandName { get; private set; }
        public CommandParentType CommandParentType { get; private set; }
        public CommandResultsBase CommandResults { get; private set; }
        public DateTime FailedExecutionTime { get; private set; }
        public string Package { get; private set; }
        public string ParentName { get; private set; }
        public string Results { get; private set; }
    }
}