using System;
using System.Xml;
using Microsoft.SqlServer.Dts.Runtime;
using System.ComponentModel;

using SsisUnit.Enums;

using SsisUnitBase.Enums;
using SsisUnitBase.EventArgs;

using Package = Microsoft.SqlServer.Dts.Runtime.Package;

namespace SsisUnit
{
    public class ParameterCommand : CommandBase
    {
        private const string PropName = "name";
        private const string PropOperation = "operation";
        private const string PropParameterName = "parameterName";
        private const string PropParameterType = "parameterType";
        private const string PropValue = "value";

        public ParameterCommand(SsisTestSuite testSuite)
            : base(testSuite)
        {
            InitializeProperties();
        }

        public ParameterCommand(SsisTestSuite testSuite, object parent)
            : base(testSuite, parent)
        {
            InitializeProperties();
        }

        public ParameterCommand(SsisTestSuite testSuite, string commandXml)
            : base(testSuite, commandXml)
        {
            InitializeProperties();
        }

        public ParameterCommand(SsisTestSuite testSuite, object parent, string commandXml)
            : base(testSuite, parent, commandXml)
        {
            InitializeProperties();
        }

        public ParameterCommand(SsisTestSuite testSuite, XmlNode commandXml)
            : base(testSuite, commandXml)
        {
            InitializeProperties();
        }

        public ParameterCommand(SsisTestSuite testSuite, object parent, XmlNode commandXml)
            : base(testSuite, parent, commandXml)
        {
            InitializeProperties();
        }

        public ParameterCommand(SsisTestSuite testSuite, string name, string parameterName, ParameterType parameterType, VariableCommand.VariableOperation operation, string value)
            : this(testSuite)
        {
            Properties[PropOperation] = new CommandProperty(PropOperation, operation.ToString());
            Properties[PropName] = new CommandProperty(PropName, name);
            Properties[PropParameterName] = new CommandProperty(PropParameterName, parameterName);
            Properties[PropParameterType] = new CommandProperty(PropParameterType, parameterType.ToString());
            Properties[PropValue] = new CommandProperty(PropValue, value);
        }

        public ParameterCommand(SsisTestSuite testSuite, object parent, string name, string parameterName, ParameterType parameterType, VariableCommand.VariableOperation operation, string value)
            : this(testSuite, parent)
        {
            Properties[PropOperation] = new CommandProperty(PropOperation, operation.ToString());
            Properties[PropName] = new CommandProperty(PropName, name);
            Properties[PropParameterName] = new CommandProperty(PropParameterName, parameterName);
            Properties[PropParameterType] = new CommandProperty(PropParameterType, parameterType.ToString());
            Properties[PropValue] = new CommandProperty(PropValue, value);
        }

        public override object Execute(Package package, DtsContainer container)
        {
            return Execute((object)null, package, container);
        }

        public override object Execute(object project, Package package, DtsContainer container)
        {
#if SQL2012 || SQL2014 || SQL2017
            object returnValue;

            CommandParentType commandParentType = GetCommandParentType();

            try
            {
                OnCommandStarted(new CommandStartedEventArgs(DateTime.Now, CommandName, null, null, commandParentType));

                Parameter parameter;

                if (ParameterType == ParameterType.Project)
                {
                    Project currentProject = project as Project;

                    if (currentProject == null)
                        throw new Exception("The package's project was not loaded.");

                    parameter = currentProject.Parameters[ParameterName];

                    if (parameter == null)
                        throw new Exception(string.Format("The project parameter {0} could not be found.", ParameterName ?? "<NULL>"));
                }
                else
                {
                    if (package == null)
                        throw new ArgumentNullException("package", "The package was not passed correctly or loaded.");

                    parameter = package.Parameters[ParameterName];

                    if (parameter == null)
                        throw new Exception(string.Format("The package parameter {0} could not be found.", ParameterName ?? "<NULL>"));
                }

                if (Operation == VariableCommand.VariableOperation.Get)
                    returnValue = parameter.Value;
                else
                    returnValue = parameter.Value = Convert.ChangeType(Value, parameter.DataType);

                OnCommandCompleted(new CommandCompletedEventArgs(DateTime.Now, CommandName, null, null, string.Format("The {0} command has completed.", CommandName), commandParentType));
            }
            catch (Exception ex)
            {
                OnCommandFailed(new CommandFailedEventArgs(DateTime.Now, CommandName, null, null, ex.Message, commandParentType));

                throw;
            }

            return returnValue;
#else
            return null;
#endif
        }

        [Description("Determines whether to get or set the variable."),
        TypeConverter(typeof(EnumConverter))]
        public VariableCommand.VariableOperation Operation
        {
            get { return (VariableCommand.VariableOperation)Enum.Parse(typeof(VariableCommand.VariableOperation), Properties[PropOperation].Value, true); }
            set { Properties[PropOperation].Value = value.ToString(); }
        }

        [Description("The name of the parameter to operate on.  Do NOT include the namespace, ONLY specify the name of the parameter.")]
        public string ParameterName
        {
            get { return Properties[PropParameterName].Value; }
            set { Properties[PropParameterName].Value = value; }
        }

        [Description("Determines whether this command interacts with a project or package parameter."),
        TypeConverter(typeof(EnumConverter))]
        public ParameterType ParameterType
        {
            get { return (ParameterType)Enum.Parse(typeof(ParameterType), Properties[PropParameterType].Value, true); }
            set { Properties[PropParameterType].Value = value.ToString(); }
        }

        [Description("The value to set the parameter to.")]
        public string Value
        {
            get { return Properties[PropValue].Value; }
            set { Properties[PropValue].Value = value; }
        }

        private void InitializeProperties()
        {
            if (!Properties.ContainsKey(PropName))
                Properties.Add(PropName, new CommandProperty(PropName, string.Empty));

            if (!Properties.ContainsKey(PropOperation))
                Properties.Add(PropOperation, new CommandProperty(PropOperation, VariableCommand.VariableOperation.Get.ToString()));

            if (!Properties.ContainsKey(PropParameterName))
                Properties.Add(PropParameterName, new CommandProperty(PropParameterName, string.Empty));

            if (!Properties.ContainsKey(PropParameterType))
                Properties.Add(PropParameterType, new CommandProperty(PropParameterType, ParameterType.Project.ToString()));

            if (!Properties.ContainsKey(PropValue))
                Properties.Add(PropValue, new CommandProperty(PropValue, string.Empty));
        }
    }
}