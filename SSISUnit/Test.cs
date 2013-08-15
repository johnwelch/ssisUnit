using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Microsoft.SqlServer.Dts.Runtime;
using System.Globalization;
using System.ComponentModel;

namespace SsisUnit
{
    public class Test : SsisUnitBaseObject
    {
        public event EventHandler<CommandCompletedEventArgs> CommandCompleted;
        public event EventHandler<CommandFailedEventArgs> CommandFailed;
        public event EventHandler<CommandStartedEventArgs> CommandStarted;

        private string _taskName;

        public Test(SsisTestSuite testSuite, string name, string package, string task)
            : this(testSuite, name, package, task, DTSExecResult.Success)
        {
        }

        //public Test(SsisTestSuite testSuite, string name, string package, string task, string taskName)
        //    : this(testSuite, name, package, task, taskName, DTSExecResult.Success)
        //{
        //}

        public Test(SsisTestSuite testSuite, string name, string package, string task, DTSExecResult taskResult)
        {
            Asserts = new Dictionary<string, SsisAssert>();
            TestSuite = testSuite;
            Name = name;
            Task = task;
            //TaskName = taskName;
            PackageLocation = package;
            TestSetup = new CommandSet(string.IsNullOrEmpty(Name) ? "Setup" : Name + " Setup", TestSuite);
            TestTeardown = new CommandSet(string.IsNullOrEmpty(Name) ? "Teardown" : Name + " Teardown", TestSuite);
            TaskResult = taskResult;
        }

        //public Test(SsisTestSuite testSuite, string name, string package, string task, string taskName, DTSExecResult taskResult)
        //{
        //    Asserts = new Dictionary<string, SsisAssert>();
        //    TestSuite = testSuite;
        //    Name = name;
        //    Task = task;
        //    //TaskName = taskName;
        //    PackageLocation = package;
        //    TestSetup = new CommandSet(string.IsNullOrEmpty(Name) ? "Setup" : Name + " Setup", TestSuite);
        //    TestTeardown = new CommandSet(string.IsNullOrEmpty(Name) ? "Teardown" : Name + " Teardown", TestSuite);
        //    TaskResult = taskResult;
        //}

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

        ///// <summary>
        ///// The task's name is solely used for display and informational purposes.  The <see cref="Task"/> property determines what gets executed when executing the <see cref="Test"/>.
        ///// </summary>
        //[Browsable(false)]
        //public string TaskName { get; set; }

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
        public Dictionary<string, SsisAssert> Asserts { get; private set; }

        [Browsable(false)]
        public CommandSet TestSetup { get; private set; }

        [Browsable(false)]
        public CommandSet TestTeardown { get; private set; }

        [Description("Determines the expected result of the task."), TypeConverter(typeof(EnumConverter))]
        public DTSExecResult TaskResult { get; set; }

        #endregion

