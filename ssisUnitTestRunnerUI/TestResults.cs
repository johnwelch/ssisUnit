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
        EventHandler<AssertCompletedEventArgs> ehAssertCompleted;
        EventHandler<SetupCompletedEventArgs> ehSetupCompleted;
        EventHandler<TeardownCompletedEventArgs> ehTeardownCompleted;
        EventHandler<TestCompletedEventArgs> ehTestCompleted;

        public TestResults(SsisTestSuite testSuite)
        {
            InitializeComponent();
            _testSuite = testSuite;

            ehAssertCompleted = new EventHandler<AssertCompletedEventArgs>(_testSuite_AssertCompleted);
            ehSetupCompleted= new EventHandler<SetupCompletedEventArgs>(_testSuite_SetupCompleted);
            ehTeardownCompleted= new EventHandler<TeardownCompletedEventArgs>(_testSuite_TeardownCompleted);
            ehTestCompleted = new EventHandler<TestCompletedEventArgs>(_testSuite_TestCompleted);

            _testSuite.AssertCompleted += ehAssertCompleted;
            _testSuite.SetupCompleted += ehSetupCompleted;
            _testSuite.TeardownCompleted += ehTeardownCompleted;
            _testSuite.TestCompleted += ehTestCompleted;
        }

        public void RunSuite()
        {
            try
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
            catch (Exception ex)
            {
                MessageBox.Show("The following error occurred when executing the test: " + ex.Message);
            }

        }

        public void RunTest(string testName)
        {
            this.Show();
            _testSuite.Tests[testName].Execute();
        }

        void _testSuite_TestCompleted(object sender, TestCompletedEventArgs e)
        {
            dataGridView1.Rows.Add("Test", e.TestExecResult.TestExecutionTime, e.TestExecResult.PackageName, e.TestExecResult.TaskName,
                e.TestExecResult.TestName, e.TestExecResult.TestResultMsg, e.TestExecResult.TestPassed);
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

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StringBuilder sb = new StringBuilder();

            for (int rowIndex = 0; rowIndex < dataGridView1.Rows.Count; rowIndex++)
            {
                for (int colIndex = 0; colIndex < dataGridView1.Columns.Count; colIndex++)
                {
                    if (colIndex == (dataGridView1.Columns.Count - 1))
                    {
                        sb.Append(dataGridView1[colIndex, rowIndex].Value.ToString() + Environment.NewLine);
                    }
                    else
                    {
                        sb.Append(dataGridView1[colIndex, rowIndex].Value.ToString() + ",");
                    }
                }
            }

            string fileName = string.Empty;

            bool retry = true;
            while (retry)
            {
                try
                {
                    if (UIHelper.ShowSaveAs(ref fileName, UIHelper.FileFilter.CSV, false) == DialogResult.OK)
                    {
                        System.IO.File.WriteAllText(fileName, sb.ToString());
                    }
                    retry = false;
                }
                catch (Exception)
                {
                    MessageBox.Show("The selected file is in use.");
                    retry = true;
                }
            }
            
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void runTestSuiteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!keepResultsToolStripItem.Checked)
            {
                dataGridView1.Rows.Clear();
            }
            this.RunSuite();
        }

        private void TestResults_FormClosing(object sender, FormClosingEventArgs e)
        {
            _testSuite.AssertCompleted -= ehAssertCompleted;
            _testSuite.SetupCompleted -= ehSetupCompleted;
            _testSuite.TeardownCompleted -= ehTeardownCompleted;
            _testSuite.TestCompleted -= ehTestCompleted;

        }
    }
}
