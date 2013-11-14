using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Xml;

using Microsoft.SqlServer.Dts.Runtime;

using SsisUnitBase;
using SsisUnitBase.Enums;
using SsisUnitBase.EventArgs;

namespace SsisUnit
{
    public abstract class CommandBase : SsisUnitBaseObject
    {
        private const string PropName = "name";

        #region Public Events
        
        public event EventHandler<CommandCompletedEventArgs> CommandCompleted;
        public event EventHandler<CommandFailedEventArgs> CommandFailed;
        public event EventHandler<CommandStartedEventArgs> CommandStarted;

        #endregion

        #region Fields

        private readonly Dictionary<string, CommandProperty> _properties;

        private readonly SsisTestSuite _testSuite;

        #endregion

        #region Constructors and Destructors

        protected CommandBase()
        {
            Body = string.Empty;
            _properties = new Dictionary<string, CommandProperty> { { PropName, new CommandProperty(PropName, string.Empty) } };
        }

        protected CommandBase(SsisTestSuite testSuite)
            : this()
        {
            _testSuite = testSuite;
        }

        protected CommandBase(SsisTestSuite testSuite, object parent)
            : this(testSuite)
        {
            Parent = parent;
        }

        protected CommandBase(SsisTestSuite testSuite, string commandXml)
            : this(testSuite)
        {
            LoadFromXml(commandXml);
        }

        protected CommandBase(SsisTestSuite testSuite, object parent, string commandXml)
            : this(testSuite, commandXml)
        {
            Parent = parent;
        }

        protected CommandBase(SsisTestSuite testSuite, XmlNode commandXml)
            : this(testSuite)
        {
            LoadFromXml(commandXml);
        }

        protected CommandBase(SsisTestSuite testSuite, object parent, XmlNode commandXml)
            : this(testSuite, commandXml)
        {
            Parent = parent;
        }

        #endregion

        #region Public Properties

        [Browsable(false)]
        public string CommandName
        {
            get
            {
                return GetType().Name;
            }
        }

        [Browsable(false)]
        public object Parent { get; private set; }

        [Description("Determines the expected result of the task."), TypeConverter(typeof(EnumConverter))]
        public DTSExecResult TaskResult { get; set; }

        [Browsable(false)]
        public SsisTestSuite TestSuite
        {
            get
            {
                return _testSuite;
            }
        }

        #endregion

        #region Properties

        protected string Body { get; set; }

        protected Dictionary<string, CommandProperty> Properties
        {
            get
            {
                return _properties;
            }
        }

        public new string Name
        {
            get
            {
                return !string.IsNullOrEmpty(Properties[PropName].Value) ? Properties[PropName].Value : CommandName;
            }
            set
            {
                Properties[PropName].Value = !string.IsNullOrEmpty(value) ? value : CommandName;
            }
        }

        #endregion

        #region Public Methods and Operators

        public static CommandBase CreateCommand(SsisTestSuite testSuite, string command)
        {
            CommandBase returnValue = null;

            foreach (Type t in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (typeof(CommandBase).IsAssignableFrom(t) && (!ReferenceEquals(t, typeof(CommandBase))) && (!t.IsAbstract) && (t.Name == command))
                {
                    Type[] @params = { typeof(SsisTestSuite) };

                    ConstructorInfo con = t.GetConstructor(@params);

                    if (con == null)
                        throw new ApplicationException(string.Format(CultureInfo.CurrentCulture, "The Command type {0} could not be loaded because it has no constructor.", t.Name));
                    
                    returnValue = (CommandBase)con.Invoke(new object[] { testSuite });
                }
            }
            return returnValue;
        }

        public static CommandBase CreateCommand(SsisTestSuite testSuite, object parent, string command)
        {
            CommandBase returnValue = null;

            foreach (Type t in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (typeof(CommandBase).IsAssignableFrom(t) && (!ReferenceEquals(t, typeof(CommandBase))) && (!t.IsAbstract) && (t.Name == command))
                {
                    Type[] @params = { typeof(SsisTestSuite), typeof(object) };

                    ConstructorInfo con = t.GetConstructor(@params);

                    if (con == null)
                        throw new ApplicationException(string.Format(CultureInfo.CurrentCulture, "The Command type {0} could not be loaded because it has no constructor.", t.Name));

                    returnValue = (CommandBase)con.Invoke(new[] { testSuite, parent });
                }
            }
            return returnValue;
        }

        public static CommandBase CreateCommand(SsisTestSuite testSuite, XmlNode commandXml)
        {
            CommandBase returnValue = null;

            foreach (Type t in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (typeof(CommandBase).IsAssignableFrom(t) && (!ReferenceEquals(t, typeof(CommandBase))) && (!t.IsAbstract) && (t.Name == commandXml.Name))
                {
                    Type[] @params = { typeof(SsisTestSuite), typeof(XmlNode) };

                    ConstructorInfo con = t.GetConstructor(@params);
                    if (con == null)
                        throw new ApplicationException(string.Format(CultureInfo.CurrentCulture, "The Command type {0} could not be loaded because it has no constructor.", t.Name));
                    returnValue = (CommandBase)con.Invoke(new object[] { testSuite, commandXml });
                }
            }

            return returnValue;
        }

