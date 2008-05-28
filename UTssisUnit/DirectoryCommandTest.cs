using SsisUnit;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml;
using Microsoft.SqlServer.Dts.Runtime;
using System;

namespace UTssisUnit
{
    
    
    /// <summary>
    ///This is a test class for DirectoryCommandTest and is intended
    ///to contain all DirectoryCommandTest Unit Tests
    ///</summary>
    [TestClass()]
    public class DirectoryCommandTest
    {
        private const string TEST_XML_FILE_PATH = "C:\\Projects\\SSISUnit\\UTssisUnit\\UTSsisUnit_Directory.xml";
        private SsisTestSuite testSuite;
        //private XmlNode connections;
        private XmlDocument testCaseDoc;
        //private XmlNamespaceManager namespaceMgr;


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        [TestInitialize()]
        public void MyTestInitialize()
        {
            testSuite = new SsisTestSuite(TEST_XML_FILE_PATH);
            testCaseDoc = SsisTestSuite.LoadTestXmlFromFile(TEST_XML_FILE_PATH);
            //connections = testCaseDoc.DocumentElement["ConnectionList"];
            //namespaceMgr = new XmlNamespaceManager(testCaseDoc.NameTable);
            //namespaceMgr.AddNamespace("SsisUnit", "http://tempuri.org/SsisUnit.xsd");
        }        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        [TestMethod()]
        public void CreateTest()
        {
            DirectoryCommand target = new DirectoryCommand(testSuite);
            XmlNode command = testCaseDoc.DocumentElement["Setup"].ChildNodes[0];
            object actual;
            actual = target.Execute(command, null, null);
            Assert.AreEqual(0, actual);
        }

        [TestMethod()]
        public void MoveTest()
        {
            DirectoryCommand target = new DirectoryCommand(testSuite);
            XmlNode command = testCaseDoc.DocumentElement["Setup"].ChildNodes[1];
            object actual;
            actual = target.Execute(command, null, null);
            Assert.AreEqual(0, actual);
        }

        public void MoveWithoutTargetTest()
        {
            DirectoryCommand target = new DirectoryCommand(testSuite);
            XmlNode command = testCaseDoc.DocumentElement["Setup"].ChildNodes[2];
            object actual;
            try
            {
                actual = target.Execute(command, null, null);
                Assert.Fail("Method did not throw the expected ArgumentException.");
            }
            catch (ArgumentException)
            {
                Assert.IsTrue(true);
            }
        }

        [TestMethod()]
        public void ExistsTest()
        {
            DirectoryCommand target = new DirectoryCommand(testSuite);
            XmlNode command = testCaseDoc.DocumentElement["Setup"].ChildNodes[3];
            object actual;
            actual = target.Execute(command, null, null);
            Assert.AreEqual(true, actual);
        }

        [TestMethod()]
        public void DoesntExistTest()
        {
            DirectoryCommand target = new DirectoryCommand(testSuite);
            XmlNode command = testCaseDoc.DocumentElement["Setup"].ChildNodes[4];
            object actual;
            actual = target.Execute(command, null, null);
            Assert.AreEqual(false, actual);
        }


        [TestMethod()]
        public void DeleteTest()
        {
            DirectoryCommand target = new DirectoryCommand(testSuite);
            XmlNode command = testCaseDoc.DocumentElement["Setup"].ChildNodes[5];
            object actual;
            actual = target.Execute(command, null, null);
            Assert.AreEqual(0, actual);
        }

        [TestMethod()]
        public void FileCountTest()
        {
            DirectoryCommand target = new DirectoryCommand(testSuite);
            XmlNode command = testCaseDoc.DocumentElement["Setup"].ChildNodes[6];
            object actual;
            actual = target.Execute(command, null, null);
            Assert.AreEqual(3, actual);
        }
    }
}
