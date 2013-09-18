using System;

namespace SsisUnit
{
    public class TeardownCompletedEventArgs : EventArgs
    {
        public TeardownCompletedEventArgs(DateTime teardownExecutionTime, string testName, string package, string taskId, string results)
        {
            TestName = testName;
            TeardownExecutionTime = teardownExecutionTime;
            TaskGuid = taskId;
            Package = package;
            Results = results;
        }

        public DateTime TeardownExecutionTime { get; private set; }

        public string Results { get; private set; }

        public string Package { get; private set; }

        public string TestName { get; private set; }

        public string TaskGuid { get; private set; }
    }
}