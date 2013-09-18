using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ssisUnitTestRunnerUI
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            string testCaseFile = string.Empty;

            if (args.Length > 0)
            {
                testCaseFile = args[0];
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new testSuiteUI(testCaseFile));
        }
    }
}
