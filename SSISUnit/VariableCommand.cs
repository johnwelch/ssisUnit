using System;
using System.Xml;
using Microsoft.SqlServer.Dts.Runtime;
using System.ComponentModel;

namespace SsisUnit
{
    public class VariableCommand : CommandBase
    {
        private const string PropName = "name";
        private const string PropValue = "value";
        private const string PropOperation = "operation";

        public VariableCommand(SsisTestSuite testSuite)
            : base(testSuite)
        {
            Properties.Add(PropOperation, new CommandProperty(PropOperation, VariableOperation.Get.ToString()));
            Properties.Add(PropName, new CommandProperty(PropName, string.Empty));
            Properties.Add(PropValue, new CommandProperty(PropValue, string.Empty));
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
            Properties.Add(PropOperation, new CommandProperty(PropOperation, operation.ToString()));
            Properties.Add(PropName, new CommandProperty(PropName, name));
            Properties.Add(PropValue, new CommandProperty(PropValue, value));
        }

        public override object Execute(Package package, DtsContainer container)
        {
            object returnValue;

            try
            {
                OnCommandStarted(new CommandStartedEventArgs(DateTime.Now, Name, null, null));

                Variables vars = null;
                VariableDispenser dispenser = container.VariableDispenser;

                string varName = VariableName;

                if (Operation == VariableOperation.Get)
                {
                    dispenser.LockOneForRead(varName, ref vars);
                    returnValue = vars[varName].Value;
                    vars.Unlock();
                }
                else
                {
                    // writing to the variable
                    object varValue = Value;
                    dispenser.LockOneForWrite(varName, ref vars);
                    vars[varName].Value = Convert.ChangeType(varValue, vars[varName].DataType);
                    vars.Unlock();
                    returnValue = varValue;
                }

                OnCommandFailed(new CommandFailedEventArgs(DateTime.Now, Name, null, null, string.Format("The {0} command has completed.", Name)));
            }
            catch (Exception ex)
            {
                OnCommandFailed(new CommandFailedEventArgs(DateTime.Now, Name, null, null, ex.Message));
                
                throw;
            }

            return returnValue;
        }

        // TODO: Add a TypeConverter that lists the variables in the package?
        [Description("The name of the variable to operate on.")]
        public string VariableName
        {
            get { return Properties[PropName].Value; }
            set { Properties[PropName].Value = value; }
        }

        [Description("The value to set the variable to.")]
        public string Value
        {
            get { return Properties[PropValue].Value; }
            set { Properties[PropValue].Value = value; }
        }

        [Description("Determines whether to get or set the variable."),
        TypeConverter(typeof(EnumConverter))]
        public VariableOperation Operation
        {
            get { return (VariableOperation)Enum.Parse(typeof(VariableOperation), Properties[PropOperation].Value, true); }
            set { Properties[PropOperation].Value = value.ToString(); }
        }

        public enum VariableOperation
        {
            Get = 0,
            Set = 1
        }
    }
}
