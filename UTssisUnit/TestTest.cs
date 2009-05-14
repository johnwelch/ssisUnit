using SsisUnit;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml;
using System.Collections.Generic;
using Microsoft.SqlServer.Dts.Runtime;

namespace UTssisUnit
{


    /// <summary>
    ///This is a test class for TestTest and is intended
    ///to contain all TestTest Unit Tests
    ///</summary>
    [TestClass()]
    public class TestTest
    {
        string _xmlTest;
        string _xmlTestFull;

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        [TestInitialize()]
        public void MyTestInitialize()
        {
            _xmlTest = "<Test name=\"Test\" package=\"C:\\Projects\\SSISUnit\\SSIS2005\\SSIS2005\\UT Basic Scenario.dtsx\" task=\"SELECT COUNT\" taskResult=\"Success\">";
            _xmlTest += "<Assert name=\"Test\" expectedResult=\"100\" testBefore=\"false\" expression=\"false\">";
            _xmlTest += "<SqlCommand connectionRef=\"AdventureWorks\" returnsValue=\"true\">";
            _xmlTest += "SELECT COUNT(*) FROM Production.Product";
            _xmlTest += "</SqlCommand>";
            _xmlTest += "</Assert>";
            _xmlTest += "</Test>";

            _xmlTestFull = "<Test name=\"Test\" package=\"C:\\Projects\\SSISUnit\\SSIS2005\\SSIS2005\\UT Basic Scenario.dtsx\" task=\"SELECT COUNT\" taskResult=\"Success\">";
            _xmlTestFull += "<TestSetup>";
            _xmlTestFull += "<SqlCommand connectionRef=\"Sandbox\" returnsValue=\"false\">";
            _xmlTestFull += "INSERT INTO UTTable VALUES('Test')";
            _xmlTestFull += "</SqlCommand>";
            _xmlTestFull += "</TestSetup>";
            _xmlTestFull += "<Assert name=\"Test\" expectedResult=\"100\" testBefore=\"false\" expression=\"false\">";
            _xmlTestFull += "<SqlCommand connectionRef=\"AdventureWorks\" returnsValue=\"true\">";
            _xmlTestFull += "SELECT COUNT(*) FROM Production.Product";
            _xmlTestFull += "</SqlCommand>";
            _xmlTestFull += "</Assert>";
            _xmlTestFull += "<TestTeardown>";
            _xmlTestFull += "<SqlCommand connectionRef=\"Sandbox\" returnsValue=\"false\">";
            _xmlTestFull += "DELETE FROM UTTable";
            _xmlTestFull += "</SqlCommand>";
            _xmlTestFull += "</TestTeardown>";
            _xmlTestFull += "</Test>";
        }
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        [TestMethod()]
        public void NewTestTest()
        {
            SsisTestSuite target = new SsisTestSuite();
            Test ssisTest = new Test(target, "Test", "C:\\Projects\\SSISUnit\\SSIS2005\\SSIS2005\\UT Basic Scenario.dtsx", "SELECT COUNT");
            target.Tests.Add("Test", ssisTest);

            Assert.AreEqual<int>(1, target.Tests.Count);
            Assert.AreEqual<string>("Test", target.Tests["Test"].Name);
            Assert.AreEqual<string>("C:\\Projects\\SSISUnit\\SSIS2005\\SSIS2005\\UT Basic Scenario.dtsx", target.Tests["Test"].PackageLocation);
            Assert.AreEqual<string>("SELECT COUNT", target.Tests["Test"].Task);
        }

        /// <summary>
        ///A test for PersistToXml
        ///</summary>
        [TestMethod()]
        public void PersistToXmlTest()
        {
            SsisTestSuite testSuite = new SsisTestSuite();
            Test target = new Test(testSuite, "Test", "C:\\Projects\\SSISUnit\\SSIS2005\\SSIS2005\\UT Basic Scenario.dtsx", "SELECT COUNT");
            target.Asserts.Add("Test", new SsisAssert(testSuite, "Test", 100, false));
            target.Asserts["Test"].Command = new SqlCommand(testSuite, "AdventureWorks", true, "SELECT COUNT(*) FROM Production.Product");


            string actual = target.PersistToXml();
            Assert.AreEqual(_xmlTest, actual);
        }

