using System;

namespace SsisUnit
{
    public class SetupCompletedEventArgs : EventArgs
    {
        public SetupCompletedEventArgs(DateTime setupExecutionTime, string testName, string package, string taskId, string results)
        {
            TestName = testName;
            SetupExecutionTime = setupExecutionTime;
            TaskGuid = taskId;
            Package = package;
            Results = results;
        }

        public DateTime SetupExecutionTime { get; private set; }

        public string Results { get; private set; }

        public string Package { get; private set; }

        public string TestName { get; private set; }

        public string TaskGuid { get; private set; }
    }
}