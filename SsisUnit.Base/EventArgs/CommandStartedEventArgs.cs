using System;

namespace SsisUnitBase.EventArgs
{
    public class CommandStartedEventArgs : System.EventArgs
    {
        public CommandStartedEventArgs(DateTime startedExecutionTime, string commandName, string parentName, string package)
        {
            StartedExecutionTime = startedExecutionTime;
            CommandName = commandName;
            Package = package;
            ParentName = parentName;
        }

        public DateTime StartedExecutionTime { get; private set; }
        public string Package { get; private set; }
        public string CommandName { get; private set; }
        public string ParentName { get; private set; }
    }
}