using SsisUnit;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml;
using System;

using SsisUnit.Enums;
using SsisUnit.Packages;

using SsisUnitBase.Enums;

namespace UTssisUnit
{
    [TestClass]
    public class SsisAssertTest : ExternalFileResourceTestBase
    {
        private string _dtsxFilePath;

        [TestInitialize]
        public void Initialize()
        {
            _dtsxFilePath = UnpackToFile("UTssisUnit.TestPackages.UTBasicScenario2012.dtsx");
        }

        [TestMethod]
        public void LoadFromXmlTest1()
        {
            var testSuite = new SsisTestSuite();
            var ssisTest = new Test(testSuite, "Test", _dtsxFilePath, null, "SELECT COUNT");
            string assertXml = "<Assert name=\"Test\" expectedResult=\"1\" testBefore=\"false\" expression=\"false\">";
            assertXml += "<SqlCommand name=\"\" connectionRef=\"AdventureWorks\" returnsValue=\"true\">";
            assertXml += "SELECT COUNT(*) FROM Production.Product";
            assertXml += "</SqlCommand>";
            assertXml += "</Assert>";
            XmlNode assertXml1 = Helper.GetXmlNodeFromString(assertXml);
            var target = new SsisAssert(testSuite, ssisTest, string.Empty, null, true);
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
            var ssisTest = new Test(testSuite, "Test", _dtsxFilePath, null, "SELECT COUNT");
            var target = new SsisAssert(testSuite, ssisTest, string.Empty, null, true);
            target.LoadFromXml(assertXml);
            Assert.AreEqual(assertXml, target.PersistToXml());
        }

        [TestMethod]
        public void SsisAssertConstructorTest()
        {
            var testSuite = new SsisTestSuite();
            const string Name = "Test";
            object expectedResult = 100;

            var ssisTest = new Test(testSuite, "Test", "C:\\Projects\\SSISUnit\\SSIS2005\\SSIS2005\\UT Basic Scenario.dtsx", null, "SELECT COUNT");
            testSuite.Tests.Add("Test", ssisTest);
            var target = new SsisAssert(testSuite, ssisTest, Name, expectedResult, false);
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
            var ssisTest = new Test(testSuite, "Test", _dtsxFilePath, null, "SELECT COUNT");
            var target = new SsisAssert(testSuite, ssisTest, assertXml);

            Assert.AreEqual(assertXml, target.PersistToXml());
        }

        [TestMethod]
        public void SsisAssertConstructorTest1()
        {
            var testSuite = new SsisTestSuite();
            var ssisTest = new Test(testSuite, "Test", _dtsxFilePath, null, "SELECT COUNT");
            string assertXml = "<Assert name=\"Test\" expectedResult=\"1\" testBefore=\"false\" expression=\"false\">";
            assertXml += "<SqlCommand name=\"\" connectionRef=\"AdventureWorks\" returnsValue=\"true\">";
            assertXml += "SELECT COUNT(*) FROM Production.Product";
            assertXml += "</SqlCommand>";
            assertXml += "</Assert>";
            XmlNode assertXml1 = Helper.GetXmlNodeFromString(assertXml);
            var target = new SsisAssert(testSuite, ssisTest, assertXml1);
            Assert.AreEqual("Test", target.Name);
            Assert.AreEqual("1", target.ExpectedResult.ToString());
            Assert.AreEqual(false, target.TestBefore);
            Assert.AreEqual("SqlCommand", target.Command.CommandName);
        }

        [TestMethod]
        public void SsisAssertConstructorTest2()
        {
            var testSuite = new SsisTestSuite();
            var ssisTest = new Test(testSuite, "Test", _dtsxFilePath, null, "SELECT COUNT");
            string assertXml = "<Assert name=\"Test\" expectedResult=\"1\" testBefore=\"false\" expression=\"false\">";
            assertXml += "<SqlCommand name=\"\" connectionRef=\"AdventureWorks\" returnsValue=\"true\">";
            assertXml += "SELECT COUNT(*) FROM Production.Product";
            assertXml += "</SqlCommand>";
            assertXml += "</Assert>";
            var target = new SsisAssert(testSuite, ssisTest, assertXml);
            Assert.AreEqual("Test", target.Name);
            Assert.AreEqual("1", target.ExpectedResult.ToString());
            Assert.AreEqual(false, target.TestBefore);
            Assert.AreEqual("SqlCommand", target.Command.CommandName);
        }

