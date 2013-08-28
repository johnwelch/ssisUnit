using System;

namespace SsisUnitBase.EventArgs
{
    public class CommandCompletedEventArgs : System.EventArgs
    {
        public CommandCompletedEventArgs(DateTime completedExecutionTime, string commandName, string parentName, string package, string results)
        {
            CompletedExecutionTime = completedExecutionTime;
            CommandName = commandName;
            Package = package;
            ParentName = parentName;
            Results = results;
        }

        public DateTime CompletedExecutionTime { get; private set; }
        public string Package { get; private set; }
        public string CommandName { get; private set; }
        public string ParentName { get; private set; }
        public string Results { get; private set; }
    }
}