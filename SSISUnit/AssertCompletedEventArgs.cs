using System;

namespace SsisUnit
{
    public class AssertCompletedEventArgs : EventArgs
    {
        public AssertCompletedEventArgs(DateTime testExecutionTime, string packageName, string taskName, string testName, string testResultMsg, bool testPassed)
        {
            TestExecResult = new TestResult(testExecutionTime, packageName, taskName, testName, testResultMsg, testPassed);
        }

        public AssertCompletedEventArgs(DateTime testExecutionTime, string packageName, string taskName, string testName, string testResultMsg, bool testPassed, CommandBase assertCommand)
            : this(testExecutionTime, packageName, taskName, testName, testResultMsg, testPassed)
        {
            AssertCommand = assertCommand;
        }

        public AssertCompletedEventArgs(TestResult testResult)
        {
            TestExecResult = testResult;
        }

        public AssertCompletedEventArgs(TestResult testResult, CommandBase assertCommand)
            : this(testResult)
        {
            AssertCommand = assertCommand;
        }

        public CommandBase AssertCommand { get; private set; }

        public TestResult TestExecResult { get; private set; }
    }
}