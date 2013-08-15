using System;

using Microsoft.SqlServer.Dts.Runtime;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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

            result = SsisUnit.Helper.FindExecutable(packageToTest, @"Package\Data Flow Task\OLE DB Source");
            Assert.AreEqual("Data Flow Task", result.Name);
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

    }
}
