using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Microsoft.SqlServer.Dts.Runtime;
using System.Globalization;
using System.Reflection;
using System.IO;
using System.ComponentModel;

namespace SsisUnit
{
    public class Test : SsisUnitBaseObject
    {
        private SsisTestSuite _testSuite;
        //private string _name;
        private string _task;
        private string _taskName;
        private string _package;
        private Dictionary<string, SsisAssert> _asserts = new Dictionary<string, SsisAssert>();
        private CommandSet _setup;
        private CommandSet _teardown;
        //private string _validationMessages = string.Empty;

        public Test(SsisTestSuite testSuite, string name, string package, string task)
        {
            _testSuite = testSuite;
            Name = name;
            _task = task;
            _package = package;
            _setup = new CommandSet(_testSuite);
            _teardown = new CommandSet(_testSuite);
            return;
        }

        public Test(SsisTestSuite testSuite, XmlNode testXml)
        {
            _testSuite = testSuite;
            LoadFromXml(testXml);
            return;
        }

        public Test(SsisTestSuite testSuite, string testXml)
        {
            _testSuite = testSuite;
            LoadFromXml(testXml);
            return;
        }

        #region Properties

        [Browsable(false)]
        public SsisTestSuite TestSuite
        {
            get { return _testSuite; }
        }

        //[Description("The name of this test.")]
        //public string Name
        //{
        //    get { return _name; }
        //    set { _name = value; }
        //}

