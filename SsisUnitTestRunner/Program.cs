using System;
using System.Collections.Generic;
using SsisUnit;
using System.IO;

using SsisUnitBase.Enums;
using SsisUnitBase.EventArgs;

namespace ssisUnitTestRunner
{
    public class Program
    {
        private const string ARG_TEST_CASE = "/TESTCASE";
        private const string ARG_OUTPUT_FILE = "/OUTPUT";
        private const string ARG_REPORT_LEVEL = "/REPORT";
        private const string ARG_PARAM = "/PARAM";

        static int Main(string[] args)
        {
            string testCaseFile = string.Empty;
            string packageFile = string.Empty;
            string outputFile = string.Empty;
            string report = string.Empty;
            var parameters = new Dictionary<string, string>();

            if (args.Length == 0 || args[0].Contains("?"))
            {
                Console.WriteLine("Usage: ssisUnitTestRunner /TESTCASE test-case-file [/OUTPUT output-file] [/REPORT <ALL:PASSED:FAILED>]");
                Console.WriteLine("/TESTCASE\tFilename for test case");
                Console.WriteLine("[/OUTPUT]\tFilename for results");
                Console.WriteLine("[/REPORT]\tALL - Reports all results (inlcuding setup and teardown)");
                Console.WriteLine("         \tPASSED - Reports only results for successful tests");
                Console.WriteLine("         \tFAILED - Reports only results for failed tests");
                Console.WriteLine("[/PARAM] \tName|Value for a parameter value specified in the test");
                return 0;
            }

            for (int i = 0; i < args.Length - 1; i++)
            {
                if (args[i].ToUpper() == ARG_TEST_CASE)
                {
                    testCaseFile = args[i + 1];
                }

                if (args[i].ToUpper() == ARG_OUTPUT_FILE)
                {
                    outputFile = args[i + 1];
                }

                if (args[i].ToUpper() == ARG_REPORT_LEVEL)
                {
                    report = args[i + 1];
                }

                if (args[i].ToUpper() == ARG_PARAM)
                {
                    var paramValue = args[i + 1].Split('|');

                    if (paramValue.Length == 2)
                    {
                        parameters.Add(paramValue[0], paramValue[1]);
                    }
                    else
                    {
                        Console.WriteLine("Parameter value {0} was ignored, as it wasn't in a valid format.", args[i + 1]);
                    }
                }
            }

            var run = new TestRun();

            try
            {
                return run.RunTest(testCaseFile, outputFile, report, parameters);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error occurred: " + ex.Message);
                return 2;
            }
        }

        private class TestRun
        {
            private const string REPORT_LEVEL_ALL = "ALL";
            private const string REPORT_LEVEL_PASSED = "PASSED";
            private const string REPORT_LEVEL_FAILED = "FAILED";

            private int _reportLevel = 0;
            private string outputFileName = string.Empty;


            public int RunTest(string testCase, string outputFileName, string reportLevel, IDictionary<string, string> parameters)
            {
                if (reportLevel.ToUpper() == REPORT_LEVEL_PASSED)
                {
                    _reportLevel = 1;
                }
                else if (reportLevel.ToUpper() == REPORT_LEVEL_FAILED)
                {
                    _reportLevel = 2;
                }
                else
                {
                    _reportLevel = 0;
                }

                var unitTest = new SsisTestSuite(testCase);
                unitTest.TestCompleted += unitRunner_TestCompleted;
                unitTest.AssertCompleted += unitTest_AssertCompleted;

                if (_reportLevel == 0)
                {
                    unitTest.SetupCompleted += unitTest_SetupCompleted;
                    unitTest.TeardownCompleted += unitTest_TeardownCompleted;
                }

                object[] vals = new object[] { "Execution Time", "Package Name", "Task Name", "Test Name", "Test Result", "Passed" };
                Console.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}\t{5}", vals);
                if (outputFileName != string.Empty)
                {
                    this.outputFileName = outputFileName;
                    var outputFile = new StreamWriter(outputFileName, true);
                    outputFile.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}\t{5}", vals);
                    outputFile.Close();
                }
                unitTest.Execute(parameters);
                if (unitTest.Statistics.GetStatistic(StatisticEnum.TestFailedCount) > 0)
                {
                    return 1;
                }
                return 0;

            }

            void unitTest_AssertCompleted(object sender, AssertCompletedEventArgs e)
            {
                object[] vals = new object[] { e.TestExecResult.TestExecutionTime.ToString(), e.TestExecResult.PackageName, e.TestExecResult.TaskName, e.TestExecResult.TestName, e.TestExecResult.TestResultMsg, e.TestExecResult.TestPassed };
                switch (_reportLevel)
                {
                    case 1:
                        if (e.TestExecResult.TestPassed)
                        {
                            WriteOutput(vals);
                        }
                        break;
                    case 2:
                        if (!e.TestExecResult.TestPassed)
                        {
                            WriteOutput(vals);
                        }
                        break;
                    default:
                        WriteOutput(vals);
                        break;
                }
            }

            void unitTest_TeardownCompleted(object sender, TeardownCompletedEventArgs e)
            {
                object[] vals = new object[] { e.TeardownExecutionTime.ToString(), e.Package, e.TaskGuid, e.TestName, e.Results, string.Empty };
                WriteOutput(vals);
            }

            void unitTest_SetupCompleted(object sender, SetupCompletedEventArgs e)
            {
                object[] vals = new object[] { e.SetupExecutionTime, e.Package, e.TaskGuid, e.TestName, e.Results, string.Empty };
                WriteOutput(vals);
            }

            private void unitRunner_TestCompleted(object sender, TestCompletedEventArgs e)
            {
                object[] vals = new object[] { e.TestExecResult.TestExecutionTime.ToString(), e.TestExecResult.PackageName, e.TestExecResult.TaskName, e.TestExecResult.TestName, e.TestExecResult.TestResultMsg, e.TestExecResult.TestPassed };
                switch (_reportLevel)
                {
                    case 1:
                        if (e.TestExecResult.TestPassed)
                        {
                            WriteOutput(vals);
                        }
                        break;
                    case 2:
                        if (!e.TestExecResult.TestPassed)
                        {
                            WriteOutput(vals);
                        }
                        break;
                    default:
                        WriteOutput(vals);
                        break;
                }
            }

            private void WriteOutput(object[] vals)
            {
                Console.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}\t{5}", vals);
                if (outputFileName != string.Empty)
                {
                    var outputFile = new StreamWriter(outputFileName, true);
                    outputFile.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}\t{5}", vals);
                    outputFile.Close();
                }
            }
        }
    }
}