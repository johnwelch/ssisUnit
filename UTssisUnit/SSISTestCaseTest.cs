using SsisUnit;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;
using System.Xml;
using System.Collections.Generic;
using Microsoft.SqlServer.Dts.Runtime;
using System;
namespace UTssisUnit
{


    /// <summary>
    ///This is a test class for SSISTestCaseTest and is intended
    ///to contain all SSISTestCaseTest Unit Tests
    ///</summary>
    [TestClass()]
    public class SSISTestCaseTest
    {
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
            XmlDocument doc;
            doc = SsisTestSuite.LoadTestXmlFromFile(TEST_XML_FILE_PATH);
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
            SsisTestSuite target = new SsisTestSuite(TEST_XML_FILE_PATH);
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
            SsisTestSuite target = new SsisTestSuite(TEST_XML_FILE_PATH);
            XmlDocument doc = new XmlDocument();
            doc.Load(TEST_XML_FILE_PATH);
            XmlNode setup = doc.DocumentElement["Setup"];
            Application ssisApp = new Application();
            Package packageToTest = ssisApp.LoadPackage(TEST_DTSX_FILE_PATH, null);
            DtsContainer task = SsisTestSuite.FindExecutable(packageToTest, "SELECT COUNT");

            int result = target.Setup(setup, packageToTest, task);
            Assert.AreEqual(6, result);
        }

        [TestMethod()]
        public void TeardownTest()
        {
            SsisTestSuite target = new SsisTestSuite(TEST_XML_FILE_PATH);
            XmlDocument doc = new XmlDocument();
            doc.Load(TEST_XML_FILE_PATH);
            XmlNode teardown = doc.DocumentElement["Teardown"];
            Application ssisApp = new Application();
            Package packageToTest = ssisApp.LoadPackage(TEST_DTSX_FILE_PATH, null);
            DtsContainer task = SsisTestSuite.FindExecutable(packageToTest, "SELECT COUNT");

            int result = target.Teardown(teardown, packageToTest, task);
            Assert.AreEqual(2, result);
        }

        //[TestMethod()]
        //public void RunSQLCommandResultsTest()
        //{
        //    SsisTestSuite target = new SsisTestSuite(TEST_XML_FILE_PATH);

        //    XmlDocument doc = new XmlDocument();
        //    doc.Load(TEST_XML_FILE_PATH);
        //    //target.InitializeTestCase(TEST_XML_FILE_PATH);
        //    XmlNode command = doc.DocumentElement["Setup"]["SqlCommand"];

        //    object result = target.RunSQLCommand(command);
        //    Assert.AreEqual(504, (int)result);
        //}

        //[TestMethod()]
        //public void RunSQLCommandNoResultsTest()
        //{
        //    SsisTestSuite target = new SsisTestSuite(TEST_XML_FILE_PATH);

        //    XmlDocument doc = new XmlDocument();
        //    doc.Load(TEST_XML_FILE_PATH);
        //    XmlNode command = doc.DocumentElement["Setup"].ChildNodes[1];

        //    object result = target.RunSQLCommand(command);
        //    Assert.IsNull(result);
        //}

        //[TestMethod()]
        //public void RunSQLCommandInvalidConnectionRefTest()
        //{
        //    SsisTestSuite target = new SsisTestSuite(TEST_XML_FILE_BAD_DATA_PATH);

        //    XmlDocument doc = new XmlDocument();
        //    doc.Load(TEST_XML_FILE_BAD_DATA_PATH);
        //    XmlNode command = doc.DocumentElement["Setup"].ChildNodes[0];

        //    try
        //    {
        //        object result = target.RunSQLCommand(command);
        //        Assert.Fail("The method did not throw the expected argument exception.");
        //    }
        //    catch (System.ArgumentException)
        //    {
        //        Assert.IsTrue(true);
        //    }
        //    catch (System.Exception)
        //    {
        //        Assert.Fail("The method did not throw the expected argument exception.");
        //    }

        //}

        [TestMethod()]
        public void RunTestTest()
        {
            SsisTestSuite target = new SsisTestSuite(TEST_XML_FILE_PATH);

            XmlDocument doc = new XmlDocument();
            doc.Load(TEST_XML_FILE_PATH);
            XmlNode test = doc.DocumentElement["Tests"].ChildNodes[0];
            //target.InitializeTestCase(TEST_XML_FILE_PATH);

            bool result = target.Test(test);
            Assert.IsTrue(result);
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
    }
}
