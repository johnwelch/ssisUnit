using System;
using System.Diagnostics;
using System.Xml;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using SsisUnit;

namespace UTssisUnit.Commands
{
    [TestClass]
    public class SqlCommandTest : ExternalFileResourceTestBase
    {
        private string _badTestFile;
        private string _testFile;
        private SsisTestSuite _testSuite;

        [TestInitialize]
        public void Initialize()
        {
            _badTestFile = UnpackToFile("UTssisUnit.SampleSsisUnitTests.UTSsisUnit_BadData.ssisUnit");
            _testFile = UnpackToFile("UTssisUnit.SampleSsisUnitTests.UTSsisUnit.ssisUnit");
            _testSuite = new SsisTestSuite(_testFile);
            _testSuite.ConnectionRefs["AdventureWorks"].ConnectionString = "Provider=SQLNCLI11;Data Source=localhost;Integrated Security=SSPI;Initial Catalog=tempdb";
        }

        [TestMethod]
        public void SqlCommandConstructorTest()
        {
            var target = new SqlCommand(_testSuite);
            Assert.IsNotNull(target);
        }

        [TestMethod]
        public void ExecuteNoResultsTest()
        {
            var target = new SqlCommand(_testSuite);
            var doc = new XmlDocument();
            doc.Load(_testFile);
            Debug.Assert(doc.DocumentElement != null, "doc.DocumentElement != null");
            Debug.Assert(doc.DocumentElement["Setup"] != null, "doc.DocumentElement != null");
            XmlNode command = doc.DocumentElement["Setup"].ChildNodes[1];

            object result = target.Execute(command, null, null);
            Assert.IsNull(result);
        }

        [TestMethod]
        public void ExecuteResultsTest()
        {
            var target = new SqlCommand(_testSuite);
            var doc = new XmlDocument();
            doc.Load(_testFile);
            Debug.Assert(doc.DocumentElement != null, "doc.DocumentElement != null");
            Debug.Assert(doc.DocumentElement["Setup"] != null, "doc.DocumentElement != null");
            XmlNode command = doc.DocumentElement["Setup"]["SqlCommand"];

            object result = target.Execute(command, null, null);
            Assert.AreEqual(80, result);
        }

        [TestMethod]
        public void ExecuteNoConnectionRefTest()
        {
            var testCaseDoc = new XmlDocument();
            testCaseDoc.Load(_badTestFile);

            Debug.Assert(testCaseDoc.DocumentElement != null, "doc.DocumentElement != null");
            Debug.Assert(testCaseDoc.DocumentElement["Setup"] != null, "doc.DocumentElement != null");

            var target = new SqlCommand(new SsisTestSuite(_badTestFile), testCaseDoc.DocumentElement["Setup"].ChildNodes[0]);

            try
            {
                target.Execute(null, null);
                Assert.Fail("The method did not throw the expected key not found exception.");
            }
            catch (ApplicationException)
            {
                Assert.IsTrue(true);
            }
            catch (Exception)
            {
                Assert.Fail("The method did not throw the expected key not found exception.");
            }
        }

        [TestMethod]
        public void CommandTypeTest()
        {
            var target = new SqlCommand(_testSuite);
            Assert.AreEqual("SqlCommand", target.CommandName);
        }

        [TestMethod]
        public void CommandPersistTest()
        {
            var target = new SqlCommand(_testSuite)
                {
                    ConnectionReference = new ConnectionRef("AdventureWorks", string.Empty, ConnectionRef.ConnectionTypeEnum.ConnectionString),
                    ReturnsValue = false,
                    SQLStatement = "DROP TABLE dbo.TestTable"
                };
            string result = target.PersistToXml();
            Assert.AreEqual("<SqlCommand name=\"\" connectionRef=\"AdventureWorks\" returnsValue=\"false\">DROP TABLE dbo.TestTable</SqlCommand>", result);
        }

        [TestMethod]
        public void CommandLoadTest()
        {
            var target = new SqlCommand(_testSuite);
            target.LoadFromXml("<SqlCommand connectionRef=\"AdventureWorks\" returnsValue=\"false\">DROP TABLE dbo.TestTable</SqlCommand>");
            Assert.AreEqual("AdventureWorks", target.ConnectionReference.ReferenceName);
            Assert.AreEqual(false, target.ReturnsValue);
            Assert.AreEqual("DROP TABLE dbo.TestTable", target.SQLStatement);
        }
    }
}