        [TestMethod]
        public void TestDynamicAssert()
        {
            var target = new SsisTestSuite();

            target.ConnectionList.Add("AdventureWorks", new ConnectionRef("AdventureWorks",
                "Provider=SQLNCLI11;Data Source=localhost;Integrated Security=SSPI;Initial Catalog=tempdb",
                ConnectionRef.ConnectionTypeEnum.ConnectionString));
            target.PackageList.Add("UT Basic Scenario", new PackageRef("UT Basic Scenario", _dtsxFilePath, PackageStorageType.FileSystem));

            target.TestSuiteSetup.Commands.Add(new SqlCommand(target, "AdventureWorks", false, "CREATE TABLE dbo.Test (ID INT)"));
            target.TestSuiteSetup.Commands.Add(new SqlCommand(target, "AdventureWorks", false, "INSERT INTO dbo.Test VALUES (1)"));
            target.TestSuiteSetup.Commands.Add(new SqlCommand(target, "AdventureWorks", false, "INSERT INTO dbo.Test VALUES (2)"));

            var ssisTest = new Test(target, "Test", "UT Basic Scenario", null, "SELECT COUNT");
            target.Tests.Add("Test", ssisTest);

            var ssisAssert = new SsisAssert(target, ssisTest, "Test Count", "(int)result==2", false, true);
            ssisAssert.Command = new SqlCommand(target, "AdventureWorks", true, "SELECT COUNT(*) FROM dbo.Test");

            ssisTest.Asserts.Add("Test Count", ssisAssert);

            ssisAssert = new SsisAssert(target, ssisTest, "Test Count 2", "(int)result<=2", false, true);
            ssisAssert.Command = new SqlCommand(target, "AdventureWorks", true, "SELECT COUNT(*) FROM dbo.Test");

            ssisTest.Asserts.Add("Test Count 2", ssisAssert);

            ssisAssert = new SsisAssert(target, ssisTest, "Test Count 3", "\"test\"==\"test\"", false, true);
            ssisAssert.Command = new SqlCommand(target, "AdventureWorks", true, "SELECT COUNT(*) FROM dbo.Test");

            ssisTest.Asserts.Add("Test Count 3", ssisAssert);

            ssisAssert = new SsisAssert(target, ssisTest, "Test Count 4", "DateTime.Now.Date==((DateTime)result).Date", false, true);
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
            Assert.AreEqual(1, target.Statistics.GetStatistic(StatisticEnum.TestPassedCount));
            Assert.AreEqual(5, target.Statistics.GetStatistic(StatisticEnum.AssertPassedCount));
            Assert.AreEqual(0, target.Statistics.GetStatistic(StatisticEnum.AssertFailedCount));
        }

