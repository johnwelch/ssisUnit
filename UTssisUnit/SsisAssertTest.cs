using SsisUnit;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml;
using System;

namespace UTssisUnit
{


    /// <summary>
    ///This is a test class for SsisAssertTest and is intended
    ///to contain all SsisAssertTest Unit Tests
    ///</summary>
    [TestClass()]
    public class SsisAssertTest
    {


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
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for LoadFromXml
        ///</summary>
        [TestMethod()]
        public void LoadFromXmlTest1()
        {
            SsisTestSuite testSuite = new SsisTestSuite();
            string assertXml = "<Assert name=\"Test\" expectedResult=\"1\" testBefore=\"false\" expression=\"false\">";
            assertXml += "<SqlCommand connectionRef=\"AdventureWorks\" returnsValue=\"true\">";
            assertXml += "SELECT COUNT(*) FROM Production.Product";
            assertXml += "</SqlCommand>";
            assertXml += "</Assert>";
            XmlNode assertXml1 = ssisUnit_UTHelper.GetXmlNodeFromString(assertXml);
            SsisAssert target = new SsisAssert(testSuite, "", null, true);
            target.LoadFromXml(assertXml1);
            Assert.AreEqual<string>(assertXml, target.PersistToXml());
        }

        /// <summary>
        ///A test for LoadFromXml
        ///</summary>
        [TestMethod()]
        public void LoadFromXmlTest()
        {
            string assertXml = "<Assert name=\"Test\" expectedResult=\"1\" testBefore=\"false\" expression=\"false\">";
            assertXml += "<SqlCommand connectionRef=\"AdventureWorks\" returnsValue=\"true\">";
            assertXml += "SELECT COUNT(*) FROM Production.Product";
            assertXml += "</SqlCommand>";
            assertXml += "</Assert>";

            SsisTestSuite testSuite = new SsisTestSuite();
            SsisAssert target = new SsisAssert(testSuite, "", null, true);
            target.LoadFromXml(assertXml);
            Assert.AreEqual<string>(assertXml, target.PersistToXml());
        }

        /// <summary>
        ///A test for SsisAssert Constructor
        ///</summary>
        [TestMethod()]
        public void SsisAssertConstructorTest()
        {
            SsisTestSuite testSuite = new SsisTestSuite();
            string name = "Test";
            object expectedResult = 100;
            bool testBefore = false;
            Test ssisTest = new Test(testSuite, "Test", "C:\\Projects\\SSISUnit\\SSIS2005\\SSIS2005\\UT Basic Scenario.dtsx", "SELECT COUNT");
            testSuite.Tests.Add("Test", ssisTest);
            SsisAssert target = new SsisAssert(testSuite, name, expectedResult, testBefore);
            ssisTest.Asserts.Add("Test", target);
            Assert.AreEqual<int>(1, ssisTest.Asserts.Count);
            Assert.AreEqual<string>("Test", ssisTest.Asserts["Test"].Name);
            Assert.AreEqual(100, ssisTest.Asserts["Test"].ExpectedResult);
            Assert.AreEqual<bool>(false, ssisTest.Asserts["Test"].TestBefore);
         }

        /// <summary>
        ///A test for PersistToXml
        ///</summary>
        [TestMethod()]
        public void PersistToXmlTest()
        {
            string assertXml = "<Assert name=\"Test\" expectedResult=\"1\" testBefore=\"false\" expression=\"false\">";
            assertXml += "<SqlCommand connectionRef=\"AdventureWorks\" returnsValue=\"true\">";
            assertXml += "SELECT COUNT(*) FROM Production.Product";
            assertXml += "</SqlCommand>";
            assertXml += "</Assert>";

            SsisTestSuite testSuite = new SsisTestSuite();
            SsisAssert target = new SsisAssert(testSuite, assertXml);

            string expected = assertXml;
            string actual;
            actual = target.PersistToXml();
            Assert.AreEqual(assertXml, actual);
        }

