using System;

using SsisUnitBase.Enums;

namespace SsisUnitBase.EventArgs
{
    public class CommandStartedEventArgs : System.EventArgs
    {
        public CommandStartedEventArgs(DateTime startedExecutionTime, string commandName, string parentName, string package, CommandParentType commandParentType)
        {
            StartedExecutionTime = startedExecutionTime;
            CommandName = commandName;
            Package = package;
            ParentName = parentName;
            CommandParentType = commandParentType;
        }

        public string CommandName { get; private set; }
        public CommandParentType CommandParentType { get; private set; }
        public string Package { get; private set; }
        public string ParentName { get; private set; }
        public DateTime StartedExecutionTime { get; private set; }
    }
}