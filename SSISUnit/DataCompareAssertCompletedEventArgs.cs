using System;

using SsisUnitBase;
using SsisUnitBase.EventArgs;

namespace SsisUnit
{
    public sealed class DataCompareAssertCompletedEventArgs : AssertCompletedEventArgs
    {
        public DataCompareAssertCompletedEventArgs(DateTime testExecutionTime, string packageName, string taskName, string testName, string assertName, string testResultMsg, bool testPassed, CommandBase assertCommand, DataCompareCommandResults dataCompareCommandResults)
            : base(testExecutionTime, packageName, taskName, testName, assertName, testResultMsg, testPassed, assertCommand)
        {
            if (dataCompareCommandResults == null)
                throw new ArgumentNullException("dataCompareCommandResults");

            DataCompareCommandResults = dataCompareCommandResults;
        }

        public DataCompareAssertCompletedEventArgs(string assertName, TestResult testResult, CommandBase assertCommand, DataCompareCommandResults dataCompareCommandResults)
            : base(assertName, testResult, assertCommand)
        {
            if (dataCompareCommandResults == null)
                throw new ArgumentNullException("dataCompareCommandResults");

            DataCompareCommandResults = dataCompareCommandResults;
        }

        public DataCompareCommandResults DataCompareCommandResults { get; private set; }
    }
}