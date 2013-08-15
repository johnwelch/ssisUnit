using SsisUnit;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml;
using System;

namespace UTssisUnit
{
    [TestClass]
    public class SsisAssertTest : ExternalFileResourceTestBase
    {
        private string _dtsxFilePath;

        [TestInitialize]
        public void Initialize()
        {
            _dtsxFilePath = this.UnpackToFile("UTssisUnit.TestPackages.UTBasicScenario2012.dtsx");
        }

        [TestMethod]
        public void LoadFromXmlTest1()
        {
            var testSuite = new SsisTestSuite();
            string assertXml = "<Assert name=\"Test\" expectedResult=\"1\" testBefore=\"false\" expression=\"false\">";
            assertXml += "<SqlCommand name=\"\" connectionRef=\"AdventureWorks\" returnsValue=\"true\">";
            assertXml += "SELECT COUNT(*) FROM Production.Product";
            assertXml += "</SqlCommand>";
            assertXml += "</Assert>";
            XmlNode assertXml1 = Helper.GetXmlNodeFromString(assertXml);
            var target = new SsisAssert(testSuite, string.Empty, null, true);
            target.LoadFromXml(assertXml1);
            Assert.AreEqual(assertXml, target.PersistToXml());
        }

        [TestMethod]
        public void LoadFromXmlTest()
        {
            string assertXml = "<Assert name=\"Test\" expectedResult=\"1\" testBefore=\"false\" expression=\"false\">";
            assertXml += "<SqlCommand name=\"\" connectionRef=\"AdventureWorks\" returnsValue=\"true\">";
            assertXml += "SELECT COUNT(*) FROM Production.Product";
            assertXml += "</SqlCommand>";
            assertXml += "</Assert>";

            var testSuite = new SsisTestSuite();
            var target = new SsisAssert(testSuite, string.Empty, null, true);
            target.LoadFromXml(assertXml);
            Assert.AreEqual(assertXml, target.PersistToXml());
        }

        [TestMethod]
        public void SsisAssertConstructorTest()
        {
            var testSuite = new SsisTestSuite();
            const string Name = "Test";
            object expectedResult = 100;

            var ssisTest = new Test(testSuite, "Test", "C:\\Projects\\SSISUnit\\SSIS2005\\SSIS2005\\UT Basic Scenario.dtsx", "SELECT COUNT");
            testSuite.Tests.Add("Test", ssisTest);
            var target = new SsisAssert(testSuite, Name, expectedResult, false);
            ssisTest.Asserts.Add("Test", target);
            Assert.AreEqual(1, ssisTest.Asserts.Count);
            Assert.AreEqual("Test", ssisTest.Asserts["Test"].Name);
            Assert.AreEqual(100, ssisTest.Asserts["Test"].ExpectedResult);
            Assert.AreEqual(false, ssisTest.Asserts["Test"].TestBefore);
         }

        [TestMethod]
        public void PersistToXmlTest()
        {
            string assertXml = "<Assert name=\"Test\" expectedResult=\"1\" testBefore=\"false\" expression=\"false\">";
            assertXml += "<SqlCommand name=\"\" connectionRef=\"AdventureWorks\" returnsValue=\"true\">";
            assertXml += "SELECT COUNT(*) FROM Production.Product";
            assertXml += "</SqlCommand>";
            assertXml += "</Assert>";

            var testSuite = new SsisTestSuite();
            var target = new SsisAssert(testSuite, assertXml);

            Assert.AreEqual(assertXml, target.PersistToXml());
        }

        [TestMethod]
        public void SsisAssertConstructorTest1()
        {
            var testSuite = new SsisTestSuite();
            string assertXml = "<Assert name=\"Test\" expectedResult=\"1\" testBefore=\"false\" expression=\"false\">";
            assertXml += "<SqlCommand name=\"\" connectionRef=\"AdventureWorks\" returnsValue=\"true\">";
            assertXml += "SELECT COUNT(*) FROM Production.Product";
            assertXml += "</SqlCommand>";
            assertXml += "</Assert>";
            XmlNode assertXml1 = Helper.GetXmlNodeFromString(assertXml);
            var target = new SsisAssert(testSuite, assertXml1);
            Assert.AreEqual("Test", target.Name);
            Assert.AreEqual("1", target.ExpectedResult.ToString());
            Assert.AreEqual(false, target.TestBefore);
            Assert.AreEqual("SqlCommand", target.Command.CommandName);
        }

        [TestMethod]
        public void SsisAssertConstructorTest2()
        {
            var testSuite = new SsisTestSuite();
            string assertXml = "<Assert name=\"Test\" expectedResult=\"1\" testBefore=\"false\" expression=\"false\">";
            assertXml += "<SqlCommand name=\"\" connectionRef=\"AdventureWorks\" returnsValue=\"true\">";
            assertXml += "SELECT COUNT(*) FROM Production.Product";
            assertXml += "</SqlCommand>";
            assertXml += "</Assert>";
            var target = new SsisAssert(testSuite, assertXml);
            Assert.AreEqual("Test", target.Name);
            Assert.AreEqual("1", target.ExpectedResult.ToString());
            Assert.AreEqual(false, target.TestBefore);
            Assert.AreEqual("SqlCommand", target.Command.CommandName);
        }

