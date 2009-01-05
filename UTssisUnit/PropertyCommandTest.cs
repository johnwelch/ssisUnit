using SsisUnit;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.SqlServer.Dts.Runtime;

namespace UTssisUnit
{


    /// <summary>
    ///This is a test class for PropertyCommandTest and is intended
    ///to contain all PropertyCommandTest Unit Tests
    ///</summary>
    [TestClass()]
    public class PropertyCommandTest
    {


        private const string TEST_XML_FILENAME = "UTSsisUnit_Property.ssisUnit";

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
        ///A test for VariableCommand Constructor
        ///</summary>
        [TestMethod()]
        public void PropertyCommandConstructorTest()
        {
            PropertyCommand target = new PropertyCommand(new SsisTestSuite(ssisUnit_UTHelper.CreateUnitTestStream(TEST_XML_FILENAME)));
            Assert.IsNotNull(target);
        }

        [TestMethod()]
        public void RunPropertyCommandSetTest()
        {
            SsisTestSuite ts = new SsisTestSuite(ssisUnit_UTHelper.CreateUnitTestStream(TEST_XML_FILENAME));
            PropertyCommand target = (PropertyCommand)ts.SetupCommands.Commands[1];

            Application ssisApp = new Application();
            Package package = ssisApp.LoadPackage(ts.PackageRefs["PropertyTest"].PackagePath, null);
            DtsContainer container = package;
            object actual;
            actual = target.Execute(package, container);
            Assert.AreEqual(1, actual);
            Assert.AreEqual(1, package.Variables["TestInt"].Value);

            ts.Execute();
            Assert.AreEqual(2, ts.Statistics.GetStatistic(TestSuiteResults.StatisticEnum.AssertPassedCount));
        }

        [TestMethod()]
        public void RunPropertyCommandGetTest()
        {
            SsisTestSuite ts = new SsisTestSuite(ssisUnit_UTHelper.CreateUnitTestStream(TEST_XML_FILENAME));
            PropertyCommand target = (PropertyCommand)ts.SetupCommands.Commands[0];

            Application ssisApp = new Application();
            Package package = ssisApp.LoadPackage(ts.PackageRefs["PropertyTest"].PackagePath, null);
            DtsContainer container = package;
            object actual;
            actual = target.Execute(package, container);
            Assert.AreEqual("TestValue", actual);
        }

        [TestMethod()]
        public void CheckVariousPathsTest()
        {
            SsisTestSuite ts = new SsisTestSuite();
            ts.PackageRefs.Add("PackageA", new PackageRef("PackageA", "C:\\Projects\\SSISUnit\\UTssis2008packages\\PropertyTest.dtsx", PackageRef.PackageStorageType.FileSystem));

            ts.SetupCommands.Commands.Add(new PropertyCommand(ts, "Set", "\\Package\\Sequence Container\\Script Task.Properties[Description]", "Test Descr"));
            ts.SetupCommands.Commands.Add(new PropertyCommand(ts, "Set", "\\Package\\Sequence Container.Properties[Description]", "Test Descr"));
            ts.SetupCommands.Commands.Add(new PropertyCommand(ts, "Set", "\\Package\\Execute SQL Task.Properties[Description]", "Test Descr"));
            ts.SetupCommands.Commands.Add(new PropertyCommand(ts, "Set", "\\Package.Properties[CreationDate]", "2000-01-01"));
            ts.SetupCommands.Commands.Add(new PropertyCommand(ts, "Set", "\\Package.Connections[localhost.AdventureWorksDW2008].Properties[Description]", "Test Descr"));
            ts.SetupCommands.Commands.Add(new PropertyCommand(ts, "Set", "\\Package.EventHandlers[OnError].Variables[System::Cancel].Properties[Value]", false));
            ts.SetupCommands.Commands.Add(new PropertyCommand(ts, "Set", "\\Package.EventHandlers[OnError].Properties[Description]", "Test Descr"));
            ts.SetupCommands.Commands.Add(new PropertyCommand(ts, "Set", "\\Package.EventHandlers[OnError]\\Script Task.Properties[Description]", "Test Descr"));

            ts.Tests.Add("Test", new Test(ts, "Test", "PackageA", "{5A32107F-F3A6-4345-BEB5-0B8434DDB102}"));
            ts.Tests["Test"].Asserts.Add("TestA", AddNewAssert(ts, "TestA", "Test Descr", "\\Package\\Sequence Container\\Script Task.Properties[Description]"));
            ts.Tests["Test"].Asserts.Add("TestB", AddNewAssert(ts, "TestB", "Test Descr", "\\Package\\Sequence Container.Properties[Description]"));
            ts.Tests["Test"].Asserts.Add("TestC", AddNewAssert(ts, "TestC", "Test Descr", "\\Package\\Execute SQL Task.Properties[Description]"));
            ts.Tests["Test"].Asserts.Add("TestD", AddNewAssert(ts, "TestD", "1/1/2000 12:00:00 AM", "\\Package.Properties[CreationDate]"));
            ts.Tests["Test"].Asserts.Add("TestE", AddNewAssert(ts, "TestE", "Test Descr", "\\Package.Connections[localhost.AdventureWorksDW2008].Properties[Description]"));
            ts.Tests["Test"].Asserts.Add("TestF", AddNewAssert(ts, "TestF", false, "\\Package.EventHandlers[OnError].Variables[System::Cancel].Properties[Value]"));
            ts.Tests["Test"].Asserts.Add("TestG", AddNewAssert(ts, "TestG", "Test Descr", "\\Package.EventHandlers[OnError].Properties[Description]"));
            ts.Tests["Test"].Asserts.Add("TestH", AddNewAssert(ts, "TestH", "Test Descr", "\\Package.EventHandlers[OnError]\\Script Task.Properties[Description]"));

            ts.Execute();
            Assert.AreEqual(8, ts.Statistics.GetStatistic(TestSuiteResults.StatisticEnum.AssertPassedCount));
            Assert.AreEqual(0, ts.Statistics.GetStatistic(TestSuiteResults.StatisticEnum.AssertFailedCount));
        }

        private SsisAssert AddNewAssert(SsisTestSuite ts, string assertName, object result, string propertyPath)
        {
            SsisAssert a = new SsisAssert(ts, assertName, result, false);
            a.Command = new PropertyCommand(ts, "Get", propertyPath, string.Empty);
            return a;
        }
    }
}
