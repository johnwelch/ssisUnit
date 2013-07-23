using System;

namespace SsisUnit
{
    public sealed class DataCompareAssertCompletedEventArgs : AssertCompletedEventArgs
    {
        public DataCompareAssertCompletedEventArgs(DateTime testExecutionTime, string packageName, string taskName, string testName, string testResultMsg, bool testPassed, CommandBase assertCommand, DataCompareCommandResults dataCompareCommandResults)
            : base(testExecutionTime, packageName, taskName, testName, testResultMsg, testPassed, assertCommand)
        {
            if (dataCompareCommandResults == null)
                throw new ArgumentNullException("dataCompareCommandResults");

            DataCompareCommandResults = dataCompareCommandResults;
        }

        public DataCompareAssertCompletedEventArgs(TestResult testResult, CommandBase assertCommand, DataCompareCommandResults dataCompareCommandResults)
            : base(testResult, assertCommand)
        {
            if (dataCompareCommandResults == null)
                throw new ArgumentNullException("dataCompareCommandResults");

            DataCompareCommandResults = dataCompareCommandResults;
        }

        public DataCompareCommandResults DataCompareCommandResults { get; private set; }
    }
}