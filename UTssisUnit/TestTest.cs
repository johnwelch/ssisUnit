using System.IO;
using System.Xml;
using Microsoft.SqlServer.Dts.Runtime;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SsisUnit;
using SsisUnit.Commands;

using SsisUnitBase.Enums;

namespace UTssisUnit
{
    /// <summary>
    /// This is a test class for TestTest and is intended
    /// to contain all TestTest Unit Tests
    /// </summary>
    [TestClass]
    public class TestTest : ExternalFileResourceTestBase
    {
        string _xmlTest;
        string _xmlTestFull;

        [TestInitialize]
        public void MyTestInitialize()
        {
            _xmlTest = "<Test name=\"Test\" package=\"C:\\Projects\\SSISUnit\\SSIS2005\\SSIS2005\\UT Basic Scenario.dtsx\" task=\"SELECT COUNT\" taskResult=\"Success\">";
            _xmlTest += "<Assert name=\"Test\" expectedResult=\"100\" testBefore=\"false\" expression=\"false\">";
            _xmlTest += "<SqlCommand name=\"\" connectionRef=\"AdventureWorks\" returnsValue=\"true\">";
            _xmlTest += "SELECT COUNT(*) FROM Production.Product";
            _xmlTest += "</SqlCommand>";
            _xmlTest += "</Assert>";
            _xmlTest += "</Test>";

            _xmlTestFull = "<Test name=\"Test\" package=\"C:\\Projects\\SSISUnit\\SSIS2005\\SSIS2005\\UT Basic Scenario.dtsx\" task=\"SELECT COUNT\" taskResult=\"Success\">";
            _xmlTestFull += "<TestSetup>";
            _xmlTestFull += "<SqlCommand name=\"\" connectionRef=\"Sandbox\" returnsValue=\"false\">";
            _xmlTestFull += "INSERT INTO UTTable VALUES('Test')";
            _xmlTestFull += "</SqlCommand>";
            _xmlTestFull += "</TestSetup>";
            _xmlTestFull += "<Assert name=\"Test\" expectedResult=\"100\" testBefore=\"false\" expression=\"false\">";
            _xmlTestFull += "<SqlCommand name=\"\" connectionRef=\"AdventureWorks\" returnsValue=\"true\">";
            _xmlTestFull += "SELECT COUNT(*) FROM Production.Product";
            _xmlTestFull += "</SqlCommand>";
            _xmlTestFull += "</Assert>";
            _xmlTestFull += "<TestTeardown>";
            _xmlTestFull += "<SqlCommand name=\"\" connectionRef=\"Sandbox\" returnsValue=\"false\">";
            _xmlTestFull += "DELETE FROM UTTable";
            _xmlTestFull += "</SqlCommand>";
            _xmlTestFull += "</TestTeardown>";
            _xmlTestFull += "</Test>";
        }

        [TestMethod]
        public void NewTestTest()
        {
            var target = new SsisTestSuite();
            var ssisTest = new Test(target, "Test", "C:\\Projects\\SSISUnit\\SSIS2005\\SSIS2005\\UT Basic Scenario.dtsx", "SELECT COUNT");
            target.Tests.Add("Test", ssisTest);

            Assert.AreEqual(1, target.Tests.Count);
            Assert.AreEqual("Test", target.Tests["Test"].Name);
            Assert.AreEqual("C:\\Projects\\SSISUnit\\SSIS2005\\SSIS2005\\UT Basic Scenario.dtsx", target.Tests["Test"].PackageLocation);
            Assert.AreEqual("SELECT COUNT", target.Tests["Test"].Task);
        }

        [TestMethod]
        public void PersistToXmlTest()
        {
            var testSuite = new SsisTestSuite();
            var target = new Test(testSuite, "Test", "C:\\Projects\\SSISUnit\\SSIS2005\\SSIS2005\\UT Basic Scenario.dtsx", "SELECT COUNT");
            target.Asserts.Add("Test", new SsisAssert(testSuite, target, "Test", 100, false));
            target.Asserts["Test"].Command = new SqlCommand(testSuite, "AdventureWorks", true, "SELECT COUNT(*) FROM Production.Product");

            string actual = target.PersistToXml();
            Assert.AreEqual(_xmlTest, actual);
        }

        [TestMethod]
        public void LoadFromXmlTest1()
        {
            var testSuite = new SsisTestSuite();
            var target = new Test(testSuite, string.Empty, string.Empty, string.Empty);
            target.LoadFromXml(_xmlTest);

            Assert.AreEqual(_xmlTest, target.PersistToXml());
        }

        [TestMethod]
        public void LoadFromXmlTest()
        {
            var testSuite = new SsisTestSuite();
            var target = new Test(testSuite, string.Empty, string.Empty, string.Empty);
            XmlNode node = Helper.GetXmlNodeFromString(_xmlTest);
            target.LoadFromXml(node);
            Assert.AreEqual(_xmlTest, target.PersistToXml());
        }

