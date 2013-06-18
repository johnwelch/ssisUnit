using System;
using System.Xml;
using System.Diagnostics;
using System.ComponentModel;

namespace SsisUnit
{
    public class ProcessCommand : CommandBase
    {
        private const string PropProcess = "process";
        private const string PropArguments = "arguments";

        public ProcessCommand(SsisTestSuite testSuite)
            : base(testSuite)
        {
            Properties.Add(PropProcess, new CommandProperty(PropProcess, string.Empty));
            Properties.Add(PropArguments, new CommandProperty(PropArguments, string.Empty));
        }

        public ProcessCommand(SsisTestSuite testSuite, string commandXml)
            : base(testSuite, commandXml)
        {
            if (!Properties.ContainsKey(PropProcess))
            {
                Properties.Add(PropProcess, new CommandProperty(PropProcess, string.Empty));
            }

            if (!Properties.ContainsKey(PropArguments))
            {
                Properties.Add(PropArguments, new CommandProperty(PropArguments, string.Empty));
            }
        }

        public ProcessCommand(SsisTestSuite testSuite, XmlNode commandXml)
            : base(testSuite, commandXml)
        {
            if (!Properties.ContainsKey(PropProcess))
            {
                Properties.Add(PropProcess, new CommandProperty(PropProcess, string.Empty));
            }

            if (!Properties.ContainsKey(PropArguments))
            {
                Properties.Add(PropArguments, new CommandProperty(PropArguments, string.Empty));
            }
        }

        public ProcessCommand(SsisTestSuite testSuite, string process, string arguments)
            : this(testSuite)
        {
            Process = process;
            Arguments = arguments;
        }

        public override object Execute(Microsoft.SqlServer.Dts.Runtime.Package package, Microsoft.SqlServer.Dts.Runtime.DtsContainer container)
        {
            int exitCode;

            Process proc = null;

            try
            {
                string args = Properties[PropArguments].Value;
                string process = Properties[PropProcess].Value;

                OnCommandStarted(new CommandStartedEventArgs(DateTime.Now, Name, null, null));

                proc = args == string.Empty ? System.Diagnostics.Process.Start(process) : System.Diagnostics.Process.Start(process, args);

                if (proc == null)
                    exitCode = 0;
                else
                {
                    while (!proc.WaitForExit(app.Default.ProcessCheckForExitDelay))
                    {
                        if (proc.StartTime.AddSeconds(app.Default.ProcessTimeout).CompareTo(DateTime.Now) >= 0)
                            continue;

                        try
                        {
                            proc.CloseMainWindow();
                        }
                        catch (InvalidOperationException)
                        {
                            break;
                        }
                    }

                    exitCode = proc.ExitCode;
                }

                OnCommandCompleted(new CommandCompletedEventArgs(DateTime.Now, Name, null, null, string.Format("The {0} command has completed.", Name)));
            }
            catch (Exception ex)
            {
                OnCommandFailed(new CommandFailedEventArgs(DateTime.Now, Name, null, null, ex.Message));

                throw;
            }
            finally
            {
                if (proc != null)
                    proc.Close();
            }

            return exitCode;
        }

        [Description("The process to execute."),
         Editor("System.Windows.Forms.Design.FileNameEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
        public string Process
        {
            get { return Properties[PropProcess].Value; }
            set { Properties[PropProcess].Value = value; }
        }

        [Description("The arguments to pass to the process.")]
        public string Arguments
        {
            get { return Properties[PropArguments].Value; }
            set { Properties[PropArguments].Value = value; }
        }
    }
}