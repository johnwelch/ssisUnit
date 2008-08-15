using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Diagnostics;

namespace SsisUnit
{
    public class ProcessCommand : CommandBase
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
            if (!Properties.ContainsKey(PROP_ARGUMENTS))
            {
                Properties.Add(PROP_ARGUMENTS, new CommandProperty(PROP_ARGUMENTS, string.Empty));
            }
        }

        public ProcessCommand(SsisTestSuite testSuite, XmlNode commandXml)
            : base(testSuite, commandXml)
        {
            if (!Properties.ContainsKey(PROP_ARGUMENTS))
            {
                Properties.Add(PROP_ARGUMENTS, new CommandProperty(PROP_ARGUMENTS, string.Empty));
            }
        }

        public ProcessCommand(SsisTestSuite testSuite, string process, string arguments)
            : base(testSuite)
        {
            if (!Properties.ContainsKey(PROP_ARGUMENTS))
            {
                Properties.Add(PROP_ARGUMENTS, new CommandProperty(PROP_ARGUMENTS, string.Empty));
            }
        }

        public override object Execute(Microsoft.SqlServer.Dts.Runtime.Package package, Microsoft.SqlServer.Dts.Runtime.DtsContainer container)
        {
            int exitCode;

            Process proc = null;
            try
            {
                string args = Properties[PROP_ARGUMENTS].Value;
                string process = Properties[PROP_PROCESS].Value;
                if (args == string.Empty)
                {
                    proc = System.Diagnostics.Process.Start(process);
                }
                else
                {
                    proc = System.Diagnostics.Process.Start(process, args);
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

        public string Process
        {
            get { return this.Properties[PROP_PROCESS].Value; }
            set { this.Properties[PROP_PROCESS].Value = value; }
        }

        public string Arguments
        {
            get { return this.Properties[PROP_ARGUMENTS].Value; }
            set { this.Properties[PROP_ARGUMENTS].Value = value; }
        }

    }
}
