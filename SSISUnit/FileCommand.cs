using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using System.Globalization;
using System.ComponentModel;

namespace SsisUnit
{
    public class FileCommand : CommandBase
    {
        private const string PROP_OPERATION = "operation";
        private const string PROP_SOURCE_PATH = "sourcePath";
        private const string PROP_TARGET_PATH = "targetPath";

        public FileCommand(SsisTestSuite testSuite)
            : base(testSuite)
        {
            Properties.Add(PROP_OPERATION, new CommandProperty(PROP_OPERATION, FileOperation.Exists.ToString()));
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
                OnCommandStarted(new CommandStartedEventArgs(DateTime.Now, Name, null, null));

                switch (operation)
                {
                    case "Copy":
                        File.Copy(sourcePath, targetPath, true);
                        returnValue = 0;
                        break;
                    case "Exists":
                        returnValue = File.Exists(sourcePath);
                        break;
                    case "Move":
                        File.Move(sourcePath, targetPath);
                        returnValue = 0;
                        break;
                    case "Delete":
                        File.Delete(sourcePath);
                        returnValue = 0;
                        break;
                    case "LineCount":
                        returnValue = File.ReadAllLines(sourcePath).Length;
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

        [Description("Defines the operation to perform on a file.")]
        public FileOperation Operation
        {
            get { return ConvertFileOperationString(Properties[PROP_OPERATION].Value); }
            set { Properties[PROP_OPERATION].Value = value.ToString(); }
        }

        [Description("Sets the source file for file operations."),
         Editor("System.Windows.Forms.Design.FileNameEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
        public string SourcePath
        {
            get { return Properties[PROP_SOURCE_PATH].Value; }
            set { Properties[PROP_SOURCE_PATH].Value = value; }
        }

        [Description("Sets the target path for file operations that need it."),
         Editor("System.Windows.Forms.Design.FileNameEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
        public string TargetPath
        {
            get { return Properties[PROP_TARGET_PATH].Value; }
            set { Properties[PROP_TARGET_PATH].Value = value; }
        }

        private static FileOperation ConvertFileOperationString(string type)
        {
            if (type == "Copy") return FileOperation.Copy;
            else if (type == "Move") return FileOperation.Move;
            else if (type == "Delete") return FileOperation.Delete;
            else if (type == "Exists") return FileOperation.Exists;
            else if (type == "LineCount") return FileOperation.LineCount;
            else
            {
                throw new ArgumentException(String.Format("The provided file operation ({0}) is not recognized.", type));
            }
        }

        public enum FileOperation : int
        {
            Copy = 0,
            Move = 1,
            Exists = 2,
            Delete = 3,
            LineCount = 4
        }
    }


}