        /// <summary>
        ///A test for SsisAssert Constructor
        ///</summary>
        [TestMethod()]
        public void SsisAssertConstructorTest1()
        {
            SsisTestSuite testSuite = new SsisTestSuite();
            string assertXml = "<Assert name=\"Test\" expectedResult=\"1\" testBefore=\"false\" expression=\"false\">";
            assertXml += "<SqlCommand connectionRef=\"AdventureWorks\" returnsValue=\"true\">";
            assertXml += "SELECT COUNT(*) FROM Production.Product";
            assertXml += "</SqlCommand>";
            assertXml += "</Assert>";
            XmlNode assertXml1 = ssisUnit_UTHelper.GetXmlNodeFromString(assertXml);
            SsisAssert target = new SsisAssert(testSuite, assertXml1);
            Assert.AreEqual<string>("Test", target.Name);
            Assert.AreEqual<string>("1", target.ExpectedResult.ToString());
            Assert.AreEqual<bool>(false, target.TestBefore);
            Assert.AreEqual<string>("SqlCommand", target.Command.CommandName);
        }

        /// <summary>
        ///A test for SsisAssert Constructor
        ///</summary>
        [TestMethod()]
        public void SsisAssertConstructorTest2()
        {
            SsisTestSuite testSuite = new SsisTestSuite();
            string assertXml = "<Assert name=\"Test\" expectedResult=\"1\" testBefore=\"false\" expression=\"false\">";
            assertXml += "<SqlCommand connectionRef=\"AdventureWorks\" returnsValue=\"true\">";
            assertXml += "SELECT COUNT(*) FROM Production.Product";
            assertXml += "</SqlCommand>";
            assertXml += "</Assert>";
            SsisAssert target = new SsisAssert(testSuite, assertXml);
            Assert.AreEqual<string>("Test", target.Name);
            Assert.AreEqual<string>("1", target.ExpectedResult.ToString());
            Assert.AreEqual<bool>(false, target.TestBefore);
            Assert.AreEqual<string>("SqlCommand", target.Command.CommandName);
        }

        [TestMethod()]
        public void TestDynamicAssert()
        {
            SsisTestSuite target = new SsisTestSuite();

            //Add connection ref
            target.ConnectionRefs.Add("AdventureWorks", new ConnectionRef("AdventureWorks",
                "Provider=SQLNCLI;Data Source=localhost;Integrated Security=SSPI;Initial Catalog=Adventureworks",
                ConnectionRef.ConnectionTypeEnum.ConnectionString));
            //Add package ref
            target.PackageRefs.Add("UT Basic Scenario", new PackageRef("UT Basic Scenario", "C:\\Projects\\SSISUnit\\SSIS2005\\SSIS2005\\UT Basic Scenario.dtsx", PackageRef.PackageStorageType.FileSystem));

            target.TestSuiteSetup.Commands.Add(new SqlCommand(target, "AdventureWorks", false, "CREATE TABLE dbo.Test (ID INT)"));
            target.TestSuiteSetup.Commands.Add(new SqlCommand(target, "AdventureWorks", false, "INSERT INTO dbo.Test VALUES (1)"));
            target.TestSuiteSetup.Commands.Add(new SqlCommand(target, "AdventureWorks", false, "INSERT INTO dbo.Test VALUES (2)"));

            Test ssisTest = new Test(target, "Test", "UT Basic Scenario", "SELECT COUNT");
            target.Tests.Add("Test", ssisTest);

            SsisAssert ssisAssert = new SsisAssert(target, "Test Count", "(int)result==2", false, true);
            ssisAssert.Command = new SqlCommand(target, "AdventureWorks", true, "SELECT COUNT(*) FROM dbo.Test");

            ssisTest.Asserts.Add("Test Count", ssisAssert);

            ssisAssert = new SsisAssert(target, "Test Count 2", "(int)result<=2", false, true);
            ssisAssert.Command = new SqlCommand(target, "AdventureWorks", true, "SELECT COUNT(*) FROM dbo.Test");

            ssisTest.Asserts.Add("Test Count 2", ssisAssert);

            ssisAssert = new SsisAssert(target, "Test Count 3", "\"test\"==\"test\"", false, true);
            ssisAssert.Command = new SqlCommand(target, "AdventureWorks", true, "SELECT COUNT(*) FROM dbo.Test");

            ssisTest.Asserts.Add("Test Count 3", ssisAssert);

            ssisAssert = new SsisAssert(target, "Test Count 4", "DateTime.Now.Date==((DateTime)result).Date", false, true);
            ssisAssert.Command = new VariableCommand(target, VariableCommand.VariableOperation.Get, "System::StartTime", string.Empty);

            ssisTest.Asserts.Add("Test Count 4", ssisAssert);


            target.TestSuiteTeardown.Commands.Add(new SqlCommand(target, "AdventureWorks", false, "DROP TABLE dbo.Test"));

            int testCount = 0;

            try
            {
                testCount = target.Execute();
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }

            Assert.AreEqual<int>(1, testCount);
            Assert.AreEqual<int>(1, target.Statistics.GetStatistic(TestSuiteResults.StatisticEnum.TestPassedCount));
            Assert.AreEqual<int>(4, target.Statistics.GetStatistic(TestSuiteResults.StatisticEnum.AssertPassedCount));
            Assert.AreEqual<int>(0, target.Statistics.GetStatistic(TestSuiteResults.StatisticEnum.AssertFailedCount));

        }

