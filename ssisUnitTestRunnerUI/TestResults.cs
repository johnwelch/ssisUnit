using System;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using SsisUnit;

namespace ssisUnitTestRunnerUI
{
    [Localizable(false)]
    public partial class TestResults : Form
    {
        private readonly SsisTestSuite _testSuite;

        private readonly EventHandler<AssertCompletedEventArgs> _assertCompleted;
        private readonly EventHandler<SetupCompletedEventArgs> _setupCompleted;
        private readonly EventHandler<TeardownCompletedEventArgs> _teardownCompleted;
        private readonly EventHandler<TestCompletedEventArgs> _testCompleted;
        private readonly EventHandler<CommandStartedEventArgs> _commandStarted;
        private readonly EventHandler<CommandCompletedEventArgs> _commandCompleted;
        private readonly EventHandler<CommandFailedEventArgs> _commandFailed;

        public TestResults(SsisTestSuite testSuite)
        {
            InitializeComponent();
            _testSuite = testSuite;

            _assertCompleted = TestSuiteAssertCompleted;
            _setupCompleted = TestSuiteSetupCompleted;
            _teardownCompleted = TestSuiteTeardownCompleted;
            _testCompleted = TestSuiteTestCompleted;
            _commandStarted = _testSuite_CommandStarted;
            _commandCompleted = TestSuiteCommandCompleted;
            _commandFailed = TestSuiteCommandFailed;

            _testSuite.AssertCompleted += _assertCompleted;
            _testSuite.SetupCompleted += _setupCompleted;
            _testSuite.TeardownCompleted += _teardownCompleted;
            _testSuite.TestCompleted += _testCompleted;
            _testSuite.CommandStarted += _commandStarted;
            _testSuite.CommandCompleted += _commandCompleted;
            _testSuite.CommandFailed += _commandFailed;
        }

        public void RunSuite()
        {
            try
            {
                Show();

                _testSuite.Execute();
                
                lblTestCount.Text = "Test Count: " + _testSuite.Statistics.GetStatistic(TestSuiteResults.StatisticEnum.TestCount).ToString("N0");
                lblTestPassed.Text = "Tests Passed: " + _testSuite.Statistics.GetStatistic(TestSuiteResults.StatisticEnum.TestPassedCount).ToString("N0");
                lblTestsFailed.Text = "Tests Failed: " + _testSuite.Statistics.GetStatistic(TestSuiteResults.StatisticEnum.TestFailedCount).ToString("N0");
                lblAssertCount.Text = "Assert Count: " + _testSuite.Statistics.GetStatistic(TestSuiteResults.StatisticEnum.AssertCount).ToString("N0");
                lblAssertPassed.Text = "Asserts Passed: " + _testSuite.Statistics.GetStatistic(TestSuiteResults.StatisticEnum.AssertPassedCount).ToString("N0");
                lblAssertsFailed.Text = "Asserts Failed: " + _testSuite.Statistics.GetStatistic(TestSuiteResults.StatisticEnum.AssertFailedCount).ToString("N0");
            }
            catch (Exception ex)
            {
                MessageBox.Show("The following error occurred when executing the test: " + ex.Message);
            }
        }

        public void RunTest(string testName)
        {
            Show();

            Test test = _testSuite.Tests[testName];

            test.Execute();
        }

        void TestSuiteTestCompleted(object sender, TestCompletedEventArgs e)
        {
            dataGridView1.Rows.Add("Test", e.TestExecResult.TestExecutionTime, e.TestExecResult.PackageName, e.TestExecResult.TaskName,
                e.TestExecResult.TestName, e.TestExecResult.TestResultMsg, e.TestExecResult.TestPassed);
        }

        void TestSuiteTeardownCompleted(object sender, TeardownCompletedEventArgs e)
        {
            dataGridView1.Rows.Add("Teardown", e.TeardownExecutionTime, e.Package, e.TaskGuid,
                 e.TestName, e.Results, true);
        }

        void TestSuiteSetupCompleted(object sender, SetupCompletedEventArgs e)
        {
            dataGridView1.Rows.Add("Setup", e.SetupExecutionTime, e.Package, e.TaskGuid,
                 e.TestName, e.Results, true);
        }