        [TestMethod]
        public void TestCompileFail()
        {
            var target = new SsisTestSuite();

            target.ConnectionList.Add("AdventureWorks", new ConnectionRef("AdventureWorks",
                "Provider=SQLNCLI11;Data Source=localhost;Integrated Security=SSPI;Initial Catalog=tempdb",
                ConnectionRef.ConnectionTypeEnum.ConnectionString));
            target.PackageList.Add("UT Basic Scenario", new PackageRef("UT Basic Scenario", _dtsxFilePath, PackageStorageType.FileSystem));

            target.TestSuiteSetup.Commands.Add(new SqlCommand(target, "AdventureWorks", false, "CREATE TABLE dbo.Test (ID INT)"));
            target.TestSuiteSetup.Commands.Add(new SqlCommand(target, "AdventureWorks", false, "INSERT INTO dbo.Test VALUES (1)"));
            target.TestSuiteSetup.Commands.Add(new SqlCommand(target, "AdventureWorks", false, "INSERT INTO dbo.Test VALUES (2)"));

            var ssisTest = new Test(target, "Test", "UT Basic Scenario", null, "SELECT COUNT");
            target.Tests.Add("Test", ssisTest);

            var ssisAssert = new SsisAssert(target, ssisTest, "Test Count", "result==2", false);
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
            Assert.AreEqual(1, target.Statistics.GetStatistic(StatisticEnum.TestPassedCount));
            Assert.AreEqual(1, target.Statistics.GetStatistic(StatisticEnum.AssertPassedCount));
            Assert.AreEqual(1, target.Statistics.GetStatistic(StatisticEnum.AssertFailedCount));
        }

        [TestMethod]
        public void TestAssertCreation()
        {
            string assertXml = "<Assert name=\"Test\" expectedResult=\"\" testBefore=\"false\" expression=\"false\" ";
            assertXml += "/>";

            var testSuite = new SsisTestSuite();
            var ssisTest = new Test(testSuite, "Test", _dtsxFilePath, null, "SELECT COUNT");
            var ssisAssert = new SsisAssert(testSuite, ssisTest, "Test", null, false, false);

            Assert.AreEqual(assertXml, ssisAssert.PersistToXml());
        }

        [TestMethod]
        public void TestAssertCommandFailure()
        {
            var target = new SsisTestSuite();

            target.ConnectionList.Add("AdventureWorks", new ConnectionRef("AdventureWorks",
                "Provider=SQLNCLI11;Data Source=localhost;Integrated Security=SSPI;Initial Catalog=tempdb",
                ConnectionRef.ConnectionTypeEnum.ConnectionString));
            target.PackageList.Add("UT Basic Scenario", new PackageRef("UT Basic Scenario", _dtsxFilePath, PackageStorageType.FileSystem));

            var ssisTest = new Test(target, "Test", "UT Basic Scenario", null, "SELECT COUNT");
            target.Tests.Add("Test", ssisTest);

            var ssisAssert = new SsisAssert(target, ssisTest, "Test Count", "(int)result==2", false, true);
            ssisAssert.Command = new SqlCommand(target, "AdventureWorks", true, "SELECT COUNT(*) FROM sys,.tables WHERE 1='test'");

            ssisTest.Asserts.Add("Test Count", ssisAssert);

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
            Assert.AreEqual(1, target.Statistics.GetStatistic(StatisticEnum.TestPassedCount));
            Assert.AreEqual(1, target.Statistics.GetStatistic(StatisticEnum.AssertPassedCount));
            Assert.AreEqual(1, target.Statistics.GetStatistic(StatisticEnum.AssertFailedCount));
        }

        [TestMethod]
        public void TestAssertCommandBoolean()
        {
            var target = new SsisTestSuite();

            target.PackageList.Add("UT Basic Scenario", new PackageRef("UT Basic Scenario", _dtsxFilePath, PackageStorageType.FileSystem));
            var ssisTest = new Test(target, "Test", "UT Basic Scenario", null, "SELECT COUNT");
            target.Tests.Add("Test", ssisTest);

            var ssisAssert = new SsisAssert(target, ssisTest, "Test Count", true, false, false);
            ssisAssert.Command = new Commands.TestCommand();
            ssisTest.Asserts.Add("Test Count", ssisAssert);
            
            ssisAssert = new SsisAssert(target, ssisTest, "Test Count 1", "True", false, false);
            ssisAssert.Command = new Commands.TestCommand();
            ssisTest.Asserts.Add("Test Count 1", ssisAssert);

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
            Assert.AreEqual(1, target.Statistics.GetStatistic(StatisticEnum.TestPassedCount));
            Assert.AreEqual(3, target.Statistics.GetStatistic(StatisticEnum.AssertPassedCount));
        }
    }
}
