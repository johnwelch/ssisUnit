using Microsoft.VisualStudio.TestTools.UnitTesting;

using SsisUnit;

namespace UTssisUnit.Commands
{
    /// <summary>
    /// This is a test class for ProcessCommandTest and is intended
    /// to contain all ProcessCommandTest Unit Tests
    /// </summary>
    [TestClass]
    public class ProcessCommandTest : ExternalFileResourceTestBase
    {
        /// <summary>
        /// A test for ProcessCommand Constructor
        /// </summary>
        [TestMethod]
        public void ProcessCommandConstructorTest()
        {
            var target = new ProcessCommand(null);
            Assert.IsNotNull(target);
        }

        [TestMethod]
        public void RunProcessCommandTest()
        {
            string path = GetTempPath("FileCopy", true);
            string sourceFile = CreateTempFile(path, "SourceFile.tst");
            string targetFile = CreateTempFile(path, "TargetFile.tst");

            var command = new ProcessCommand(null)
                { Process = "CMD.EXE", Arguments = string.Format(@"/c COPY {0} {1}", sourceFile, targetFile) };
            object actual = command.Execute();
            Assert.AreEqual(0, actual);
        }

        [TestMethod]
        public void RunProcessCommandEmptyArgsTest()
        {            
            var command = new ProcessCommand(null) { Process = "NOTEPAD.EXE", Arguments = string.Empty };
            object actual = command.Execute();
            Assert.AreEqual(0, actual);
        }

        [TestMethod]
        public void RunProcessCommandNoArgsTest()
        {
            var command = new ProcessCommand(null) { Process = "NOTEPAD.EXE" };
            object actual = command.Execute();

            Assert.AreEqual(0, actual);
        }

        [TestMethod]
        public void RunProcessCommandReturnsFailure()
        {
            var command = new ProcessCommand(null) { Process = "CMD.EXE", Arguments = "/c BADCOMMAND" };
            object actual = command.Execute();
            Assert.AreEqual(1, actual);
        }
    }
}