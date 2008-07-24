using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using System.Globalization;

namespace SsisUnit
{
    class DirectoryCommand : CommandBase
    {
        private const string PROP_OPERATION = "operation";
        private const string PROP_ARGUMENT_1 = "argument1";
        private const string PROP_ARGUMENT_2 = "argument2";

        public DirectoryCommand(SsisTestSuite testSuite)
            : base(testSuite)
        {
            Properties.Add(PROP_OPERATION, new CommandProperty(PROP_OPERATION, string.Empty));
            Properties.Add(PROP_ARGUMENT_1, new CommandProperty(PROP_ARGUMENT_1, string.Empty));
            Properties.Add(PROP_ARGUMENT_2, new CommandProperty(PROP_ARGUMENT_2, string.Empty));
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
            Properties.Add(PROP_OPERATION, new CommandProperty(PROP_OPERATION, operation));
            Properties.Add(PROP_ARGUMENT_1, new CommandProperty(PROP_ARGUMENT_1, argument1));
            Properties.Add(PROP_ARGUMENT_2, new CommandProperty(PROP_ARGUMENT_2, argument2));
        }

        //public DirectoryCommand(XmlNode connections, XmlNamespaceManager namespaceMgr)
        //    : base(connections, namespaceMgr)
        //{ }

        //#region Public Properties

        //public DirectoryOperation Operation
        //{
        //    get { return DirectoryOperationFromString(_operation); }
        //    set { _operation = DirectoryOperationToString(value); }
        //}


        //#endregion

        //public string PersistToXml()
        //{
        //    string returnValue = "<" + this.CommandName + " operation=\"" + _operation + "\" argument1=\"" + _argument1 + "\"";
        //    if (_argument2 != string.Empty) returnValue += " argument2=\"" + _argument2 + "\"";
        //    returnValue += "</" + this.CommandName + ">";
        //    return returnValue;
        //}

        public override object Execute(System.Xml.XmlNode command, Microsoft.SqlServer.Dts.Runtime.Package package, Microsoft.SqlServer.Dts.Runtime.DtsContainer container)
        {
            
            object returnValue = null;


            this.LoadFromXml(command);

            string argument1 = Properties[PROP_ARGUMENT_1].Value;
            string argument2 = Properties[PROP_ARGUMENT_2].Value;
            DirectoryOperation operation = DirectoryOperationFromString(Properties[PROP_OPERATION].Value);

            if (operation == DirectoryOperation.Move)
            {
                if (argument2 == string.Empty)
                {
                    throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "argument2 must specify a target path for the {0} operation.", operation));
                }
            }

            try
            {
                if (operation == DirectoryOperation.Exists)
                {
                    returnValue = Directory.Exists(argument1);
                }
                else if (operation == DirectoryOperation.Move)
                {
                    Directory.Move(argument1, argument2);
                    returnValue = 0;
                }
                else if (operation == DirectoryOperation.Create)
                {
                    Directory.CreateDirectory(argument1);
                    returnValue = 0;
                }
                else if (operation == DirectoryOperation.Delete)
                {
                    Directory.Delete(argument1);
                    returnValue = 0;
                }
                else if (operation == DirectoryOperation.FileCount)
                {
                    if (argument2 == string.Empty)
                    {
                        argument2 = "*.*";
                    }
                    return Directory.GetFiles(argument1, argument2).Length;
                }
            }
            catch (Exception)
            {
                returnValue = -1;
            }

            return returnValue;
        }

        private string DirectoryOperationToString(DirectoryOperation dirOperation)
        {
            switch (dirOperation)
            {
                case DirectoryOperation.FileCount:
                    return "FileCount";
                case DirectoryOperation.Create:
                    return "Create";
                case DirectoryOperation.Move:
                    return "Move";
                case DirectoryOperation.Delete:
                    return "Delete";
                case DirectoryOperation.Exists:
                    return "Exists";
                default:
                    return "Unknown";
            }
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

    }

    public enum DirectoryOperation
    {
        FileCount, Create, Move, Delete, Exists
    }
}
