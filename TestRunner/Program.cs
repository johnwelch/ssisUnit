using System;
using System.Collections.Generic;
using System.Text;
using SsisUnit;
using System.IO;

namespace ssisUnitTestRunner
{
    class Program
    {
        private const string ARG_TEST_CASE = "/TESTCASE";
        //private const string ARG_PACKAGE = "/PACKAGE";
        private const string ARG_OUTPUT_FILE = "/OUTPUT";
        private const string ARG_REPORT_LEVEL = "/REPORT";


        static int Main(string[] args)
        {
            string testCaseFile = string.Empty;
            string packageFile = string.Empty;
            string outputFile = string.Empty;
            string report = string.Empty;

            if (args.Length == 0 || args[0].Contains("?"))
            {
                //Removed package override capability - not sure it's needed
                //Console.WriteLine("Usage: ssisUnitTestRunner /TESTCASE test-case-file [/PACKAGE package-file] [/OUTPUT output-file] [/REPORT <ALL:PASSED:FAILED>]");
                Console.WriteLine("Usage: ssisUnitTestRunner /TESTCASE test-case-file [/OUTPUT output-file] [/REPORT <ALL:PASSED:FAILED>]");
                Console.WriteLine("/TESTCASE\tFilename for test case");
                //Console.WriteLine("[/PACKAGE]\tFilename for package - overrides value in test case");
                Console.WriteLine("[/OUTPUT]\tFilename for results");
                Console.WriteLine("[/REPORT]\tALL - Reports all results (inlcuding setup and teardown)");
                Console.WriteLine("         \tPASSED - Reports only results for successful tests");
                Console.WriteLine("         \tFAILED - Reports only results for failed tests");
                return 0;
            }
            else
            {
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
                    //if (args[i].ToUpper() == ARG_PACKAGE)
                    //{
                    //    packageFile = args[i + 1];
                    //}
                    if (args[i].ToUpper() == ARG_REPORT_LEVEL)
                    {
                        report = args[i + 1];
                    }
                }
                TestRun run = new TestRun();
                //if (run.RunTest(testCaseFile, packageFile, outputFile, report))
                try
                {
                    run.RunTest(testCaseFile, outputFile, report);
                    return 0;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error occurred: " + ex.Message);
                    return 1;
                }
            }
        }

        private class TestRun
        {
            private const string REPORT_LEVEL_ALL = "ALL";
            private const string REPORT_LEVEL_PASSED = "PASSED";
            private const string REPORT_LEVEL_FAILED = "FAILED";

            private int reportLevel = 0;
            private string outputFileName = string.Empty;


            //public bool RunTest(string testCase, string packageFile, string outputFileName, string reportLevel)
            public void RunTest(string testCase, string outputFileName, string reportLevel)
            {
                if (reportLevel.ToUpper() == REPORT_LEVEL_PASSED)
                {
                    this.reportLevel = 1;
                }
                else if (reportLevel.ToUpper() == REPORT_LEVEL_FAILED)
                {
                    this.reportLevel = 2;
                }
                else
                {
                    this.reportLevel = 0;
                }

                SsisTestSuite unitTest = new SsisTestSuite(testCase);
                unitTest.TestCompleted += new EventHandler<TestCompletedEventArgs>(unitRunner_TestCompleted);

                if (this.reportLevel==0)
                {
                    //Only show setup and teardown messages if all reporting is enabled.
                    unitTest.SetupCompleted += new EventHandler<SetupCompletedEventArgs>(unitTest_SetupCompleted);
                    unitTest.TeardownCompleted += new EventHandler<TeardownCompletedEventArgs>(unitTest_TeardownCompleted);
                }




                object[] vals = new object[] { "Execution Time", "Package Name", "Task Name", "Test Name", "Test Result", "Passed" };
                Console.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}\t{5}", vals);
                if (outputFileName != string.Empty)
                {
                    this.outputFileName = outputFileName;
                    StreamWriter outputFile = new StreamWriter(outputFileName, true);
                    outputFile.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}\t{5}", vals);
                    outputFile.Close();
                }
                unitTest.Execute();
                return;

            }

            void unitTest_TeardownCompleted(object sender, TeardownCompletedEventArgs e)
            {
                object[] vals = new object[] { e.TeardownExecutionTime.ToString(), e.Package, e.TaskGuid, e.TestName, e.Results, string.Empty};
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
                switch (reportLevel)
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
                    StreamWriter outputFile = new StreamWriter(outputFileName, true);
                    outputFile.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}\t{5}", vals);
                    outputFile.Close();
                }
            }

        }



    }
}