        //TODO: Add converter for tasks?
        [Description("The task that this test will run against."),
         TypeConverter("SsisUnit.Design.TaskConverter, SsisUnit.Design, Version=1.0.0.0, Culture=neutral, PublicKeyToken=5afc101ee8f7d482"),
         Editor("SsisUnit.Design.PackageBrowserEditor, SsisUnit.Design, Version=1.0.0.0, Culture=neutral, PublicKeyToken=5afc101ee8f7d482", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
        public string Task
        {
            get { return _task; }
            set { _task = value; }
        }

        [Description("The package that this test will run against."),
         TypeConverter("SsisUnit.Design.PackageRefConverter, SsisUnit.Design, Version=1.0.0.0, Culture=neutral, PublicKeyToken=5afc101ee8f7d482")]
        public string PackageLocation
        {
            //TODO: Add a custom type editor to pick file or package ref
            get { return _package; }
            set { _package = value; }
        }

        [Browsable(false)]
        public Dictionary<string, SsisAssert> Asserts
        {
            get { return _asserts; }
        }

        [Browsable(false)]
        public CommandSet TestSetup
        {
            get { return _setup; }
        }

        [Browsable(false)]
        public CommandSet TestTeardown
        {
            get { return _teardown; }
        }

        #endregion

        public bool Execute()
        {
            _testSuite.Statistics.IncrementStatistic(TestSuiteResults.StatisticEnum.TestCount);

            bool returnValue = false;

            Package packageToTest = null;
            DtsContainer taskHost = null;

            LoadPackageAndTask(_package, _task, ref packageToTest, ref taskHost);

            string setupResults = string.Empty;
            bool setupSucceeded = false;
            _taskName = taskHost.Name;

            try
            {
                //TODO: Decide if this behavior is correct - not sure if we really need to run the parent setups
                if (_testSuite.ParentTestSuite != null)
                {
                    _testSuite.ParentTestSuite.SetupCommands.Execute(packageToTest, taskHost);
                }

                _testSuite.SetupCommands.Execute(packageToTest, taskHost);

                _setup.Execute(packageToTest, taskHost);

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
                _testSuite.OnRaiseSetupCompleted(new SetupCompletedEventArgs(DateTime.Now, Name, _package, _taskName, setupResults));
            }

            if (setupSucceeded)
            {
                object validationResult = null;
                string resultMessage = string.Empty;

                try
                {
                    foreach (SsisAssert assert in _asserts.Values)
                    {
                        if (!assert.TestBefore) continue;

                        validationResult = assert.Execute(packageToTest, taskHost);
                    }

                    taskHost.Execute(packageToTest.Connections, taskHost.Variables, null, null, null);
                    foreach (DtsError err in packageToTest.Errors)
                    {
                        _testSuite.OnRaiseAssertCompleted(new AssertCompletedEventArgs(new TestResult(DateTime.Now, _package, _taskName, this.Name, "Task Error: " + err.Description.Replace(Environment.NewLine, string.Empty), false)));
                    }

                    foreach (SsisAssert assert in _asserts.Values)
                    {
                        if (assert.TestBefore) continue;

                        validationResult = assert.Execute(packageToTest, taskHost);
                    }
                    resultMessage = "All asserts were completed.";
                    returnValue = true;
                    _testSuite.Statistics.IncrementStatistic(TestSuiteResults.StatisticEnum.TestPassedCount);

                }
                catch (Exception ex)
                {
                    _testSuite.Statistics.IncrementStatistic(TestSuiteResults.StatisticEnum.TestFailedCount);
                    returnValue = false;
                    resultMessage = "Exception occurred: " + ex.Message;
                }
                finally
                {
                    _testSuite.OnRaiseTestCompleted(new TestCompletedEventArgs(DateTime.Now, _package, _taskName, this.Name, resultMessage, returnValue));
                }

            }
            if (setupSucceeded)
            {
                string teardownResults = string.Empty;

                try
                {
                    _teardown.Execute(packageToTest, taskHost);
                    _testSuite.TeardownCommands.Execute(packageToTest, taskHost);

                    if (_testSuite.ParentTestSuite != null)
                    {
                        _testSuite.ParentTestSuite.TeardownCommands.Execute(packageToTest, taskHost);
                    }
                    teardownResults = "Teardown succeeded.";
                }
                catch (Exception ex)
                {
                    teardownResults = "Teardown failed: " + ex.Message + " : " + ex.InnerException;
                }
                finally
                {
                    _testSuite.OnRaiseTeardownCompleted(new TeardownCompletedEventArgs(DateTime.Now, this.Name, _package, _taskName, teardownResults));
                }
            }

            return returnValue;
        }

        private void LoadPackageAndTask(string packagePath, string taskID, ref Package package, ref DtsContainer taskHost)
        {
            string pathToPackage = string.Empty;

            try
            {
                //if (packagePath.Contains("\\"))
                //{
                //    //Assume that it is a file path.
                //    package = _testSuite.SsisApplication.LoadPackage(packagePath, null);
                //}
                //else
                //{
                //    //PackageList Reference
                //    PackageRef packageRef = _testSuite.PackageRefs[packagePath];

                //    if (packageRef.StorageType == PackageRef.PackageStorageType.FileSystem)
                //    {
                //        package = _testSuite.SsisApplication.LoadPackage(packageRef.PackagePath, null);
                //    }
                //    else if (packageRef.StorageType == PackageRef.PackageStorageType.MSDB)
                //    {
                //        package = _testSuite.SsisApplication.LoadFromSqlServer(packageRef.PackagePath, packageRef.Server, null, null, null);
                //    }
                //    else if (packageRef.StorageType == PackageRef.PackageStorageType.PackageStore)
                //    {
                //        package = _testSuite.SsisApplication.LoadFromDtsServer(packageRef.PackagePath, packageRef.Server, null);
                //    }
                //}
                package = Helper.LoadPackage(_testSuite, packagePath);

                taskHost = Helper.FindExecutable(package, taskID);
                if (taskHost == null)
                {
                    throw new Exception("The task host was not found.");
                }
            }
            catch (KeyNotFoundException)
            {
                throw new KeyNotFoundException(String.Format(CultureInfo.CurrentCulture, "The package attribute is {0}, which does not reference a valid package.", packagePath));
            }
            catch (Exception ex)
            {
                throw new ArgumentException(string.Format("The package path ({0}) or the task host ({1}) is not valid." + ex.Message, packagePath, taskID));
            }
        }

        public override string PersistToXml()
        {
            StringBuilder xml = new StringBuilder();
            xml.Append("<Test ");
            xml.Append("name=\"" + this.Name + "\" ");
            xml.Append("package=\"" + this.PackageLocation + "\" ");
            xml.Append("task=\"" + this.Task + "\">");
            if (_setup.Commands.Count > 0)
            {
                xml.Append("<TestSetup>" + _setup.PersistToXml() + "</TestSetup>");
            }

            foreach (SsisAssert assert in Asserts.Values)
            {
                xml.Append(assert.PersistToXml());
            }

            if (_teardown.Commands.Count > 0)
            {
                xml.Append("<TestTeardown>" + _teardown.PersistToXml() + "</TestTeardown>");
            }
            xml.Append("</Test>");
            return xml.ToString();
        }

        public override void LoadFromXml(string testXml)
        {
            LoadFromXml(Helper.GetXmlNodeFromString(testXml));
        }

        public override void LoadFromXml(XmlNode testXml)
        {
            if (testXml.Name != "Test")
            {
                throw new ArgumentException(string.Format("The Xml does not contain the correct type ({0}).", "Test"));
            }

            this.Name = testXml.Attributes["name"].Value;
            _package = testXml.Attributes["package"].Value;
            _task = testXml.Attributes["task"].Value;
            _asserts = LoadAsserts(testXml);
            _setup = new CommandSet(_testSuite, testXml["TestSetup"]);
            _teardown = new CommandSet(_testSuite, testXml["TestTeardown"]);
        }

        public override bool Validate()
        {
            _validationMessages = string.Empty;
            if (this.Asserts.Count < 1)
            {
                _validationMessages += "There must be one or more asserts for each test." + Environment.NewLine;
            }
            if (_validationMessages == string.Empty)
            {
                return true;
            }
            else
            {
                return false;
            }
            //try
            //{
            //    Assembly asm = Assembly.GetExecutingAssembly();
            //    Stream strm = asm.GetManifestResourceStream(asm.GetName().Name + ".SsisUnit.xsd");


            //    XmlReaderSettings settings = new XmlReaderSettings();
            //    settings.Schemas.Add("http://tempuri.org/SsisUnit.xsd", XmlReader.Create(strm));
            //    settings.ValidationType = ValidationType.Schema;

            //    Byte[] bytes = System.Text.Encoding.ASCII.GetBytes(this.PersistToXml());

            //    XmlDocument test = new XmlDocument();
            //    test.Load(XmlReader.Create(new MemoryStream(bytes), settings));

            //    //Don't test for existence of the schema node at this level
            //    //if (test.SchemaInfo.Validity != System.Xml.Schema.XmlSchemaValidity.Valid)
            //    //{
            //    //    return false;
            //    //}

            //    return true;
            //}
            //catch (System.Xml.Schema.XmlSchemaValidationException)
            //{
            //    return false;
            //}
            //catch (Exception ex)
            //{
            //    throw new ArgumentException("The test case could not be loaded: " + ex.Message);
            //}
        }

        //[Browsable(false)]
        //public string ValidationMessages
        //{
        //    get { return _validationMessages; }
        //}

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
                    returnValue.Add(assert.Attributes["name"].Value, new SsisAssert(_testSuite, assert));
                }
            }

            return returnValue;
        }

    }
}
