using System.Text;

using Microsoft.SqlServer.Dts.Runtime;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using SsisUnit;
using System;
using System.IO;

using SsisUnit.Enums;

namespace UTssisUnit
{
    [TestClass]
    public class SsisTestCaseTest : ExternalFileResourceTestBase
    {
        private const string TestXmlFilename = "UTSsisUnit.ssisUnit";

        private const string TestPackageResource = "UTssisUnit.TestPackages.UTBasicScenario2012.dtsx";

        private string _testPackageFile;

        [TestInitialize]
        public void Initialize()
        {
            var packageFile = UnpackToFile(TestPackageResource);
            _testPackageFile = CreateTempFile(GetTempPath("Test", true), "TestPackage.dtsx");
            File.Copy(packageFile, _testPackageFile, true);
        }

        /// <summary>
        /// A test for SSISTestCase Constructor
        /// </summary>
        [TestMethod]
        public void SsisTestCaseConstructorTest()
        {
            var target = new SsisTestSuite(Helper.CreateUnitTestStream(TestXmlFilename));
            Assert.IsNotNull(target);
        }

        [TestMethod]
        public void LoadXmlFileTest()
        {
            string filename = Helper.CreateUnitTestFile("UTSsisUnit");
            var ts = new SsisTestSuite(filename);
            Assert.IsNotNull(ts);
        }

        // TODO: Investigate, not sure why this test is failing. 
        // Doesn't look like it should have ever worked, but may be new .NET 4.0 behavior.
        ////[TestMethod]
        ////public void LoadInvalidXmlFileTest()
        ////{
        ////    try
        ////    {
        ////        var ts = new SsisTestSuite(Helper.CreateUnitTestStream("UT_BadTestFile.ssisUnit"));
        ////        Assert.IsInstanceOfType(ts, typeof(SsisTestSuite));
        ////        Assert.Fail("Test Case was not reported as invalid.");
        ////    }
        ////    catch (ApplicationException ex)
        ////    {
        ////        Assert.AreEqual("The unit test file is malformed or corrupt. Please verify that the file format conforms to the ssisUnit schema, provided in the SsisUnit.xsd file.", ex.Message);
        ////    }
        ////}

