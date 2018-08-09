using Microsoft.SqlServer.Dts.Pipeline.Wrapper;
using Microsoft.SqlServer.Dts.Runtime;

//using NUnit.Framework;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using SsisUnit;
using SsisUnit.Enums;
using SsisUnit.Packages;

namespace UTssisUnit
{
    [TestClass]
    public class HelperTests : ExternalFileResourceTestBase
    {
        private const string TestPackageResource = "UTssisUnit.TestPackages.UTBasicScenario2012.dtsx";

        [TestMethod]
        public void FindExecutableByIdTest()
        {
            var ssisApp = new Application();

            Package packageToTest = ssisApp.LoadPackage(UnpackToFile(TestPackageResource), null);

            DtsContainer result = SsisUnit.Helper.FindExecutable(packageToTest, "{B439F9C9-2B1D-47E4-B328-7603126875FF}");
            Assert.AreEqual("INSERT ROW", result.Name);

            result = SsisUnit.Helper.FindExecutable(packageToTest, "{D2D9295A-45D0-4681-B021-F5077CB2EC22}");
            Assert.AreEqual("UT Basic Scenario", result.Name);
        }

        [TestMethod]
        public void FindExecutableByNameTest()
        {
            var ssisApp = new Application();

            Package packageToTest = ssisApp.LoadPackage(UnpackToFile(TestPackageResource), null);

            DtsContainer result = SsisUnit.Helper.FindExecutable(packageToTest, "SELECT COUNT");
            Assert.AreEqual("SELECT COUNT", result.Name);
        }

        [TestMethod]
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

        [TestMethod]
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

        [TestMethod]
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

        [TestMethod]
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

        [TestMethod]
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


        [TestMethod]
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

        [TestMethod]
        public void LoadPackageFromProjectInFileSystemTest()
        {
            var projectFile = UnpackToFile("UTssisUnit.TestPackages.ISPACTesting.ispac", true);
            var testSuite = new SsisTestSuite();
            var packageRef = new PackageRef("TestPackage", "ExecuteSqlTask.dtsx", PackageStorageType.FileSystem, null) { ProjectPath = projectFile };
            testSuite.PackageList.Add(packageRef.Name, packageRef);

            object loadedProject;

            var packageToTest = SsisUnit.Helper.LoadPackage(testSuite, packageRef.PackagePath, null, packageRef.ProjectPath, out loadedProject);

            Assert.IsNotNull(packageToTest);
            Assert.IsNotNull(loadedProject);
        }

        [TestMethod]
        //[Ignore("The SSIS Catalog cannot be tested statically.")]
        public void LoadPackageFromProjectInSsisCatalogTest()
        {
            SsisTestSuite testSuite = new SsisTestSuite();

            var packageRef = new PackageRef("15_Users_Dataset.dtsx", "15_Users_Dataset.dtsx", PackageStorageType.SsisCatalog, @".\SQL2017") { ProjectPath = @"ssisUnit\ssisUnitLearning" };

            testSuite.PackageList.Add(packageRef.Name, packageRef);

            object loadedProject;

            var packageToTest = SsisUnit.Helper.LoadPackage(testSuite, packageRef.PackagePath, null, packageRef.ProjectPath, out loadedProject);

            Assert.IsNotNull(packageToTest);
            Assert.IsNotNull(loadedProject);
        }
    }
}
