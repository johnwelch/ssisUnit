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
        private const string PROP_OPERATION = "operation";

        public VariableCommand(SsisTestSuite testSuite)
            : base(testSuite)
        {
            //TODO: Got a problem here with Values - can be a number of types - or not provided
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

        public override object Execute(Package package, DtsContainer container)
        {
            object returnValue;
            Variables vars = null;
            VariableDispenser dispenser = container.VariableDispenser;

            string varName = this.Name;

            if (this.Operation=="Get")
            {
                dispenser.LockOneForRead(varName, ref vars);
                returnValue = vars[varName].Value;
                vars.Unlock();
            }
            else
            {
                //writing to the variable
                object varValue = this.Value;
                dispenser.LockOneForWrite(varName, ref vars);
                vars[varName].Value = System.Convert.ChangeType(varValue, vars[varName].DataType);
                vars.Unlock();
                returnValue = varValue;
            }
            return returnValue;
        }

        public string Name
        {
            get { return Properties[PROP_NAME].Value; }
            set { Properties[PROP_NAME].Value = value; }
        }

        public string Value
        {
            get { return Properties[PROP_VALUE].Value; }
            set { Properties[PROP_VALUE].Value = value; }
        }

        public string Operation
        {
            get { return Properties[PROP_OPERATION].Value; }
            set { Properties[PROP_OPERATION].Value = value; }
        }
    }
}
