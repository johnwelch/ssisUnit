using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Xml;
using Microsoft.SqlServer.Dts.Runtime;
using System.Text;

[assembly: InternalsVisibleTo("UTssisUnit")]
[assembly: InternalsVisibleTo("UTssisUnit_SQL2008")]
namespace SsisUnit
{
    public class SsisTestSuite : IssisTestSuite
    {
        private XmlDocument _testCaseDoc;
        private XmlNode _connections;
        private XmlNode _packageList;
        private Application _ssisApp = new Application();
        private XmlNamespaceManager _namespaceMgr;
        private SsisTestSuite _parentSuite;
        private Dictionary<string, CommandBase> _commands = new Dictionary<string, CommandBase>(3);
        private Dictionary<string, ConnectionRef> _connectionRefs = new Dictionary<string, ConnectionRef>();
        private Dictionary<string, PackageRef> _packageRefs = new Dictionary<string, PackageRef>();
        private Dictionary<string, Test> _tests = new Dictionary<string, Test>();
        private CommandSet _testSuiteSetup;
        private CommandSet _testSuiteTeardown;
        private CommandSet _setup;
        private CommandSet _teardown;
        private TestSuiteStatistics _stats = new TestSuiteStatistics();

        #region Constructors

        public SsisTestSuite()
        {
            Stream baseTest = GetStreamFromAssembly("BaseTest.xml");
            Initialize(baseTest);
        }

        public SsisTestSuite(string testCaseFile)
        {
            InitializeTestCase(testCaseFile);
            LoadCommands();
        }

        public SsisTestSuite(Stream testCase)
        {
            Initialize(testCase);
        }

        #endregion

        #region Properties

        public TestSuiteStatistics Statistics
        {
            get { return _stats; }
        }

        public Dictionary<string, ConnectionRef> ConnectionRefs
        {
            get { return _connectionRefs; }
        }

        public Dictionary<string, Test> Tests
        {
            get { return _tests; }
        }

        public Dictionary<string, PackageRef> PackageRefs
        {
            get { return _packageRefs; }
        }

        public CommandSet TestSuiteSetup
        {
            get { return _testSuiteSetup; }
        }

        public CommandSet TestSuiteTeardown
        {
            get { return _testSuiteTeardown; }
        }

        public CommandSet SetupCommands
        {
            get { return _setup; }
        }

        public CommandSet TeardownCommands
        {
            get { return _teardown; }
        }

        internal Application SsisApplication
        {
            get { return _ssisApp; }
        }

        internal SsisTestSuite ParentTestSuite
        {
            get { return _parentSuite; }
        }

        #endregion

        //TODO: Add parameters - replaceable values that can be defined one and used anywhere.
        //TODO: Add creation logic

        #region Events

        public event EventHandler<SetupCompletedEventArgs> SetupCompleted;
        public event EventHandler<TestCompletedEventArgs> TestCompleted;
        public event EventHandler<AssertCompletedEventArgs> AssertCompleted;
        public event EventHandler<TeardownCompletedEventArgs> TeardownCompleted;

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

        #region ISSISTestCase Members

        public void Setup()
        {
            this.Setup(null, null);
        }

        public void Test()
        {
            throw new NotImplementedException();
        }

        public void Teardown()
        {
            this.Teardown(null, null);
        }

        #endregion

        public string PersistToXml()
        {
            StringBuilder xml = new StringBuilder();
            //xml.Append("<Package ");
            //xml.Append("name=\"" + _name + "\" ");
            //xml.Append("packagePath=\"" + _packagePath + "\" ");
            //if (_server != string.Empty)
            //{
            //    xml.Append("server=\"" + _server + "\" ");
            //}
            //xml.Append("storageType=\"" + this.StorageType.ToString() + "\"");
            //xml.Append("/>");
            return xml.ToString();
        }

        public void LoadFromXml(string packageXml)
        {
            LoadFromXml(Helper.GetXmlNodeFromString(packageXml));
        }