        void TestSuiteAssertCompleted(object sender, AssertCompletedEventArgs e)
        {
            DataCompareAssertCompletedEventArgs dataCompareEventArgs = e as DataCompareAssertCompletedEventArgs;

            if (dataCompareEventArgs != null)
            {
                var rowId = dataGridView1.Rows.Add(
                    "Assert",
                    dataCompareEventArgs.TestExecResult.TestExecutionTime,
                    dataCompareEventArgs.TestExecResult.PackageName,
                    dataCompareEventArgs.TestExecResult.TaskName,
                    dataCompareEventArgs.TestExecResult.TestName,
                    dataCompareEventArgs.TestExecResult.TestResultMsg,
                    dataCompareEventArgs.DataCompareCommandResults.IsDatasetsSame);

                dataGridView1.Rows[rowId].Cells[0].Style.BackColor = !dataCompareEventArgs.DataCompareCommandResults.IsDatasetsSame ? Color.Red : Color.White;
            }
            else
            {
                var rowId = dataGridView1.Rows.Add("Assert", e.TestExecResult.TestExecutionTime, e.TestExecResult.PackageName, e.TestExecResult.TaskName,
                    e.TestExecResult.TestName, e.TestExecResult.TestResultMsg, e.TestExecResult.TestPassed);

                dataGridView1.Rows[rowId].Cells[0].Style.BackColor = !e.TestExecResult.TestPassed ? Color.Red : Color.White;
            }
        }

        private void TestSuiteCommandFailed(object sender, CommandFailedEventArgs e)
        {
            dataGridView1.Rows.Add("Command", e.FailedExecutionTime, e.Package, e.CommandName,
                 null, e.Results, false);
        }

        private void TestSuiteCommandCompleted(object sender, CommandCompletedEventArgs e)
        {
            dataGridView1.Rows.Add(
                "Command",
                e.CompletedExecutionTime,
                e.Package,
                e.CommandName,
                null,
                e.Results,
                true);
        }

        private void _testSuite_CommandStarted(object sender, CommandStartedEventArgs e)
        {
            // dataGridView1.Rows.Add("Command", e.StartedExecutionTime, e.Package, e.CommandName,
            //      null, null, true);
        }

        private void DataGridView1CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
        }

        private void SaveToolStripMenuItemClick(object sender, EventArgs e)
        {
            StringBuilder sb = new StringBuilder();

            for (int rowIndex = 0; rowIndex < dataGridView1.Rows.Count; rowIndex++)
            {
                for (int colIndex = 0; colIndex < dataGridView1.Columns.Count; colIndex++)
                {
                    if (colIndex == (dataGridView1.Columns.Count - 1))
                        sb.Append(dataGridView1[colIndex, rowIndex].Value + Environment.NewLine);
                    else
                        sb.Append(dataGridView1[colIndex, rowIndex].Value + ",");
                }
            }

            string fileName = string.Empty;

            bool retry = true;

            while (retry)
            {
                try
                {
                    if (UIHelper.ShowSaveAs(ref fileName, UIHelper.FileFilter.CSV, false) == DialogResult.OK)
                        System.IO.File.WriteAllText(fileName, sb.ToString());

                    retry = false;
                }
                catch (Exception)
                {
                    MessageBox.Show("The selected file is in use.");
                    retry = true;
                }
            }
        }

        private void ExitToolStripMenuItemClick(object sender, EventArgs e)
        {
            Close();
        }

        private void RunTestSuiteToolStripMenuItemClick(object sender, EventArgs e)
        {
            if (!keepResultsToolStripItem.Checked)
            {
                dataGridView1.Rows.Clear();
            }

            RunSuite();
        }

        private void TestResultsFormClosing(object sender, FormClosingEventArgs e)
        {
            _testSuite.AssertCompleted -= _assertCompleted;
            _testSuite.SetupCompleted -= _setupCompleted;
            _testSuite.TeardownCompleted -= _teardownCompleted;
            _testSuite.TestCompleted -= _testCompleted;
            _testSuite.CommandStarted -= _commandStarted;
            _testSuite.CommandCompleted -= _commandCompleted;
            _testSuite.CommandFailed -= _commandFailed;
        }
    }
}