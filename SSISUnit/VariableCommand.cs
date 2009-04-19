using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Microsoft.SqlServer.Dts.Runtime;
using System.ComponentModel;

namespace SsisUnit
{
    public class VariableCommand:CommandBase
    {
        private const string PROP_NAME = "name";
        private const string PROP_VALUE = "value";
        private const string PROP_OPERATION = "operation";

        public VariableCommand(SsisTestSuite testSuite)
            : base(testSuite)
        {
            Properties.Add(PROP_OPERATION, new CommandProperty(PROP_OPERATION, VariableOperation.Get.ToString()));
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

        public VariableCommand(SsisTestSuite testSuite, VariableOperation operation, string name, string value)
            : base(testSuite)
        {
            Properties.Add(PROP_OPERATION, new CommandProperty(PROP_OPERATION, operation.ToString()));
            Properties.Add(PROP_NAME, new CommandProperty(PROP_NAME, name));
            Properties.Add(PROP_VALUE, new CommandProperty(PROP_VALUE, value));
        }

        public override object Execute(Package package, DtsContainer container)
        {
            object returnValue;
            Variables vars = null;
            VariableDispenser dispenser = container.VariableDispenser;

            string varName = this.VariableName;

            if (this.Operation== VariableOperation.Get)
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

        //TODO: Add a TypeConverter that lists the variables in the package?
        [Description("The name of the variable to operate on.")]
        public string VariableName
        {
            get { return Properties[PROP_NAME].Value; }
            set { Properties[PROP_NAME].Value = value; }
        }

        [Description("The value to set the variable to.")]
        public string Value
        {
            get { return Properties[PROP_VALUE].Value; }
            set { Properties[PROP_VALUE].Value = value; }
        }

        [Description("Determines whether to get or set the variable."),
        TypeConverter(typeof(System.ComponentModel.EnumConverter))]
        public VariableOperation Operation
        {
            get { return (VariableOperation)Enum.Parse(typeof(VariableOperation), Properties[PROP_OPERATION].Value, true);}
            set { Properties[PROP_OPERATION].Value = value.ToString(); }
        }

        //private VariableOperation GetVariableOperationFromString(string operation)
        //{
        //    if (operation == "Get") return VariableOperation.Get;
        //    else if (operation == "Set") return VariableOperation.Set;
        //    else
        //    {
        //        throw new ArgumentException("The operation provided was not valid.");
        //    }
        //}

        public enum VariableOperation
        {
            Get = 0,
            Set = 1
        }
    }
}