        [TestMethod]
        public void ExecuteTest()
        {
            var target = new SsisTestSuite(Helper.CreateUnitTestStream(TestXmlFilename));
            foreach (var test in target.Tests.Values)
            {
                test.PackageLocation = _testPackageFile;
            }

            target.Execute();
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void SetupTest()
        {
            var target = new SsisTestSuite(Helper.CreateUnitTestStream(TestXmlFilename));
            var ssisApp = new Application();
            target.ConnectionRefs["AdventureWorks"].ConnectionString =
                "Provider=SQLNCLI11;Data Source=localhost;Integrated Security=SSPI;Initial Catalog=tempdb";
            Package packageToTest = ssisApp.LoadPackage(UnpackToFile(TestPackageResource), null);
            string remainingPath;
            DtsContainer task = SsisUnit.Helper.FindExecutable(packageToTest, "SELECT COUNT", out remainingPath);

            int result = target.SetupCommands.Execute(packageToTest, task);
            Assert.AreEqual(4, result);
        }

        [TestMethod]
        public void TeardownTest()
        {
            var target = new SsisTestSuite(Helper.CreateUnitTestStream(TestXmlFilename));
            target.ConnectionRefs["AdventureWorks"].ConnectionString =
                "Provider=SQLNCLI11;Data Source=localhost;Integrated Security=SSPI;Initial Catalog=tempdb";
            var ssisApp = new Application();
            Package packageToTest = ssisApp.LoadPackage(UnpackToFile(TestPackageResource), null);
            string remainingPath;
            DtsContainer task = SsisUnit.Helper.FindExecutable(packageToTest, "SELECT COUNT", out remainingPath);

            int result = target.TeardownCommands.Execute(packageToTest, task);
            Assert.AreEqual(2, result);
        }

        // TODO: Need to renable - fix environmental hardcoding
        ////[TestMethod]
        ////public void RunTestTest()
        ////{
        ////    var target = new SsisTestSuite(Helper.CreateUnitTestStream(TestXmlFilename));
        ////    target.ConnectionRefs["AdventureWorks"].ConnectionString =
        ////        "Provider=SQLNCLI11;Data Source=localhost;Integrated Security=SSPI;Initial Catalog=tempdb";
        ////    bool result = target.Tests["PassedTestSQL"].Execute();
        ////    Assert.IsTrue(result);
        ////}

        [TestMethod]
        public void RunPackageFromFileTest()
        {
            var packageFile = UnpackToFile("UTssisUnit.TestPackages.SimplePackage.dtsx");

            var ts = new SsisTestSuite();
            ts.PackageRefs.Add("filePkg", new PackageRef("filePkg", packageFile, PackageStorageType.FileSystem));
            var test = new Test(ts, "Main", "filePkg", "SimplePackage");
            ts.Tests.Add("Main", test);
            var assert = new SsisAssert(ts, test, "A1", 0, false);
            test.Asserts.Add("A1", assert);
            assert.Command = new VariableCommand(ts, VariableCommand.VariableOperation.Get, "RowCount", "0");
            ts.Execute();
            Assert.IsTrue(true);
        }

        ////[TestMethod]
        ////public void RunPackageFromMsdbTest()
        ////{
        ////    var packageFile = this.UnpackToFile("UTssisUnit.TestPackages.SimplePackage.dtsx");
        ////    var app = new Application();
        ////    var package = app.LoadPackage(packageFile, null);
        ////    app.SaveToSqlServer(package, null, "localhost", );
        ////    var ts = new SsisTestSuite();
        ////    ts.PackageRefs.Add("filePkg", new PackageRef("filePkg", packageFile, PackageRef.PackageStorageType.MSDB));
        ////    var test = new Test(ts, "Main", "filePkg", "SimplePackage");
        ////    ts.Tests.Add("Main", test);
        ////    var assert = new SsisAssert(ts, "A1", 0, false);
        ////    test.Asserts.Add("A1", assert);
        ////    assert.Command = new VariableCommand(ts, VariableCommand.VariableOperation.Get, "RowCount", "0");
        ////    ts.Execute();
        ////    Assert.IsTrue(true);
        ////}

        ////[TestMethod]
        ////public void RunPackageFromPackageStoreTest()
        ////{
        ////    var packageFile = this.UnpackToFile("UTssisUnit.TestPackages.SimplePackage.dtsx");

        ////    var ts = new SsisTestSuite();
        ////    ts.PackageRefs.Add("filePkg", new PackageRef("filePkg", packageFile, PackageRef.PackageStorageType.PackageStore));
        ////    var test = new Test(ts, "Main", "filePkg", "SimplePackage");
        ////    ts.Tests.Add("Main", test);
        ////    var assert = new SsisAssert(ts, "A1", 0, false);
        ////    test.Asserts.Add("A1", assert);
        ////    assert.Command = new VariableCommand(ts, VariableCommand.VariableOperation.Get, "RowCount", "0");
        ////    ts.Execute();
        ////    Assert.IsTrue(true);
        ////}

        // TODO: Add support for msdb, package store, and 2012 catalog - consider deprecating PackageStore

        // TODO: Need to renable - fix environmental hardcoding
        ////[TestMethod]
        ////public void RunTestRef()
        ////{
        ////    var refPackage = this.UnpackToFile()
        ////    var target = new SsisTestSuite(Helper.CreateUnitTestStream("UTSsisUnitParent.xml"));
        ////    target.ConnectionRefs["AdventureWorks"].ConnectionString =
        ////        "Provider=SQLNCLI11;Data Source=localhost;Integrated Security=SSPI;Initial Catalog=tempdb";
        ////    try
        ////    {
        ////        target.Execute();
        ////        Assert.AreEqual(4, target.Statistics.GetStatistic(TestSuiteResults.StatisticEnum.AssertCount));
        ////        Assert.AreEqual(4, target.Statistics.GetStatistic(TestSuiteResults.StatisticEnum.AssertPassedCount));
        ////    }
        ////    catch (Exception ex)
        ////    {
        ////        Assert.Fail(ex.Message);
        ////    }
        ////}

        // TODO: Need to renable - fix environmental hardcoding
        ////[TestMethod]
        ////public void TestExecuteWithParent()
        ////{
        ////    var target = new SsisTestSuite(Helper.CreateUnitTestStream(TestXmlFilename));

        ////    try
        ////    {
        ////        target.TestRefs["C:\\Projects\\SSISUnit\\UTssisUnit\\UTssisUnit_Package.xml"].Execute();
        ////        Assert.IsTrue(true);
        ////    }
        ////    catch (Exception ex)
        ////    {
        ////        Assert.Fail(ex.Message);
        ////    }
        ////}

        // TODO: Need to renable - fix environmental hardcoding
        ////[TestMethod]
        ////public void TestSuiteSetupTest()
        ////{
        ////    var target = new SsisTestSuite(Helper.CreateUnitTestStream("UTssisUnit_TestSuite.xml"));
        ////    target.ConnectionRefs["Sandbox"].ConnectionString =
        ////        "Provider=SQLNCLI11;Data Source=localhost;Integrated Security=SSPI;Initial Catalog=tempdb";
        
        ////    try
        ////    {
        ////        target.Execute();
        ////        Assert.AreEqual(3, target.Statistics.GetStatistic(TestSuiteResults.StatisticEnum.TestCount));
        ////        Assert.AreEqual(2, target.Statistics.GetStatistic(TestSuiteResults.StatisticEnum.AssertPassedCount));
        ////        Assert.AreEqual(1, target.Statistics.GetStatistic(TestSuiteResults.StatisticEnum.TestPassedCount));
        ////    }
        ////    catch (Exception ex)
        ////    {
        ////        Assert.Fail(ex.Message);
        ////    }
        ////}

        [TestMethod]
        public void TestSetupAndTeardownTest()
        {
            var target = new SsisTestSuite(Helper.CreateUnitTestStream("UTSsisUnit_TestSetup_Teardown.xml"));
            target.ConnectionRefs["Sandbox"].ConnectionString =
                "Provider=SQLNCLI11;Data Source=localhost;Integrated Security=SSPI;Initial Catalog=tempdb";
            target.PackageRefs["filePkg"].PackagePath = _testPackageFile;

            try
            {
                target.Execute();
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }

            Assert.AreEqual(2, target.Statistics.GetStatistic(TestSuiteResults.StatisticEnum.AssertPassedCount));
            Assert.AreEqual(1, target.Statistics.GetStatistic(TestSuiteResults.StatisticEnum.TestCount));
        }

        [TestMethod]
        public void CreateNewTestSuiteTest()
        {
            string packageFile = UnpackToFile(TestPackageResource);
            var target = new SsisTestSuite();

            Assert.AreEqual(0, target.ConnectionRefs.Count);

            target.ConnectionRefs.Add("AdventureWorks", new ConnectionRef("AdventureWorks",
                "Provider=SQLNCLI11;Data Source=localhost;Integrated Security=SSPI;Initial Catalog=tempdb",
                ConnectionRef.ConnectionTypeEnum.ConnectionString));

            Assert.AreEqual(1, target.ConnectionRefs.Count);
            Assert.AreEqual("AdventureWorks", target.ConnectionRefs["AdventureWorks"].ReferenceName);
            Assert.AreEqual("Provider=SQLNCLI11;Data Source=localhost;Integrated Security=SSPI;Initial Catalog=tempdb", target.ConnectionRefs["AdventureWorks"].ConnectionString);
            Assert.AreEqual(ConnectionRef.ConnectionTypeEnum.ConnectionString, target.ConnectionRefs["AdventureWorks"].ConnectionType);

            target.PackageRefs.Add("UT Basic Scenario", new PackageRef("UT Basic Scenario", packageFile, PackageStorageType.FileSystem));

            target.TestSuiteSetup.Commands.Add(new SqlCommand(target, "AdventureWorks", false, "CREATE TABLE dbo.Test (ID INT)"));
            target.TestSuiteSetup.Commands.Add(new SqlCommand(target, "AdventureWorks", false, "INSERT INTO dbo.Test VALUES (1)"));
            target.TestSuiteSetup.Commands.Add(new SqlCommand(target, "AdventureWorks", false, "INSERT INTO dbo.Test VALUES (2)"));

            var sb = new StringBuilder();
            sb.AppendLine("Test Line Count 1");
            sb.AppendLine("Test Line Count 2");
            sb.AppendLine("Test Line Count 3");

            string path = GetTempPath("FileTest", true);
            string lineCountFile = CreateTempFile(path, "SourceFile.tst", sb.ToString());
            var lineCount2File = GetTemporaryFileName();
            var lineCount3File = GetTemporaryFileName();

            target.SetupCommands.Commands.Add(new FileCommand(target, "Copy", lineCountFile, lineCount2File));

            Assert.AreEqual(0, target.Tests.Count);
            var ssisTest = new Test(target, "Test", "UT Basic Scenario", "SELECT COUNT");
            target.Tests.Add("Test", ssisTest);

            Assert.AreEqual(1, target.Tests.Count);
            Assert.AreEqual("Test", target.Tests["Test"].Name);
            Assert.AreEqual("UT Basic Scenario", target.Tests["Test"].PackageLocation);
            Assert.AreEqual("SELECT COUNT", target.Tests["Test"].Task);

            target.Tests["Test"].TestSetup.Commands.Add(new FileCommand(target, "Copy", lineCountFile, lineCount3File));

            var ssisAssert = new SsisAssert(target, ssisTest, "Test Count", 2, false) { Command = new SqlCommand(target, "AdventureWorks", true, "SELECT COUNT(*) FROM dbo.Test") };

            ssisTest.Asserts.Add("Test Count", ssisAssert);
            Assert.AreEqual(1, ssisTest.Asserts.Count);
            Assert.AreEqual("Test Count", ssisTest.Asserts["Test Count"].Name);
            Assert.AreEqual(2, ssisTest.Asserts["Test Count"].ExpectedResult);
            Assert.AreEqual(false, ssisTest.Asserts["Test Count"].TestBefore);
            Assert.AreEqual("<SqlCommand name=\"\" connectionRef=\"AdventureWorks\" returnsValue=\"true\">SELECT COUNT(*) FROM dbo.Test</SqlCommand>", ssisTest.Asserts["Test Count"].Command.PersistToXml());

            ssisAssert = new SsisAssert(target, ssisTest, "Test File", true, false) { Command = new FileCommand(target, "Exists", lineCount2File, string.Empty) };
            ssisTest.Asserts.Add("Test File", ssisAssert);

            ssisAssert = new SsisAssert(target, ssisTest, "Test File 2", true, false) { Command = new FileCommand(target, "Exists", lineCount3File, string.Empty) };
            ssisTest.Asserts.Add("Test File 2", ssisAssert);

            target.Tests["Test"].TestTeardown.Commands.Add(new FileCommand(target, "Delete", lineCount3File, string.Empty));

            // TODO: Add a TestRef test

            target.TeardownCommands.Commands.Add(new FileCommand(target, "Delete", lineCount2File, string.Empty));

            target.TestSuiteTeardown.Commands.Add(new SqlCommand(target, "AdventureWorks", false, "DROP TABLE dbo.Test"));

            var saveFile = GetTemporaryFileName();
            target.Save(saveFile);
            Assert.IsTrue(File.Exists(saveFile));

            int testCount = target.Execute();
            Assert.AreEqual(1, testCount);
            Assert.AreEqual(1, target.Statistics.GetStatistic(TestSuiteResults.StatisticEnum.TestPassedCount));
            Assert.AreEqual(4, target.Statistics.GetStatistic(TestSuiteResults.StatisticEnum.AssertPassedCount));
            Assert.AreEqual(0, target.Statistics.GetStatistic(TestSuiteResults.StatisticEnum.AssertFailedCount));
            Assert.IsFalse(File.Exists(lineCount2File));

            // TODO: add ability to gracefully handle bad package refs - right now it blows the test case out of the water - no teardown
            target = new SsisTestSuite(saveFile);
            testCount = target.Execute();
            Assert.AreEqual(1, testCount);
            Assert.AreEqual(1, target.Statistics.GetStatistic(TestSuiteResults.StatisticEnum.TestPassedCount));
            Assert.AreEqual(4, target.Statistics.GetStatistic(TestSuiteResults.StatisticEnum.AssertPassedCount));
            Assert.AreEqual(0, target.Statistics.GetStatistic(TestSuiteResults.StatisticEnum.AssertFailedCount));
            Assert.IsFalse(File.Exists(lineCount2File));
        }
    }
}
