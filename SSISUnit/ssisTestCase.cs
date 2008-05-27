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
        private Dictionary<string, Test> _tests = new Dictionary<string, Test>();



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

        public Dictionary<string, ConnectionRef> ConnectionRefs
        {
            get { return _connectionRefs; }
        }

        public Dictionary<string, Test> Tests
        {
            get { return _tests; }
        }

        #endregion

        //TODO: Add parameters - replaceable values that can be defined one and used anywhere.
        //TODO: Add creation logic
        #region Events

        public event EventHandler<SetupCompletedEventArgs> SetupCompleted;
        public event EventHandler<TestCompletedEventArgs> TestCompleted;
        public event EventHandler<TeardownCompletedEventArgs> TeardownCompleted;

        protected virtual void OnRaiseTestCompleted(TestCompletedEventArgs e)
        {
            // Make a temporary copy of the event to avoid possibility of
            // a race condition if the last subscriber unsubscribes
            // immediately after the null check and before the event is raised.
            EventHandler<TestCompletedEventArgs> handler = TestCompleted;

            // Event will be null if there are no subscribers
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void OnRaiseSetupCompleted(SetupCompletedEventArgs e)
        {
            // Make a temporary copy of the event to avoid possibility of
            // a race condition if the last subscriber unsubscribes
            // immediately after the null check and before the event is raised.
            EventHandler<SetupCompletedEventArgs> handler = SetupCompleted;

            // Event will be null if there are no subscribers
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void OnRaiseTeardownCompleted(TeardownCompletedEventArgs e)
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

        public int Execute()
        {
            int testCount = 0;

            Setup(_testCaseDoc.SelectSingleNode("SsisUnit:TestSuite/SsisUnit:TestSuiteSetup", _namespaceMgr), null, null);

            foreach (XmlNode test in _testCaseDoc.SelectNodes("SsisUnit:TestSuite/SsisUnit:Tests/SsisUnit:Test", _namespaceMgr))
            {
                this.Test(test);
                testCount++;
            }

            foreach (XmlNode testRef in _testCaseDoc.SelectNodes("SsisUnit:TestSuite/SsisUnit:Tests/SsisUnit:TestRef", _namespaceMgr))
            {
                this.RunTestSuite(testRef);
                testCount++;
            }

            Teardown(_testCaseDoc.SelectSingleNode("SsisUnit:TestSuite/SsisUnit:TestSuiteTeardown", _namespaceMgr), null, null);

            return testCount;
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
            if (setup == null)
            {
                return 0;
            }
            int commandCount = 0;
            foreach (XmlNode command in setup)
            {
                RunCommand(command, pkg, task);

                commandCount += 1;
            }

            return commandCount;
        }

        internal void Setup(Package pkg, DtsContainer task)
        {
            this.Setup(_testCaseDoc.DocumentElement["Setup"], pkg, task);
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

                    if (packageRef.Attributes["storageType"].Value == "File System")
                    {
                        package = _ssisApp.LoadPackage(packageRef.Attributes["packagePath"].Value, null);
                    }
                    else if (packageRef.Attributes["storageType"].Value == "MSDB")
                    {
                        package = _ssisApp.LoadFromSqlServer(packageRef.Attributes["packagePath"].Value, packageRef.Attributes["server"].Value, null, null, null);
                    }
                    else if (packageRef.Attributes["storageType"].Value == "Package Store")
                    {
                        package = _ssisApp.LoadFromDtsServer(packageRef.Attributes["packagePath"].Value, packageRef.Attributes["server"].Value, null);
                    }

                }

                taskHost = FindExecutable(package, taskID);
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

        internal bool Test(XmlNode test)
        {
            bool returnValue = false;

            if (test.Name != "Test")
            {
                throw new ArgumentException("The node passed to the test argument is not a Test element.");
            }

            Package packageToTest = null;
            DtsContainer taskHost = null;

            LoadPackageAndTask(test.Attributes["package"].Value, test.Attributes["task"].Value, ref packageToTest, ref taskHost);

            string setupResults = string.Empty;
            bool setupSucceeded = false;

            try
            {
                //TODO: Decide if this behavior is correct - not sure if we really need to run the parent setups
                if (_parentSuite != null)
                {
                    _parentSuite.Setup(packageToTest, taskHost);
                }
                Setup(test.OwnerDocument.DocumentElement["Setup"], packageToTest, taskHost);
                Setup(test["TestSetup"], packageToTest, taskHost);
                setupResults = "Setup succeeded.";
                setupSucceeded = true;
            }
            catch (Exception ex)
            {
                setupResults = "Setup failed: " + ex.Message + " : " + ex.InnerException;
                setupSucceeded = false;
            }
            finally
            {
                OnRaiseSetupCompleted(new SetupCompletedEventArgs(DateTime.Now, test.Attributes["name"].Value, test.Attributes["package"].Value, test.Attributes["task"].Value, setupResults));
            }

            if (setupSucceeded)
            {
                object validationResult = null;
                string resultMessage = string.Empty;

                try
                {
                    //Run Pre Asserts
                    foreach (XmlNode assert in test.SelectNodes("SsisUnit:Assert[@testBefore='true']", _namespaceMgr))
                    {
                        validationResult = RunCommand(assert.ChildNodes[0], packageToTest, taskHost);
                        returnValue = (assert.Attributes["expectedResult"].Value == validationResult.ToString());

                        if (returnValue)
                        {
                            resultMessage = String.Format(CultureInfo.CurrentCulture, "The actual result ({0}) matched the expected result ({1}).", validationResult.ToString(), assert.Attributes["expectedResult"].Value);
                        }
                        else
                        {
                            resultMessage = String.Format(CultureInfo.CurrentCulture, "The actual result ({0}) did not match the expected result ({1}).", validationResult.ToString(), assert.Attributes["expectedResult"].Value);
                        }
                        OnRaiseTestCompleted(new TestCompletedEventArgs(DateTime.Now, test.Attributes["package"].Value,
                        test.Attributes["task"].Value, test.Attributes["name"].Value, resultMessage, returnValue));
                    }

                    taskHost.Execute(packageToTest.Connections, taskHost.Variables, null, null, null);

                    foreach (XmlNode assert in test.SelectNodes("SsisUnit:Assert[@testBefore='false']", _namespaceMgr))
                    {
                        validationResult = RunCommand(assert.ChildNodes[0], packageToTest, taskHost);
                        returnValue = (assert.Attributes["expectedResult"].Value == validationResult.ToString());

                        if (returnValue)
                        {
                            resultMessage = String.Format(CultureInfo.CurrentCulture, "The actual result ({0}) matched the expected result ({1}).", validationResult.ToString(), assert.Attributes["expectedResult"].Value);
                        }
                        else
                        {
                            resultMessage = String.Format(CultureInfo.CurrentCulture, "The actual result ({0}) did not match the expected result ({1}).", validationResult.ToString(), assert.Attributes["expectedResult"].Value);
                        }
                        OnRaiseTestCompleted(new TestCompletedEventArgs(DateTime.Now, test.Attributes["package"].Value,
                        test.Attributes["task"].Value, test.Attributes["name"].Value, resultMessage, returnValue));
                    }
                    resultMessage = "All asserts were completed.";

                }
                catch (Exception ex)
                {
                    returnValue = false;
                    resultMessage = "Exception occurred: " + ex.Message;
                }
                finally
                {
                    OnRaiseTestCompleted(new TestCompletedEventArgs(DateTime.Now, test.Attributes["package"].Value,
                        test.Attributes["task"].Value, test.Attributes["name"].Value, resultMessage, returnValue));
                }

            }
            if (setupSucceeded)
            {
                string teardownResults = string.Empty;

                try
                {
                    Teardown(test["TestTeardown"], packageToTest, taskHost);
                    Teardown(test.OwnerDocument.DocumentElement["Teardown"], packageToTest, taskHost);
                    if (_parentSuite != null)
                    {
                        _parentSuite.Teardown(packageToTest, taskHost);
                    }
                    teardownResults = "Teardown succeeded.";
                }
                catch (Exception ex)
                {
                    teardownResults = "Teardown failed: " + ex.Message + " : " + ex.InnerException;
                }
                finally
                {
                    OnRaiseTeardownCompleted(new TeardownCompletedEventArgs(DateTime.Now, test.Attributes["name"].Value, test.Attributes["package"].Value, test.Attributes["task"].Value, teardownResults));
                }
            }


            return returnValue;
        }

        internal int Teardown(XmlNode teardown, Package pkg, DtsContainer task)
        {
            if (teardown == null)
            {
                return 0;
            }
            int commandCount = 0;
            foreach (XmlNode command in teardown)
            {
                RunCommand(command, pkg, task);

                commandCount += 1;
            }

            return commandCount;
        }

        internal void Teardown(Package pkg, DtsContainer task)
        {
            this.Teardown(_testCaseDoc.DocumentElement["Teardown"], pkg, task);
        }

        static internal DtsContainer FindExecutable(IDTSSequence parentExecutable, string taskId)
        {

            //TODO: Determine what to do when name is used in mutiple containers, think it just finds the first one now

            DtsContainer matchingExecutable = null;
            DtsContainer parent = (DtsContainer)parentExecutable;

            if (parent.ID == taskId || parent.Name == taskId)
            {
                matchingExecutable = parent;
            }
            else
            {

                if (parentExecutable.Executables.Contains(taskId))
                {
                    matchingExecutable = (TaskHost)parentExecutable.Executables[taskId];
                }
                else
                {
                    foreach (Executable e in parentExecutable.Executables)
                    {
                        if (e is IDTSSequence)
                        {
                            matchingExecutable = FindExecutable((IDTSSequence)e, taskId);
                        }
                    }
                }
            }

            return matchingExecutable;
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
            if (connectionXml.Name!="Connection")
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
}
