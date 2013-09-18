using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using SsisUnit;

namespace UTssisUnit
{
    [TestClass]
    public class DatasetTest
    {
        [TestMethod]
        public void RetrieveDataTableTest()
        {
            var ts = new SsisTestSuite();
            var connRef = new ConnectionRef("TestConn", "Data Source=localhost;Initial Catalog=AdventureWorks2012;Integrated Security=SSPI", ConnectionRef.ConnectionTypeEnum.AdoNet, "System.Data.SqlClient");
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

            var result = dataset.RetrieveDataTable();
            Assert.AreEqual(1, result.Rows.Count);
            Assert.AreEqual(1, result.Rows[0][0]);
            Assert.AreEqual("Test", result.Rows[0][1]);
            Assert.AreEqual("Test", result.Rows[0][2]);
            Assert.AreEqual(new DateTime(1900,1,1), result.Rows[0][3]);
        }
    }
}
