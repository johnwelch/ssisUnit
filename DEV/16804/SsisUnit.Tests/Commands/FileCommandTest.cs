using System;
using System.Text;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using SsisUnit;

namespace UTssisUnit.Commands
{
    /// <summary>
    /// This is a test class for FileCommandTest and is intended
    /// to contain all FileCommandTest Unit Tests
    /// </summary>
    [TestClass]
    public class FileCommandTest : ExternalFileResourceTestBase
    {
        #region Public Methods and Operators

        /// <summary>
        /// A test for Execute
        /// </summary>
        [TestMethod]
        public void CopyTest()
        {
            string path = GetTempPath("FileCopy", true);
            string sourceFile = CreateTempFile(path, "SourceFile.tst");
            string targetFile = CreateTempFile(path, "TargetFile.tst");
            var target = new FileCommand(new SsisTestSuite()) { Operation = FileCommand.FileOperation.Copy, SourcePath = sourceFile, TargetPath = targetFile };

            object actual = target.Execute();
            Assert.AreEqual(0, actual);
        }

        [TestMethod]
        public void CopyWithoutTargetTest()
        {
            string path = GetTempPath("FileCopy", true);
            string sourceFile = CreateTempFile(path, "SourceFile.tst");

            var target = new FileCommand(new SsisTestSuite()) { Operation = FileCommand.FileOperation.Copy, SourcePath = sourceFile };
            try
            {
                target.Execute();
                Assert.Fail("Method did not throw the expected ArgumentException.");
            }
            catch (ArgumentException)
            {
                Assert.IsTrue(true);
            }
        }

        [TestMethod]
        public void CreateTest()
        {
            string path = GetTempPath("FileCopy", true);
            string sourceFile = CreateTempFile(path, "SourceFile.tst");
            string targetFile = CreateTempFile(path, "TargetFile.tst");
            var target = new FileCommand(new SsisTestSuite(), "Copy", sourceFile, targetFile);
            object actual = target.Execute();
            Assert.AreEqual(0, (int)actual);
        }

        [TestMethod]
        public void DeleteTest()
        {
            string path = GetTempPath("FileCopy", true);
            string sourceFile = CreateTempFile(path, "SourceFile.tst");
            var target = new FileCommand(new SsisTestSuite()) { Operation = FileCommand.FileOperation.Delete, SourcePath = sourceFile };

            object actual = target.Execute();
            Assert.AreEqual(0, actual);
        }

        [TestMethod]
        public void DoesntExistTest()
        {
            var target = new FileCommand(null) { Operation = FileCommand.FileOperation.Exists, SourcePath = @"C:\FileThatDoesntExist.tst" };
            object actual = target.Execute();
            Assert.AreEqual(false, actual);
        }

        [TestMethod]
        public void ExistsTest()
        {
            string path = GetTempPath("FileCopy", true);
            string sourceFile = CreateTempFile(path, "SourceFile.tst");
            var target = new FileCommand(null) { Operation = FileCommand.FileOperation.Exists, SourcePath = sourceFile };

            object actual = target.Execute();
            Assert.AreEqual(true, actual);
        }

        [TestMethod]
        public void LineCountTest()
        {
            var sb = new StringBuilder();
            sb.AppendLine("Test Line Count 1");
            sb.AppendLine("Test Line Count 2");
            sb.AppendLine("Test Line Count 3");

            string path = GetTempPath("FileTest", true);
            string sourceFile = CreateTempFile(path, "SourceFile.tst", sb.ToString());

            var target = new FileCommand(null) { Operation = FileCommand.FileOperation.LineCount, SourcePath = sourceFile };
            object actual = target.Execute();
            Assert.AreEqual(3, actual);
        }

        [TestMethod]
        public void MoveTest()
        {
            string path = GetTempPath("FileCopy", true);
            string sourceFile = CreateTempFile(path, "SourceFile.tst");
            string targetFile = System.IO.Path.Combine(path, "TargetFile.tst");
            var target = new FileCommand(null) { Operation = FileCommand.FileOperation.Move, SourcePath = sourceFile, TargetPath = targetFile };

            object actual = target.Execute();
            Assert.AreEqual(0, actual);
        }

        #endregion
    }
}