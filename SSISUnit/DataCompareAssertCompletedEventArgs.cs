using System;

namespace SsisUnit
{
    public sealed class DataCompareAssertCompletedEventArgs : AssertCompletedEventArgs
    {
        public DataCompareAssertCompletedEventArgs(DateTime testExecutionTime, string packageName, string taskName, string testName, string testResultMsg, bool testPassed, DataCompareCommandResults dataCompareCommandResults)
            : base(testExecutionTime, packageName, taskName, testName, testResultMsg, testPassed)
        {
            if (dataCompareCommandResults == null)
                throw new ArgumentNullException("dataCompareCommandResults");

            DataCompareCommandResults = dataCompareCommandResults;
        }

        public DataCompareCommandResults DataCompareCommandResults { get; private set; }

        public DataCompareAssertCompletedEventArgs(TestResult testResult, DataCompareCommandResults dataCompareCommandResults)
            : base(testResult)
        {
            if (dataCompareCommandResults == null)
                throw new ArgumentNullException("dataCompareCommandResults");

            DataCompareCommandResults = dataCompareCommandResults;
        }
    }
}