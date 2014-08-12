using Microsoft.SqlServer.Dts.Runtime;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using SsisUnit;
using SsisUnit.Enums;
using SsisUnit.Packages;

using SsisUnitBase.Enums;

namespace UTssisUnit.Commands
{
    [TestClass]
    public class PropertyCommandTest : ExternalFileResourceTestBase
    {
        private const string TestXmlFilename = "UTSsisUnit_Property.ssisUnit";

        [TestMethod]
        public void PropertyCommandConstructorTest()
        {
            var target = new PropertyCommand(new SsisTestSuite(Helper.CreateUnitTestStream(TestXmlFilename)));
            Assert.IsNotNull(target);
        }

        [TestMethod]
        public void RunPropertyCommandSetTest()
        {
            var ts = new SsisTestSuite(Helper.CreateUnitTestStream(TestXmlFilename));
            var packageFile = UnpackToFile("UTssisUnit.TestPackages.PropertyTest.dtsx");
            ts.PackageList["PropertyTest"].PackagePath = packageFile;
            var target = (PropertyCommand)ts.SetupCommands.Commands[1];

            var ssisApp = new Application();
            var package = ssisApp.LoadPackage(ts.PackageList["PropertyTest"].PackagePath, null);
            var container = package;
            object actual = target.Execute(package, container);
            Assert.AreEqual(1, actual);
            Assert.AreEqual(1, package.Variables["TestInt"].Value);

            ts.Execute();
            Assert.AreEqual(3, ts.Statistics.GetStatistic(StatisticEnum.AssertPassedCount));
        }

        [TestMethod]
        public void RunPropertyCommandGetTest()
        {
            var ts = new SsisTestSuite(Helper.CreateUnitTestStream(TestXmlFilename));
            var target = (PropertyCommand)ts.SetupCommands.Commands[0];

            var ssisApp = new Application();
            var packageFile = UnpackToFile("UTssisUnit.TestPackages.PropertyTest.dtsx");
            ts.PackageList["PropertyTest"].PackagePath = packageFile;
            Package package = ssisApp.LoadPackage(ts.PackageList["PropertyTest"].PackagePath, null);
            DtsContainer container = package;
            object actual = target.Execute(package, container);
            Assert.AreEqual("TestValue", actual);
        }

        [TestMethod]
        public void CheckVariousPathsTest()
        {
            string packageFilepath;
#if SQL2005
            packageFilepath = UnpackToFile("UTssisUnit.TestPackages.PropertyTest.dtsx");
#elif SQL2008
            packageFilepath = UnpackToFile("UTssisUnit.TestPackages.PropertyTest.dtsx");
#elif SQL2014 || SQL2012
            packageFilepath = UnpackToFile("UTssisUnit.TestPackages.PropertyTest2012.dtsx");
#endif

            var ts = new SsisTestSuite();
            ts.PackageList.Add("PackageA", new PackageRef("PackageA", packageFilepath, PackageStorageType.FileSystem));

            ts.SetupCommands.Commands.Add(new PropertyCommand(ts, "Set", @"\Package\Sequence Container\Script Task.Properties[Description]", "Test Descr"));
            ts.SetupCommands.Commands.Add(new PropertyCommand(ts, "Set", @"\Package\Sequence Container.Properties[Description]", "Test Descr"));
            ts.SetupCommands.Commands.Add(new PropertyCommand(ts, "Set", @"\Package\Execute SQL Task.Properties[Description]", "Test Descr"));
            ts.SetupCommands.Commands.Add(new PropertyCommand(ts, "Set", @"\Package.Properties[CreationDate]", "2000-01-01"));
            ts.SetupCommands.Commands.Add(new PropertyCommand(ts, "Set", @"\Package.Connections[localhost.AdventureWorksDW2008].Properties[Description]", "Test Descr"));
            ts.SetupCommands.Commands.Add(new PropertyCommand(ts, "Set", @"\Package.EventHandlers[OnError].Variables[System::Cancel].Properties[Value]", false));
            ts.SetupCommands.Commands.Add(new PropertyCommand(ts, "Set", @"\Package.EventHandlers[OnError].Properties[Description]", "Test Descr"));
            ts.SetupCommands.Commands.Add(new PropertyCommand(ts, "Set", @"\Package.EventHandlers[OnError]\Script Task.Properties[Description]", "Test Descr"));

            // Added to verify work item #7188 - multiple periods in object names
            ts.SetupCommands.Commands.Add(new PropertyCommand(ts, "Set", @"\Package.Connections[test.multple.periods.in.path].Properties[Description]", "Test Descr"));

            Test ssisTest = new Test(ts, "Test", "PackageA", null, "{7874CCC9-C3C6-40F5-9E8B-1DD62903D845}");

            ts.Tests.Add("Test", ssisTest);

            ts.Tests["Test"].Asserts.Add("TestA", AddNewAssert(ts, ssisTest, "TestA", "Test Descr", "\\Package\\Sequence Container\\Script Task.Properties[Description]"));
            ts.Tests["Test"].Asserts.Add("TestB", AddNewAssert(ts, ssisTest, "TestB", "Test Descr", "\\Package\\Sequence Container.Properties[Description]"));
            ts.Tests["Test"].Asserts.Add("TestC", AddNewAssert(ts, ssisTest, "TestC", "Test Descr", "\\Package\\Execute SQL Task.Properties[Description]"));
            ts.Tests["Test"].Asserts.Add("TestD", AddNewAssert(ts, ssisTest, "TestD", "1/1/2000 12:00:00 AM", "\\Package.Properties[CreationDate]"));
            ts.Tests["Test"].Asserts.Add("TestE", AddNewAssert(ts, ssisTest, "TestE", "Test Descr", "\\Package.Connections[localhost.AdventureWorksDW2008].Properties[Description]"));
            ts.Tests["Test"].Asserts.Add("TestF", AddNewAssert(ts, ssisTest, "TestF", false, "\\Package.EventHandlers[OnError].Variables[System::Cancel].Properties[Value]"));
            ts.Tests["Test"].Asserts.Add("TestG", AddNewAssert(ts, ssisTest, "TestG", "Test Descr", "\\Package.EventHandlers[OnError].Properties[Description]"));
            ts.Tests["Test"].Asserts.Add("TestH", AddNewAssert(ts, ssisTest, "TestH", "Test Descr", "\\Package.EventHandlers[OnError]\\Script Task.Properties[Description]"));
            ts.Tests["Test"].Asserts.Add("TestI", AddNewAssert(ts, ssisTest, "TestI", "Test Descr", "\\Package.Connections[test.multple.periods.in.path].Properties[Description]"));

            ts.Execute();
            Assert.AreEqual(10, ts.Statistics.GetStatistic(StatisticEnum.AssertPassedCount));
            Assert.AreEqual(0, ts.Statistics.GetStatistic(StatisticEnum.AssertFailedCount));
        }

