using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Microsoft.SqlServer.Dts.Runtime;
using System.Globalization;

namespace SsisUnit
{
    public class Test
    {
        private SsisTestSuite _testSuite;
        private string _name;
        private string _task;
        private string _package;
        private Dictionary<string, SsisAssert> _asserts = new Dictionary<string, SsisAssert>();

        public Test(SsisTestSuite testSuite, string name, string package, string task)
        {
            _testSuite = testSuite;
            _name = name;
            _task = task;
            _package = package;
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

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public string Task
        {
            get { return _task; }
            set { _task = value; }
        }

        public string PackageLocation
        {
            //TODO: Add validation that package is a path or that it exists in PackageRef
            get { return _package; }
            set { _package = value; }
        }

        public Dictionary<string, SsisAssert> Asserts
        {
            get { return _asserts; }
        }

        #endregion

        public bool Execute()
        {
            _testSuite.Statistics.IncrementStatistic(TestSuiteStatistics.StatisticEnum.TestCount);

            bool returnValue = false;

            Package packageToTest = null;
            DtsContainer taskHost = null;

            LoadPackageAndTask(_package, _task, ref packageToTest, ref taskHost);

            //TODO: Complete Test Execute - Assert, etc.

            return returnValue;
        }

        private void LoadPackageAndTask(string packagePath, string taskID, ref Package package, ref DtsContainer taskHost)
        {
            string pathToPackage = string.Empty;

            try
            {
                if (packagePath.Contains("\\"))
                {
                    //Assume that it is a file path.
                    package = _testSuite.SsisApplication.LoadPackage(packagePath, null);
                }
                else
                {
                    //PackageList Reference
                    PackageRef packageRef = _testSuite.PackageRefs[packagePath];

                    if (packageRef.StorageType == PackageRef.PackageStorageType.FileSystem)
                    {
                        package = _testSuite.SsisApplication.LoadPackage(packageRef.PackagePath, null);
                    }
                    else if (packageRef.StorageType == PackageRef.PackageStorageType.MSDB)
                    {
                        package = _testSuite.SsisApplication.LoadFromSqlServer(packageRef.PackagePath, packageRef.Server, null, null, null);
                    }
                    else if (packageRef.StorageType == PackageRef.PackageStorageType.PackageStore)
                    {
                        package = _testSuite.SsisApplication.LoadFromDtsServer(packageRef.PackagePath, packageRef.Server, null);
                    }
                }

                taskHost = Helper.FindExecutable(package, taskID);
                if (taskHost == null)
                {
                    throw new Exception("The task host was not found.");
                }
            }
            catch (KeyNotFoundException ex)
            {
                throw new KeyNotFoundException(String.Format(CultureInfo.CurrentCulture, "The package attribute is {0}, which does not reference a valid package.", packagePath));
            }
            catch (Exception ex)
            {
                throw new ArgumentException(string.Format("The package path ({0}) or the task host ({1}) is not valid." + ex.Message, packagePath, taskID));
            }
        }

        public string PersistToXml()
        {
            StringBuilder xml = new StringBuilder();
            xml.Append("<Test ");
            xml.Append("name=\"" + this.Name + "\" ");
            xml.Append("package=\"" + this.PackageLocation + "\" ");
            xml.Append("task=\"" + this.Task + "\">");

            foreach (SsisAssert assert in Asserts.Values)
            {
                xml.Append(assert.PersistToXml());
            }

            xml.Append("</Test>");
            return xml.ToString();
        }

        public void LoadFromXml(string testXml)
        {
            LoadFromXml(Helper.GetXmlNodeFromString(testXml));
        }

        public void LoadFromXml(XmlNode testXml)
        {
            if (testXml.Name != "Test")
            {
                throw new ArgumentException(string.Format("The Xml does not contain the correct type ({0}).", "Test"));
            }

            _name = testXml.Attributes["name"].Value;
            _package = testXml.Attributes["package"].Value;
            _task = testXml.Attributes["task"].Value;
            _asserts = LoadAsserts(testXml);
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
                returnValue.Add(assert.Attributes["name"].Value, new SsisAssert(_testSuite, assert));
            }

            return returnValue;
        }

    }
}
