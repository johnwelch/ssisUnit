using System;
using System.Collections.Generic;
using System.Text;

namespace SsisUnit
{
    public class DataCompareCommandCompletedEventArgs : CommandCompletedEventArgs
    {
        public DataCompareCommandCompletedEventArgs(DateTime completedExecutionTime, string commandName, string parentName, string package, string results, DataCompareCommandResults dataCompareCommandResults)
            : base(completedExecutionTime, commandName, parentName, package, results)
        {
            if (dataCompareCommandResults == null)
                throw new ArgumentNullException("dataCompareCommandResults");

            DataCompareCommandResults = dataCompareCommandResults;
        }

        public DataCompareCommandResults DataCompareCommandResults { get; private set; }
    }
}
