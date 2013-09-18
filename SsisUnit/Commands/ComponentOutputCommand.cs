using System;
using System.Xml;

using Microsoft.SqlServer.Dts.Pipeline.Wrapper;
using Microsoft.SqlServer.Dts.Runtime;

#if SQL2012 || SQL2008
using IDTSComponentMetaData = Microsoft.SqlServer.Dts.Pipeline.Wrapper.IDTSComponentMetaData100;
using IDTSOutput = Microsoft.SqlServer.Dts.Pipeline.Wrapper.IDTSOutput100;
#elif SQL2005
using IDTSComponentMetaData = Microsoft.SqlServer.Dts.Pipeline.Wrapper.IDTSComponentMetaData90;
using IDTSOutput = Microsoft.SqlServer.Dts.Pipeline.Wrapper.IDTSOutput90;
#endif

namespace SsisUnit.Commands
{
    class ComponentOutputCommand : CommandBase
    {
        private const string PropName = "name";
        private const string PropDataSet = "dataset";
        private const string PropOutput = "output";

        #region Constructors

        public ComponentOutputCommand(SsisTestSuite testSuite)
            : base(testSuite)
        {
            InitializeProperties();
        }

        public ComponentOutputCommand(SsisTestSuite testSuite, object parent)
            : base(testSuite, parent)
        {
            InitializeProperties();
        }

        public ComponentOutputCommand(SsisTestSuite testSuite, string commandXml)
            : base(testSuite, commandXml)
        {
            InitializeProperties();
        }

        public ComponentOutputCommand(SsisTestSuite testSuite, object parent, string commandXml)
            : base(testSuite, parent, commandXml)
        {
            InitializeProperties();
        }

        public ComponentOutputCommand(SsisTestSuite testSuite, XmlNode commandXml)
            : base(testSuite, commandXml)
        {
            InitializeProperties();
        }

        public ComponentOutputCommand(SsisTestSuite testSuite, object parent, XmlNode commandXml)
            : base(testSuite, parent, commandXml)
        {
            InitializeProperties();
        }

        public ComponentOutputCommand(SsisTestSuite testSuite, string name, string dataset, string output)
            : this(testSuite)
        {
            Properties[PropName] = new CommandProperty(PropName, name);
            Properties[PropDataSet] = new CommandProperty(PropDataSet, dataset);
            Properties[PropOutput] = new CommandProperty(PropOutput, output);
        }
       
        #endregion

        public override object Execute(object project, Package package, DtsContainer container)
        {
            return Execute(package, container);
        }

        public override object Execute(Package package, DtsContainer container)
        {
            string remainingPath;
            var taskHost = Helper.FindExecutable(package, Properties[PropOutput].Value, out remainingPath) as TaskHost;
            if (taskHost == null)
            {
                throw new ArgumentException("Output property did not match a valid component output.", "package");
            }
            var mainPipe = taskHost.InnerObject as MainPipe;
            if (mainPipe == null)
            {
                throw new ArgumentException("Output property did not match a valid component output.", "package");
            }

            IDTSOutput output = Helper.FindComponentOutput(mainPipe, remainingPath);
            if (output == null)
            {
                throw new ArgumentException("Output property did not match a valid component output.", "package");
            }

            throw new NotImplementedException();
            // return true;
        }

        private void InitializeProperties()
        {
            AddProperty(PropName, string.Empty);
            AddProperty(PropDataSet, string.Empty);
            AddProperty(PropOutput, string.Empty);
        }
    }
}
