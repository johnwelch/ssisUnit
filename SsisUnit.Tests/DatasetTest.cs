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
            ts.ConnectionList.Add(connRef.ReferenceName, connRef);
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
            Assert.AreEqual(new DateTime(1900, 1, 1), result.Rows[0][3]);
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void RetrieveDataTableTestIsResultsStoredTrue()
        {
            var ts = new SsisTestSuite();
            var connRef = new ConnectionRef("TestConn", "Data Source=localhost;Initial Catalog=AdventureWorks2012;Integrated Security=SSPI", ConnectionRef.ConnectionTypeEnum.AdoNet, "System.Data.SqlClient");
            ts.ConnectionList.Add(connRef.ReferenceName, connRef);
            var dataset = new Dataset(
                ts,
                "Test",
                connRef,
                true,
                @"SELECT 
CAST(1 AS INT) AS ColInt, 
CAST('Test' AS VARCHAR(50)) AS ColVarChar, 
CAST(N'Test' AS NVARCHAR(50)) AS ColNVarChar, 
CAST('1900-01-01' AS DATETIME) AS ColDateTime");

            var result = dataset.RetrieveDataTable();
        }

        [TestMethod]
        public void PersistDatasetWithoutResultsIsResultsStoredFalseTest()
        {
            var ts = new SsisTestSuite();
            var connRef = new ConnectionRef("TestConn", "Data Source=localhost;Initial Catalog=AdventureWorks2012;Integrated Security=SSPI", ConnectionRef.ConnectionTypeEnum.AdoNet, "System.Data.SqlClient");
            ts.ConnectionList.Add(connRef.ReferenceName, connRef);
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

            string datasetXml = "<Dataset name=\"Test\" connection=\"TestConn\" isResultsStored=\"false\"><query><![CDATA[SELECT \r\nCAST(1 AS INT) AS ColInt, \r\nCAST('Test' AS VARCHAR(50)) AS ColVarChar, \r\nCAST(N'Test' AS NVARCHAR(50)) AS ColNVarChar, \r\nCAST('1900-01-01' AS DATETIME) AS ColDateTime]]></query></Dataset>";

            string result = dataset.PersistToXml();
            Assert.AreEqual(datasetXml, result);
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void PersistDatasetWithoutResultsIsResultsStoredTrueTest()
        {
            var ts = new SsisTestSuite();
            var connRef = new ConnectionRef("TestConn", "Data Source=localhost;Initial Catalog=AdventureWorks2012;Integrated Security=SSPI", ConnectionRef.ConnectionTypeEnum.AdoNet, "System.Data.SqlClient");
            ts.ConnectionList.Add(connRef.ReferenceName, connRef);
            var dataset = new Dataset(
                ts,
                "Test",
                connRef,
                true,
                @"SELECT 
CAST(1 AS INT) AS ColInt, 
CAST('Test' AS VARCHAR(50)) AS ColVarChar, 
CAST(N'Test' AS NVARCHAR(50)) AS ColNVarChar, 
CAST('1900-01-01' AS DATETIME) AS ColDateTime");

            //string datasetXml = "<Dataset name=\"Test\" connection=\"TestConn\" isResultsStored=\"true\"><query><![CDATA[SELECT \r\nCAST(1 AS INT) AS ColInt, \r\nCAST('Test' AS VARCHAR(50)) AS ColVarChar, \r\nCAST(N'Test' AS NVARCHAR(50)) AS ColNVarChar, \r\nCAST('1900-01-01' AS DATETIME) AS ColDateTime]]></query></Dataset>";

            dataset.Results = dataset.RetrieveDataTable();
        }

        [TestMethod]
        public void PersistDataSetWithResultsIsResultsStoredTrueTest()
        {
            var ts = new SsisTestSuite();
            var connRef = new ConnectionRef("TestConn", "Data Source=localhost;Initial Catalog=AdventureWorks2012;Integrated Security=SSPI", ConnectionRef.ConnectionTypeEnum.AdoNet, "System.Data.SqlClient");
            ts.ConnectionList.Add(connRef.ReferenceName, connRef);
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

            dataset.Results = dataset.RetrieveDataTable();
            dataset.IsResultsStored = true;

            string datasetXml = @"<Dataset name=""Test"" connection=""TestConn"" isResultsStored=""true""><query><![CDATA[SELECT 
CAST(1 AS INT) AS ColInt, 
CAST('Test' AS VARCHAR(50)) AS ColVarChar, 
CAST(N'Test' AS NVARCHAR(50)) AS ColNVarChar, 
CAST('1900-01-01' AS DATETIME) AS ColDateTime]]></query><results><![CDATA[<NewDataSet>
  <xs:schema id=""NewDataSet"" xmlns="""" xmlns:xs=""http://www.w3.org/2001/XMLSchema"" xmlns:msdata=""urn:schemas-microsoft-com:xml-msdata"">
    <xs:element name=""NewDataSet"" msdata:IsDataSet=""true"" msdata:MainDataTable=""Results"" msdata:UseCurrentLocale=""true"">
      <xs:complexType>
        <xs:choice minOccurs=""0"" maxOccurs=""unbounded"">
          <xs:element name=""Results"">
            <xs:complexType>
              <xs:sequence>
                <xs:element name=""ColInt"" msdata:ReadOnly=""true"" type=""xs:int"" minOccurs=""0"" />
                <xs:element name=""ColVarChar"" msdata:ReadOnly=""true"" minOccurs=""0"">
                  <xs:simpleType>
                    <xs:restriction base=""xs:string"">
                      <xs:maxLength value=""50"" />
                    </xs:restriction>
                  </xs:simpleType>
                </xs:element>
                <xs:element name=""ColNVarChar"" msdata:ReadOnly=""true"" minOccurs=""0"">
                  <xs:simpleType>
                    <xs:restriction base=""xs:string"">
                      <xs:maxLength value=""50"" />
                    </xs:restriction>
                  </xs:simpleType>
                </xs:element>
                <xs:element name=""ColDateTime"" msdata:ReadOnly=""true"" type=""xs:dateTime"" minOccurs=""0"" />
              </xs:sequence>
            </xs:complexType>
          </xs:element>
        </xs:choice>
      </xs:complexType>
    </xs:element>
  </xs:schema>
  <Results>
    <ColInt>1</ColInt>
    <ColVarChar>Test</ColVarChar>
    <ColNVarChar>Test</ColNVarChar>
    <ColDateTime>1900-01-01T00:00:00+01:00</ColDateTime>
  </Results>
</NewDataSet>]]></results></Dataset>";

            string result = dataset.PersistToXml();
            Assert.AreEqual(datasetXml, result);
        }

        [TestMethod]
        public void PersistDataSetWithResultsIsResultsStoredFalseTest()
        {
            var ts = new SsisTestSuite();
            var connRef = new ConnectionRef("TestConn", "Data Source=localhost;Initial Catalog=AdventureWorks2012;Integrated Security=SSPI", ConnectionRef.ConnectionTypeEnum.AdoNet, "System.Data.SqlClient");
            ts.ConnectionList.Add(connRef.ReferenceName, connRef);
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

            dataset.Results = dataset.RetrieveDataTable();

            string datasetXml = @"<Dataset name=""Test"" connection=""TestConn"" isResultsStored=""false""><query><![CDATA[SELECT 
CAST(1 AS INT) AS ColInt, 
CAST('Test' AS VARCHAR(50)) AS ColVarChar, 
CAST(N'Test' AS NVARCHAR(50)) AS ColNVarChar, 
CAST('1900-01-01' AS DATETIME) AS ColDateTime]]></query><results><![CDATA[<NewDataSet>
  <xs:schema id=""NewDataSet"" xmlns="""" xmlns:xs=""http://www.w3.org/2001/XMLSchema"" xmlns:msdata=""urn:schemas-microsoft-com:xml-msdata"">
    <xs:element name=""NewDataSet"" msdata:IsDataSet=""true"" msdata:MainDataTable=""Results"" msdata:UseCurrentLocale=""true"">
      <xs:complexType>
        <xs:choice minOccurs=""0"" maxOccurs=""unbounded"">
          <xs:element name=""Results"">
            <xs:complexType>
              <xs:sequence>
                <xs:element name=""ColInt"" msdata:ReadOnly=""true"" type=""xs:int"" minOccurs=""0"" />
                <xs:element name=""ColVarChar"" msdata:ReadOnly=""true"" minOccurs=""0"">
                  <xs:simpleType>
                    <xs:restriction base=""xs:string"">
                      <xs:maxLength value=""50"" />
                    </xs:restriction>
                  </xs:simpleType>
                </xs:element>
                <xs:element name=""ColNVarChar"" msdata:ReadOnly=""true"" minOccurs=""0"">
                  <xs:simpleType>
                    <xs:restriction base=""xs:string"">
                      <xs:maxLength value=""50"" />
                    </xs:restriction>
                  </xs:simpleType>
                </xs:element>
                <xs:element name=""ColDateTime"" msdata:ReadOnly=""true"" type=""xs:dateTime"" minOccurs=""0"" />
              </xs:sequence>
            </xs:complexType>
          </xs:element>
        </xs:choice>
      </xs:complexType>
    </xs:element>
  </xs:schema>
  <Results>
    <ColInt>1</ColInt>
    <ColVarChar>Test</ColVarChar>
    <ColNVarChar>Test</ColNVarChar>
    <ColDateTime>1900-01-01T00:00:00+01:00</ColDateTime>
  </Results>
</NewDataSet>]]></results></Dataset>";
            string result = dataset.PersistToXml();
            Assert.AreEqual(datasetXml, result);
        }
    }
}
