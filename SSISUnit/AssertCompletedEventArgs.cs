using System;

namespace SsisUnit
{
    public class AssertCompletedEventArgs : EventArgs
    {
        public AssertCompletedEventArgs(DateTime testExecutionTime, string packageName, string taskName, string testName, string testResultMsg, bool testPassed)
        {
            TestExecResult = new TestResult(testExecutionTime, packageName, taskName, testName, testResultMsg, testPassed);
        }

        public AssertCompletedEventArgs(TestResult testResult)
        {
            TestExecResult = testResult;
        }

        public TestResult TestExecResult { get; private set; }
    }
}