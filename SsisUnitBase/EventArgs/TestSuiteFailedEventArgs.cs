using System;

namespace SsisUnitBase.EventArgs
{
    public class TestSuiteFailedEventArgs : System.EventArgs
    {
        public TestSuiteFailedEventArgs(DateTime testExecutionTime, string testSuiteResultMessage)
        {
            TestExecResult = new TestResult(testExecutionTime, null, null, null, testSuiteResultMessage, false);
        }

        public TestSuiteFailedEventArgs(TestResult testResult)
        {
            if (testResult == null)
                throw new ArgumentNullException("testResult");

            TestExecResult = new TestResult(testResult.TestExecutionTime, testResult.PackageName, testResult.TaskName, testResult.TestName, testResult.TestResultMsg, false);
        }

        public TestResult TestExecResult { get; private set; }
    }
}