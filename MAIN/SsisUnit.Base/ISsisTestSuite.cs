using System;

using SsisUnitBase.EventArgs;

namespace SsisUnitBase
{
    public interface ISsisTestSuite
    {
        event EventHandler<AssertCompletedEventArgs> AssertCompleted;
        event EventHandler<CommandCompletedEventArgs> CommandCompleted;
        event EventHandler<CommandFailedEventArgs> CommandFailed;
        event EventHandler<CommandStartedEventArgs> CommandStarted;
        event EventHandler<SetupCompletedEventArgs> SetupCompleted;
        event EventHandler<TeardownCompletedEventArgs> TeardownCompleted;
        event EventHandler<TestCompletedEventArgs> TestCompleted;
        event EventHandler<TestSuiteCompletedEventArgs> TestSuiteCompleted;
        event EventHandler<TestSuiteFailedEventArgs> TestSuiteFailed;

        int SsisVersion { get; }
        TestSuiteResults Statistics { get; }

        ISsisTestSuite CreateSsisTestSuite(string testSuiteFilepath);
        int Execute();
        void Save(string fileName);
        void Setup();
        void Teardown();
        void Test();
        bool Validate();
    }
}