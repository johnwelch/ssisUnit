using System;

using SsisUnitBase.Enums;
using SsisUnitBase.EventArgs;

namespace SsisUnit
{
    public class DataCompareCommandCompletedEventArgs : CommandCompletedEventArgs
    {
        public DataCompareCommandCompletedEventArgs(DateTime completedExecutionTime, string commandName, string parentName, string package, string results, DataCompareCommandResults dataCompareCommandResults, CommandParentType commandParentType)
            : base(completedExecutionTime, commandName, parentName, package, results, commandParentType)
        {
            if (dataCompareCommandResults == null)
                throw new ArgumentNullException("dataCompareCommandResults");

            DataCompareCommandResults = dataCompareCommandResults;
        }

        public DataCompareCommandResults DataCompareCommandResults { get; private set; }
    }
}