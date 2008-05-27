using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Diagnostics;

namespace SsisUnit
{
    class ProcessCommand : CommandBase
    {
        public ProcessCommand(SsisTestSuite testSuite)
            : base(testSuite)
        { }

        public override object Execute(System.Xml.XmlNode command, Microsoft.SqlServer.Dts.Runtime.Package package, Microsoft.SqlServer.Dts.Runtime.DtsContainer container)
        {
            int exitCode;

            this.CheckCommandType(command.Name);

            Process proc = null;
            try
            {
                XmlNode args = command.Attributes.GetNamedItem("arguments");
                if (args == null)
                {
                    proc = Process.Start(command.Attributes["process"].Value);
                }
                else
                {
                    proc = Process.Start(command.Attributes["process"].Value, command.Attributes["arguments"].Value);
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