        [TestMethod()]
        public void TestCompileFail()
        {
            SsisTestSuite target = new SsisTestSuite();

            //Add connection ref
            target.ConnectionRefs.Add("AdventureWorks", new ConnectionRef("AdventureWorks",
                "Provider=SQLNCLI;Data Source=localhost;Integrated Security=SSPI;Initial Catalog=Adventureworks",
                ConnectionRef.ConnectionTypeEnum.ConnectionString));
            //Add package ref
            target.PackageRefs.Add("UT Basic Scenario", new PackageRef("UT Basic Scenario", "C:\\Projects\\SSISUnit\\SSIS2005\\SSIS2005\\UT Basic Scenario.dtsx", PackageRef.PackageStorageType.FileSystem));

            target.TestSuiteSetup.Commands.Add(new SqlCommand(target, "AdventureWorks", false, "CREATE TABLE dbo.Test (ID INT)"));
            target.TestSuiteSetup.Commands.Add(new SqlCommand(target, "AdventureWorks", false, "INSERT INTO dbo.Test VALUES (1)"));
            target.TestSuiteSetup.Commands.Add(new SqlCommand(target, "AdventureWorks", false, "INSERT INTO dbo.Test VALUES (2)"));

            Test ssisTest = new Test(target, "Test", "UT Basic Scenario", "SELECT COUNT");
            target.Tests.Add("Test", ssisTest);

            SsisAssert ssisAssert = new SsisAssert(target, "Test Count", "result==2", false);
            ssisAssert.Command = new SqlCommand(target, "AdventureWorks", true, "SELECT COUNT(*) FROM dbo.Test");

            ssisTest.Asserts.Add("Test Count", ssisAssert);

            target.TestSuiteTeardown.Commands.Add(new SqlCommand(target, "AdventureWorks", false, "DROP TABLE dbo.Test"));

            int testCount = 0;

            try
            {
                testCount = target.Execute();
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }

            Assert.AreEqual<int>(1, testCount);
            Assert.AreEqual<int>(1, target.Statistics.GetStatistic(TestSuiteResults.StatisticEnum.TestPassedCount));
            Assert.AreEqual<int>(0, target.Statistics.GetStatistic(TestSuiteResults.StatisticEnum.AssertPassedCount));
            Assert.AreEqual<int>(1, target.Statistics.GetStatistic(TestSuiteResults.StatisticEnum.AssertFailedCount));

        }

        [TestMethod()]
        public void TestAssertCreation()
        {

            string assertXml = "<Assert name=\"Test\" expectedResult=\"\" testBefore=\"false\" expression=\"false\" ";
            assertXml += "/>";

            SsisTestSuite testSuite = new SsisTestSuite();
            SsisAssert ssisAssert = new SsisAssert(testSuite, "Test", null, false, false);

            string expected = assertXml;
            string actual;
            actual = ssisAssert.PersistToXml();
            Assert.AreEqual(assertXml, actual);
        }
    }
}
