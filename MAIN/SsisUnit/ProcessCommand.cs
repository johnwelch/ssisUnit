using System;
using System.Xml;
using System.Diagnostics;
using System.ComponentModel;

using Microsoft.SqlServer.Dts.Runtime;

using SsisUnitBase.EventArgs;

#if SQL2012 || SQL2008
using IDTSComponentMetaData = Microsoft.SqlServer.Dts.Pipeline.Wrapper.IDTSComponentMetaData100;
#elif SQL2005
using IDTSComponentMetaData = Microsoft.SqlServer.Dts.Pipeline.Wrapper.IDTSComponentMetaData90;
#endif

namespace SsisUnit
{
    public class ProcessCommand : CommandBase
    {
        private const string PropProcess = "process";
        private const string PropArguments = "arguments";

        public ProcessCommand(SsisTestSuite testSuite)
            : base(testSuite)
        {
            InitializeProperties();
        }

        public ProcessCommand(SsisTestSuite testSuite, object parent)
            : base(testSuite, parent)
        {
            InitializeProperties();
        }

        public ProcessCommand(SsisTestSuite testSuite, string commandXml)
            : base(testSuite, commandXml)
        {
            InitializeProperties();
        }

        public ProcessCommand(SsisTestSuite testSuite, object parent, string commandXml)
            : base(testSuite, parent, commandXml)
        {
            InitializeProperties();
        }

        public ProcessCommand(SsisTestSuite testSuite, XmlNode commandXml)
            : base(testSuite, commandXml)
        {
            InitializeProperties();
        }

        public ProcessCommand(SsisTestSuite testSuite, object parent, XmlNode commandXml)
            : base(testSuite, parent, commandXml)
        {
            InitializeProperties();
        }

        public ProcessCommand(SsisTestSuite testSuite, string process, string arguments)
            : this(testSuite)
        {
            InitializeProperties();

            Process = process;
            Arguments = arguments;
        }

        public ProcessCommand(SsisTestSuite testSuite, object parent, string process, string arguments)
            : this(testSuite, parent)
        {
            InitializeProperties();

            Process = process;
            Arguments = arguments;
        }

        public override object Execute(object project, Package package, DtsContainer container)
        {
            return Execute(package, container);
        }

        public override object Execute(Package package, DtsContainer container)
        {
            int exitCode;

            Process proc = null;

            try
            {
                string args = Properties[PropArguments].Value;
                string process = Properties[PropProcess].Value;

                OnCommandStarted(new CommandStartedEventArgs(DateTime.Now, CommandName, null, null));

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

                OnCommandCompleted(new CommandCompletedEventArgs(DateTime.Now, CommandName, null, null, string.Format("The {0} command has completed.", CommandName)));
            }
            catch (Exception ex)
            {
                OnCommandFailed(new CommandFailedEventArgs(DateTime.Now, CommandName, null, null, ex.Message));

                throw new ArgumentException("The RunProcessNode contained an invalid command or process.", ex);
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

        private void InitializeProperties()
        {
            if (!Properties.ContainsKey(PropProcess))
                Properties.Add(PropProcess, new CommandProperty(PropProcess, string.Empty));

            if (!Properties.ContainsKey(PropArguments))
                Properties.Add(PropArguments, new CommandProperty(PropArguments, string.Empty));
        }
    }
}