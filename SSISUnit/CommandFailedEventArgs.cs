using System;

namespace SsisUnit
{
    public class CommandFailedEventArgs : EventArgs
    {
        public CommandFailedEventArgs(DateTime failedExecutionTime, string commandName, string parentName, string package, string results)
        {
            CommandName = commandName;
            FailedExecutionTime = failedExecutionTime;
            Package = package;
            Results = results;
        }

        public string CommandName { get; private set; }
        public DateTime FailedExecutionTime { get; private set; }
        public string Package { get; private set; }
        public string ParentName { get; private set; }
        public string Results { get; private set; }
    }
}