        /// <summary>
        ///A test for LoadFromXml
        ///</summary>
        [TestMethod()]
        public void LoadFromXmlTest1()
        {
            SsisTestSuite testSuite = new SsisTestSuite();
            Test target = new Test(testSuite, "", "", "");
            target.LoadFromXml(_xmlTest);

            Assert.AreEqual<string>(_xmlTest, target.PersistToXml());
        }

        /// <summary>
        ///A test for LoadFromXml
        ///</summary>
        [TestMethod()]
        public void LoadFromXmlTest()
        {
            SsisTestSuite testSuite = new SsisTestSuite();
            Test target = new Test(testSuite, "", "", "");
            XmlNode node = ssisUnit_UTHelper.GetXmlNodeFromString(_xmlTest);
            target.LoadFromXml(node);
            Assert.AreEqual<string>(_xmlTest, target.PersistToXml());
        }

        [TestMethod()]
        public void LoadFromXmlTestFull()
        {
            SsisTestSuite testSuite = new SsisTestSuite();
            Test target = new Test(testSuite, "", "", "");
            XmlNode node = ssisUnit_UTHelper.GetXmlNodeFromString(_xmlTestFull);
            target.LoadFromXml(node);
            Assert.AreEqual<string>(_xmlTestFull, target.PersistToXml());
        }

        /// <summary>
        ///A test for Test Constructor
        ///</summary>
        [TestMethod()]
        public void TestConstructorTest()
        {
            string name = "Test";
            string package = "C:\\Projects\\SSISUnit\\SSIS2005\\SSIS2005\\UT Basic Scenario.dtsx";
            string task = "SELECT COUNT";
            Test target = new Test(new SsisTestSuite(), name, package, task);

            Assert.AreEqual<string>("Test", target.Name);
            Assert.AreEqual<string>("C:\\Projects\\SSISUnit\\SSIS2005\\SSIS2005\\UT Basic Scenario.dtsx", target.PackageLocation);
            Assert.AreEqual<string>("SELECT COUNT", target.Task);
        }

        [TestMethod()]
        public void TaskThatFailsTest()
        {
            SsisTestSuite ts = new SsisTestSuite();
            Test target = new Test(ts, "Test Task That Fails", "C:\\Projects\\SSISUnit\\UTssis2008packages\\UT Basic Scenario.dtsx", "SELECT COUNT", DTSExecResult.Failure);
            target.TestSetup.Commands.Add(new PropertyCommand(ts, "Set", "\\Package\\SELECT COUNT.Properties[SqlStatementSource]", "SELECT ''"));
            ts.Tests.Add("Test Task That Fails", target);
            SsisAssert assert = new SsisAssert(ts, "Test Row Count", 504, false, false);
            target.Asserts.Add("Test Row Count", assert);
            assert.Command = new VariableCommand(ts, VariableCommand.VariableOperation.Get, "User::ProductRowCount", null);
            ts.Execute();
            Assert.AreEqual<int>(1, ts.Statistics.GetStatistic(TestSuiteResults.StatisticEnum.AssertPassedCount));
            Assert.AreEqual<int>(1, ts.Statistics.GetStatistic(TestSuiteResults.StatisticEnum.AssertFailedCount));

        }
        [TestMethod()]
        public void DataFlowTaskWithExpressionTest()
        {
            SsisTestSuite ts = new SsisTestSuite();
            Test target = new Test(ts, "DataFlowExpression", "C:\\Projects\\SSISUnit\\UTssis2008packages\\TestDataFlowExpression.dtsx", "Data Flow Task");
            ts.Tests.Add("Test Task That Fails", target);
            SsisAssert assert = new SsisAssert(ts, "Test Anything", "true==true", false, true);
            target.Asserts.Add("Test Row Count", assert);
            assert.Command = new PropertyCommand(ts, "Get", "\\Package.Properties[Description]", null);
            ts.Save("c:\\temp\\ut_file.ssisUnit");
            ts.Execute();
            Assert.AreEqual<int>(1, ts.Statistics.GetStatistic(TestSuiteResults.StatisticEnum.AssertPassedCount));
            Assert.AreEqual<int>(1, ts.Statistics.GetStatistic(TestSuiteResults.StatisticEnum.AssertFailedCount));

        }
    }
}
