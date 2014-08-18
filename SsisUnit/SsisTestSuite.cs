using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Xml;
using System.Xml.Schema;

using Microsoft.SqlServer.Dts.Runtime;
using System.Text;

using SsisUnit.DynamicValues;
using SsisUnit.Packages;

using SsisUnitBase;
using SsisUnitBase.Enums;
using SsisUnitBase.EventArgs;

#if SQL2012 || SQL2014
using System.ComponentModel.Composition;
#endif

namespace SsisUnit
{
#if SQL2012 || SQL2014
    [Export(typeof(ISsisTestSuite))]
#endif
    public class SsisTestSuite : ISsisTestSuite, IValidate
    {
        private XmlDocument _testCaseDoc;
        private XmlNamespaceManager _namespaceMgr;

        #region Constructors

        private SsisTestSuite(bool isLoadDefaultTest)
        {
            PackageList = new PackageList();
            ConnectionList = new Dictionary<string, ConnectionRef>();
            Datasets = new Dictionary<string, Dataset>();
            Tests = new Tests();
            TestRefs = new Dictionary<string, TestRef>();
            Statistics = new TestSuiteResults();
            ValidationMessages = string.Empty;
            SsisApplication = new Application();
            Parameters = new Dictionary<string, string>();
            DynamicValues = new DynamicValues.DynamicValues(this);
            //Variables = new Dictionary<string, object>();

            
            if (!isLoadDefaultTest)
                return;

            Stream baseTest = GetStreamFromExecutingAssembly("BaseTest.xml");

            InitializeTestCase(baseTest);
        }

        public SsisTestSuite()
            : this(true)
        {
        }

        public SsisTestSuite(string testCaseFile)
            : this(false)
        {
            if (testCaseFile == null)
                throw new ArgumentNullException("testCaseFile");

            InitializeTestCase(testCaseFile);
        }

        public SsisTestSuite(Stream testCase)
            : this(false)
        {
            if (testCase == null)
                throw new ArgumentNullException("testCase");

            InitializeTestCase(testCase);
        }

        #endregion

        #region Properties

        public int SsisVersion
        {
            get
            {
#if SQL2005
                return 2005;
#elif SQL2008
                return 2008;
#elif SQL2012
                return 2012;
#elif SQL2014
                return 2014;
#endif
            }
        }

        public TestSuiteResults Statistics { get; private set; }

        public Dictionary<string, ConnectionRef> ConnectionList { get; private set; }

        public Dictionary<string, Dataset> Datasets { get; private set; }

        public Tests Tests { get; private set; }

        public Dictionary<string, TestRef> TestRefs { get; private set; }

        public PackageList PackageList { get; private set; }

        public CommandSet TestSuiteSetup { get; private set; }

        public CommandSet TestSuiteTeardown { get; private set; }

        public CommandSet SetupCommands { get; private set; }

        public CommandSet TeardownCommands { get; private set; }

        internal Application SsisApplication { get; private set; }

        internal SsisTestSuite ParentTestSuite { get; private set; }

        public string ValidationMessages { get; set; }

        public DynamicValues.DynamicValues DynamicValues { get; private set; }
        public Dictionary<string, string> Parameters { get; private set; }

        //public Dictionary<string, object> Variables { get; private set; }


        #endregion

        #region Events

        public event EventHandler<AssertCompletedEventArgs> AssertCompleted;
        public event EventHandler<CommandCompletedEventArgs> CommandCompleted;
        public event EventHandler<CommandFailedEventArgs> CommandFailed;
        public event EventHandler<CommandStartedEventArgs> CommandStarted;
        public event EventHandler<SetupCompletedEventArgs> SetupCompleted;
        public event EventHandler<TeardownCompletedEventArgs> TeardownCompleted;
        public event EventHandler<TestCompletedEventArgs> TestCompleted;
        public event EventHandler<TestStartedEventArgs> TestStarted;
        public event EventHandler<TestSuiteCompletedEventArgs> TestSuiteCompleted;
        public event EventHandler<TestSuiteFailedEventArgs> TestSuiteFailed;
        public event EventHandler<TestSuiteStartedEventArgs> TestSuiteStarted;

        internal virtual void OnRaiseCommandCompleted(object sender, CommandCompletedEventArgs e)
        {
            EventHandler<CommandCompletedEventArgs> handler = CommandCompleted;

            if (handler != null)
                handler(sender, e);
        }

        internal virtual void OnRaiseCommandFailed(object sender, CommandFailedEventArgs e)
        {
            EventHandler<CommandFailedEventArgs> handler = CommandFailed;

            if (handler != null)
                handler(sender, e);
        }

