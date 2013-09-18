using System;

namespace SsisUnit
{
    public class TestSuiteCompletedEventArgs : EventArgs
    {
        public TestSuiteCompletedEventArgs(DateTime testExecutionTime, int totalTests, int failedTests, int passedTests, string testResultMessage, bool isTestSuitePassed)
        {
            TotalTests = totalTests;
            FailedTests = failedTests;
            PassedTests = passedTests;
            TestExecResult = new TestResult(testExecutionTime, null, null, null, testResultMessage, isTestSuitePassed);
        }

        public TestSuiteCompletedEventArgs(int totalTests, int failedTests, int passedTests, TestResult testResult)
        {
            TotalTests = totalTests;
            FailedTests = failedTests;
            PassedTests = passedTests;
            TestExecResult = testResult;
        }

        public int FailedTests { get; private set; }
        public int PassedTests { get; private set; }
        public int TotalTests { get; private set; }
        public TestResult TestExecResult { get; private set; }
    }
}