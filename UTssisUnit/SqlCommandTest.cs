using SsisUnit;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml;
using Microsoft.SqlServer.Dts.Runtime;
using System.Data.Common;

namespace UTssisUnit
{
    
    
    /// <summary>
    ///This is a test class for SqlCommandTest and is intended
    ///to contain all SqlCommandTest Unit Tests
    ///</summary>
    [TestClass()]
    public class SqlCommandTest
    {
        private const string TEST_XML_FILE_PATH = "C:\\Projects\\SSISUnit\\UTssisUnit\\UTSsisUnit.xml";
        private const string TEST_XML_FILE_BAD_DATA_PATH = "C:\\Projects\\SSISUnit\\UTssisUnit\\UTSsisUnit_BadData.xml";
        private const string TEST_DTSX_FILE_PATH = "C:\\Projects\\SSISUnit\\SSIS2005\\SSIS2005\\UT Basic Scenario.dtsx";

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
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion

        /// <summary>
        ///A test for SqlCommand Constructor
        ///</summary>
        [TestMethod()]
        public void SqlCommandConstructorTest()
        {
            XmlDocument testCaseDoc = SsisTestSuite.LoadTestXmlFromFile(TEST_XML_FILE_PATH);
            XmlNamespaceManager namespaceMgr = new XmlNamespaceManager(testCaseDoc.NameTable);
            namespaceMgr.AddNamespace("SsisUnit", "http://tempuri.org/SsisUnit.xsd");
            XmlNode connections = testCaseDoc.DocumentElement["ConnectionList"];

            SqlCommand target = new SqlCommand(new SsisTestSuite(TEST_XML_FILE_PATH));
            Assert.IsNotNull(target);
        }

        /// <summary>
        ///A test for Execute
        ///</summary>
        [TestMethod()]
        public void ExecuteNoResultsTest()
        {
            SqlCommand target = new SqlCommand(new SsisTestSuite(TEST_XML_FILE_PATH));
            XmlDocument doc = new XmlDocument();
            doc.Load(TEST_XML_FILE_PATH);
            XmlNode command = doc.DocumentElement["Setup"].ChildNodes[1];

            object result = target.Execute(command, null, null);
            Assert.IsNull(result);
        }

        [TestMethod()]
        public void ExecuteResultsTest()
        {
            SqlCommand target = new SqlCommand(new SsisTestSuite(TEST_XML_FILE_PATH));
            XmlDocument doc = new XmlDocument();
            doc.Load(TEST_XML_FILE_PATH);
            XmlNode command = doc.DocumentElement["Setup"]["SqlCommand"];

            object result = target.Execute(command, null, null);
            Assert.AreEqual(504, result);
        }

        [TestMethod()]
        public void ExecuteNoConnectionRefTest()
        {
            XmlDocument testCaseDoc = SsisTestSuite.LoadTestXmlFromFile(TEST_XML_FILE_BAD_DATA_PATH);
            XmlNamespaceManager namespaceMgr = new XmlNamespaceManager(testCaseDoc.NameTable);
            namespaceMgr.AddNamespace("SsisUnit", "http://tempuri.org/SsisUnit.xsd");
            XmlNode connections = testCaseDoc.DocumentElement["ConnectionList"];

            SqlCommand target = new SqlCommand(new SsisTestSuite(TEST_XML_FILE_BAD_DATA_PATH));
            XmlNode command = testCaseDoc.DocumentElement["Setup"].ChildNodes[0];

            try
            {
                object result = target.Execute(command, null, null);
                Assert.Fail("The method did not throw the expected key not found exception.");
            }
            catch (System.Collections.Generic.KeyNotFoundException)
            {
                Assert.IsTrue(true);
            }
            catch (System.Exception)
            {
                Assert.Fail("The method did not throw the expected key not found exception.");
            }
        }

        [TestMethod()]
        public void CommandTypeTest()
        {
            SqlCommand target = new SqlCommand(new SsisTestSuite(TEST_XML_FILE_PATH));
            Assert.AreEqual("SqlCommand", target.CommandName);
        }

        [TestMethod()]
        public void CommandPersistTest()
        {
            SqlCommand target = new SqlCommand(new SsisTestSuite(TEST_XML_FILE_PATH));
            target.ConnectionReference = new ConnectionRef("AdventureWorks", "", ConnectionRef.ConnectionTypeEnum.ConnectionString);
            target.ReturnsValue = false;
            target.SQLStatement = "DROP TABLE dbo.TestTable";
            string result = target.PersistToXml();
            Assert.AreEqual("<SqlCommand connectionRef=\"AdventureWorks\" returnsValue=\"false\">DROP TABLE dbo.TestTable</SqlCommand>", result);
        }

        [TestMethod()]
        public void CommandLoadTest()
        {
            SqlCommand target = new SqlCommand(new SsisTestSuite(TEST_XML_FILE_PATH));
            target.LoadFromXml("<SqlCommand connectionRef=\"AdventureWorks\" returnsValue=\"false\">DROP TABLE dbo.TestTable</SqlCommand>");
            Assert.AreEqual<string>("AdventureWorks", target.ConnectionReference.ReferenceName) ;
            Assert.AreEqual<bool>(false ,target.ReturnsValue);
            Assert.AreEqual<string>("DROP TABLE dbo.TestTable", target.SQLStatement);
        }
    }
}
