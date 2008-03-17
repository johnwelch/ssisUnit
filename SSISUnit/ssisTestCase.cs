using System;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using System.Runtime.CompilerServices;
using System.Reflection;
using System.Data.Common;
using System.Diagnostics;
using System.Globalization;
using Microsoft.SqlServer.Dts.Runtime;

[assembly: InternalsVisibleTo("UTssisUnit")]
namespace SsisUnit
{
    public class SsisTestSuite : IssisTestSuite
    {
        private const string TAG_OLEDB = "OLEDB";
        private const string TAG_SQL = "SqlClient";
        private const string FACTORY_OLEDB = "System.Data.OleDb";
        private const string FACTORY_SQL = "System.Data.SqlClient";
        private XmlDocument _testCaseDoc;
        private XmlNode _connections;
        private Application _ssisApp = new Application();
        private XmlNamespaceManager _namespaceMgr;
        private SsisTestSuite _parentSuite;
        private Dictionary<string, CommandBase> _commands = new Dictionary<string, CommandBase>(3);

        public SsisTestSuite(string testCaseFile)
        {
            InitializeTestCase(testCaseFile);

            LoadCommands();
        }


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

        public void Execute()
        {
            foreach (XmlNode test in _testCaseDoc.SelectNodes("SsisUnit:TestSuite/SsisUnit:Tests/SsisUnit:Test", _namespaceMgr))
            {
                this.Test(test);
            }

            foreach (XmlNode testRef in _testCaseDoc.SelectNodes("SsisUnit:TestSuite/SsisUnit:Tests/SsisUnit:TestRef", _namespaceMgr))
            {
                this.RunTestSuite(testRef);
            }
        }

        public void Execute(SsisTestSuite ssisTestCase)
        {
            _parentSuite = ssisTestCase;

            this.Execute();
        }



        #region Helper Functions

