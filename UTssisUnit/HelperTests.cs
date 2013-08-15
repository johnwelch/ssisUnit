using System;

using Microsoft.SqlServer.Dts.Runtime;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UTssisUnit
{
    [TestClass]
    public class HelperTests : ExternalFileResourceTestBase
    {
        [TestMethod]
        
        public void TestFindExecutable()
        {
            var file = UnpackToFile("UTssisUnit.TestPackages.UTBasicScenario2012.dtsx");
            var app = new Application();
            var pkg = app.LoadPackage(file, null);

            var result = SsisUnit.Helper.FindExecutable(pkg, "{B439F9C9-2B1D-47E4-B328-7603126875FF}");
            Assert.AreEqual("INSERT ROW", result.Name);

            result = SsisUnit.Helper.FindExecutable(pkg, @"Package\INSERT ROW");
            Assert.AreEqual("INSERT ROW", result.Name);

            result = SsisUnit.Helper.FindExecutable(pkg, "{D2D9295A-45D0-4681-B021-F5077CB2EC22}");
            Assert.AreEqual("UT Basic Scenario", result.Name);

            result = SsisUnit.Helper.FindExecutable(pkg, "Package");
            Assert.AreEqual("UT Basic Scenario", result.Name);

            //result = SsisUnit.Helper.FindExecutable(pkg, @"Package\Data Flow Task\OLE DB Source");
            //Assert.AreEqual("Data Flow Task", result.Name);
        }
    }
}
