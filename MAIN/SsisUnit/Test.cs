using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Xml;

using Microsoft.SqlServer.Dts.Runtime;
using System.Globalization;
using System.ComponentModel;

using SsisUnitBase;
using SsisUnitBase.Enums;
using SsisUnitBase.EventArgs;

#if SQL2014 || SQL2012 || SQL2008
using IDTSComponentMetaData = Microsoft.SqlServer.Dts.Pipeline.Wrapper.IDTSComponentMetaData100;
#elif SQL2005
using IDTSComponentMetaData = Microsoft.SqlServer.Dts.Pipeline.Wrapper.IDTSComponentMetaData90;
#endif

namespace SsisUnit
{
    public class Test : SsisUnitBaseObject
    {
        internal Package InternalPackage;

        public event EventHandler<CommandCompletedEventArgs> CommandCompleted;
        public event EventHandler<CommandFailedEventArgs> CommandFailed;
        public event EventHandler<CommandStartedEventArgs> CommandStarted;
        public event EventHandler<TestStartedEventArgs> TestStarted;
        public event EventHandler<TestCompletedEventArgs> TestCompleted;

        private string _taskName;

        private SecureString _securePassword;

        public Test(SsisTestSuite testSuite, string name, string package, string password, string task)
            : this(testSuite, name, null, package, password, task, DTSExecResult.Success) { }

        public Test(SsisTestSuite testSuite, string name, string project, string package, string password, string task)
            : this(testSuite, name, project, package, password, task, DTSExecResult.Success) { }

        public Test(SsisTestSuite testSuite, string name, string package, string password, string task, DTSExecResult taskResult)
            : this(testSuite, name, null, package, password, task, taskResult) { }

        public Test(SsisTestSuite testSuite, string name, string project, string package, string password, string task, DTSExecResult taskResult)
        {
            Asserts = new Dictionary<string, SsisAssert>();
            TestSuite = testSuite;
            Name = name;
            Task = task;
            PackageLocation = package;

            if (password != null)
                _securePassword = Helper.ConvertToSecureString(password);

            ProjectLocation = project;
            TestSetup = new CommandSet(string.IsNullOrEmpty(Name) ? "Setup" : Name + " Setup", TestSuite);
            TestTeardown = new CommandSet(string.IsNullOrEmpty(Name) ? "Teardown" : Name + " Teardown", TestSuite);
            TaskResult = taskResult;
        }

        public Test(SsisTestSuite testSuite, XmlNode testXml)
        {
            TaskResult = DTSExecResult.Success;
            Asserts = new Dictionary<string, SsisAssert>();
            TestSuite = testSuite;

            LoadFromXml(testXml);
        }

        public Test(SsisTestSuite testSuite, string testXml)
        {
            TaskResult = DTSExecResult.Success;
            Asserts = new Dictionary<string, SsisAssert>();
            TestSuite = testSuite;

            LoadFromXml(testXml);
        }

        #region Properties

        [Browsable(false)]
        public SsisTestSuite TestSuite { get; private set; }

