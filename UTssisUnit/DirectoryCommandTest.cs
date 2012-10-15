using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SsisUnit;

namespace UTssisUnit
{
    /// <summary>
    /// This is a test class for DirectoryCommandTest and is intended
    /// to contain all DirectoryCommandTest Unit Tests
    /// </summary>
    [TestClass]
    public class DirectoryCommandTest : ExternalFileResourceTestBase
    {
        /// <summary>
        /// Gets or sets the test context which provides
        /// information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext { get; set; }

        private SsisTestSuite _testSuite;

        #region Additional test attributes

        [TestInitialize]
        public new void Setup()
        {
            base.Setup();
            _testSuite = new SsisTestSuite();
        }

        [TestCleanup]
        public new void Teardown()
        {
            base.Teardown();
        }

        #endregion

        [TestMethod]
        public void CreateTest()
        {
            var tempPath = GetTempPath(@"SsisUnitTests\");

            var target = new DirectoryCommand(_testSuite)
                             {
                                 Operation = DirectoryOperation.Create,
                                 Argument1 = tempPath
                             };

            object actual = target.Execute();
            Assert.AreEqual(0, actual);
        }

        [TestMethod]
        public void MoveTest()
        {
            var fromPath = GetTempPath(@"MoveFromTest", true);
            var toPath = GetTempPath(@"MoveToTest");

            var target = new DirectoryCommand(_testSuite)
                             {
                                 Operation = DirectoryOperation.Move,
                                 Argument1 = fromPath,
                                 Argument2 = toPath
                             };

            object actual = target.Execute();
            Assert.AreEqual(0, actual);
        }

        [TestMethod]
        public void MoveWithoutTargetTest()
        {
            var fromPath = GetTempPath(@"MoveFromTest");

            var target = new DirectoryCommand(_testSuite)
                             {
                                 Operation = DirectoryOperation.Move,
                                 Argument1 = fromPath
                             };

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
        public void ExistsTest()
        {
            var tempPath = GetTempPath(@"SsisUnitTests\", true);

            var target = new DirectoryCommand(_testSuite)
                             {
                                 Operation = DirectoryOperation.Exists,
                                 Argument1 = tempPath
                             };

            object actual = target.Execute();
            Assert.AreEqual(true, actual);
        }

        [TestMethod]
        public void DoesntExistTest()
        {
            var tempPath = GetTempPath("DoesntExist");

            var target = new DirectoryCommand(_testSuite)
                        {
                            Operation = DirectoryOperation.Exists,
                            Argument1 = tempPath
                        };

            object actual = target.Execute();
            Assert.AreEqual(false, actual);
        }

        [TestMethod]
        public void DeleteTest()
        {
            var tempPath = GetTempPath("DeleteMe", true);

            var target = new DirectoryCommand(_testSuite)
                             {
                                 Operation = DirectoryOperation.Delete,
                                 Argument1 = tempPath
                             };

            object actual = target.Execute();
            Assert.AreEqual(0, actual);
        }

        [TestMethod]
        public void FileCountTest()
        {
            var tempPath = GetTempPath("FileCount", true);
            CreateTempFile(tempPath, "test1.aaa");
            CreateTempFile(tempPath, "test2.aaa");
            CreateTempFile(tempPath, "test3.aaa");

            var target = new DirectoryCommand(_testSuite)
                             {
                                 Operation = DirectoryOperation.FileCount,
                                 Argument1 = tempPath,
                                 Argument2 = "*.aaa"
                             };

            object actual = target.Execute();
            Assert.AreEqual(3, actual);
        }
    }
}
