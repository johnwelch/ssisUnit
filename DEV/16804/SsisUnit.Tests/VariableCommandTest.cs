using SsisUnit;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.SqlServer.Dts.Runtime;

namespace UTssisUnit
{
    [TestClass]
    public class VariableCommandTest : ExternalFileResourceTestBase
    {
        private string _dtsxFilePath;

        [TestInitialize]
        public void Initialize()
        {
            _dtsxFilePath = UnpackToFile("UTssisUnit.TestPackages.SimplePackage.dtsx");
        }

        [TestMethod]
        public void VariableCommandConstructorTest()
        {
            var target = new VariableCommand(new SsisTestSuite());
            Assert.IsNotNull(target);
        }

        [TestMethod]
        public void RunVariableCommandSetTest()
        {
            var ts = new SsisTestSuite();
            var target = new VariableCommand(ts, VariableCommand.VariableOperation.Set, "VariableTest", "10");
            var ssisApp = new Application();
            Package package = ssisApp.LoadPackage(_dtsxFilePath, null);
            DtsContainer container = package;
            string actual = target.Execute(package, container).ToString();
            Assert.AreEqual("10", actual);
        }

        [TestMethod]
        public void RunVariableCommandGetTest()
        {
            var ts = new SsisTestSuite();
            var target = new VariableCommand(ts, VariableCommand.VariableOperation.Get, "VariableTest", "0");
            var ssisApp = new Application();
            Package package = ssisApp.LoadPackage(_dtsxFilePath, null);
            DtsContainer container = package;
            string actual = target.Execute(package, container).ToString();
            Assert.AreEqual("55", actual);
        }
    }
}