        /// <summary>
        /// The task's GUID ID or Name to execute during the execution of the <see cref="Test"/>.  
        /// If a task's name is specified, the first encountered task with the specified name will be executed.  
        /// If there is more than one task with the same name, use the task's GUID instead.
        /// </summary>
#if SQL2005
        [Description("The task that this test will run against."),
         TypeConverter("SsisUnit.Design.TaskConverter, SsisUnit.Design, Version=1.0.0.0, Culture=neutral, PublicKeyToken=6fbed22cbef36cab"),
         Editor("SsisUnit.Design.PackageBrowserEditor, SsisUnit.Design, Version=1.0.0.0, Culture=neutral, PublicKeyToken=6fbed22cbef36cab", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
#elif SQL2008
        [Description("The task that this test will run against."),
         TypeConverter("SsisUnit.Design.TaskConverter, SsisUnit2008.Design, Version=1.0.0.0, Culture=neutral, PublicKeyToken=6fbed22cbef36cab"),
         Editor("SsisUnit.Design.PackageBrowserEditor, SsisUnit2008.Design, Version=1.0.0.0, Culture=neutral, PublicKeyToken=6fbed22cbef36cab", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
#elif SQL2012
        [Description("The task that this test will run against."),
         TypeConverter("SsisUnit.Design.TaskConverter, SsisUnit.Design.2012, Version=1.0.0.0, Culture=neutral, PublicKeyToken=6fbed22cbef36cab"),
         Editor("SsisUnit.Design.PackageBrowserEditor, SsisUnit.Design.2012, Version=1.0.0.0, Culture=neutral, PublicKeyToken=6fbed22cbef36cab", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
#endif
        public string Task { get; set; }

#if SQL2005
        [Description("The package that this test will run against."),
         TypeConverter("SsisUnit.Design.PackageRefConverter, SsisUnit.Design, Version=1.0.0.0, Culture=neutral, PublicKeyToken=6fbed22cbef36cab")]
#elif SQL2008
        [Description("The package that this test will run against."),
         TypeConverter("SsisUnit.Design.PackageRefConverter, SsisUnit2008.Design, Version=1.0.0.0, Culture=neutral, PublicKeyToken=6fbed22cbef36cab")]
#elif SQL2012
        [Description("The package that this test will run against."),
         TypeConverter("SsisUnit.Design.PackageRefConverter, SsisUnit.Design.2012, Version=1.0.0.0, Culture=neutral, PublicKeyToken=6fbed22cbef36cab")]
#endif
        public string PackageLocation { get; set; }

        [Browsable(false)]
        internal SecureString StoredPassword
        {
            get
            {
                return _securePassword;
            }
        }

        /*
        #if SQL2005
            [Description("The package that this test will run against."),
            TypeConverter("SsisUnit.Design.ProjectRefConverter, SsisUnit.Design, Version=1.0.0.0, Culture=neutral, PublicKeyToken=6fbed22cbef36cab")]
        #elif SQL2008
            [Description("The package that this test will run against."),
            TypeConverter("SsisUnit.Design.ProjectRefConverter, SsisUnit2008.Design, Version=1.0.0.0, Culture=neutral, PublicKeyToken=6fbed22cbef36cab")]
        #elif SQL2012
            [Description("The package that this test will run against."),
            TypeConverter("SsisUnit.Design.ProjectRefConverter, SsisUnit.Design.2012, Version=1.0.0.0, Culture=neutral, PublicKeyToken=6fbed22cbef36cab")]
        #endif
         */
        [Browsable(false)]
        public string ProjectLocation { get; set; }

        [Browsable(false)]
        public Dictionary<string, SsisAssert> Asserts { get; private set; }

        [Browsable(false)]
        public CommandSet TestSetup { get; private set; }

        [Browsable(false)]
        public CommandSet TestTeardown { get; private set; }

        [Description("Determines the expected result of the task."), TypeConverter(typeof(EnumConverter))]
        public DTSExecResult TaskResult { get; set; }

        #endregion

        /// <summary>
        /// Execute the setup, asserts, and teardown associated with this test.
        /// </summary>
        /// <returns>True if the test was executed with no errors, false if it encountered errors.</returns>
        public bool Execute()
        {
            OnRaiseTestStarted(new TestStartedEventArgs());

            TestSuite.Statistics.IncrementStatistic(StatisticEnum.TestCount);

            bool returnValue = false;

            object loadedProject = null;

            try
            {
                Package packageToTest;
                DtsContainer taskHost;

                try
                {
                    LoadPackageAndTask(PackageLocation, StoredPassword, ProjectLocation, Task, out packageToTest, out taskHost, out loadedProject);
                }
                catch (Exception)
                {
                    TestSuite.Statistics.IncrementStatistic(StatisticEnum.TestFailedCount);

                    throw;
                }

                InternalPackage = packageToTest;

                string setupResults = string.Empty;
                bool setupSucceeded;
                _taskName = taskHost.Name;

                try
                {
                    ExecuteCommandSet(TestSuite.SetupCommands, loadedProject, packageToTest, taskHost);
                    ExecuteCommandSet(TestSetup, loadedProject, packageToTest, taskHost);

                    setupResults = "Setup succeeded.";
                    setupSucceeded = true;
                }
                catch (Exception ex)
                {
                    setupResults = "Setup failed: " + ex.Message;

                    if (ex.InnerException != null)
                        setupResults += " : " + ex.InnerException;

                    setupSucceeded = false;

                    TestSuite.Statistics.IncrementStatistic(StatisticEnum.TestFailedCount);
                }
                finally
                {
                    TestSuite.OnRaiseSetupCompleted(new SetupCompletedEventArgs(DateTime.Now, Name, PackageLocation, _taskName, setupResults));
                }

                if (!setupSucceeded)
                    return false;

                string resultMessage = string.Empty;

                try
                {
                    foreach (SsisAssert assert in Asserts.Values.Where(assert=>assert.TestBefore))
                    {
                        assert.Execute(loadedProject, packageToTest, taskHost);
                    }

                    var events = new SsisEvents();

#if SQL2012 || SQL2014
                    Project project = loadedProject as Project;

                    if (project != null)
                    {
                        foreach (var connectionManagerItem in project.ConnectionManagerItems)
                        {
                            packageToTest.Connections.Join(connectionManagerItem.ConnectionManager);
                        }
                    }
#else
                loadedProject = null;
#endif

                    taskHost.Execute(packageToTest.Connections, taskHost.Variables, events, null, null);

                    DTSExecResult result = taskHost.ExecutionResult;

                    TestSuite.Statistics.IncrementStatistic(StatisticEnum.AssertCount);

                    if (result == TaskResult)
                    {
                        TestSuite.OnRaiseAssertCompleted(new AssertCompletedEventArgs(null, new TestResult(DateTime.Now, PackageLocation, _taskName, Name, string.Format("Task Completed: Actual result ({0}) was equal to the expected result ({1}).", result.ToString(), TaskResult.ToString()), true)));
                        TestSuite.Statistics.IncrementStatistic(StatisticEnum.AssertPassedCount);

                        foreach (SsisAssert assert in Asserts.Values)
                        {
                            if (assert.TestBefore)
                                continue;

                            assert.Execute(loadedProject, packageToTest, taskHost);
                        }

                        resultMessage = "All asserts were completed.";
                        returnValue = true;

                        TestSuite.Statistics.IncrementStatistic(StatisticEnum.TestPassedCount);
                    }
                    else
                    {
                        TestSuite.OnRaiseAssertCompleted(new AssertCompletedEventArgs(null, new TestResult(DateTime.Now, PackageLocation, _taskName, Name, string.Format("Task Completed: Actual result ({0}) was not equal to the expected result ({1}).", result.ToString(), TaskResult.ToString()), false)));
                        TestSuite.Statistics.IncrementStatistic(StatisticEnum.AssertFailedCount);

                        foreach (DtsError err in packageToTest.Errors)
                        {
                            TestSuite.OnRaiseAssertCompleted(new AssertCompletedEventArgs(null, new TestResult(DateTime.Now, PackageLocation, _taskName, Name, "Task Error: " + err.Description.Replace(Environment.NewLine, string.Empty), false)));
                            TestSuite.Statistics.IncrementStatistic(StatisticEnum.AssertFailedCount);
                        }

                        resultMessage = "The task " + _taskName + " did not execute successfully.";

                        TestSuite.Statistics.IncrementStatistic(StatisticEnum.TestFailedCount);
                    }
                }
                catch (Exception ex)
                {
                    TestSuite.Statistics.IncrementStatistic(StatisticEnum.TestFailedCount);
                    returnValue = false;
                    resultMessage = "Exception occurred: " + ex.Message;
                }
                finally
                {
                    TestSuite.OnRaiseTestCompleted(new TestCompletedEventArgs(DateTime.Now, PackageLocation, _taskName, Name, resultMessage, returnValue));
                }

                if (!returnValue)
                    return false;

                string teardownResults = string.Empty;

                try
                {
                    ExecuteCommandSet(TestTeardown, loadedProject, packageToTest, taskHost);
                    ExecuteCommandSet(TestSuite.TeardownCommands, loadedProject, packageToTest, taskHost);

                    teardownResults = "Teardown succeeded.";
                }
                catch (Exception ex)
                {
                    teardownResults = "Teardown failed: " + ex.Message;

                    if (ex.InnerException != null)
                        teardownResults += " : " + ex.InnerException;

                    returnValue = false;

                    TestSuite.Statistics.IncrementStatistic(StatisticEnum.TestFailedCount);
                }
                finally
                {
                    TestSuite.OnRaiseTeardownCompleted(new TeardownCompletedEventArgs(DateTime.Now, Name, PackageLocation, _taskName, teardownResults));
                }

                return returnValue;
            }
            finally
            {
#if SQL2012
                Project project = loadedProject as Project;

                if (project != null)
                    project.Dispose();
#else
                loadedProject = null;
#endif

                OnRaiseTestCompleted(new TestCompletedEventArgs(DateTime.Now, Name, PackageLocation, _taskName, string.Format("The {0} unit test has completed.", Name), returnValue));
            }
        }

        private void ExecuteCommandSet(CommandSet commandSet, object project, Package packageToTest, DtsContainer taskHost)
        {
            if (commandSet == null)
                throw new ArgumentNullException("commandSet");

            if (packageToTest == null)
                throw new ArgumentNullException("packageToTest");

            if (taskHost == null)
                throw new ArgumentNullException("taskHost");

            try
            {
                commandSet.CommandStarted += CommandOnCommandStarted;
                commandSet.CommandCompleted += CommandOnCommandCompleted;
                commandSet.CommandFailed += CommandOnCommandFailed;

                commandSet.Execute(project, packageToTest, taskHost);
            }
            finally
            {
                commandSet.CommandStarted -= CommandOnCommandStarted;
                commandSet.CommandCompleted -= CommandOnCommandCompleted;
                commandSet.CommandFailed -= CommandOnCommandFailed;
            }
        }

        private void CommandOnCommandStarted(object sender, CommandStartedEventArgs e)
        {
            EventHandler<CommandStartedEventArgs> handler = CommandStarted;

            if (handler != null)
                handler(sender, e);
        }

        private void CommandOnCommandCompleted(object sender, CommandCompletedEventArgs e)
        {
            EventHandler<CommandCompletedEventArgs> handler = CommandCompleted;

            if (handler != null)
                handler(sender, e);
        }

        private void CommandOnCommandFailed(object sender, CommandFailedEventArgs e)
        {
            EventHandler<CommandFailedEventArgs> handler = CommandFailed;

            if (handler != null)
                handler(sender, e);
        }

        private void OnRaiseTestCompleted(TestCompletedEventArgs e)
        {
            EventHandler<TestCompletedEventArgs> handler = TestCompleted;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void OnRaiseTestStarted(TestStartedEventArgs e)
        {
            EventHandler<TestStartedEventArgs> handler = this.TestStarted;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void LoadPackageAndTask(string packagePath, SecureString packagePassword, string projectPath, string taskId, out Package package, out DtsContainer taskHost, out object loadedProject)
        {
            try
            {
                package = Helper.LoadPackage(TestSuite, packagePath, packagePassword, projectPath, out loadedProject);
                string remainingPath;
                taskHost = Helper.FindExecutable(package, taskId, out remainingPath);
                if (taskHost == null)
                {
                    throw new Exception("The task host was not found.");
                }
            }
            catch (DtsPackageStoreException)
            {
                throw;
            }
            catch (KeyNotFoundException)
            {
                throw new KeyNotFoundException(string.Format(CultureInfo.CurrentCulture, "The package attribute is {0}, which does not reference a valid package.", packagePath));
            }
            catch (Exception ex)
            {
                throw new ArgumentException(string.Format("The package path ({0}) or the task host ({1}) is not valid. {2}", string.IsNullOrEmpty(packagePath) ? "<NOT PROVIDED>" : packagePath, taskId, ex.Message));
            }
        }

        public override string PersistToXml()
        {
            var xml = new StringBuilder();
            var writerSettings = new XmlWriterSettings { ConformanceLevel = ConformanceLevel.Fragment, OmitXmlDeclaration = true };

            XmlWriter xmlWriter = XmlWriter.Create(xml, writerSettings);
            xmlWriter.WriteStartElement("Test");
            xmlWriter.WriteAttributeString("name", Name);
            xmlWriter.WriteAttributeString("package", PackageLocation);
            xmlWriter.WriteAttributeString("task", Task);
            xmlWriter.WriteAttributeString("taskResult", TaskResult.ToString());

            if (TestSetup.Commands.Count > 0)
            {
                xmlWriter.WriteStartElement("TestSetup");
                xmlWriter.WriteRaw(TestSetup.PersistToXml());
                xmlWriter.WriteEndElement();
            }

            foreach (SsisAssert assert in Asserts.Values)
            {
                xmlWriter.WriteRaw(assert.PersistToXml());
            }

            if (TestTeardown.Commands.Count > 0)
            {
                xmlWriter.WriteStartElement("TestTeardown");
                xmlWriter.WriteRaw(TestTeardown.PersistToXml());
                xmlWriter.WriteEndElement();
            }

            xmlWriter.WriteEndElement();
            xmlWriter.Close();
            return xml.ToString();
        }

        public override sealed void LoadFromXml(string testXml)
        {
            LoadFromXml(Helper.GetXmlNodeFromString(testXml));
        }

        public override sealed void LoadFromXml(XmlNode testXml)
        {
            if (testXml.Name != "Test")
            {
                throw new ArgumentException(string.Format("The Xml does not contain the correct type ({0}).", "Test"));
            }

            Name = testXml.Attributes != null && testXml.Attributes["name"] != null ? testXml.Attributes["name"].Value : null;
            PackageLocation = testXml.Attributes != null && testXml.Attributes["package"] != null ? testXml.Attributes["package"].Value : null;
            Task = testXml.Attributes != null && testXml.Attributes["task"] != null ? testXml.Attributes["task"].Value : null;
            Asserts = LoadAsserts(testXml);
            TestSetup = new CommandSet(string.IsNullOrEmpty(Name) ? "Setup" : Name + " Setup", TestSuite, testXml["TestSetup"]);
            TestTeardown = new CommandSet(string.IsNullOrEmpty(Name) ? "Teardown" : Name + " Teardown", TestSuite, testXml["TestTeardown"]);

            XmlNode xmlNode = testXml.Attributes != null ? testXml.Attributes.GetNamedItem("taskResult") : null;

            if (xmlNode == null)
            {
                TaskResult = DTSExecResult.Success;
            }
            else
            {
                TaskResult = (DTSExecResult)Enum.Parse(typeof(DTSExecResult), xmlNode.Value, true);
            }
        }

        public override bool Validate()
        {
            ValidationMessages = string.Empty;

            if (Asserts.Count >= 1)
                return true;

            ValidationMessages += "There must be one or more asserts for each test." + Environment.NewLine;

            return false;
        }

        private Dictionary<string, SsisAssert> LoadAsserts(XmlNode asserts)
        {
            if (asserts == null)
            {
                return new Dictionary<string, SsisAssert>();
            }

            var returnValue = new Dictionary<string, SsisAssert>(asserts.ChildNodes.Count);

            foreach (XmlNode assert in asserts)
            {
                if (assert.Name == "Assert")
                {
                    if (assert.Attributes == null)
                        continue;

                    returnValue.Add(assert.Attributes["name"].Value, new SsisAssert(TestSuite, this, assert));
                }
            }

            return returnValue;
        }

        private class SsisEvents : DefaultEvents
        {
            public override bool OnError(DtsObject source, int errorCode, string subComponent, string description, string helpFile, int helpContext, string idofInterfaceWithError)
            {
                return base.OnError(source, errorCode, subComponent, description, helpFile, helpContext, idofInterfaceWithError);
            }
        }
    }
}