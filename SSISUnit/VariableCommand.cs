using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Microsoft.SqlServer.Dts.Runtime;

namespace SsisUnit
{
    class VariableCommand:CommandBase
    {
        private const string PROP_NAME = "name";
        private const string PROP_VALUE = "value";

        public VariableCommand(SsisTestSuite testSuite)
            : base(testSuite)
        {
            Properties.Add(PROP_NAME, new CommandProperty(PROP_NAME, string.Empty));
            Properties.Add(PROP_VALUE, new CommandProperty(PROP_VALUE, string.Empty));
        }

        public VariableCommand(SsisTestSuite testSuite, string commandXml)
            : base(testSuite, commandXml)
        {
        }

        public VariableCommand(SsisTestSuite testSuite, XmlNode commandXml)
            : base(testSuite, commandXml)
        {
        }

        public VariableCommand(SsisTestSuite testSuite, string name, string value)
            : base(testSuite)
        {
            Properties.Add(PROP_NAME, new CommandProperty(PROP_NAME, name));
            Properties.Add(PROP_VALUE, new CommandProperty(PROP_VALUE, value));
        }


        public override object Execute(System.Xml.XmlNode command, Microsoft.SqlServer.Dts.Runtime.Package package, Microsoft.SqlServer.Dts.Runtime.DtsContainer container)
        {
            object returnValue;
            Variables vars = null;
            VariableDispenser dispenser = container.VariableDispenser;

            this.LoadFromXml(command);

            string varName = Properties[PROP_NAME].Value;

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
