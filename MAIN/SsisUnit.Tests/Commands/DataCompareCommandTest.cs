using System;
using System.Data;

using Microsoft.SqlServer.Dts.Runtime;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using SsisUnit;
using SsisUnit.Enums;

using SsisUnitBase.Enums;

namespace UTssisUnit.Commands
{
    [TestClass]
    public class DataCompareCommandTest : ExternalFileResourceTestBase
    {
        private string _dtsxFilePath;

        [TestInitialize]
        public void Initialize()
        {
            _dtsxFilePath = UnpackToFile("UTssisUnit.TestPackages.SimplePackage.dtsx");
        }

        [TestMethod]
        public void DataCompareCommandConstructorTest()
        {
            var target = new DataCompareCommand(new SsisTestSuite());
            Assert.IsNotNull(target);
        }

        [TestMethod]
        public void RunDataCompareCommandSetTest()
        {
                        var ts = new SsisTestSuite();
            var connRef = new ConnectionRef("TestConn", "Data Source=DEV-QA-SQL2012;Initial Catalog=AdventureWorks;Integrated Security=SSPI", ConnectionRef.ConnectionTypeEnum.AdoNet, "System.Data.SqlClient");
            ts.ConnectionRefs.Add(connRef.ReferenceName, connRef);
            var dataset = new Dataset(
                ts,
                "Test",
                connRef,
                false,
                @"SELECT 
CAST(1 AS INT) AS ColInt, 
CAST('Test' AS VARCHAR(50)) AS ColVarChar, 
CAST(N'Test' AS NVARCHAR(50)) AS ColNVarChar, 
CAST('1900-01-01' AS DATETIME) AS ColDateTime");

            var target = new DataCompareCommand(ts, "Test", dataset, dataset);
            var actual = (DataCompareCommandResults) target.Execute();
            Assert.AreEqual(true, actual.IsDatasetsSame);
        }

        [TestMethod]
        public void RunDataCompareCommandSetWithOtherTestTest()
        {
            var ts = new SsisTestSuite();
            var connRef = new ConnectionRef("TestConn", "Data Source=DEV-QA-SQL2012;Initial Catalog=AdventureWorks;Integrated Security=SSPI", ConnectionRef.ConnectionTypeEnum.AdoNet, "System.Data.SqlClient");
            var pkgRef = new PackageRef("pkg", _dtsxFilePath, PackageStorageType.FileSystem);
            ts.ConnectionRefs.Add(connRef.ReferenceName, connRef);
            ts.PackageRefs.Add(pkgRef.Name, pkgRef);
            var dataset = new Dataset(
                ts,
                "Test",
                connRef,
                false,
                @"SELECT 
CAST(1 AS INT) AS ColInt, 
CAST('Test' AS VARCHAR(50)) AS ColVarChar, 
CAST(N'Test' AS NVARCHAR(50)) AS ColNVarChar, 
CAST('1900-01-01' AS DATETIME) AS ColDateTime");
            ts.Datasets.Add(dataset.Name, dataset);

            var target = new DataCompareCommand(ts, "Test", dataset, dataset);
            var test = new Test(ts, "TestCase1", "pkg", null, "\\Package");
            var assert = new SsisAssert(ts,test, "Assert1", true, false);
            assert.Command = target;
            test.Asserts.Add("Assert1", assert);
            ts.Tests.Add("TestCase1", test);
            var test2 = new Test(ts, "TestCase2", "pkg", null, "\\Package");
            var assert2 = new SsisAssert(ts, test2, "Assert2", false, false);
            assert2.Command = new FileCommand(ts, "Exists", @"C:\Test\Test.pkg", string.Empty);
            test2.Asserts.Add("Assert2", assert2);
            ts.Tests.Add("TestCase2", test2);

            var actual = ts.Execute();
            Assert.AreEqual(4, ts.Statistics.GetStatistic(StatisticEnum.AssertPassedCount));
        }
    }
}
