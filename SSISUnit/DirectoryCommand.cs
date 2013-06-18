﻿using System;
using System.Xml;
using System.IO;
using System.Globalization;
using System.ComponentModel;

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
            Properties.Add(PropOperation, new CommandProperty(PropOperation, DirectoryOperation.Exists.ToString()));
            Properties.Add(PropArgument1, new CommandProperty(PropArgument1, string.Empty));
            Properties.Add(PropArgument2, new CommandProperty(PropArgument2, string.Empty));
        }

        public DirectoryCommand(SsisTestSuite testSuite, string commandXml)
            : base(testSuite, commandXml)
        {
        }

        public DirectoryCommand(SsisTestSuite testSuite, XmlNode commandXml)
            : base(testSuite, commandXml)
        {
        }

        public DirectoryCommand(SsisTestSuite testSuite, string operation, string argument1, string argument2)
            : base(testSuite)
        {
            Properties.Add(PropOperation, new CommandProperty(PropOperation, operation));
            Properties.Add(PropArgument1, new CommandProperty(PropArgument1, argument1));
            Properties.Add(PropArgument2, new CommandProperty(PropArgument2, argument2));
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
                OnCommandStarted(new CommandStartedEventArgs(DateTime.Now, Name, null, null));

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

                OnCommandCompleted(new CommandCompletedEventArgs(DateTime.Now, Name, null, null, string.Format("The {0} command has completed.", Name)));
            }
            catch (Exception ex)
            {
                OnCommandFailed(new CommandFailedEventArgs(DateTime.Now, Name, null, null, ex.Message));

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
    }

    public enum DirectoryOperation
    {
        FileCount, Create, Move, Delete, Exists
    }
}
