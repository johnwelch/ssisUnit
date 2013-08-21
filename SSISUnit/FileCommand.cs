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
    public class FileCommand : CommandBase
    {
        private const string PropOperation = "operation";
        private const string PropSourcePath = "sourcePath";
        private const string PropTargetPath = "targetPath";

        public FileCommand(SsisTestSuite testSuite)
            : base(testSuite)
        {
            InitializeProperties();
        }

        public FileCommand(SsisTestSuite testSuite, object parent)
            : base(testSuite, parent)
        {
            InitializeProperties();
        }

        public FileCommand(SsisTestSuite testSuite, string commandXml)
            : base(testSuite, commandXml)
        {
            InitializeProperties();
        }

        public FileCommand(SsisTestSuite testSuite, object parent, string commandXml)
            : base(testSuite, parent, commandXml)
        {
            InitializeProperties();
        }

        public FileCommand(SsisTestSuite testSuite, XmlNode commandXml)
            : base(testSuite, commandXml)
        {
            InitializeProperties();
        }

        public FileCommand(SsisTestSuite testSuite, object parent, XmlNode commandXml)
            : base(testSuite, parent, commandXml)
        {
            InitializeProperties();
        }

        public FileCommand(SsisTestSuite testSuite, string operation, string sourcePath, string targetPath)
            : this(testSuite)
        {
            Properties[PropOperation] = new CommandProperty(PropOperation, operation);
            Properties[PropSourcePath] = new CommandProperty(PropSourcePath, sourcePath);
            Properties[PropTargetPath] = new CommandProperty(PropTargetPath, targetPath);
        }

        public FileCommand(SsisTestSuite testSuite, object parent, string operation, string sourcePath, string targetPath)
            : this(testSuite, parent)
        {
            Properties[PropOperation] = new CommandProperty(PropOperation, operation);
            Properties[PropSourcePath] = new CommandProperty(PropSourcePath, sourcePath);
            Properties[PropTargetPath] = new CommandProperty(PropTargetPath, targetPath);
        }

        public override object Execute()
        {
            string sourcePath = Properties[PropSourcePath].Value;
            string targetPath = Properties[PropTargetPath].Value;
            string operation = Properties[PropOperation].Value;

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
                OnCommandStarted(new CommandStartedEventArgs(DateTime.Now, CommandName, null, null));

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

                OnCommandCompleted(new CommandCompletedEventArgs(DateTime.Now, CommandName, null, null, string.Format("The {0} command has completed.", CommandName)));
            }
            catch (Exception ex)
            {
                OnCommandFailed(new CommandFailedEventArgs(DateTime.Now, CommandName, null, null, ex.Message));

                returnValue = -1;
            }

            return returnValue;
        }

        //public override object Execute(Microsoft.SqlServer.Dts.Runtime.Package package)
        //{
        //    return Execute();
        //}

        public override object Execute(Microsoft.SqlServer.Dts.Runtime.Package package, Microsoft.SqlServer.Dts.Runtime.DtsContainer container)
        {
            return Execute();
        }

        public override object Execute(XmlNode command, Microsoft.SqlServer.Dts.Runtime.Package package, Microsoft.SqlServer.Dts.Runtime.DtsContainer container)
        {
            LoadFromXml(command);
            return Execute();
        }

        [Description("Defines the operation to perform on a file.")]
        public FileOperation Operation
        {
            get { return ConvertFileOperationString(Properties[PropOperation].Value); }
            set { Properties[PropOperation].Value = value.ToString(); }
        }

        [Description("Sets the source file for file operations."),
         Editor("System.Windows.Forms.Design.FileNameEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
        public string SourcePath
        {
            get { return Properties[PropSourcePath].Value; }
            set { Properties[PropSourcePath].Value = value; }
        }

        [Description("Sets the target path for file operations that need it."),
         Editor("System.Windows.Forms.Design.FileNameEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
        public string TargetPath
        {
            get { return Properties[PropTargetPath].Value; }
            set { Properties[PropTargetPath].Value = value; }
        }

        private static FileOperation ConvertFileOperationString(string type)
        {
            if (type == "Copy")
                return FileOperation.Copy;

            if (type == "Move")
                return FileOperation.Move;

            if (type == "Delete")
                return FileOperation.Delete;

            if (type == "Exists")
                return FileOperation.Exists;

            if (type == "LineCount")
                return FileOperation.LineCount;

            throw new ArgumentException(string.Format("The provided file operation ({0}) is not recognized.", type));
        }

        private void InitializeProperties()
        {
            if (!Properties.ContainsKey(PropOperation))
                Properties.Add(PropOperation, new CommandProperty(PropOperation, FileOperation.Exists.ToString()));

            if (!Properties.ContainsKey(PropSourcePath))
                Properties.Add(PropSourcePath, new CommandProperty(PropSourcePath, string.Empty));

            if (!Properties.ContainsKey(PropTargetPath))
                Properties.Add(PropTargetPath, new CommandProperty(PropTargetPath, string.Empty));
        }

        public enum FileOperation
        {
            Copy = 0,
            Move = 1,
            Exists = 2,
            Delete = 3,
            LineCount = 4
        }
    }
}