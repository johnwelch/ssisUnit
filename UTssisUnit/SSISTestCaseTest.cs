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
        private const string TEST_XML_FILENAME = "UTSsisUnit.ssisUnit";

        //private const string TEST_XML_FILE_PATH = "C:\\Projects\\SSISUnit\\UTssisUnit\\UTSsisUnit.ssisUnit";
        //private const string TEST_XML_FILE_BAD_DATA_PATH = "C:\\Projects\\SSISUnit\\UTssisUnit\\UTSsisUnit_BadData.xml";
        //TODO: Abstract references to packages
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
            SsisTestSuite target = new SsisTestSuite(ssisUnit_UTHelper.CreateUnitTestStream(TEST_XML_FILENAME));
            Assert.IsNotNull(target);
        }

        [TestMethod()]
        public void LoadXMLFileTest()
        {
            string filename = ssisUnit_UTHelper.CreateUnitTestFile("UTSsisUnit");
            SsisTestSuite ts = new SsisTestSuite(filename);
            Assert.IsNotNull(ts);
        }

        [TestMethod()]
        public void LoadInvalidXMLFileTest()
        {
            try
            {
                SsisTestSuite ts = new SsisTestSuite(ssisUnit_UTHelper.CreateUnitTestStream("UT_BadTestFile.ssisUnit"));
                Assert.Fail("Test Case was not reported as invalid.");
            }
            catch (ApplicationException ex)
            {
                Assert.AreEqual<string>("The unit test file is malformed or corrupt. Please verify that the file format conforms to the ssisUnit schema, provided in the SsisUnit.xsd file.", ex.Message);
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
            Application ssisApp = new Application();
            Package packageToTest = ssisApp.LoadPackage(TEST_DTSX_FILE_PATH, null);
            DtsContainer task = ssisUnit_UTHelper.FindExecutable(packageToTest, "SELECT COUNT");

            int result = target.SetupCommands.Execute(packageToTest, task);
            Assert.AreEqual(4, result);
        }

        [TestMethod()]
        public void TeardownTest()
        {
            SsisTestSuite target = new SsisTestSuite(ssisUnit_UTHelper.CreateUnitTestStream(TEST_XML_FILENAME));
            Application ssisApp = new Application();
            Package packageToTest = ssisApp.LoadPackage(TEST_DTSX_FILE_PATH, null);
            DtsContainer task = ssisUnit_UTHelper.FindExecutable(packageToTest, "SELECT COUNT");

            int result = target.TeardownCommands.Execute(packageToTest, task);
            Assert.AreEqual(2, result);
        }

        [TestMethod()]
        public void RunTestTest()
        {
            SsisTestSuite target = new SsisTestSuite(ssisUnit_UTHelper.CreateUnitTestStream(TEST_XML_FILENAME));


            //XmlDocument doc = new XmlDocument();
            //doc.Load(TEST_XML_FILE_PATH);
            //XmlNode test = doc.DocumentElement["Tests"].ChildNodes[0];
            //target.InitializeTestCase(TEST_XML_FILE_PATH);

            //bool result = target.Test(test);
            bool result = target.Tests["PassedTestSQL"].Execute();
            Assert.IsTrue(result);
        }

        [TestMethod()]
        public void RunPackageListTest()
        {
            SsisTestSuite target = new SsisTestSuite(ssisUnit_UTHelper.CreateUnitTestStream("UTSsisUnit_PackageList.xml"));

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
            SsisTestSuite target = new SsisTestSuite(ssisUnit_UTHelper.CreateUnitTestStream("UTSsisUnitParent.xml"));

            try
            {
                target.Execute();
                Assert.AreEqual<int>(2, target.Statistics.GetStatistic(TestSuiteResults.StatisticEnum.AssertCount));
                Assert.AreEqual<int>(2, target.Statistics.GetStatistic(TestSuiteResults.StatisticEnum.AssertPassedCount));
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod()]
        public void TestExecuteWithParent()
        {
            SsisTestSuite target = new SsisTestSuite(ssisUnit_UTHelper.CreateUnitTestStream(TEST_XML_FILENAME));

            //XmlNode test = doc.DocumentElement["Tests"]["TestRef"];

            try
            {
                target.TestRefs["C:\\Projects\\SSISUnit\\UTssisUnit\\UTssisUnit_Package.xml"].Execute();
                //target.RunTestSuite(test);
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
            SsisTestSuite target = new SsisTestSuite(ssisUnit_UTHelper.CreateUnitTestStream("UTssisUnit_TestSuite.xml"));

            try
            {
                target.Execute();
                Assert.AreEqual(3, target.Statistics.GetStatistic(TestSuiteResults.StatisticEnum.TestCount));
                Assert.AreEqual(1, target.Statistics.GetStatistic(TestSuiteResults.StatisticEnum.AssertPassedCount));
                Assert.AreEqual(1, target.Statistics.GetStatistic(TestSuiteResults.StatisticEnum.TestPassedCount));
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

            DtsContainer result = ssisUnit_UTHelper.FindExecutable(packageToTest, "SELECT COUNT");
            Assert.IsNotNull(result);
        }

        [TestMethod()]
        public void RunFindExecutableByIDTest()
        {
            Application ssisApp = new Application();

            Package packageToTest = ssisApp.LoadPackage(TEST_DTSX_FILE_PATH, null);

            DtsContainer result = ssisUnit_UTHelper.FindExecutable(packageToTest, "{64E40F9C-FC42-4AE8-AEDF-C99909861EED}");
            Assert.IsNotNull(result);
        }

        [TestMethod()]
        public void RunFindExecutable2ByIDTest()
        {
            Application ssisApp = new Application();

            Package packageToTest = ssisApp.LoadPackage(TEST_DTSX_FILE_PATH, null);

            DtsContainer result = ssisUnit_UTHelper.FindExecutable(packageToTest, "{64E40F9C-FC42-4AE8-AEDF-C99909861EED}");
            Assert.IsNotNull(result);
        }

        [TestMethod()]
        public void FindExecutable2ByPackageIDTest()
        {
            Application ssisApp = new Application();

            Package packageToTest = ssisApp.LoadPackage(TEST_DTSX_FILE_PATH, null);

            DtsContainer result = ssisUnit_UTHelper.FindExecutable(packageToTest, "{D2D9295A-45D0-4681-B021-F5077CB2EC22}");
            Assert.IsNotNull(result);
        }

        [TestMethod()]
        public void FindExecutable2DoesNotExistTest()
        {
            Application ssisApp = new Application();

            Package packageToTest = ssisApp.LoadPackage(TEST_DTSX_FILE_PATH, null);

            DtsContainer result = ssisUnit_UTHelper.FindExecutable(packageToTest, "Does Not Exist");
            Assert.IsNull(result);
        }

        [TestMethod()]
        public void TestSetupAndTeardownTest()
        {
            SsisTestSuite target = new SsisTestSuite(ssisUnit_UTHelper.CreateUnitTestStream("UTSsisUnit_TestSetup_Teardown.xml"));

            try
            {
                target.Execute();
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
            Assert.AreEqual<int>(1, target.Statistics.GetStatistic(TestSuiteResults.StatisticEnum.AssertPassedCount));
            Assert.AreEqual<int>(1, target.Statistics.GetStatistic(TestSuiteResults.StatisticEnum.TestCount));

        }
        [TestMethod()]
        public void CreateNewTestSuiteTest()
        {
            SsisTestSuite target = new SsisTestSuite();

            Assert.AreEqual(0, target.ConnectionRefs.Count);

            //Add connection ref
            target.ConnectionRefs.Add("AdventureWorks", new ConnectionRef("AdventureWorks",
                "Provider=SQLNCLI;Data Source=localhost;Integrated Security=SSPI;Initial Catalog=Adventureworks",
                ConnectionRef.ConnectionTypeEnum.ConnectionString));

            Assert.AreEqual<int>(1, target.ConnectionRefs.Count);
            Assert.AreEqual<string>("AdventureWorks", target.ConnectionRefs["AdventureWorks"].ReferenceName);
            Assert.AreEqual<string>("Provider=SQLNCLI;Data Source=localhost;Integrated Security=SSPI;Initial Catalog=Adventureworks", target.ConnectionRefs["AdventureWorks"].ConnectionString);
            Assert.AreEqual<ConnectionRef.ConnectionTypeEnum>(ConnectionRef.ConnectionTypeEnum.ConnectionString, target.ConnectionRefs["AdventureWorks"].ConnectionType);

            //Add package ref
            target.PackageRefs.Add("UT Basic Scenario", new PackageRef("UT Basic Scenario", "C:\\Projects\\SSISUnit\\SSIS2005\\SSIS2005\\UT Basic Scenario.dtsx", PackageRef.PackageStorageType.FileSystem));

            target.TestSuiteSetup.Commands.Add(new SqlCommand(target, "AdventureWorks", false, "CREATE TABLE dbo.Test (ID INT)"));
            target.TestSuiteSetup.Commands.Add(new SqlCommand(target, "AdventureWorks", false, "INSERT INTO dbo.Test VALUES (1)"));
            target.TestSuiteSetup.Commands.Add(new SqlCommand(target, "AdventureWorks", false, "INSERT INTO dbo.Test VALUES (2)"));

            target.SetupCommands.Commands.Add(new FileCommand(target, "Copy", "C:\\Temp\\LineCount.txt", "C:\\Temp\\LineCount2.txt"));

            Assert.AreEqual<int>(0, target.Tests.Count);
            Test ssisTest = new Test(target, "Test", "UT Basic Scenario", "SELECT COUNT");
            target.Tests.Add("Test", ssisTest);

            Assert.AreEqual<int>(1, target.Tests.Count);
            Assert.AreEqual<string>("Test", target.Tests["Test"].Name);
            Assert.AreEqual<string>("UT Basic Scenario", target.Tests["Test"].PackageLocation);
            Assert.AreEqual<string>("SELECT COUNT", target.Tests["Test"].Task);

            target.Tests["Test"].TestSetup.Commands.Add(new FileCommand(target, "Copy", "C:\\Temp\\LineCount.txt", "C:\\Temp\\LineCount3.txt"));

            SsisAssert ssisAssert = new SsisAssert(target, "Test Count", 2, false);
            ssisAssert.Command = new SqlCommand(target, "AdventureWorks", true, "SELECT COUNT(*) FROM dbo.Test");

            ssisTest.Asserts.Add("Test Count", ssisAssert);
            Assert.AreEqual<int>(1, ssisTest.Asserts.Count);
            Assert.AreEqual<string>("Test Count", ssisTest.Asserts["Test Count"].Name);
            Assert.AreEqual(2, ssisTest.Asserts["Test Count"].ExpectedResult);
            Assert.AreEqual<bool>(false, ssisTest.Asserts["Test Count"].TestBefore);
            Assert.AreEqual<string>("<SqlCommand connectionRef=\"AdventureWorks\" returnsValue=\"true\">SELECT COUNT(*) FROM dbo.Test</SqlCommand>", ssisTest.Asserts["Test Count"].Command.PersistToXml());

            ssisAssert = new SsisAssert(target, "Test File", true, false);
            ssisAssert.Command = new FileCommand(target, "Exists", "C:\\Temp\\LineCount2.txt", string.Empty);
            ssisTest.Asserts.Add("Test File", ssisAssert);

            ssisAssert = new SsisAssert(target, "Test File 2", true, false);
            ssisAssert.Command = new FileCommand(target, "Exists", "C:\\Temp\\LineCount3.txt", string.Empty);
            ssisTest.Asserts.Add("Test File 2", ssisAssert);

            target.Tests["Test"].TestTeardown.Commands.Add(new FileCommand(target, "Delete", "C:\\Temp\\LineCount3.txt", string.Empty));

            //TODO: Add a TestRef test

            target.TeardownCommands.Commands.Add(new FileCommand(target, "Delete", "C:\\Temp\\LineCount2.txt", string.Empty));

            target.TestSuiteTeardown.Commands.Add(new SqlCommand(target, "AdventureWorks", false, "DROP TABLE dbo.Test"));

            //Test Save of File
            target.Save("C:\\Temp\\Test.ssisUnit");
            Assert.IsTrue(File.Exists("C:\\Temp\\Test.ssisUnit"));
            //Assert.AreEqual<string>(File.OpenText("C:\\Temp\\Test.ssisUnit").ReadToEnd(), 


            int testCount = 0;

            try
            {
                testCount = target.Execute();
                Assert.AreEqual<int>(1, testCount);
                Assert.AreEqual<int>(1, target.Statistics.GetStatistic(TestSuiteResults.StatisticEnum.TestPassedCount));
                Assert.AreEqual<int>(3, target.Statistics.GetStatistic(TestSuiteResults.StatisticEnum.AssertPassedCount));
                Assert.AreEqual<int>(0, target.Statistics.GetStatistic(TestSuiteResults.StatisticEnum.AssertFailedCount));
                Assert.IsFalse(File.Exists("C:\\Temp\\LineCount2.txt"));
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
            //TODO: add ability to gracefully handle bad package refs - right now it blows the test case out of the water - no teardown
            try
            {
                target = new SsisTestSuite("C:\\Temp\\Test.ssisUnit");
                testCount = target.Execute();
                Assert.AreEqual<int>(1, testCount);
                Assert.AreEqual<int>(1, target.Statistics.GetStatistic(TestSuiteResults.StatisticEnum.TestPassedCount));
                Assert.AreEqual<int>(3, target.Statistics.GetStatistic(TestSuiteResults.StatisticEnum.AssertPassedCount));
                Assert.AreEqual<int>(0, target.Statistics.GetStatistic(TestSuiteResults.StatisticEnum.AssertFailedCount));
                Assert.IsFalse(File.Exists("C:\\Temp\\LineCount2.txt"));
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }



        }
    }
}