        public void LoadFromXml(XmlNode packageXml)
        {
            //if (packageXml.Name != "Package")
            //{
            //    throw new ArgumentException(string.Format("The Xml does not contain the correct type ({0}).", "Package"));
            //}

            //_packagePath = packageXml.Attributes["packagePath"].Value;
            //_storageType = packageXml.Attributes["storageType"].Value;
            //_name = packageXml.Attributes["name"].Value;
            //_server = packageXml.Attributes["server"].Value;
        }
        
        public int Execute()
        {
            _testSuiteSetup.Execute();

            foreach (Test test in this.Tests.Values)
            {
                test.Execute();
            }

            foreach (XmlNode testRef in _testCaseDoc.SelectNodes("SsisUnit:TestSuite/SsisUnit:Tests/SsisUnit:TestRef", _namespaceMgr))
            {
                this.RunTestSuite(testRef);
            }

            _testSuiteTeardown.Execute();

            return _stats.GetStatistic(TestSuiteStatistics.StatisticEnum.TestCount);
        }

        public void Execute(SsisTestSuite ssisTestCase)
        {
            _parentSuite = ssisTestCase;
            this.Execute();
        }

        #region Helper Functions

        private void Initialize(Stream testCase)
        {
            InitializeTestCase(testCase);
            LoadCommands();
        }

        private static Stream GetStreamFromAssembly(string resourceName)
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            Stream resource = asm.GetManifestResourceStream(asm.GetName().Name + "." + resourceName);
            return resource;
        }


        private void InitializeTestCase(string testCaseFile)
        {
            _testCaseDoc = LoadTestXmlFromFile(testCaseFile);
            CommonSetup();
        }

        private void CommonSetup()
        {
            _namespaceMgr = new XmlNamespaceManager(_testCaseDoc.NameTable);
            _namespaceMgr.AddNamespace("SsisUnit", "http://tempuri.org/SsisUnit.xsd");
            _connections = _testCaseDoc.DocumentElement["ConnectionList"];
            _packageList = _testCaseDoc.DocumentElement["PackageList"];
            _connectionRefs = LoadConnectionRefs(_connections);
            _testSuiteSetup = new CommandSet(this, _testCaseDoc.DocumentElement["TestSuiteSetup"]);
            _testSuiteTeardown = new CommandSet(this, _testCaseDoc.DocumentElement["TestSuiteTeardown"]);
            _setup = new CommandSet(this, _testCaseDoc.DocumentElement["Setup"]);
            _teardown = new CommandSet(this, _testCaseDoc.DocumentElement["Teardown"]);

            foreach (XmlNode pkgRef in _testCaseDoc.SelectNodes("SsisUnit:TestSuite/SsisUnit:PackageList/SsisUnit:Package", _namespaceMgr))
            {
                _packageRefs.Add(pkgRef.Attributes["name"].Value, new PackageRef(pkgRef));
            }

            foreach (XmlNode test in _testCaseDoc.SelectNodes("SsisUnit:TestSuite/SsisUnit:Tests/SsisUnit:Test", _namespaceMgr))
            {
                _tests.Add(test.Attributes["name"].Value, new Test(this, test));
            }

        }

