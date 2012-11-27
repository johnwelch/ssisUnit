using ssisUnitTestRunner;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;

namespace UTssisUnit
{
    [TestClass]
    public class ProgramTest
    {
        [TestMethod]
        public void ProgramConstructorTest()
        {
            var target = new Program();
            Assert.IsNotNull(target);
        }

        [TestMethod]
        [DeploymentItem("ssisUnitTestRunner.exe")]
        public void MainTest()
        {
            var proc = Process.Start("ssisUnitTestRunner.exe", "/TESTCASE C:\\Projects\\SSISUnit\\UTssisUnit\\UTssisUnit_Package.xml");
            Debug.Assert(proc != null, "proc != null");
            proc.WaitForExit(50000);
            Assert.AreEqual(0, proc.ExitCode, "Test Runner fails");
        }
    }
}
