using System;

namespace SsisUnitBase
{
    public class TestResult
    {
        public TestResult(DateTime testExecutionTime, string packageName, string taskName, string testName, string testResultMsg, bool testPassed)
        {
            TestName = testName;
            PackageName = packageName;
            TaskName = taskName;
            TestResultMsg = testResultMsg;
            TestExecutionTime = testExecutionTime;
            TestPassed = testPassed;
        }

        public DateTime TestExecutionTime { get; private set; }

        public string TestName { get; private set; }

        public string PackageName { get; private set; }

        public string TaskName { get; private set; }

        public string TestResultMsg { get; private set; }

        public bool TestPassed { get; private set; }
    }
}