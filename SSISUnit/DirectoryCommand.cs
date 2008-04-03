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
        private string _operation = string.Empty;
        private string _argument1 = string.Empty;
        private string _argument2 = string.Empty;


        public DirectoryCommand(XmlNode connections, XmlNamespaceManager namespaceMgr)
            : base(connections, namespaceMgr)
        { }

        //#region Public Properties

        //public DirectoryOperation Operation
        //{
        //    get { return DirectoryOperationFromString(_operation); }
        //    set { _operation = DirectoryOperationToString(value); }
        //}


        //#endregion

        public string PersistToXml()
        {
            string returnValue = "<" + this.CommandName + " operation=\"" + _operation + "\" argument1=\"" + _argument1 + "\"";
            if (_argument2 != string.Empty) returnValue += " argument2=\"" + _argument2 + "\"";
            returnValue += "</" + this.CommandName + ">";
            return returnValue;
        }

        public override object Execute(System.Xml.XmlNode command, Microsoft.SqlServer.Dts.Runtime.Package package, Microsoft.SqlServer.Dts.Runtime.DtsContainer container)
        {
            string argument1;
            string argument2 = string.Empty;
            DirectoryOperation operation;

            object returnValue = null;

            this.CheckCommandType(command.Name);

            XmlNode targetPathAttr = command.Attributes.GetNamedItem("argument2");

            if (targetPathAttr != null)
            {
                argument2 = targetPathAttr.Value;
            }
            argument1 = command.Attributes["argument1"].Value;
            operation = DirectoryOperationFromString(command.Attributes["operation"].Value);

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
