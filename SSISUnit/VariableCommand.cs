using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Microsoft.SqlServer.Dts.Runtime;

namespace SsisUnit
{
    class VariableCommand:CommandBase
    {
         public VariableCommand(XmlNode connections, XmlNamespaceManager namespaceMgr)
            : base(connections, namespaceMgr)
        { }

        public override object Execute(System.Xml.XmlNode command, Microsoft.SqlServer.Dts.Runtime.Package package, Microsoft.SqlServer.Dts.Runtime.DtsContainer container)
        {
            object returnValue;
            Variables vars = null;
            VariableDispenser dispenser = container.VariableDispenser;

            this.CheckCommandType(command.Name);

            string varName = command.Attributes["name"].Value;

            if (command.Attributes["value"] == null)
            {
                dispenser.LockOneForRead(varName, ref vars);
                returnValue = vars[varName].Value;
                vars.Unlock();
            }
            else
            {
                //writing to the variable
                object varValue = command.Attributes["value"].Value;
                dispenser.LockOneForWrite(varName, ref vars);
                vars[varName].Value = System.Convert.ChangeType(varValue, vars[varName].DataType);
                vars.Unlock();
                returnValue = varValue;
            }
            return returnValue;

        }
    }
}