        internal virtual void OnRaiseCommandStarted(object sender, CommandStartedEventArgs e)
        {
            EventHandler<CommandStartedEventArgs> handler = CommandStarted;

            if (handler != null)
                handler(sender, e);
        }

        internal virtual void OnRaiseAssertCompleted(AssertCompletedEventArgs e)
        {
            EventHandler<AssertCompletedEventArgs> handler = AssertCompleted;

            // Event will be null if there are no subscribers
            if (handler != null)
            {
                handler(this, e);
            }
        }

        internal virtual void OnRaiseTestCompleted(TestCompletedEventArgs e)
        {
            EventHandler<TestCompletedEventArgs> handler = TestCompleted;

            // Event will be null if there are no subscribers
            if (handler != null)
            {
                handler(this, e);
            }
        }

        internal virtual void OnRaiseTestStarted(TestStartedEventArgs e)
        {
            EventHandler<TestStartedEventArgs> handler = TestStarted;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        internal virtual void OnRaiseTestSuiteCompleted(TestSuiteCompletedEventArgs e)
        {
            EventHandler<TestSuiteCompletedEventArgs> handler = TestSuiteCompleted;

            // Event will be null if there are no subscribers
            if (handler != null)
            {
                handler(this, e);
            }
        }

        internal virtual void OnRaiseTestSuiteFailed(TestSuiteFailedEventArgs e)
        {
            EventHandler<TestSuiteFailedEventArgs> handler = TestSuiteFailed;

            // Event will be null if there are no subscribers
            if (handler != null)
            {
                handler(this, e);
            }
        }

        internal virtual void OnRaiseTestSuiteStarted(TestSuiteStartedEventArgs e)
        {
            EventHandler<TestSuiteStartedEventArgs> handler = TestSuiteStarted;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        internal virtual void OnRaiseSetupCompleted(SetupCompletedEventArgs e)
        {
            EventHandler<SetupCompletedEventArgs> handler = SetupCompleted;

            // Event will be null if there are no subscribers
            if (handler != null)
            {
                handler(this, e);
            }
        }

        internal virtual void OnRaiseTeardownCompleted(TeardownCompletedEventArgs e)
        {
            // Make a temporary copy of the event to avoid possibility of
            // a race condition if the last subscriber unsubscribes
            // immediately after the null check and before the event is raised.
            EventHandler<TeardownCompletedEventArgs> handler = TeardownCompleted;

            // Event will be null if there are no subscribers
            if (handler != null)
            {
                handler(this, e);
            }
        }

        #endregion

        #region ISsisTestSuite Members

        public ISsisTestSuite CreateSsisTestSuite(string testSuiteFilepath)
        {
            return new SsisTestSuite(testSuiteFilepath);
        }

        public void Setup()
        {
            throw new NotImplementedException();
        }

        public void Test()
        {
            throw new NotImplementedException();
        }

        public void Teardown()
        {
            throw new NotImplementedException();
        }

        #endregion

        public Context CreateContext()
        {
            return new Context(this);
        }

        public void Save(string fileName)
        {
            File.WriteAllText(fileName, PersistToXml(), Encoding.UTF8);
        }

        internal string PersistToXml()
        {
            using (StringWriter stringWriter = new StringWriter())
            {
                XmlWriterSettings settings = new XmlWriterSettings { Indent = true, Encoding = Encoding.UTF8, ConformanceLevel = ConformanceLevel.Document, OmitXmlDeclaration = false, NewLineOnAttributes = false };

                using (XmlWriter xmlWriter = XmlWriter.Create(stringWriter, settings))
                {
                    xmlWriter.WriteProcessingInstruction("xml", @"version=""1.0"" encoding=""utf-8"" ");
                    xmlWriter.WriteStartElement("TestSuite", "http://tempuri.org/SsisUnit.xsd");

                    xmlWriter.WriteStartElement("ConnectionList");

                    foreach (ConnectionRef conn in ConnectionList.Values)
                    {
                        xmlWriter.WriteRaw(conn.PersistToXml());
                    }

                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("PackageList");

                    foreach (PackageRef pkg in PackageList.Values)
                    {
                        xmlWriter.WriteRaw(pkg.PersistToXml());
                    }

                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("DatasetList");

                    foreach (Dataset dataset in Datasets.Values)
                    {
                        xmlWriter.WriteRaw(dataset.PersistToXml());
                    }

                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("TestSuiteSetup");
                    xmlWriter.WriteRaw(TestSuiteSetup.PersistToXml());
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("Setup");
                    xmlWriter.WriteRaw(SetupCommands.PersistToXml());
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("Tests");

                    foreach (Test test in Tests.Values)
                    {
                        xmlWriter.WriteRaw(test.PersistToXml());
                    }

                    foreach (TestRef testRef in TestRefs.Values)
                    {
                        xmlWriter.WriteRaw(testRef.PersistToXml());
                    }

                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("Teardown");
                    xmlWriter.WriteRaw(TeardownCommands.PersistToXml());
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("TestSuiteTeardown");
                    xmlWriter.WriteRaw(TestSuiteTeardown.PersistToXml());
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteEndElement();
                    xmlWriter.WriteEndDocument();

                    xmlWriter.Close();
                }

                return stringWriter.ToString();
            }
        }

        public bool Validate()
        {
            try
            {
                Stream strm = GetStreamFromExecutingAssembly("SsisUnit.xsd");

                using (XmlReader reader = XmlReader.Create(strm))
                {
                    reader.Read();

                    var settings = new XmlReaderSettings();
                    settings.Schemas.Add("http://tempuri.org/SsisUnit.xsd", reader);
                    settings.ValidationType = ValidationType.Schema;

                    var bytes = Encoding.ASCII.GetBytes(PersistToXml());

                    var test = new XmlDocument();
                    test.Load(XmlReader.Create(new MemoryStream(bytes), settings));

                    return test.SchemaInfo.Validity == XmlSchemaValidity.Valid;
                }
            }
            catch (XmlSchemaValidationException)
            {
                return false;
            }
            catch (Exception ex)
            {
                throw new ArgumentException("The test case could not be loaded: " + ex.Message);
            }
        }

        public int Execute(IDictionary<string, string> parameters)
        {
            if (parameters != null)
            {
                foreach (var parameter in parameters)
                {
                    Parameters[parameter.Key] = parameter.Value;
                }                
            }

            return Execute();
        }

        public int Execute()
        {
            return Execute(CreateContext());
        }

        public int Execute(Context context)
        {
            if (!Validate())
            {
                ValidationMessages += "The test suite does not validate.";
                //throw new ApplicationException("The test suite is not in a valid format. It cannot be executed until the errors have been corrected.");
            }

            if (ParentTestSuite == null)
            {
                Statistics.Reset();
            }

            DynamicValues.Apply();

            OnRaiseTestSuiteStarted(new TestSuiteStartedEventArgs());

            try
            {
                ExecuteCommandSet(TestSuiteSetup);
            }
            catch (Exception)
            {
                OnRaiseTestSuiteFailed(new TestSuiteFailedEventArgs(DateTime.Now, "The test suite setup failed."));

                throw;
            }

            foreach (Test test in Tests.Values)
            {
                try
                {
                    test.TestStarted += TestStarted;
                    test.TestCompleted += TestCompleted;

                    test.Execute(context);
                }
                catch (Exception ex)
                {
                    OnRaiseTestCompleted(new TestCompletedEventArgs(DateTime.Now, test.PackageLocation, test.Task, test.Name, ex.Message, false));
                }
                finally
                {
                    test.TestCompleted -= TestCompleted;
                    test.TestStarted -= TestStarted;
                }
            }

            foreach (TestRef testRef in TestRefs.Values)
            {
                // TODO: Add context
                testRef.Execute();
            }

            try
            {
                ExecuteCommandSet(TestSuiteTeardown);
            }
            catch (Exception)
            {
                OnRaiseTestSuiteFailed(new TestSuiteFailedEventArgs(DateTime.Now, "The test suite teardown failed."));

                throw;
            }

            int totalTests = Statistics.GetStatistic(StatisticEnum.TestCount);
            int failedTests = Statistics.GetStatistic(StatisticEnum.TestFailedCount);
            int passedTests = Statistics.GetStatistic(StatisticEnum.TestPassedCount);

            string completionMessage;

            if (failedTests < 1)
                completionMessage = "The test suite has completed successfully, " + (totalTests == 1 ? "1 unit test has passed." : string.Format("{0} unit tests have passed.", totalTests.ToString("N0")));
            else
                completionMessage = string.Format("The test suite has completed with {0} unit test failure{1} and {2} unit test success{3}.", failedTests.ToString("N0"), failedTests == 1 ? string.Empty : "s", passedTests.ToString("N0"), passedTests == 1 ? string.Empty : "es");

            OnRaiseTestSuiteCompleted(new TestSuiteCompletedEventArgs(DateTime.Now, totalTests, failedTests, passedTests, completionMessage, failedTests < 1));

            return Statistics.GetStatistic(StatisticEnum.TestCount);
        }

        public void Execute(SsisTestSuite ssisTestCase)
        {
            ParentTestSuite = ssisTestCase;
            Statistics = ssisTestCase.Statistics;

            Execute();
        }

        #region Helper Functions

        private static Stream GetStreamFromExecutingAssembly(string resourceName)
        {
            Assembly asm = Assembly.GetExecutingAssembly();
#if SQL2005
            Stream resource = asm.GetManifestResourceStream(asm.GetName().Name + "." + resourceName);
#elif SQL2014 || SQL2008 || SQL2012
            Stream resource = asm.GetManifestResourceStream("SsisUnit." + resourceName);
#endif
            return resource;
        }

        private void InitializeTestCase(string testCaseFile)
        {
            _testCaseDoc = LoadTestXmlFromFile(testCaseFile);
            CommonSetup();
        }

        private void CommonSetup()
        {
            try
            {
                _namespaceMgr = new XmlNamespaceManager(_testCaseDoc.NameTable);
                _namespaceMgr.AddNamespace("SsisUnit", "http://tempuri.org/SsisUnit.xsd");

                var xmlPackageReferences = _testCaseDoc.SelectNodes("SsisUnit:TestSuite/SsisUnit:PackageList/SsisUnit:Package", _namespaceMgr);

                if (xmlPackageReferences != null)
                {
                    foreach (XmlNode pkgRef in xmlPackageReferences)
                    {
                        if (pkgRef.Attributes == null)
                            continue;

                        PackageList.Add(pkgRef.Attributes["name"].Value, new PackageRef(pkgRef));
                    }
                }

                ConnectionList = _testCaseDoc.DocumentElement != null ? LoadConnectionRefs(_testCaseDoc.DocumentElement["ConnectionList"]) : new Dictionary<string, ConnectionRef>();

                var xmlDatasets = _testCaseDoc.SelectNodes("SsisUnit:TestSuite/SsisUnit:DatasetList/SsisUnit:Dataset", _namespaceMgr);

                if (xmlDatasets != null)
                {
                    foreach (XmlNode xmlDataset in xmlDatasets)
                    {
                        if (xmlDataset.Attributes == null)
                            continue;

                        Datasets.Add(xmlDataset.Attributes["name"].Value, new Dataset(this, xmlDataset));
                    }
                }

                TestSuiteSetup = _testCaseDoc.DocumentElement != null ? new CommandSet("Test Suite Setup", this, _testCaseDoc.DocumentElement["TestSuiteSetup"]) : new CommandSet(this);
                TestSuiteTeardown = _testCaseDoc.DocumentElement != null ? new CommandSet("Test Suite Teardown", this, _testCaseDoc.DocumentElement["TestSuiteTeardown"]) : new CommandSet(this);
                SetupCommands = _testCaseDoc.DocumentElement != null ? new CommandSet("Unit Test Setup", this, _testCaseDoc.DocumentElement["Setup"]) : new CommandSet(this);
                TeardownCommands = _testCaseDoc.DocumentElement != null ? new CommandSet("Unit Test Teardown", this, _testCaseDoc.DocumentElement["Teardown"]) : new CommandSet(this);

                var xmlTests = _testCaseDoc.SelectNodes("SsisUnit:TestSuite/SsisUnit:Tests/SsisUnit:Test", _namespaceMgr);

                if (xmlTests != null)
                {
                    foreach (XmlNode test in xmlTests)
                    {
                        if (test.Attributes == null)
                            continue;

                        Test newTest = new Test(this, test);

                        newTest.CommandCompleted += OnRaiseCommandCompleted;
                        newTest.CommandFailed += OnRaiseCommandFailed;
                        newTest.CommandStarted += OnRaiseCommandStarted;

                        Tests.Add(test.Attributes["name"].Value, newTest);
                    }
                }

                var xmlTestReferences = _testCaseDoc.SelectNodes("SsisUnit:TestSuite/SsisUnit:Tests/SsisUnit:TestRef", _namespaceMgr);

                if (xmlTestReferences != null)
                {
                    foreach (XmlNode testRef in xmlTestReferences)
                    {
                        if (testRef.Attributes == null)
                            continue;

                        TestRefs.Add(testRef.Attributes["path"].Value, new TestRef(this, testRef));
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException(string.Format("The unit test file is malformed or corrupt. Please verify that the file format conforms to the ssisUnit schema, provided in the SsisUnit.xsd file."), ex);
            }
        }

        private void ExecuteCommandSet(CommandSet commandSet)
        {
            if (commandSet == null)
                throw new ArgumentNullException("commandSet");

            try
            {
                commandSet.CommandStarted += OnRaiseCommandStarted;
                commandSet.CommandCompleted += OnRaiseCommandCompleted;
                commandSet.CommandFailed += OnRaiseCommandFailed;

                commandSet.Execute();
            }
            finally
            {
                commandSet.CommandStarted -= OnRaiseCommandStarted;
                commandSet.CommandCompleted -= OnRaiseCommandCompleted;
                commandSet.CommandFailed -= OnRaiseCommandFailed;
            }
        }

        private Dictionary<string, ConnectionRef> LoadConnectionRefs(XmlNode connections)
        {
            Dictionary<string, ConnectionRef> refs = new Dictionary<string, ConnectionRef>(connections.ChildNodes.Count);

            foreach (XmlNode conn in connections)
            {
                if (conn.Attributes == null)
                    continue;

                refs.Add(conn.Attributes["name"].Value, new ConnectionRef(conn));
            }

            return refs;
        }

        private void InitializeTestCase(Stream testCase)
        {
            _testCaseDoc = LoadTestXmlFromStream(testCase);

            CommonSetup();
        }

        /// <summary>
        /// Loads a test case from an XML file.
        /// </summary>
        /// <param name="fileName">The complete filename (including path) for the test case.</param>
        /// <returns>An xml document</returns>
        static private XmlDocument LoadTestXmlFromFile(string fileName)
        {
            try
            {
                if (fileName == null)
                {
                    return null;
                }

                return LoadTestXmlFromStream(new StreamReader(fileName).BaseStream);
            }
            catch (Exception ex)
            {
                throw new ArgumentException("The test case could not be loaded: " + ex.Message);
            }
        }

        static internal XmlDocument LoadTestXmlFromStream(Stream file)
        {
            if (file == null)
            {
                return null;
            }

            try
            {
                Stream strm = GetStreamFromExecutingAssembly("SsisUnit.xsd");

                XmlReaderSettings settings = new XmlReaderSettings();
                settings.Schemas.Add("http://tempuri.org/SsisUnit.xsd", XmlReader.Create(strm));
                XmlDocument test = new XmlDocument();
                test.Load(XmlReader.Create(file, settings));

                return test;
            }
            catch (XmlSchemaValidationException)
            {
                throw new XmlSchemaValidationException("The test case is not in a valid format. Please ensure that it conforms to the schema.");
            }
            catch (Exception ex)
            {
                throw new ArgumentException("The test case could not be loaded: " + ex.Message);
            }
            finally
            {
                file.Close();
            }
        }

        internal void RunTestSuite(SsisTestSuite ts)
        {
            if (ts == null)
                throw new ArgumentNullException("ts");

            try
            {
                ts.CommandStarted += TestCaseCommandStarted;
                ts.CommandCompleted += TestCaseCommandCompleted;
                ts.CommandFailed += TestCaseCommandFailed;
                ts.SetupCompleted += TestCaseSetupCompleted;
                ts.TestCompleted += TestCaseTestCompleted;
                ts.TeardownCompleted += TestCaseTeardownCompleted;
                ts.AssertCompleted += TestCaseAssertCompleted;

                ts.Execute(this);
            }
            finally
            {
                ts.CommandStarted -= TestCaseCommandStarted;
                ts.CommandCompleted -= TestCaseCommandCompleted;
                ts.CommandFailed -= TestCaseCommandFailed;
                ts.SetupCompleted -= TestCaseSetupCompleted;
                ts.TestCompleted -= TestCaseTestCompleted;
                ts.TeardownCompleted -= TestCaseTeardownCompleted;
                ts.AssertCompleted -= TestCaseAssertCompleted;
            }
        }

        #region Event Handlers

        private void TestCaseCommandCompleted(object sender, CommandCompletedEventArgs e)
        {
            OnRaiseCommandCompleted(sender, e);
        }

        private void TestCaseCommandFailed(object sender, CommandFailedEventArgs e)
        {
            OnRaiseCommandFailed(sender, e);
        }

        private void TestCaseCommandStarted(object sender, CommandStartedEventArgs e)
        {
            OnRaiseCommandStarted(sender, e);
        }

        private void TestCaseTeardownCompleted(object sender, TeardownCompletedEventArgs e)
        {
            OnRaiseTeardownCompleted(e);
        }

        private void TestCaseTestCompleted(object sender, TestCompletedEventArgs e)
        {
            OnRaiseTestCompleted(e);
        }

        private void TestCaseSetupCompleted(object sender, SetupCompletedEventArgs e)
        {
            OnRaiseSetupCompleted(e);
        }

        private void TestCaseAssertCompleted(object sender, AssertCompletedEventArgs e)
        {
            OnRaiseAssertCompleted(e);
        }

        #endregion

        #endregion
    }
}