        private void InitializeTestCase(string testCaseFile)
        {
            _testCaseDoc = LoadTestXmlFromFile(testCaseFile);
            _namespaceMgr = new XmlNamespaceManager(_testCaseDoc.NameTable);
            _namespaceMgr.AddNamespace("SsisUnit", "http://tempuri.org/SsisUnit.xsd");
            _connections = _testCaseDoc.DocumentElement["ConnectionList"];
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
                Assembly asm = Assembly.GetExecutingAssembly();
                Stream strm = asm.GetManifestResourceStream(asm.GetName().Name + ".SsisUnit.xsd");

                XmlReaderSettings settings = new XmlReaderSettings();
                settings.Schemas.Add("http://tempuri.org/SsisUnit.xsd", XmlReader.Create(strm));
                settings.ValidationType = ValidationType.Schema;

                XmlDocument test = new XmlDocument();
                test.Load(XmlReader.Create(new StreamReader(fileName), settings));
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
                    System.Type[] @params = { typeof(XmlNode), typeof(XmlNamespaceManager) };
                    System.Reflection.ConstructorInfo con;

                    con = t.GetConstructor(@params);
                    if (con == null)
                    {
                        throw new ApplicationException(String.Format(CultureInfo.CurrentCulture, "The Command type {0} could not be loaded because it has no constructor.", t.Name));
                    }
                    command = (CommandBase)con.Invoke(new object[] { _connections, _namespaceMgr });
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
            try
            {
                package = _ssisApp.LoadPackage(packagePath, null);
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
                if (_parentSuite != null)
                {
                    _parentSuite.Setup(packageToTest, taskHost);
                }
                Setup(test.OwnerDocument.DocumentElement["Setup"], packageToTest, taskHost);
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
                    if (_parentSuite != null)
                    {
                        _parentSuite.Teardown(packageToTest, taskHost);
                    }
                    Teardown(test.OwnerDocument.DocumentElement["Teardown"], packageToTest, taskHost);
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

        //internal object RunSQLCommand(XmlNode command)
        //{
        //    string provider = string.Empty;
        //    object result = null;

        //    if (command.Name != "SqlCommand")
        //    {
        //        throw new ArgumentException("The node passed to the command argument is not a SqlCommand element.");
        //    }

        //    XmlNode connection = _connections.SelectSingleNode("ssisUnit:Connection[@name='" + command.Attributes["connectionRef"].Value + "']", _namespaceMgr);
        //    if (connection == null)
        //    {
        //        throw new ArgumentException(String.Format(CultureInfo.CurrentCulture, "The connectionRef attribute is {0}, which does not reference a valid connection.", command.Attributes["connectionRef"].Value));
        //    }

        //    using (DbCommand dbCommand = GetCommand(connection, command.InnerText))
        //    {
        //        dbCommand.Connection.Open();
        //        if (command.Attributes["returnsValue"].Value == "true")
        //        {
        //            result = dbCommand.ExecuteScalar();
        //        }
        //        else
        //        {
        //            dbCommand.ExecuteNonQuery();
        //        }
        //        dbCommand.Connection.Close();
        //    }
        //    return result;
        //}

        //internal int RunProcessCommand(XmlNode command)
        //{
        //    int exitCode;

        //    if (command.Name != "ProcessCommand")
        //    {
        //        throw new ArgumentException("The node passed to the command argument is not a ProcessCommand element.");
        //    }
        //    Process proc = null;
        //    try
        //    {
        //        XmlNode args = command.Attributes.GetNamedItem("arguments");
        //        if (args == null)
        //        {
        //            proc = Process.Start(command.Attributes["process"].Value);
        //        }
        //        else
        //        {
        //            proc = Process.Start(command.Attributes["process"].Value, command.Attributes["arguments"].Value);
        //        }
        //        while (!proc.WaitForExit(ssisUnit.Default.ProcessCheckForExitDelay))
        //        {
        //            if (proc.StartTime.AddSeconds(ssisUnit.Default.ProcessTimeout).CompareTo(DateTime.Now) < 0)
        //            {
        //                try
        //                {
        //                    proc.CloseMainWindow();
        //                }
        //                catch (InvalidOperationException)
        //                {
        //                    break;
        //                }
        //            }
        //        }

        //        exitCode = proc.ExitCode;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new ArgumentException("The RunProcessNode contained an invalid command or process.", ex);
        //    }
        //    finally
        //    {
        //        proc.Close();
        //    }

        //    return exitCode;

        //}

        //internal object RunVariableCommand(XmlNode command, VariableDispenser dispenser)
        //{
        //    object returnValue;
        //    Variables vars = null;

        //    if (command.Name != "VariableCommand")
        //    {
        //        throw new ArgumentException("The node passed to the command argument is not a VariableCommand element.");
        //    }

        //    string varName = command.Attributes["name"].Value;

        //    if (command.Attributes["value"] == null)
        //    {
        //        dispenser.LockOneForRead(varName, ref vars);
        //        returnValue = vars[varName].Value;
        //        vars.Unlock();
        //    }
        //    else
        //    {
        //        //writing to the variable
        //        object varValue = command.Attributes["value"].Value;
        //        dispenser.LockOneForWrite(varName, ref vars);
        //        vars[varName].Value = System.Convert.ChangeType(varValue, vars[varName].DataType);
        //        vars.Unlock();
        //        returnValue = varValue;
        //    }
        //    return returnValue;
        //}


        private DbCommand GetCommand(XmlNode connection, string commandText)
        {
            DbProviderFactory dbFactory = GetFactory(connection.Attributes["connection"].Value);

            DbConnection conn = dbFactory.CreateConnection();
            conn.ConnectionString = connection.Attributes["connection"].Value;
            DbCommand dbCommand = dbFactory.CreateCommand();
            dbCommand.Connection = conn;
            dbCommand.CommandText = commandText;
            return dbCommand;

        }


        /// <summary>
        /// Tries to return the appropriate Provider Factory based on the value passed in. Creating
        /// the factory depends on having the appropriate provider name, so this method checks for 
        /// common values that indicate what type of connection it is.
        /// </summary>
        /// <param name="providerType">Value that provides a hint on the connection type</param>
        /// <returns>A generic provider factory based on the provider type passed in.</returns>
        private DbProviderFactory GetFactory(string providerType)
        {
            string factoryInvariantName = string.Empty;

            if (providerType.Contains(TAG_OLEDB))
            {
                factoryInvariantName = FACTORY_OLEDB;
            }
            else if (providerType.Contains(TAG_SQL))
            {
                factoryInvariantName = FACTORY_SQL;
            }
            else
            {
                throw (new ArgumentException("Connection type not supported"));
            }

            return DbProviderFactories.GetFactory(factoryInvariantName);

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

    public struct ConnectionRef
    {
        private string _referenceName;
        private string _connectionString;
        private string _provider;
        private string _connectionType;

        public ConnectionRef(string referenceName, string connectionString, string provider, string connectionType)
        {
            _referenceName = referenceName;
            _connectionString = connectionString;
            _provider = provider;
            _connectionType = connectionType;
            return;
        }

        public string ConnectionString
        {
            get { return _connectionString; }
        }

        public string ReferenceName
        {
            get { return _referenceName; }
        }

        public string Provider
        {
            get { return _provider; }
        }

        public string ConnectionType
        {
            get { return _connectionType; }
        }

    }
}