        public bool Execute()
        {
            TestSuite.Statistics.IncrementStatistic(TestSuiteResults.StatisticEnum.TestCount);

            bool returnValue = false;

            Package packageToTest = null;
            DtsContainer taskHost = null;

            LoadPackageAndTask(PackageLocation, Task, ref packageToTest, ref taskHost);

            string setupResults = string.Empty;
            bool setupSucceeded;
            _taskName = taskHost.Name;

            try
            {
                // TODO: Decide if this behavior is correct - not sure if we really need to run the parent setups
                // if (_testSuite.ParentTestSuite != null)
                // {
                //     _testSuite.ParentTestSuite.SetupCommands.Execute(packageToTest, taskHost);
                // }

                ExecuteCommandSet(TestSuite.SetupCommands, packageToTest, taskHost);
                ExecuteCommandSet(TestSetup, packageToTest, taskHost);

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
                TestSuite.OnRaiseSetupCompleted(new SetupCompletedEventArgs(DateTime.Now, Name, PackageLocation, _taskName, setupResults));
            }

            if (!setupSucceeded)
                return false;

            string resultMessage = string.Empty;

            try
            {
                foreach (SsisAssert assert in Asserts.Values)
                {
                    if (!assert.TestBefore)
                        continue;

                    assert.Execute(packageToTest, taskHost);
                }

                taskHost.Execute(packageToTest.Connections, taskHost.Variables, null, null, null);

                DTSExecResult result = taskHost.ExecutionResult;

                TestSuite.Statistics.IncrementStatistic(TestSuiteResults.StatisticEnum.AssertCount);

                if (result == TaskResult)
                {
                    TestSuite.OnRaiseAssertCompleted(new AssertCompletedEventArgs(new TestResult(DateTime.Now, PackageLocation, _taskName, Name, string.Format("Task Completed: Actual result ({0}) was equal to the expected result ({1}).", result.ToString(), TaskResult.ToString()), true)));
                    TestSuite.Statistics.IncrementStatistic(TestSuiteResults.StatisticEnum.AssertPassedCount);

                    foreach (SsisAssert assert in Asserts.Values)
                    {
                        if (assert.TestBefore)
                            continue;

                        assert.Execute(packageToTest, taskHost);
                    }

                    resultMessage = "All asserts were completed.";
                    returnValue = true;

                    TestSuite.Statistics.IncrementStatistic(TestSuiteResults.StatisticEnum.TestPassedCount);
                }
                else
                {
                    TestSuite.OnRaiseAssertCompleted(new AssertCompletedEventArgs(new TestResult(DateTime.Now, PackageLocation, _taskName, Name, string.Format("Task Completed: Actual result ({0}) was not equal to the expected result ({1}).", result.ToString(), TaskResult.ToString()), false)));
                    TestSuite.Statistics.IncrementStatistic(TestSuiteResults.StatisticEnum.AssertFailedCount);

                    foreach (DtsError err in packageToTest.Errors)
                    {
                        TestSuite.OnRaiseAssertCompleted(new AssertCompletedEventArgs(new TestResult(DateTime.Now, PackageLocation, _taskName, Name, "Task Error: " + err.Description.Replace(Environment.NewLine, string.Empty), false)));
                        TestSuite.Statistics.IncrementStatistic(TestSuiteResults.StatisticEnum.AssertFailedCount);
                    }

                    resultMessage = "The task " + _taskName + " did not execute successfully.";

                    TestSuite.Statistics.IncrementStatistic(TestSuiteResults.StatisticEnum.TestFailedCount);
                }
            }
            catch (Exception ex)
            {
                TestSuite.Statistics.IncrementStatistic(TestSuiteResults.StatisticEnum.TestFailedCount);
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
                ExecuteCommandSet(TestTeardown, packageToTest, taskHost);
                ExecuteCommandSet(TestSuite.TeardownCommands, packageToTest, taskHost);

                teardownResults = "Teardown succeeded.";
            }
            catch (Exception ex)
            {
                teardownResults = "Teardown failed: " + ex.Message + " : " + ex.InnerException;

                returnValue = false;
            }
            finally
            {
                TestSuite.OnRaiseTeardownCompleted(new TeardownCompletedEventArgs(DateTime.Now, Name, PackageLocation, _taskName, teardownResults));
            }

            return returnValue;
        }

        private void ExecuteCommandSet(CommandSet commandSet, Package packageToTest, DtsContainer taskHost)
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

                commandSet.Execute(packageToTest, taskHost);
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

        private void LoadPackageAndTask(string packagePath, string taskId, ref Package package, ref DtsContainer taskHost)
        {
            try
            {
                package = Helper.LoadPackage(TestSuite, packagePath);
                string remainingPath;
                taskHost = Helper.FindExecutable(package, taskId, out remainingPath);
                if (taskHost == null)
                {
                    throw new Exception("The task host was not found.");
                }
            }
            catch (KeyNotFoundException)
            {
                throw new KeyNotFoundException(string.Format(CultureInfo.CurrentCulture, "The package attribute is {0}, which does not reference a valid package.", packagePath));
            }
            catch (Exception ex)
            {
                throw new ArgumentException(string.Format("The package path ({0}) or the task host ({1}) is not valid." + ex.Message, packagePath, taskId));
            }
        }

        public override string PersistToXml()
        {
            StringBuilder xml = new StringBuilder();
            XmlWriterSettings writerSettings = new XmlWriterSettings { ConformanceLevel = ConformanceLevel.Fragment, OmitXmlDeclaration = true };

            XmlWriter xmlWriter = XmlWriter.Create(xml, writerSettings);
            xmlWriter.WriteStartElement("Test");
            xmlWriter.WriteAttributeString("name", Name);
            xmlWriter.WriteAttributeString("package", PackageLocation);
            xmlWriter.WriteAttributeString("task", Task);
            //xmlWriter.WriteAttributeString("taskName", TaskName);
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
            //TaskName = testXml.Attributes != null && testXml.Attributes["taskName"] != null ? testXml.Attributes["taskName"].Value : null;
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

            Dictionary<string, SsisAssert> returnValue = new Dictionary<string, SsisAssert>(asserts.ChildNodes.Count);

            foreach (XmlNode assert in asserts)
            {
                if (assert.Name == "Assert")
                {
                    if (assert.Attributes == null)
                        continue;

                    returnValue.Add(assert.Attributes["name"].Value, new SsisAssert(TestSuite, assert));
                }
            }

            return returnValue;
        }
    }
}