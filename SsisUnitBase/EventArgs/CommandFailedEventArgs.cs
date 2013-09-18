using System;

namespace SsisUnitBase.EventArgs
{
    public class CommandFailedEventArgs : System.EventArgs
    {
        public CommandFailedEventArgs(DateTime failedExecutionTime, string commandName, string parentName, string package, string results)
        {
            CommandName = commandName;
            FailedExecutionTime = failedExecutionTime;
            ParentName = parentName;
            Package = package;
            Results = results;
        }

        public CommandFailedEventArgs(DateTime failedExecutionTime, string commandName, string parentName, string package, string results, CommandResultsBase commandResults)
            : this(failedExecutionTime, commandName, parentName, package, results)
        {
            CommandResults = commandResults;
        }

        public string CommandName { get; private set; }
        public CommandResultsBase CommandResults { get; private set; }
        public DateTime FailedExecutionTime { get; private set; }
        public string Package { get; private set; }
        public string ParentName { get; private set; }
        public string Results { get; private set; }
    }
}