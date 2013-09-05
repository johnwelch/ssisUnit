using Microsoft.SqlServer.Dts.Pipeline.Wrapper;
using Microsoft.SqlServer.Dts.Runtime;

using NUnit.Framework;

using SsisUnit;
using SsisUnit.Enums;

namespace UTssisUnit
{
    [TestFixture]
    public class HelperTests : ExternalFileResourceTestBase
    {
        private const string TestPackageResource = "UTssisUnit.TestPackages.UTBasicScenario2012.dtsx";

        [Test]
        public void FindExecutableByIdTest()
        {
            var ssisApp = new Application();

            Package packageToTest = ssisApp.LoadPackage(UnpackToFile(TestPackageResource), null);

            DtsContainer result = SsisUnit.Helper.FindExecutable(packageToTest, "{B439F9C9-2B1D-47E4-B328-7603126875FF}");
            Assert.AreEqual("INSERT ROW", result.Name);

            result = SsisUnit.Helper.FindExecutable(packageToTest, "{D2D9295A-45D0-4681-B021-F5077CB2EC22}");
            Assert.AreEqual("UT Basic Scenario", result.Name);
        }

        [Test]
        public void FindExecutableByNameTest()
        {
            var ssisApp = new Application();

            Package packageToTest = ssisApp.LoadPackage(UnpackToFile(TestPackageResource), null);

            DtsContainer result = SsisUnit.Helper.FindExecutable(packageToTest, "SELECT COUNT");
            Assert.AreEqual("SELECT COUNT", result.Name);
        }

        [Test]
        public void FindExecutableByPathTest()
        {
            var ssisApp = new Application();

            Package packageToTest = ssisApp.LoadPackage(UnpackToFile(TestPackageResource), null);

            DtsContainer result = SsisUnit.Helper.FindExecutable(packageToTest, @"Package\INSERT ROW");
            Assert.AreEqual("INSERT ROW", result.Name);

            result = SsisUnit.Helper.FindExecutable(packageToTest, "Package");
            Assert.AreEqual("UT Basic Scenario", result.Name);

            string remainingPath;
            result = SsisUnit.Helper.FindExecutable(packageToTest, @"Package\Data Flow Task\OLE DB Source", out remainingPath);
            Assert.AreEqual("Data Flow Task", result.Name);
            Assert.AreEqual("OLE DB Source", remainingPath);
        }

        [Test]
        public void FindComponentByPathTest()
        {
            var ssisApp = new Application();

            Package packageToTest = ssisApp.LoadPackage(UnpackToFile(TestPackageResource), null);

            string remainingPath;
            var result = SsisUnit.Helper.FindExecutable(packageToTest, @"Package\Data Flow Task\OLE DB Source", out remainingPath);
            Assert.AreEqual("Data Flow Task", result.Name);
            Assert.AreEqual("OLE DB Source", remainingPath);
            var result2 = SsisUnit.Helper.FindComponent(result, remainingPath);
            Assert.AreEqual("OLE DB Source", result2.Name);
        }

        [Test]
        public void FindInputByPathTest()
        {
            var ssisApp = new Application();

            Package packageToTest = ssisApp.LoadPackage(UnpackToFile(TestPackageResource), null);

            string remainingPath;
            var result = SsisUnit.Helper.FindExecutable(packageToTest, @"Package\Data Flow Task\Derived Column.Inputs[Derived Column Input]", out remainingPath) as TaskHost;
            Assert.AreEqual("Data Flow Task", result.Name);
            var result2 = SsisUnit.Helper.FindComponentInput(result.InnerObject as MainPipe, remainingPath);
            Assert.AreEqual("Derived Column Input", result2.Name);
        }

        [Test]
        public void FindOutputByPathTest()
        {
            var ssisApp = new Application();

            Package packageToTest = ssisApp.LoadPackage(UnpackToFile(TestPackageResource), null);

            string remainingPath;
            var result = SsisUnit.Helper.FindExecutable(packageToTest, @"Package\Data Flow Task\Derived Column.Outputs[Derived Column Output]", out remainingPath) as TaskHost;
            Assert.AreEqual("Data Flow Task", result.Name);
            var result2 = SsisUnit.Helper.FindComponentOutput(result.InnerObject as MainPipe, remainingPath);
            Assert.AreEqual("Derived Column Output", result2.Name);
        }

        [Test]
        public void FindPathTest()
        {
            var ssisApp = new Application();

            Package packageToTest = ssisApp.LoadPackage(UnpackToFile(TestPackageResource), null);

            string remainingPath;
            var result = SsisUnit.Helper.FindExecutable(packageToTest, @"Package\Data Flow Task\Derived Column.Inputs[Derived Column Input]", out remainingPath) as TaskHost;
            var mainPipe = result.InnerObject as MainPipe;
            var result2 = SsisUnit.Helper.FindComponentInput(mainPipe, remainingPath);
            var result3 = SsisUnit.Helper.FindPath(mainPipe, result2);
            Assert.IsNotNull(result3);
        }


        [Test]
        public void FindExecutableDoesNotExistTest()
        {
            var ssisApp = new Application();

            Package packageToTest = ssisApp.LoadPackage(UnpackToFile(TestPackageResource), null);

            DtsContainer result = SsisUnit.Helper.FindExecutable(packageToTest, "Does Not Exist");
            Assert.IsNull(result);

            result = SsisUnit.Helper.FindExecutable(packageToTest, @"Package\Test\Does Not Exist");
            Assert.IsNull(result);

            result = SsisUnit.Helper.FindExecutable(packageToTest, "{6CABA47A-AF51-49DB-8B9B-14FEB71B1EB2}");
            Assert.IsNull(result);
        }

        [Test]
        [Ignore("A 2012 SSIS project needs to be added to the unit test resources.")]
        public void LoadPackageFromProjectInFileSystemTest()
        {
            SsisTestSuite testSuite = new SsisTestSuite();

            var packageRef = new PackageRef("BI Cleanup.dtsx", "BI Cleanup.dtsx", PackageStorageType.FileSystem, null) { ProjectPath = @"C:\Temp\LaunchParty201307.ispac" };

            testSuite.PackageRefs.Add(packageRef.Name, packageRef);

            object loadedProject;

            var packageToTest = SsisUnit.Helper.LoadPackage(testSuite, packageRef.PackagePath, packageRef.ProjectPath, out loadedProject);

            Assert.IsNotNull(packageToTest);
            Assert.IsNotNull(loadedProject);
        }

        [Test]
        [Ignore("The SSIS Catalog cannot be tested statically.")]
        public void LoadPackageFromProjectInSsisCatalogTest()
        {
            SsisTestSuite testSuite = new SsisTestSuite();

            var packageRef = new PackageRef("Period.In.My.Name.1 1.dtsx", "Period.In.My.Name.1 1.dtsx", PackageStorageType.SsisCatalog, @"FL-WS-DEV-JN21\SQL2012") { ProjectPath = @"Jeremiah SSIS Deployed Projects\SSIS2012" };

            testSuite.PackageRefs.Add(packageRef.Name, packageRef);

            object loadedProject;

            var packageToTest = SsisUnit.Helper.LoadPackage(testSuite, packageRef.PackagePath, packageRef.ProjectPath, out loadedProject);

            Assert.IsNotNull(packageToTest);
            Assert.IsNotNull(loadedProject);
        }
    }
}
