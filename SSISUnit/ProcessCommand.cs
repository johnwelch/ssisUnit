using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Diagnostics;

namespace SsisUnit
{
    class ProcessCommand : CommandBase
    {
        public ProcessCommand(XmlNode connections, XmlNamespaceManager namespaceMgr)
            : base(connections, namespaceMgr)
        { }

        public override object Execute(System.Xml.XmlNode command, Microsoft.SqlServer.Dts.Runtime.Package package, Microsoft.SqlServer.Dts.Runtime.DtsContainer container)
        {
            int exitCode;

            if (command.Name != "ProcessCommand")
            {
                throw new ArgumentException("The node passed to the command argument is not a ProcessCommand element.");
            }
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
