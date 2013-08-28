using System;

namespace SsisUnitBase.EventArgs
{
    public class AssertCompletedEventArgs : System.EventArgs
    {
        public AssertCompletedEventArgs(DateTime testExecutionTime, string packageName, string taskName, string testName, string assertName, string testResultMsg, bool testPassed)
        {
            AssertName = assertName;
            TestExecResult = new TestResult(testExecutionTime, packageName, taskName, testName, testResultMsg, testPassed);
        }

        public AssertCompletedEventArgs(DateTime testExecutionTime, string packageName, string taskName, string testName, string assertName, string testResultMsg, bool testPassed, SsisUnitBaseObject assertCommand)
            : this(testExecutionTime, packageName, taskName, testName, assertName, testResultMsg, testPassed)
        {
            AssertCommand = assertCommand;
        }

        public AssertCompletedEventArgs(string assertName, TestResult testResult)
        {
            AssertName = assertName;
            TestExecResult = testResult;
        }

        public AssertCompletedEventArgs(string assertName, TestResult testResult, SsisUnitBaseObject assertCommand)
            : this(assertName, testResult)
        {
            AssertCommand = assertCommand;
        }

        public SsisUnitBaseObject AssertCommand { get; private set; }
        public string AssertName { get; private set; }
        public TestResult TestExecResult { get; private set; }
    }
}