        [TestMethod]
        public void LoadFromXmlTestFull()
        {
            var testSuite = new SsisTestSuite();
            var target = new Test(testSuite, string.Empty, string.Empty, string.Empty);
            XmlNode node = Helper.GetXmlNodeFromString(_xmlTestFull);
            target.LoadFromXml(node);
            Assert.AreEqual(_xmlTestFull, target.PersistToXml());
        }

        [TestMethod]
        public void TestConstructorTest()
        {
            const string PackageLocation = "C:\\Projects\\SSISUnit\\SSIS2005\\SSIS2005\\UT Basic Scenario.dtsx";
            const string TaskName = "SELECT COUNT";
            const string PackageName = "Test";
            var target = new Test(new SsisTestSuite(), PackageName, PackageLocation, TaskName);

            Assert.AreEqual(PackageName, target.Name);
            Assert.AreEqual(PackageLocation, target.PackageLocation);
            Assert.AreEqual(TaskName, target.Task);
        }

        [TestMethod]
        public void TaskThatFailsTest()
        {
            var ts = new SsisTestSuite();
            var target = new Test(ts, "Test Task That Fails", "C:\\Projects\\SSISUnit\\UTssis2008packages\\UT Basic Scenario.dtsx", "SELECT COUNT", DTSExecResult.Failure);
            target.TestSetup.Commands.Add(new PropertyCommand(ts, "Set", "\\Package\\SELECT COUNT.Properties[SqlStatementSource]", "SELECT ''"));
            ts.Tests.Add("Test Task That Fails", target);
            var assert = new SsisAssert(ts, target, "Test Row Count", 504, false, false);
            target.Asserts.Add("Test Row Count", assert);
            assert.Command = new VariableCommand(ts, VariableCommand.VariableOperation.Get, "User::ProductRowCount", null);
            ts.Execute();
            Assert.AreEqual(1, ts.Statistics.GetStatistic(StatisticEnum.AssertPassedCount));
            Assert.AreEqual(1, ts.Statistics.GetStatistic(StatisticEnum.AssertFailedCount));
        }

        [TestMethod]
        public void DataFlowTaskWithExpressionTest()
        {
            var packageFile = UnpackToFile("UTssisUnit.TestPackages.TestDataFlowExpression2012.dtsx");
            var newFileName = CreateTempFile(GetTempPath("Test", true), "TestDataFlowExpression2012.dtsx");
            File.Copy(packageFile, newFileName, true);
            var ts = new SsisTestSuite();
            var target = new Test(ts, "DataFlowExpression", newFileName, "Data Flow Task");
            ts.Tests.Add("Test Task That Fails", target);
            var assert = new SsisAssert(ts, target, "Test Anything", "true==true", false, true);
            target.Asserts.Add("Test Row Count", assert);
            assert.Command = new PropertyCommand(ts, "Get", "\\Package.Properties[Description]", null);

            ts.Execute();

            // TODO: Think this might be missing something - asserts were originally 1 passed, 1 failed
            // See if I can find original test package

            Assert.AreEqual(2, ts.Statistics.GetStatistic(StatisticEnum.AssertPassedCount));
            Assert.AreEqual(0, ts.Statistics.GetStatistic(StatisticEnum.AssertFailedCount));
        }

        [TestMethod]
        public void DataFlowComponentTest()
        {
            var packageFile = UnpackToFile("UTssisUnit.TestPackages.DataFlowComponent.dtsx");
            var newFileName = CreateTempFile(GetTempPath("Test", true), "TestDataFlowExpression2012.dtsx");
            File.Copy(packageFile, newFileName, true);
            var ts = new SsisTestSuite();
            ts.ConnectionRefs.Add("AdventureWorks", new ConnectionRef("AdventureWorks", "Data Source=localhost;Initial Catalog=AdventureWorks2012;Integrated Security=SSPI;", ConnectionRef.ConnectionTypeEnum.AdoNet, "System.Data.SqlClient"));
            ts.Datasets.Add("testInput", new Dataset(ts, "testInput", ts.ConnectionRefs["AdventureWorks"], false, "SELECT "
                                                                                               + "CAST(1 AS INT) AS ColInt, "
                                                                                               + "CAST('Test' AS VARCHAR(50)) AS ColVarChar, "
                                                                                               + "CAST(N'Test' AS NVARCHAR(50)) AS ColNVarChar, "
                                                                                               + "CAST('1900-01-01' AS DATETIME) AS ColDateTime"));
            var target = new Test(ts, "DataFlowComponent", newFileName, @"Package\Data Flow Task\Derived Column");
            ts.Tests.Add("Test Data Flow Derived Column", target);
            target.TestSetup.Commands.Add(new ComponentInputCommand(ts, "ComponentInput", "testInput", @"Package\Data Flow Task\Derived Column.Inputs[Derived Column Input]"));
            var assert = new SsisAssert(ts, target, "Test Output", true, false);
            target.Asserts.Add("Test Output", assert);
            assert.Command = new ComponentOutputCommand(ts, "ComponentOutput", string.Empty, string.Empty);

            ts.Execute();

            Assert.AreEqual(2, ts.Statistics.GetStatistic(StatisticEnum.AssertPassedCount));
            Assert.AreEqual(0, ts.Statistics.GetStatistic(StatisticEnum.AssertFailedCount));
        }
    }
}
