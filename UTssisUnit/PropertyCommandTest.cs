using SsisUnit;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.SqlServer.Dts.Runtime;

namespace UTssisUnit
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
            ts.PackageRefs["PropertyTest"].PackagePath = packageFile;
            var target = (PropertyCommand)ts.SetupCommands.Commands[1];

            var ssisApp = new Application();
            var package = ssisApp.LoadPackage(ts.PackageRefs["PropertyTest"].PackagePath, null);
            var container = package;
            object actual = target.Execute(package, container);
            Assert.AreEqual(1, actual);
            Assert.AreEqual(1, package.Variables["TestInt"].Value);

            ts.Execute();
            Assert.AreEqual(3, ts.Statistics.GetStatistic(TestSuiteResults.StatisticEnum.AssertPassedCount));
        }

        [TestMethod]
        public void RunPropertyCommandGetTest()
        {
            var ts = new SsisTestSuite(Helper.CreateUnitTestStream(TestXmlFilename));
            var target = (PropertyCommand)ts.SetupCommands.Commands[0];

            var ssisApp = new Application();
            var packageFile = UnpackToFile("UTssisUnit.TestPackages.PropertyTest.dtsx");
            ts.PackageRefs["PropertyTest"].PackagePath = packageFile;
            Package package = ssisApp.LoadPackage(ts.PackageRefs["PropertyTest"].PackagePath, null);
            DtsContainer container = package;
            object actual = target.Execute(package, container);
            Assert.AreEqual("TestValue", actual);
        }

        [TestMethod]
        public void CheckVariousPathsTest()
        {
            string packageFilepath = string.Empty;

#if SQL2008
            packageFilepath = UnpackToFile("UTssisUnit.TestPackages.PropertyTest.dtsx");
#elif SQL2012
            packageFilepath = UnpackToFile("UTssisUnit.TestPackages.PropertyTest2012.dtsx");
#endif

            var ts = new SsisTestSuite();
            ts.PackageRefs.Add("PackageA", new PackageRef("PackageA", packageFilepath, PackageRef.PackageStorageType.FileSystem));

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

            ts.Tests.Add("Test", new Test(ts, "Test", "PackageA", "{7874CCC9-C3C6-40F5-9E8B-1DD62903D845}", null));
            ts.Tests["Test"].Asserts.Add("TestA", AddNewAssert(ts, "TestA", "Test Descr", "\\Package\\Sequence Container\\Script Task.Properties[Description]"));
            ts.Tests["Test"].Asserts.Add("TestB", AddNewAssert(ts, "TestB", "Test Descr", "\\Package\\Sequence Container.Properties[Description]"));
            ts.Tests["Test"].Asserts.Add("TestC", AddNewAssert(ts, "TestC", "Test Descr", "\\Package\\Execute SQL Task.Properties[Description]"));
            ts.Tests["Test"].Asserts.Add("TestD", AddNewAssert(ts, "TestD", "1/1/2000 12:00:00 AM", "\\Package.Properties[CreationDate]"));
            ts.Tests["Test"].Asserts.Add("TestE", AddNewAssert(ts, "TestE", "Test Descr", "\\Package.Connections[localhost.AdventureWorksDW2008].Properties[Description]"));
            ts.Tests["Test"].Asserts.Add("TestF", AddNewAssert(ts, "TestF", false, "\\Package.EventHandlers[OnError].Variables[System::Cancel].Properties[Value]"));
            ts.Tests["Test"].Asserts.Add("TestG", AddNewAssert(ts, "TestG", "Test Descr", "\\Package.EventHandlers[OnError].Properties[Description]"));
            ts.Tests["Test"].Asserts.Add("TestH", AddNewAssert(ts, "TestH", "Test Descr", "\\Package.EventHandlers[OnError]\\Script Task.Properties[Description]"));
            ts.Tests["Test"].Asserts.Add("TestI", AddNewAssert(ts, "TestI", "Test Descr", "\\Package.Connections[test.multple.periods.in.path].Properties[Description]"));

            ts.Execute();
            Assert.AreEqual(10, ts.Statistics.GetStatistic(TestSuiteResults.StatisticEnum.AssertPassedCount));
            Assert.AreEqual(0, ts.Statistics.GetStatistic(TestSuiteResults.StatisticEnum.AssertFailedCount));
        }

        private SsisAssert AddNewAssert(SsisTestSuite ts, string assertName, object result, string propertyPath)
        {
            var assert = new SsisAssert(ts, assertName, result, false)
                { Command = new PropertyCommand(ts, "Get", propertyPath, string.Empty) };
            return assert;
        }
    }
}
