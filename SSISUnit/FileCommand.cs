using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using System.Globalization;

namespace SsisUnit
{
    class FileCommand : CommandBase
    {
        public FileCommand(SsisTestSuite testSuite)
            : base(testSuite)
        { }


        public override object Execute(XmlNode command, Microsoft.SqlServer.Dts.Runtime.Package package, Microsoft.SqlServer.Dts.Runtime.DtsContainer container)
        {
            string sourcePath;
            string targetPath = string.Empty;
            string operation;

            object returnValue = null;

            //if (command.Name != this.CommandName)
            //{
            //    throw new ArgumentException(String.Format(CultureInfo.CurrentCulture, "The node passed to the command argument is not a {0} element.", this.CommandName));
            //}
            this.CheckCommandType(command.Name);

            XmlNode targetPathAttr = command.Attributes.GetNamedItem("targetPath");

            if (targetPathAttr != null)
            {
                targetPath = targetPathAttr.Value;
            }
            sourcePath = command.Attributes["sourcePath"].Value;
            operation = command.Attributes["operation"].Value;

            if (operation == "Copy" || operation == "Move")
            {
                if (targetPath == string.Empty)
                {
                    throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "The targetPath must be specified for the {0} operation.", operation));
                }
            }

            try
            {
                if (operation == "Copy")
                {
                    File.Copy(sourcePath, targetPath, true);
                    returnValue = 0;
                }
                else if (operation == "Exists")
                {
                    returnValue = File.Exists(sourcePath);
                }
                else if (operation == "Move")
                {
                    File.Move(sourcePath, targetPath);
                    returnValue = 0;
                }
                else if (operation=="Delete")
                {
                    File.Delete(sourcePath);
                    returnValue = 0;
                }
                else if (operation=="LineCount")
                {
                    return File.ReadAllLines(sourcePath).Length;
                }
            }
            catch (Exception)
            {
                returnValue = -1;
            }

            return returnValue;
        }
    }
}
