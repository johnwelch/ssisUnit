using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

using Microsoft.SqlServer.Dts.Pipeline.Wrapper;
using Microsoft.SqlServer.Dts.Runtime;

#if SQL2012 || SQL2008
using SsisUnit.TestComponents;

using IDTSComponentMetaData = Microsoft.SqlServer.Dts.Pipeline.Wrapper.IDTSComponentMetaData100;
using IDTSInput = Microsoft.SqlServer.Dts.Pipeline.Wrapper.IDTSInput100;
#elif SQL2005
using IDTSComponentMetaData = Microsoft.SqlServer.Dts.Pipeline.Wrapper.IDTSComponentMetaData90;
#endif

namespace SsisUnit.Commands
{
    class ComponentInputCommand : CommandBase
    {
        private const string SsisUnitSourceId = "SsisUnit.TestComponents.SsisUnitSource";
        private const string PropName = "name";
        private const string PropDataSet = "dataset";
        private const string PropInput = "input";

        #region Constructors

        public ComponentInputCommand(SsisTestSuite testSuite)
            : base(testSuite)
        {
            InitializeProperties();
        }

        public ComponentInputCommand(SsisTestSuite testSuite, object parent)
            : base(testSuite, parent)
        {
            InitializeProperties();
        }

        public ComponentInputCommand(SsisTestSuite testSuite, string commandXml)
            : base(testSuite, commandXml)
        {
            InitializeProperties();
        }

        public ComponentInputCommand(SsisTestSuite testSuite, object parent, string commandXml)
            : base(testSuite, parent, commandXml)
        {
            InitializeProperties();
        }

        public ComponentInputCommand(SsisTestSuite testSuite, XmlNode commandXml)
            : base(testSuite, commandXml)
        {
            InitializeProperties();
        }

        public ComponentInputCommand(SsisTestSuite testSuite, object parent, XmlNode commandXml)
            : base(testSuite, parent, commandXml)
        {
            InitializeProperties();
        }

        public ComponentInputCommand(SsisTestSuite testSuite, string name, string dataset, string input)
            : this(testSuite)
        {
            Properties[PropName] = new CommandProperty(PropName, name);
            Properties[PropDataSet] = new CommandProperty(PropDataSet, dataset);
            Properties[PropInput] = new CommandProperty(PropInput, input);
        }
       
        #endregion

        public override object Execute(Package package, DtsContainer container)
        {
            string remainingPath;
            var taskHost = Helper.FindExecutable(package, Properties[PropInput].Value, out remainingPath) as TaskHost;
            if (taskHost == null)
            {
                throw new ArgumentException("Input property did not match a valid component input.", "package");
            }
            var mainPipe = taskHost.InnerObject as MainPipe;
            if (mainPipe == null)
            {
                throw new ArgumentException("Input property did not match a valid component input.", "package");
            }

            IDTSInput input = Helper.FindComponentInput(mainPipe, remainingPath);
            if (input == null)
            {
                throw new ArgumentException("Input property did not match a valid component input.", "package");
            }

            ReplaceInput(mainPipe, input, TestSuite.Datasets[Properties[PropDataSet].Value]);

            return null;
        }

        private void ReplaceInput(MainPipe mainPipe, IDTSInput input, Dataset dataset)
        {
            var placeholderSource = mainPipe.ComponentMetaDataCollection.New();
            placeholderSource.ComponentClassID = typeof(SsisUnitSource).AssemblyQualifiedName;
            CManagedComponentWrapper wrapper = placeholderSource.Instantiate();
            wrapper.ProvideComponentProperties();
            wrapper.SetComponentProperty("TestData", GetDataSet(dataset));
            wrapper.ReinitializeMetaData();

            var path = Helper.FindPath(mainPipe, input);
            mainPipe.PathCollection.RemoveObjectByID(path.ID);
            path = mainPipe.PathCollection.New();
            path.AttachPathAndPropagateNotifications(placeholderSource.OutputCollection[0], input);
        }

        private string GetDataSet(Dataset dataset)
        {
            var result = dataset.RetrieveDataTable();
            using (var writer = new StringWriter())
            {
                result.WriteXml(writer);
                return writer.ToString();
            }
        }

        private void InitializeProperties()
        {
            AddProperty(PropName, string.Empty);
            AddProperty(PropDataSet, string.Empty);
            AddProperty(PropInput, string.Empty);
        }
    }
}