        private Dictionary<string, ConnectionRef> LoadConnectionRefs(XmlNode connections)
        {
            Dictionary<string, ConnectionRef> refs = new Dictionary<string, ConnectionRef>(connections.ChildNodes.Count);

            foreach (XmlNode conn in connections)
            {
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
        static internal XmlDocument LoadTestXmlFromFile(string fileName)
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
            try
            {
                if (file == null)
                {
                    return null;
                }
                Assembly asm = Assembly.GetExecutingAssembly();
                Stream strm = asm.GetManifestResourceStream(asm.GetName().Name + ".SsisUnit.xsd");


                XmlReaderSettings settings = new XmlReaderSettings();
                settings.Schemas.Add("http://tempuri.org/SsisUnit.xsd", XmlReader.Create(strm));
                settings.ValidationType = ValidationType.Schema;

                XmlDocument test = new XmlDocument();
                test.Load(XmlReader.Create(file, settings));
                if (test.SchemaInfo.Validity != System.Xml.Schema.XmlSchemaValidity.Valid)
                {
                    throw new ArgumentException(String.Format("The SsisUnit schema ({0}) was not specified in this XML document.", "http://tempuri.org/SsisUnit.xsd"));
                }

                return test;
            }
            catch (System.Xml.Schema.XmlSchemaValidationException)
            {
                throw new System.Xml.Schema.XmlSchemaValidationException("The test case is not in a valid format. Please ensure that it conforms to the schema.");
            }
            catch (Exception ex)
            {
                throw new ArgumentException("The test case could not be loaded: " + ex.Message);
            }
        }

        internal int Setup(XmlNode setup, Package pkg, DtsContainer task)
        {
            //if (setup == null)
            //{
            //    return 0;
            //}
            //int commandCount = 0;
            //foreach (XmlNode command in setup)
            //{
            //    RunCommand(command, pkg, task);

            //    commandCount += 1;
            //}

            //return commandCount;

            return this.SetupCommands.Execute(pkg, task);
        }

        internal int Setup(Package pkg, DtsContainer task)
        {
            return this.SetupCommands.Execute(pkg, task);
        }

        private void LoadCommands()
        {
            foreach (Type t in System.Reflection.Assembly.GetExecutingAssembly().GetTypes())
            {
                if (typeof(CommandBase).IsAssignableFrom(t)
                    && (!object.ReferenceEquals(t, typeof(CommandBase)))
                    && (!t.IsAbstract))
                {
                    CommandBase command;
                    System.Type[] @params = { typeof(SsisTestSuite) };
                    System.Reflection.ConstructorInfo con;

                    con = t.GetConstructor(@params);
                    if (con == null)
                    {
                        throw new ApplicationException(String.Format(CultureInfo.CurrentCulture, "The Command type {0} could not be loaded because it has no constructor.", t.Name));
                    }
                    command = (CommandBase)con.Invoke(new object[] { this });
                    _commands.Add(command.CommandName, command);
                }
            }
        }

        private object RunCommand(XmlNode command, Package pkg, DtsContainer task)
        {
            object returnValue = null;

            returnValue = _commands[command.Name].Execute(command, pkg, task);

            return returnValue;
        }

        internal void RunTestSuite(XmlNode test)
        {
            SsisTestSuite testCase = new SsisTestSuite(test.Attributes["path"].Value);
            testCase.SetupCompleted += new EventHandler<SetupCompletedEventArgs>(testCase_SetupCompleted);
            testCase.TestCompleted += new EventHandler<TestCompletedEventArgs>(testCase_TestCompleted);
            testCase.TeardownCompleted += new EventHandler<TeardownCompletedEventArgs>(testCase_TeardownCompleted);
            testCase.Execute(this);
        }

        #region Event Handlers

        void testCase_TeardownCompleted(object sender, TeardownCompletedEventArgs e)
        {
            OnRaiseTeardownCompleted(e);
        }

        void testCase_TestCompleted(object sender, TestCompletedEventArgs e)
        {
            OnRaiseTestCompleted(e);
        }

        void testCase_SetupCompleted(object sender, SetupCompletedEventArgs e)
        {
            OnRaiseSetupCompleted(e);
        }

        #endregion

        private void LoadPackageAndTask(string packagePath, string taskID, ref Package package, ref DtsContainer taskHost)
        {
            string pathToPackage = string.Empty;

            try
            {
                if (packagePath.Contains("\\"))
                {
                    //Assume that it is a file path.
                    package = _ssisApp.LoadPackage(packagePath, null);
                }
                else
                {
                    //PackageList Reference
                    XmlNode packageRef = this._packageList.SelectSingleNode("SsisUnit:Package[@name='" + packagePath + "']", this._namespaceMgr);
                    if (packageRef == null)
                    {
                        throw new ArgumentException(String.Format(CultureInfo.CurrentCulture, "The package attribute is {0}, which does not reference a valid package.", packagePath));
                    }

                    if (packageRef.Attributes["storageType"].Value == "FileSystem")
                    {
                        package = _ssisApp.LoadPackage(packageRef.Attributes["packagePath"].Value, null);
                    }
                    else if (packageRef.Attributes["storageType"].Value == "MSDB")
                    {
                        package = _ssisApp.LoadFromSqlServer(packageRef.Attributes["packagePath"].Value, packageRef.Attributes["server"].Value, null, null, null);
                    }
                    else if (packageRef.Attributes["storageType"].Value == "PackageStore")
                    {
                        package = _ssisApp.LoadFromDtsServer(packageRef.Attributes["packagePath"].Value, packageRef.Attributes["server"].Value, null);
                    }

                }

                taskHost = Helper.FindExecutable(package, taskID);
                if (taskHost == null)
                {
                    throw new Exception("The task host was not found.");
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException(string.Format("The package path ({0}) or the task host ({1}) is not valid." + ex.Message, packagePath, taskID));
            }
        }

        //internal bool Test(XmlNode test)
        //{
        //    bool returnValue = false;

        //    if (test.Name != "Test")
        //    {
        //        throw new ArgumentException("The node passed to the test argument is not a Test element.");
        //    }

        //    Package packageToTest = null;
        //    DtsContainer taskHost = null;

        //    LoadPackageAndTask(test.Attributes["package"].Value, test.Attributes["task"].Value, ref packageToTest, ref taskHost);

        //    string setupResults = string.Empty;
        //    bool setupSucceeded = false;

        //    try
        //    {
        //        //TODO: Decide if this behavior is correct - not sure if we really need to run the parent setups
        //        if (_parentSuite != null)
        //        {
        //            _parentSuite.Setup(packageToTest, taskHost);
        //        }
        //        Setup(test.OwnerDocument.DocumentElement["Setup"], packageToTest, taskHost);
        //        Setup(test["TestSetup"], packageToTest, taskHost);
        //        setupResults = "Setup succeeded.";
        //        setupSucceeded = true;
        //    }
        //    catch (Exception ex)
        //    {
        //        setupResults = "Setup failed: " + ex.Message + " : " + ex.InnerException;
        //        setupSucceeded = false;
        //    }
        //    finally
        //    {
        //        OnRaiseSetupCompleted(new SetupCompletedEventArgs(DateTime.Now, test.Attributes["name"].Value, test.Attributes["package"].Value, test.Attributes["task"].Value, setupResults));
        //    }

        //    if (setupSucceeded)
        //    {
        //        object validationResult = null;
        //        string resultMessage = string.Empty;

        //        try
        //        {
        //            //Run Pre Asserts
        //            foreach (XmlNode assert in test.SelectNodes("SsisUnit:Assert[@testBefore='true']", _namespaceMgr))
        //            {
        //                validationResult = RunCommand(assert.ChildNodes[0], packageToTest, taskHost);
        //                returnValue = (assert.Attributes["expectedResult"].Value == validationResult.ToString());

        //                if (returnValue)
        //                {
        //                    resultMessage = String.Format(CultureInfo.CurrentCulture, "The actual result ({0}) matched the expected result ({1}).", validationResult.ToString(), assert.Attributes["expectedResult"].Value);
        //                }
        //                else
        //                {
        //                    resultMessage = String.Format(CultureInfo.CurrentCulture, "The actual result ({0}) did not match the expected result ({1}).", validationResult.ToString(), assert.Attributes["expectedResult"].Value);
        //                }
        //                OnRaiseTestCompleted(new TestCompletedEventArgs(DateTime.Now, test.Attributes["package"].Value,
        //                test.Attributes["task"].Value, test.Attributes["name"].Value, resultMessage, returnValue));
        //            }

        //            taskHost.Execute(packageToTest.Connections, taskHost.Variables, null, null, null);

        //            foreach (XmlNode assert in test.SelectNodes("SsisUnit:Assert[@testBefore='false']", _namespaceMgr))
        //            {
        //                validationResult = RunCommand(assert.ChildNodes[0], packageToTest, taskHost);
        //                returnValue = (assert.Attributes["expectedResult"].Value == validationResult.ToString());

        //                if (returnValue)
        //                {
        //                    resultMessage = String.Format(CultureInfo.CurrentCulture, "The actual result ({0}) matched the expected result ({1}).", validationResult.ToString(), assert.Attributes["expectedResult"].Value);
        //                }
        //                else
        //                {
        //                    resultMessage = String.Format(CultureInfo.CurrentCulture, "The actual result ({0}) did not match the expected result ({1}).", validationResult.ToString(), assert.Attributes["expectedResult"].Value);
        //                }
        //                OnRaiseTestCompleted(new TestCompletedEventArgs(DateTime.Now, test.Attributes["package"].Value,
        //                test.Attributes["task"].Value, test.Attributes["name"].Value, resultMessage, returnValue));
        //            }
        //            resultMessage = "All asserts were completed.";

        //        }
        //        catch (Exception ex)
        //        {
        //            returnValue = false;
        //            resultMessage = "Exception occurred: " + ex.Message;
        //        }
        //        finally
        //        {
        //            OnRaiseTestCompleted(new TestCompletedEventArgs(DateTime.Now, test.Attributes["package"].Value,
        //                test.Attributes["task"].Value, test.Attributes["name"].Value, resultMessage, returnValue));
        //        }

        //    }
        //    if (setupSucceeded)
        //    {
        //        string teardownResults = string.Empty;

        //        try
        //        {
        //            Teardown(test["TestTeardown"], packageToTest, taskHost);
        //            Teardown(test.OwnerDocument.DocumentElement["Teardown"], packageToTest, taskHost);
        //            if (_parentSuite != null)
        //            {
        //                _parentSuite.Teardown(packageToTest, taskHost);
        //            }
        //            teardownResults = "Teardown succeeded.";
        //        }
        //        catch (Exception ex)
        //        {
        //            teardownResults = "Teardown failed: " + ex.Message + " : " + ex.InnerException;
        //        }
        //        finally
        //        {
        //            OnRaiseTeardownCompleted(new TeardownCompletedEventArgs(DateTime.Now, test.Attributes["name"].Value, test.Attributes["package"].Value, test.Attributes["task"].Value, teardownResults));
        //        }
        //    }


        //    return returnValue;
        //}

        internal int Teardown(XmlNode teardown, Package pkg, DtsContainer task)
        {
            //if (teardown == null)
            //{
            //    return 0;
            //}
            //int commandCount = 0;
            //foreach (XmlNode command in teardown)
            //{
            //    RunCommand(command, pkg, task);

            //    commandCount += 1;
            //}

            //return commandCount;
            return this.TeardownCommands.Execute(pkg, task);
        }

        internal int Teardown(Package pkg, DtsContainer task)
        {
            return this.TeardownCommands.Execute(pkg, task);
        }



        #endregion

    }



    public class SetupCompletedEventArgs : EventArgs
    {
        public SetupCompletedEventArgs(DateTime setupExecutionTime, string testName, string package, string taskId, string results)
        {
            this.testName = testName;
            this.setupExecutionTime = setupExecutionTime;
            this.taskGuid = taskId;
            this.package = package;
            this.results = results;
        }

        private DateTime setupExecutionTime;
        private string testName;
        private string taskGuid;
        private string package;
        private string results;

        public DateTime SetupExecutionTime
        {
            get { return setupExecutionTime; }
        }

        public string Results
        {
            get { return results; }
        }

        public string Package
        {
            get { return package; }
        }
        public string TestName
        {
            get { return testName; }
        }
        public string TaskGuid
        {
            get { return taskGuid; }
        }
    }

    public class TestCompletedEventArgs : EventArgs
    {
        public TestCompletedEventArgs(DateTime testExecutionTime, string packageName, string taskName, string testName, string testResultMsg, bool testPassed)
        {
            this.testResult = new TestResult(testExecutionTime, packageName, taskName, testName, testResultMsg, testPassed);
        }
        public TestCompletedEventArgs(TestResult testResult)
        {
            this.testResult = testResult;
        }

        private TestResult testResult;

        public TestResult TestExecResult
        {
            get { return testResult; }
        }

    }

    public class AssertCompletedEventArgs : EventArgs
    {
        public AssertCompletedEventArgs(DateTime testExecutionTime, string packageName, string taskName, string testName, string testResultMsg, bool testPassed)
        {
            this.testResult = new TestResult(testExecutionTime, packageName, taskName, testName, testResultMsg, testPassed);
        }
        public AssertCompletedEventArgs(TestResult testResult)
        {
            this.testResult = testResult;
        }

        private TestResult testResult;

        public TestResult TestExecResult
        {
            get { return testResult; }
        }

    }

    public class TeardownCompletedEventArgs : EventArgs
    {
        public TeardownCompletedEventArgs(DateTime teardownExecutionTime, string testName, string package, string taskId, string results)
        {
            this.testName = testName;
            this.teardownExecutionTime = teardownExecutionTime;
            this.taskGuid = taskId;
            this.package = package;
            this.results = results;
        }

        private DateTime teardownExecutionTime;
        private string testName;
        private string taskGuid;
        private string package;
        private string results;

        public DateTime TeardownExecutionTime
        {
            get { return teardownExecutionTime; }
        }

        public string Results
        {
            get { return results; }
        }

        public string Package
        {
            get { return package; }
        }

        public string TestName
        {
            get { return testName; }
        }
        public string TaskGuid
        {
            get { return taskGuid; }
        }
    }

    public class TestResult
    {
        public TestResult(DateTime testExecutionTime, string packageName, string taskName, string testName, string testResultMsg, bool testPassed)
        {
            this.testName = testName;
            this.packageName = packageName;
            this.taskName = taskName;
            this.testResultMsg = testResultMsg;
            this.testExecutionTime = testExecutionTime;
            this.testPassed = testPassed;
        }

        private DateTime testExecutionTime;
        private string packageName;
        private string taskName;
        private string testName;
        private string testResultMsg;
        private bool testPassed;

        public DateTime TestExecutionTime
        {
            get { return testExecutionTime; }
        }

        public string TestName
        {
            get { return testName; }
        }
        public string PackageName
        {
            get { return packageName; }
        }
        public string TaskName
        {
            get { return taskName; }
        }
        public string TestResultMsg
        {
            get { return testResultMsg; }
        }
        public bool TestPassed
        {
            get { return testPassed; }
        }
    }

    public class ConnectionRef
    {
        private string _referenceName;
        private string _connectionString;
        private string _connectionType;

        public ConnectionRef(string referenceName, string connectionString, ConnectionTypeEnum connectionType)
        {
            _referenceName = referenceName;
            _connectionString = connectionString;
            _connectionType = connectionType.ToString();
            return;
        }

        public ConnectionRef(XmlNode connectionRef)
        {
            LoadFromXml(connectionRef);
            return;
        }

        public string ConnectionString
        {
            get { return _connectionString; }
            set { _connectionString = value; }
        }

        public string ReferenceName
        {
            get { return _referenceName; }
            set { _referenceName = value; }
        }

        public ConnectionTypeEnum ConnectionType
        {
            get { return ConvertConnectionTypeString(_connectionType); }
            set { value.ToString(); }
        }

        private static ConnectionTypeEnum ConvertConnectionTypeString(string type)
        {
            if (type == "ConnectionManager")
            {
                return ConnectionTypeEnum.ConnectionManager;
            }
            else if (type == "ConnectionString")
            {
                return ConnectionTypeEnum.ConnectionString;
            }
            else
            {
                throw new ArgumentException(String.Format("The provided connection type ({0}) is not recognized.", type));
            }
        }

        public string PersistToXml()
        {
            StringBuilder xml = new StringBuilder();
            xml.Append("<Connection ");
            xml.Append("name=\"" + this.ReferenceName + "\" ");
            xml.Append("connection=\"" + this.ConnectionString + "\" ");
            xml.Append("connectionType=\"" + this.ConnectionType.ToString() + "\"");
            xml.Append("/>");
            return xml.ToString();
        }

        public void LoadFromXml(string connectionXml)
        {
            XmlDocument doc = new XmlDocument();

            XmlDocumentFragment frag = doc.CreateDocumentFragment();
            frag.InnerXml = connectionXml;

            if (frag["Connection"] == null)
            {
                throw new ArgumentException(string.Format("The Xml does not contain the correct type ({0}).", "Connection"));
            }
            LoadFromXml(frag["Connection"]);
        }

        public void LoadFromXml(XmlNode connectionXml)
        {
            if (connectionXml.Name != "Connection")
            {
                throw new ArgumentException(string.Format("The Xml does not contain the correct type ({0}).", "Connection"));
            }

            _connectionString = connectionXml.Attributes["connection"].Value;
            _connectionType = connectionXml.Attributes["connectionType"].Value;
            _referenceName = connectionXml.Attributes["name"].Value;
        }

        public enum ConnectionTypeEnum : int
        {
            ConnectionManager = 0,
            ConnectionString = 1
        }
    }

    public class TestSuiteStatistics
    {
        private Dictionary<StatisticEnum, TestSuiteStatistic> _statistics = new Dictionary<StatisticEnum, TestSuiteStatistic>(6);

        internal TestSuiteStatistics()
        {
            _statistics.Add(StatisticEnum.TestCount, new TestSuiteStatistic(StatisticEnum.TestCount.ToString()));
            _statistics.Add(StatisticEnum.AssertCount, new TestSuiteStatistic(StatisticEnum.AssertCount.ToString()));
            _statistics.Add(StatisticEnum.AssertPassedCount, new TestSuiteStatistic(StatisticEnum.AssertPassedCount.ToString()));
            _statistics.Add(StatisticEnum.AssertFailedCount, new TestSuiteStatistic(StatisticEnum.AssertFailedCount.ToString()));
            _statistics.Add(StatisticEnum.TestPassedCount, new TestSuiteStatistic(StatisticEnum.TestPassedCount.ToString()));
            _statistics.Add(StatisticEnum.TestFailedCount, new TestSuiteStatistic(StatisticEnum.TestFailedCount.ToString()));
        }

        #region Methods

        internal void IncrementStatistic(StatisticEnum statistic)
        {
            _statistics[statistic].IncrementValue();
        }

        public int GetStatistic(StatisticEnum statistic)
        {
            return _statistics[statistic].Value;
        }

        #endregion

        public enum StatisticEnum : int
        {
            TestCount = 0,
            AssertCount = 1,
            TestPassedCount = 2,
            TestFailedCount = 3,
            AssertPassedCount = 4,
            AssertFailedCount = 5
        }

        private class TestSuiteStatistic
        {
            private string _name;
            private int _value;

            internal TestSuiteStatistic(string name)
            {
                _name = name;
            }

            public string Name { get { return _name; } }
            public int Value { get { return _value; } }

            public void IncrementValue()
            {
                _value++;
            }
        }
    }

    
}
