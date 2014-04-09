using System;

using SsisUnitBase.Enums;

namespace SsisUnitBase.EventArgs
{
    public class CommandCompletedEventArgs : System.EventArgs
    {
        public CommandCompletedEventArgs(DateTime completedExecutionTime, string commandName, string parentName, string package, string results, CommandParentType commandParentType)
        {
            CompletedExecutionTime = completedExecutionTime;
            CommandName = commandName;
            Package = package;
            ParentName = parentName;
            Results = results;
            CommandParentType = commandParentType;
        }

        public string CommandName { get; private set; }
        public CommandParentType CommandParentType { get; private set; }
        public DateTime CompletedExecutionTime { get; private set; }
        public string Package { get; private set; }
        public string ParentName { get; private set; }
        public string Results { get; private set; }
    }
}