        [TestMethod]
        public void TestDynamicAssert()
        {
            var target = new SsisTestSuite();

            target.ConnectionRefs.Add("AdventureWorks", new ConnectionRef("AdventureWorks",
                "Provider=SQLNCLI11;Data Source=localhost;Integrated Security=SSPI;Initial Catalog=tempdb",
                ConnectionRef.ConnectionTypeEnum.ConnectionString));
            target.PackageRefs.Add("UT Basic Scenario", new PackageRef("UT Basic Scenario", _dtsxFilePath, PackageRef.PackageStorageType.FileSystem));

            target.TestSuiteSetup.Commands.Add(new SqlCommand(target, "AdventureWorks", false, "CREATE TABLE dbo.Test (ID INT)"));
            target.TestSuiteSetup.Commands.Add(new SqlCommand(target, "AdventureWorks", false, "INSERT INTO dbo.Test VALUES (1)"));
            target.TestSuiteSetup.Commands.Add(new SqlCommand(target, "AdventureWorks", false, "INSERT INTO dbo.Test VALUES (2)"));

            var ssisTest = new Test(target, "Test", "UT Basic Scenario", "SELECT COUNT");
            target.Tests.Add("Test", ssisTest);

            var ssisAssert = new SsisAssert(target, "Test Count", "(int)result==2", false, true);
            ssisAssert.Command = new SqlCommand(target, "AdventureWorks", true, "SELECT COUNT(*) FROM dbo.Test");

            ssisTest.Asserts.Add("Test Count", ssisAssert);

            ssisAssert = new SsisAssert(target, "Test Count 2", "(int)result<=2", false, true);
            ssisAssert.Command = new SqlCommand(target, "AdventureWorks", true, "SELECT COUNT(*) FROM dbo.Test");

            ssisTest.Asserts.Add("Test Count 2", ssisAssert);

            ssisAssert = new SsisAssert(target, "Test Count 3", "\"test\"==\"test\"", false, true);
            ssisAssert.Command = new SqlCommand(target, "AdventureWorks", true, "SELECT COUNT(*) FROM dbo.Test");

            ssisTest.Asserts.Add("Test Count 3", ssisAssert);

            ssisAssert = new SsisAssert(target, "Test Count 4", "DateTime.Now.Date==((DateTime)result).Date", false, true);
            ssisAssert.Command = new VariableCommand(target, VariableCommand.VariableOperation.Get, "System::StartTime", string.Empty);

            ssisTest.Asserts.Add("Test Count 4", ssisAssert);

            target.TestSuiteTeardown.Commands.Add(new SqlCommand(target, "AdventureWorks", false, "DROP TABLE dbo.Test"));

            int testCount = 0;

            try
            {
                testCount = target.Execute();
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }

            Assert.AreEqual(1, testCount);
            Assert.AreEqual(1, target.Statistics.GetStatistic(TestSuiteResults.StatisticEnum.TestPassedCount));
            Assert.AreEqual(5, target.Statistics.GetStatistic(TestSuiteResults.StatisticEnum.AssertPassedCount));
            Assert.AreEqual(0, target.Statistics.GetStatistic(TestSuiteResults.StatisticEnum.AssertFailedCount));
        }

        [TestMethod]
        public void TestCompileFail()
        {
            var target = new SsisTestSuite();

            target.ConnectionRefs.Add("AdventureWorks", new ConnectionRef("AdventureWorks",
                "Provider=SQLNCLI11;Data Source=localhost;Integrated Security=SSPI;Initial Catalog=tempdb",
                ConnectionRef.ConnectionTypeEnum.ConnectionString));
            target.PackageRefs.Add("UT Basic Scenario", new PackageRef("UT Basic Scenario", _dtsxFilePath, PackageRef.PackageStorageType.FileSystem));

            target.TestSuiteSetup.Commands.Add(new SqlCommand(target, "AdventureWorks", false, "CREATE TABLE dbo.Test (ID INT)"));
            target.TestSuiteSetup.Commands.Add(new SqlCommand(target, "AdventureWorks", false, "INSERT INTO dbo.Test VALUES (1)"));
            target.TestSuiteSetup.Commands.Add(new SqlCommand(target, "AdventureWorks", false, "INSERT INTO dbo.Test VALUES (2)"));

            var ssisTest = new Test(target, "Test", "UT Basic Scenario", "SELECT COUNT");
            target.Tests.Add("Test", ssisTest);

            var ssisAssert = new SsisAssert(target, "Test Count", "result==2", false);
            ssisAssert.Command = new SqlCommand(target, "AdventureWorks", true, "SELECT COUNT(*) FROM dbo.Test");

            ssisTest.Asserts.Add("Test Count", ssisAssert);

            target.TestSuiteTeardown.Commands.Add(new SqlCommand(target, "AdventureWorks", false, "DROP TABLE dbo.Test"));

            int testCount = 0;

            try
            {
                testCount = target.Execute();
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }

            Assert.AreEqual(1, testCount);
            Assert.AreEqual(1, target.Statistics.GetStatistic(TestSuiteResults.StatisticEnum.TestPassedCount));
            Assert.AreEqual(1, target.Statistics.GetStatistic(TestSuiteResults.StatisticEnum.AssertPassedCount));
            Assert.AreEqual(1, target.Statistics.GetStatistic(TestSuiteResults.StatisticEnum.AssertFailedCount));
        }

        [TestMethod]
        public void TestAssertCreation()
        {
            string assertXml = "<Assert name=\"Test\" expectedResult=\"\" testBefore=\"false\" expression=\"false\" ";
            assertXml += "/>";

            var testSuite = new SsisTestSuite();
            var ssisAssert = new SsisAssert(testSuite, "Test", null, false, false);

            Assert.AreEqual(assertXml, ssisAssert.PersistToXml());
        }
    }
}
