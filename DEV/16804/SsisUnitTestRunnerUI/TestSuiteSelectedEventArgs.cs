using System;

namespace ssisUnitTestRunnerUI
{
    public class TestSuiteSelectedEventArgs : EventArgs
    {
        public TestSuiteSelectedEventArgs(string oldFile, string newFile)
        {
            CurrentFile = newFile;
            PreviousFile = oldFile;
        }

        public string PreviousFile { get; private set; }

        public string CurrentFile { get; private set; }
    }
}