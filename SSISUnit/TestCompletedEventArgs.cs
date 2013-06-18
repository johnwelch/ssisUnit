using System;

namespace SsisUnit
{
    public class TestCompletedEventArgs : EventArgs
    {
        public TestCompletedEventArgs(DateTime testExecutionTime, string packageName, string taskName, string testName, string testResultMsg, bool testPassed)
        {
            TestExecResult = new TestResult(testExecutionTime, packageName, taskName, testName, testResultMsg, testPassed);
        }

        public TestCompletedEventArgs(TestResult testResult)
        {
            TestExecResult = testResult;
        }

        public TestResult TestExecResult { get; private set; }
    }
}