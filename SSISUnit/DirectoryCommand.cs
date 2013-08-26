using System;
using System.Xml;
using System.IO;
using System.Globalization;
using System.ComponentModel;

#if SQL2012 || SQL2008
using IDTSComponentMetaData = Microsoft.SqlServer.Dts.Pipeline.Wrapper.IDTSComponentMetaData100;
#elif SQL2005
using IDTSComponentMetaData = Microsoft.SqlServer.Dts.Pipeline.Wrapper.IDTSComponentMetaData90;
#endif

namespace SsisUnit
{
    public class DirectoryCommand : CommandBase
    {
        private const string PropOperation = "operation";
        private const string PropArgument1 = "argument1";
        private const string PropArgument2 = "argument2";

        public DirectoryCommand(SsisTestSuite testSuite)
            : base(testSuite)
        {
            InitializeProperties();
        }

        public DirectoryCommand(SsisTestSuite testSuite, object parent)
            : base(testSuite, parent)
        {
            InitializeProperties();
        }

        public DirectoryCommand(SsisTestSuite testSuite, string commandXml)
            : base(testSuite, commandXml)
        {
            InitializeProperties();
        }

        public DirectoryCommand(SsisTestSuite testSuite, object parent, string commandXml)
            : base(testSuite, parent, commandXml)
        {
            InitializeProperties();
        }

        public DirectoryCommand(SsisTestSuite testSuite, XmlNode commandXml)
            : base(testSuite, commandXml)
        {
            InitializeProperties();
        }

        public DirectoryCommand(SsisTestSuite testSuite, object parent, XmlNode commandXml)
            : base(testSuite, parent, commandXml)
        {
            InitializeProperties();
        }

        public DirectoryCommand(SsisTestSuite testSuite, string operation, string argument1, string argument2)
            : this(testSuite)
        {
            Properties[PropOperation] = new CommandProperty(PropOperation, operation);
            Properties[PropArgument1] = new CommandProperty(PropArgument1, argument1);
            Properties[PropArgument2] = new CommandProperty(PropArgument2, argument2);
        }

        public DirectoryCommand(SsisTestSuite testSuite, object parent, string operation, string argument1, string argument2)
            : this(testSuite, parent)
        {
            Properties[PropOperation] = new CommandProperty(PropOperation, operation);
            Properties[PropArgument1] = new CommandProperty(PropArgument1, argument1);
            Properties[PropArgument2] = new CommandProperty(PropArgument2, argument2);
        }

        public override object Execute(Microsoft.SqlServer.Dts.Runtime.Package package, Microsoft.SqlServer.Dts.Runtime.DtsContainer container)
        {
            object returnValue = null;

            string argument1 = Properties[PropArgument1].Value;
            string argument2 = Properties[PropArgument2].Value;
            DirectoryOperation operation = DirectoryOperationFromString(Properties[PropOperation].Value);

            if (operation == DirectoryOperation.Move)
            {
                if (argument2 == string.Empty)
                {
                    throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "argument2 must specify a target path for the {0} operation.", operation));
                }
            }

            try
            {
                OnCommandStarted(new CommandStartedEventArgs(DateTime.Now, CommandName, null, null));

                switch (operation)
                {
                    case DirectoryOperation.Exists:
                        returnValue = Directory.Exists(argument1);
                        break;
                    case DirectoryOperation.Move:
                        Directory.Move(argument1, argument2);
                        returnValue = 0;
                        break;
                    case DirectoryOperation.Create:
                        Directory.CreateDirectory(argument1);
                        returnValue = 0;
                        break;
                    case DirectoryOperation.Delete:
                        Directory.Delete(argument1);
                        returnValue = 0;
                        break;
                    case DirectoryOperation.FileCount:
                        if (argument2 == string.Empty)
                        {
                            argument2 = "*.*";
                        }
                        returnValue = Directory.GetFiles(argument1, argument2).Length;
                        break;
                }

                OnCommandCompleted(new CommandCompletedEventArgs(DateTime.Now, CommandName, null, null, string.Format("The {0} command has completed.", CommandName)));
            }
            catch (Exception ex)
            {
                OnCommandFailed(new CommandFailedEventArgs(DateTime.Now, CommandName, null, null, ex.Message));

                throw;
            }

            return returnValue;
        }

        public override object Execute(XmlNode command, Microsoft.SqlServer.Dts.Runtime.Package package, Microsoft.SqlServer.Dts.Runtime.DtsContainer container)
        {
            LoadFromXml(command);
            
            return Execute(package, container);
        }

        private DirectoryOperation DirectoryOperationFromString(string dirOperation)
        {
            if (dirOperation == "Exists") return DirectoryOperation.Exists;
            if (dirOperation == "Move") return DirectoryOperation.Move;
            if (dirOperation == "Create") return DirectoryOperation.Create;
            if (dirOperation == "Delete") return DirectoryOperation.Delete;
            if (dirOperation == "FileCount") return DirectoryOperation.FileCount;
            throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "The specified operation ({0}) is not valid.", dirOperation));
        }

        [Description("The first argument to the operation."),
         Editor("System.Windows.Forms.Design.FolderNameEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
        public string Argument1
        {
            get { return Properties[PropArgument1].Value; }
            set { Properties[PropArgument1].Value = value; }
        }

        [Description("The second argument to the operation."),
        Editor("System.Windows.Forms.Design.FolderNameEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
        public string Argument2
        {
            get { return Properties[PropArgument2].Value; }
            set { Properties[PropArgument2].Value = value; }
        }

        [Description("The operation to perform on the specified directory.")]
        public DirectoryOperation Operation
        {
            get { return DirectoryOperationFromString(Properties[PropOperation].Value); }
            set { Properties[PropOperation].Value = value.ToString(); }
        }

        private void InitializeProperties()
        {
            if (!Properties.ContainsKey(PropOperation))
                Properties.Add(PropOperation, new CommandProperty(PropOperation, DirectoryOperation.Exists.ToString()));

            if (!Properties.ContainsKey(PropArgument1))
                Properties.Add(PropArgument1, new CommandProperty(PropArgument1, string.Empty));

            if (!Properties.ContainsKey(PropArgument2))
                Properties.Add(PropArgument2, new CommandProperty(PropArgument2, string.Empty));
        }
    }

    public enum DirectoryOperation
    {
        FileCount, Create, Move, Delete, Exists
    }
}