        public static CommandBase CreateCommand(SsisTestSuite testSuite, object parent, XmlNode commandXml)
        {
            CommandBase returnValue = null;

            foreach (Type t in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (typeof(CommandBase).IsAssignableFrom(t) && (!ReferenceEquals(t, typeof(CommandBase))) && (!t.IsAbstract) && (t.Name == commandXml.Name))
                {
                    Type[] @params = { typeof(SsisTestSuite), typeof(object), typeof(XmlNode) };

                    ConstructorInfo con = t.GetConstructor(@params);
                    if (con == null)
                        throw new ApplicationException(string.Format(CultureInfo.CurrentCulture, "The Command type {0} could not be loaded because it has no constructor.", t.Name));
                    returnValue = (CommandBase)con.Invoke(new[] { testSuite, parent, commandXml });
                }
            }

            return returnValue;
        }

        public virtual object Execute(XmlNode command, Package package, DtsContainer container)
        {
            LoadFromXml(command);
            return Execute(package, container);
        }

        public abstract object Execute(object project, Package package, DtsContainer container);
        public abstract object Execute(Package package, DtsContainer container);

        public virtual object Execute()
        {
            return Execute(null, null);
        }

        public CommandParentType GetCommandParentType()
        {
            if (Parent == null)
                return CommandParentType.Unknown;
                
            if (Parent is SsisAssert)
                return CommandParentType.Assert;

            if (TestSuite == null)
                return CommandParentType.Unknown;

            if (Parent == TestSuite.TestSuiteSetup)
                return CommandParentType.TestSuiteSetup;

            if (Parent == TestSuite.TestSuiteTeardown)
                return CommandParentType.TestSuiteTeardown;

            if (Parent == TestSuite.SetupCommands)
                return CommandParentType.UnitTestSetup;

            if (Parent == TestSuite.TeardownCommands)
                return CommandParentType.UnitTestTeardown;

            if (TestSuite.Tests == null)
                return CommandParentType.Unknown;

            foreach (var test in TestSuite.Tests)
            {
                if (test.Value == null)
                    continue;

                if (Parent == test.Value.TestSetup)
                    return CommandParentType.TestSetup;

                if (Parent == test.Value.TestTeardown)
                    return CommandParentType.TestTeardown;
            }

            return CommandParentType.Unknown;
        }

        public override sealed void LoadFromXml(string commandXml)
        {
            LoadFromXml(Helper.GetXmlNodeFromString(commandXml));
        }

        public override sealed void LoadFromXml(XmlNode commandXml)
        {
            CheckCommandType(commandXml.Name);

            Body = commandXml.InnerText;

            if (commandXml.Attributes == null)
                return;

            foreach (XmlAttribute attrib in commandXml.Attributes)
            {
                if (Properties.ContainsKey(attrib.Name))
                    Properties[attrib.Name].Value = attrib.Value;
                else
                    Properties.Add(attrib.Name, new CommandProperty(attrib.Name, attrib.Value));
            }
        }

        public override string PersistToXml()
        {
            var xml = new StringBuilder();
            var writerSettings = new XmlWriterSettings { ConformanceLevel = ConformanceLevel.Fragment, OmitXmlDeclaration = true };

            XmlWriter xmlWriter = XmlWriter.Create(xml, writerSettings);
            xmlWriter.WriteStartElement(CommandName);
            
            foreach (CommandProperty prop in _properties.Values)
            {
                xmlWriter.WriteAttributeString(prop.Name, prop.Value);
            }

            if (Body != string.Empty)
                xmlWriter.WriteString(Body);

            xmlWriter.WriteEndElement();
            xmlWriter.Close();

            return xml.ToString();
        }

        #endregion

        #region Methods

        protected void AddProperty(string propertyName, string initialValue)
        {
            if (!Properties.ContainsKey(propertyName))
            {
                Properties.Add(propertyName, new CommandProperty(propertyName, initialValue));
            }
        }

        protected void CheckCommandType(string commandName)
        {
            if (commandName != CommandName)
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "The node passed to the command argument is not a {0} element.", CommandName));
        }

        protected virtual void OnCommandCompleted(CommandCompletedEventArgs e)
        {
            EventHandler<CommandCompletedEventArgs> handler = CommandCompleted;

            if (handler != null)
                handler(this, e);
        }

        protected virtual void OnCommandFailed(CommandFailedEventArgs e)
        {
            EventHandler<CommandFailedEventArgs> handler = CommandFailed;

            if (handler != null)
                handler(this, e);
        }

        protected virtual void OnCommandStarted(CommandStartedEventArgs e)
        {
            EventHandler<CommandStartedEventArgs> handler = CommandStarted;

            if (handler != null)
                handler(this, e);
        }

        #endregion
    }
}