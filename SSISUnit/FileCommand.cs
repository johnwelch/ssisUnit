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
        private const string PROP_OPERATION = "operation";
        private const string PROP_SOURCE_PATH = "sourcePath";
        private const string PROP_TARGET_PATH = "targetPath";

        public FileCommand(SsisTestSuite testSuite)
            : base(testSuite)
        {
            Properties.Add(PROP_OPERATION, new CommandProperty(PROP_OPERATION, string.Empty));
            Properties.Add(PROP_SOURCE_PATH, new CommandProperty(PROP_SOURCE_PATH, string.Empty));
            Properties.Add(PROP_TARGET_PATH, new CommandProperty(PROP_TARGET_PATH, string.Empty));
        }

        public FileCommand(SsisTestSuite testSuite, string commandXml)
            : base(testSuite, commandXml)
        {
        }

        public FileCommand(SsisTestSuite testSuite, XmlNode commandXml)
            : base(testSuite, commandXml)
        {
        }

        public FileCommand(SsisTestSuite testSuite, string operation, string sourcePath, string targetPath)
            : base(testSuite)
        {
            Properties.Add(PROP_OPERATION, new CommandProperty(PROP_OPERATION, operation));
            Properties.Add(PROP_SOURCE_PATH, new CommandProperty(PROP_SOURCE_PATH, sourcePath));
            Properties.Add(PROP_TARGET_PATH, new CommandProperty(PROP_TARGET_PATH, targetPath));
        }

        public override object Execute()
        {
            string sourcePath = Properties[PROP_SOURCE_PATH].Value;
            string targetPath = Properties[PROP_TARGET_PATH].Value;
            string operation = Properties[PROP_OPERATION].Value;

            object returnValue = null;

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
                else if (operation == "Delete")
                {
                    File.Delete(sourcePath);
                    returnValue = 0;
                }
                else if (operation == "LineCount")
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

        public override object Execute(Microsoft.SqlServer.Dts.Runtime.Package package)
        {
            return this.Execute();
        }

        public override object Execute(Microsoft.SqlServer.Dts.Runtime.Package package, Microsoft.SqlServer.Dts.Runtime.DtsContainer container)
        {
            return this.Execute();
        }

        public override object Execute(XmlNode command, Microsoft.SqlServer.Dts.Runtime.Package package, Microsoft.SqlServer.Dts.Runtime.DtsContainer container)
        {
            this.LoadFromXml(command);
            return Execute();
        }

        public string Operation
        {
            get { return Properties[PROP_OPERATION].Value; }
            set { Properties[PROP_OPERATION].Value = value; }
        }

        public string SourcePath
        {
            get { return Properties[PROP_SOURCE_PATH].Value; }
            set { Properties[PROP_SOURCE_PATH].Value = value; }
        }

        public string TargetPath
        {
            get { return Properties[PROP_TARGET_PATH].Value; }
            set { TargetPath = Properties[PROP_TARGET_PATH].Value; }
        }
    }
}