        private SsisAssert AddNewAssert(SsisTestSuite ts, Test test, string assertName, object result, string propertyPath)
        {
            return new SsisAssert(ts, test, assertName, result, false) { Command = new PropertyCommand(ts, "Get", propertyPath, string.Empty) };
        }

        [TestMethod]
        public void TestSetConnectionString()
        {
            string packageFilepath;
#if SQL2005
            packageFilepath = UnpackToFile("UTssisUnit.TestPackages.PropertyTest.dtsx");
#elif SQL2008
            packageFilepath = UnpackToFile("UTssisUnit.TestPackages.PropertyTest.dtsx");
#elif SQL2014 || SQL2012
            packageFilepath = UnpackToFile("UTssisUnit.TestPackages.PropertyTest2012.dtsx");
#endif

            var ts = new SsisTestSuite();
            ts.PackageList.Add("PackageA", new PackageRef("PackageA", packageFilepath, PackageStorageType.FileSystem));

            var ssisTest = new Test(ts, "Test", "PackageA", null, "{7874CCC9-C3C6-40F5-9E8B-1DD62903D845}");
            ssisTest.TestSetup.Commands.Add(new PropertyCommand(ts, "Set", @"\Package.Connections[localhost.AdventureWorksDW2008].Properties[ConnectionString]", "Provider=SQLNCLI11.1;Data Source=localhost;Initial Catalog=AdventureWorksDW2012;Integrated Security=SSPI;Application Name=TestValue"));

            ts.Tests.Add("Test", ssisTest);

            ts.Tests["Test"].Asserts.Add("Assert", AddNewAssert(ts, ssisTest, "Assert", "Data Source=localhost;Initial Catalog=AdventureWorksDW2012;Provider=SQLNCLI11.1;Integrated Security=SSPI;Application Name=TestValue;", "\\Package.Connections[localhost.AdventureWorksDW2008].Properties[ConnectionString]"));

            ts.Execute();

            Assert.AreEqual("Data Source=localhost;Initial Catalog=AdventureWorksDW2012;Provider=SQLNCLI11.1;Integrated Security=SSPI;Application Name=TestValue;",
                ssisTest.InternalPackage.Connections["localhost.AdventureWorksDW2008"].ConnectionString);

            Assert.AreEqual(2, ts.Statistics.GetStatistic(StatisticEnum.AssertPassedCount));
            Assert.AreEqual(0, ts.Statistics.GetStatistic(StatisticEnum.AssertFailedCount));
        }
    }
}
