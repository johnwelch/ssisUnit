using SsisUnit;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;
using System.Xml;
using System.Collections.Generic;
using Microsoft.SqlServer.Dts.Runtime;
using System;
using System.IO;
namespace UTssisUnit
{


    /// <summary>
    ///This is a test class for SSISTestCaseTest and is intended
    ///to contain all SSISTestCaseTest Unit Tests
    ///</summary>
    [TestClass()]
    public class SSISTestCaseTest
    {
        private const string TEST_XML_FILENAME = "UTSsisUnit.xml";

        private const string TEST_XML_FILE_PATH = "C:\\Projects\\SSISUnit\\UTssisUnit\\UTSsisUnit.xml";
        private const string TEST_XML_FILE_BAD_DATA_PATH = "C:\\Projects\\SSISUnit\\UTssisUnit\\UTSsisUnit_BadData.xml";
        private const string TEST_DTSX_FILE_PATH = "C:\\Projects\\SSISUnit\\SSIS2005\\SSIS2005\\UT Basic Scenario.dtsx";

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

        [ClassCleanup()]
        public static void Cleanup()
        {
            ssisUnit_UTHelper.Cleanup();
        }

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
        ///A test for SSISTestCase Constructor
        ///</summary>
        [TestMethod()]
        public void SSISTestCaseConstructorTest()
        {
            SsisTestSuite target = new SsisTestSuite(TEST_XML_FILE_PATH);
            Assert.IsNotNull(target);
        }

        [TestMethod()]
        public void LoadXMLFileTest()
        {
            string filename = ssisUnit_UTHelper.CreateUnitTestFile("UTSsisUnit");
            XmlDocument doc;
            doc = SsisTestSuite.LoadTestXmlFromFile(filename);
            Assert.IsNotNull(doc);
        }

        [TestMethod()]
        public void LoadInvalidXMLFileTest()
        {
            XmlDocument doc;

            try
            {
                doc = SsisTestSuite.LoadTestXmlFromFile("C:\\Projects\\SSISUnit\\SSIS2005\\SSIS2005\\UT_Simple.dtsx");
                Assert.Fail("Expected argument exception not thrown.");
            }
            catch (ArgumentException)
            {
                Assert.IsTrue(true);
            }
        }


