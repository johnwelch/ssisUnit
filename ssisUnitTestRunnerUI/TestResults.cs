using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using SsisUnit;

namespace ssisUnitTestRunnerUI
{
    public partial class TestResults : Form
    {
        SsisTestSuite _testSuite;

        public TestResults(SsisTestSuite testSuite)
        {
            InitializeComponent();
            _testSuite = testSuite;
            _testSuite.AssertCompleted += new EventHandler<AssertCompletedEventArgs>(_testSuite_AssertCompleted);
            _testSuite.SetupCompleted += new EventHandler<SetupCompletedEventArgs>(_testSuite_SetupCompleted);
            _testSuite.TeardownCompleted += new EventHandler<TeardownCompletedEventArgs>(_testSuite_TeardownCompleted);
            _testSuite.TestCompleted += new EventHandler<TestCompletedEventArgs>(_testSuite_TestCompleted);
        }

        public void RunSuite()
        {
            this.Show();
            _testSuite.Execute();
            lblTestCount.Text = "Test Count: " + _testSuite.Statistics.GetStatistic(TestSuiteResults.StatisticEnum.TestCount).ToString();
            lblTestPassed.Text = "Tests Passed: " + _testSuite.Statistics.GetStatistic(TestSuiteResults.StatisticEnum.TestPassedCount).ToString();
            lblTestsFailed.Text = "Tests Failed: " + _testSuite.Statistics.GetStatistic(TestSuiteResults.StatisticEnum.TestFailedCount).ToString();
            lblAssertCount.Text = "Assert Count: " + _testSuite.Statistics.GetStatistic(TestSuiteResults.StatisticEnum.AssertCount).ToString();
            lblAssertPassed.Text = "Asserts Passed: " + _testSuite.Statistics.GetStatistic(TestSuiteResults.StatisticEnum.AssertPassedCount).ToString();
            lblAssertsFailed.Text = "Asserts Failed: " + _testSuite.Statistics.GetStatistic(TestSuiteResults.StatisticEnum.AssertFailedCount).ToString();
        }

        public void RunTest(string testName)
        {
            this.Show();
            _testSuite.Tests[testName].Execute();
        }

        void _testSuite_TestCompleted(object sender, TestCompletedEventArgs e)
        {
            dataGridView1.Rows.Add("Test", e.TestExecResult.TestExecutionTime, e.TestExecResult.PackageName, e.TestExecResult.TaskName,
                e.TestExecResult.TestName, string.Empty, e.TestExecResult.TestResultMsg, e.TestExecResult.TestPassed);
        }

        void _testSuite_TeardownCompleted(object sender, TeardownCompletedEventArgs e)
        {
            dataGridView1.Rows.Add("Teardown", e.TeardownExecutionTime, e.Package, e.TaskGuid, 
                 e.TestName, e.Results, true);
        }

        void _testSuite_SetupCompleted(object sender, SetupCompletedEventArgs e)
        {
            dataGridView1.Rows.Add("Setup", e.SetupExecutionTime, e.Package, e.TaskGuid,
                 e.TestName, e.Results, true);
        }

        void _testSuite_AssertCompleted(object sender, AssertCompletedEventArgs e)
        {
            dataGridView1.Rows.Add("Assert", e.TestExecResult.TestExecutionTime, e.TestExecResult.PackageName, e.TestExecResult.TaskName,
                e.TestExecResult.TestName, e.TestExecResult.TestResultMsg, e.TestExecResult.TestPassed);
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
