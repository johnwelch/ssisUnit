using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Diagnostics;

namespace SsisUnit
{
    class ProcessCommand : CommandBase
    {
        private const string PROP_PROCESS = "process";
        private const string PROP_ARGUMENTS = "arguments";

        public ProcessCommand(SsisTestSuite testSuite)
            : base(testSuite)
        {
            Properties.Add(PROP_PROCESS, new CommandProperty(PROP_PROCESS, string.Empty));
            Properties.Add(PROP_ARGUMENTS, new CommandProperty(PROP_ARGUMENTS, string.Empty));
        }

        public ProcessCommand(SsisTestSuite testSuite, string commandXml)
            : base(testSuite, commandXml)
        {
        }

        public ProcessCommand(SsisTestSuite testSuite, XmlNode commandXml)
            : base(testSuite, commandXml)
        {
        }

        public ProcessCommand(SsisTestSuite testSuite, string process, string arguments)
            : base(testSuite)
        {
            Properties.Add(PROP_PROCESS, new CommandProperty(PROP_PROCESS, process));
            Properties.Add(PROP_ARGUMENTS, new CommandProperty(PROP_ARGUMENTS, arguments));
        }

        public override object Execute(System.Xml.XmlNode command, Microsoft.SqlServer.Dts.Runtime.Package package, Microsoft.SqlServer.Dts.Runtime.DtsContainer container)
        {
            int exitCode;

            this.LoadFromXml(command);

            Process proc = null;
            try
            {
                string args = Properties[PROP_ARGUMENTS].Value;
                string process = Properties[PROP_PROCESS].Value;
                if (args == string.Empty)
                {
                    proc = Process.Start(process);
                }
                else
                {
                    proc = Process.Start(process, args);
                }
                while (!proc.WaitForExit(app.Default.ProcessCheckForExitDelay))
                {
                    if (proc.StartTime.AddSeconds(app.Default.ProcessTimeout).CompareTo(DateTime.Now) < 0)
                    {
                        try
                        {
                            proc.CloseMainWindow();
                        }
                        catch (InvalidOperationException)
                        {
                            break;
                        }
                    }
                }

                exitCode = proc.ExitCode;
            }
            catch (Exception ex)
            {
                throw new ArgumentException("The RunProcessNode contained an invalid command or process.", ex);
            }
            finally
            {
                proc.Close();
            }

            return exitCode;
        }
    }
}