        [TestMethod()]
        public void ExecuteTest()
        {
            SsisTestSuite target = new SsisTestSuite(ssisUnit_UTHelper.CreateUnitTestStream(TEST_XML_FILENAME));
            try
            {
                target.Execute();
                Assert.IsTrue(true);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod()]
        public void SetupTest()
        {
            SsisTestSuite target = new SsisTestSuite(ssisUnit_UTHelper.CreateUnitTestStream(TEST_XML_FILENAME));
            XmlDocument doc = new XmlDocument();
            doc.Load(ssisUnit_UTHelper.CreateUnitTestStream(TEST_XML_FILENAME));
            XmlNode setup = doc.DocumentElement["Setup"];
            Application ssisApp = new Application();
            Package packageToTest = ssisApp.LoadPackage(TEST_DTSX_FILE_PATH, null);
            DtsContainer task = SsisTestSuite.FindExecutable(packageToTest, "SELECT COUNT");

            int result = target.Setup(setup, packageToTest, task);
            Assert.AreEqual(4, result);
        }

        [TestMethod()]
        public void TeardownTest()
        {
            SsisTestSuite target = new SsisTestSuite(ssisUnit_UTHelper.CreateUnitTestStream(TEST_XML_FILENAME));
            XmlDocument doc = new XmlDocument();
            doc.Load(TEST_XML_FILE_PATH);
            XmlNode teardown = doc.DocumentElement["Teardown"];
            Application ssisApp = new Application();
            Package packageToTest = ssisApp.LoadPackage(TEST_DTSX_FILE_PATH, null);
            DtsContainer task = SsisTestSuite.FindExecutable(packageToTest, "SELECT COUNT");

            int result = target.Teardown(teardown, packageToTest, task);
            Assert.AreEqual(2, result);
        }

        [TestMethod()]
        public void RunTestTest()
        {
            SsisTestSuite target = new SsisTestSuite(ssisUnit_UTHelper.CreateUnitTestStream(TEST_XML_FILENAME));

            XmlDocument doc = new XmlDocument();
            doc.Load(TEST_XML_FILE_PATH);
            XmlNode test = doc.DocumentElement["Tests"].ChildNodes[0];
            //target.InitializeTestCase(TEST_XML_FILE_PATH);

            bool result = target.Test(test);
            Assert.IsTrue(result);
        }

        [TestMethod()]
        public void RunPackageListTest()
        {
            SsisTestSuite target = new SsisTestSuite("C:\\Projects\\SSISUnit\\UTssisUnit\\UTSsisUnit_PackageList.xml");

            try
            {
                target.Execute();
                Assert.IsTrue(true);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod()]
        public void RunTestRef()
        {
            SsisTestSuite target = new SsisTestSuite("C:\\Projects\\SSISUnit\\UTssisUnit\\UTSsisUnitParent.xml");

            try
            {
                target.Execute();
                Assert.IsTrue(true);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod()]
        public void TestExecuteWithParent()
        {
            SsisTestSuite target = new SsisTestSuite(TEST_XML_FILE_PATH);

            XmlDocument doc = new XmlDocument();
            doc.Load(TEST_XML_FILE_PATH);
            XmlNode test = doc.DocumentElement["Tests"]["TestRef"];

            try
            {
                target.RunTestSuite(test);
                Assert.IsTrue(true);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod()]
        public void TestSuiteSetupTest()
        {
            SsisTestSuite target = new SsisTestSuite("C:\\Projects\\SSISUnit\\UTssisUnit\\UTssisUnit_TestSuite.xml");

            try
            {
                target.Execute();
                Assert.IsTrue(true);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod()]
        public void RunFindExecutableByNameTest()
        {
            Application ssisApp = new Application();

            Package packageToTest = ssisApp.LoadPackage(TEST_DTSX_FILE_PATH, null);

            DtsContainer result = SsisTestSuite.FindExecutable(packageToTest, "SELECT COUNT");
            Assert.IsNotNull(result);
        }

        [TestMethod()]
        public void RunFindExecutableByIDTest()
        {
            Application ssisApp = new Application();

            Package packageToTest = ssisApp.LoadPackage(TEST_DTSX_FILE_PATH, null);

            DtsContainer result = SsisTestSuite.FindExecutable(packageToTest, "{64E40F9C-FC42-4AE8-AEDF-C99909861EED}");
            Assert.IsNotNull(result);
        }

        [TestMethod()]
        public void RunFindExecutable2ByIDTest()
        {
            Application ssisApp = new Application();

            Package packageToTest = ssisApp.LoadPackage(TEST_DTSX_FILE_PATH, null);

            DtsContainer result = SsisTestSuite.FindExecutable(packageToTest, "{64E40F9C-FC42-4AE8-AEDF-C99909861EED}");
            Assert.IsNotNull(result);
        }

        [TestMethod()]
        public void FindExecutable2ByPackageIDTest()
        {
            Application ssisApp = new Application();

            Package packageToTest = ssisApp.LoadPackage(TEST_DTSX_FILE_PATH, null);

            DtsContainer result = SsisTestSuite.FindExecutable(packageToTest, "{D2D9295A-45D0-4681-B021-F5077CB2EC22}");
            Assert.IsNotNull(result);
        }

        [TestMethod()]
        public void FindExecutable2DoesNotExistTest()
        {
            Application ssisApp = new Application();

            Package packageToTest = ssisApp.LoadPackage(TEST_DTSX_FILE_PATH, null);

            DtsContainer result = SsisTestSuite.FindExecutable(packageToTest, "Does Not Exist");
            Assert.IsNull(result);
        }

        [TestMethod()]
        public void TestSetupAndTeardownTest()
        {
            SsisTestSuite target = new SsisTestSuite("C:\\Projects\\SSISUnit\\UTssisUnit\\UTSsisUnit_TestSetup_Teardown.xml");

            try
            {
                target.Execute();
                Assert.IsTrue(true);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }
        [TestMethod()]
        public void CreateNewTestSuiteTest()
        {
            SsisTestSuite target = new SsisTestSuite();

            Assert.AreEqual(0, target.ConnectionRefs.Count);

            target.ConnectionRefs.Add("AdventureWorks", new ConnectionRef("AdventureWorks",
                "Provider=SQLOLEDB;Data Source=localhost;Integrated Security=SSPI;Initial Catalog=Adventureworks",
                ConnectionRef.ConnectionTypeEnum.ConnectionString));

            Assert.AreEqual<int>(1, target.ConnectionRefs.Count);
            Assert.AreEqual<string>("AdventureWorks", target.ConnectionRefs["AdventureWorks"].ReferenceName);
            Assert.AreEqual<string>("Provider=SQLOLEDB;Data Source=localhost;Integrated Security=SSPI;Initial Catalog=Adventureworks", target.ConnectionRefs["AdventureWorks"].ConnectionString);
            Assert.AreEqual<ConnectionRef.ConnectionTypeEnum>(ConnectionRef.ConnectionTypeEnum.ConnectionString, target.ConnectionRefs["AdventureWorks"].ConnectionType);

            //TODO: TestSuiteSetup
            //TODO: Setup

            Assert.AreEqual<int>(0, target.Tests.Count);
            Test ssisTest = new Test("Test", "C:\\Projects\\SSISUnit\\SSIS2005\\SSIS2005\\UT Basic Scenario.dtsx", "SELECT COUNT");
            target.Tests.Add("Test", ssisTest);

            Assert.AreEqual<int>(1, target.Tests.Count);
            Assert.AreEqual<string>("Test", target.Tests["Test"].Name);
            Assert.AreEqual<string>("C:\\Projects\\SSISUnit\\SSIS2005\\SSIS2005\\UT Basic Scenario.dtsx", target.Tests["Test"].Package);
            Assert.AreEqual<string>("SELECT COUNT", target.Tests["Test"].Task);

            //TODO: TestSetup

            SsisAssert ssisAssert = new SsisAssert("Test Count", 504, false);
            ssisAssert.Command = new SqlCommand("AdventureWorks", true, "SELECT COUNT(*) FROM Production.Product");

            ssisTest.Asserts.Add("Test Count", ssisAssert);
            Assert.AreEqual<int>(1, ssisTest.Asserts.Count);
            Assert.AreEqual<string>("Test Count", ssisTest.Asserts["Test Count"].Name);
            Assert.AreEqual(504, ssisTest.Asserts["Test Count"].ExpectedResult);
            Assert.AreEqual<bool>(false, ssisTest.Asserts["Test Count"].TestBefore);
            Assert.AreEqual<string>("<SqlCommand connectionRef=\"AdventureWorks\" returnsValue=\"true\">SELECT COUNT(*) FROM Production.Product</SqlCommand>", ssisTest.Asserts["Test Count"].Command.PersistToXml());

            //TODO: TestTeardown


            //TODO: Teardown
            //TODO: TestSuiteTeardown

            int testCount = 0;

            try
            {
                testCount = target.Execute();
                Assert.AreEqual<int>(1, testCount);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }
    }
}
