using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;

namespace SsisUnit
{
    public class Dataset
    {
        private const string DatasetElementName = "Dataset";
        private const string ResultsDataTableName = "Results";

        public Dataset(SsisTestSuite ssisTestSuite, XmlNode datasetNode)
        {
            if (ssisTestSuite == null)
                throw new ArgumentNullException("ssisTestSuite");

            TestSuite = ssisTestSuite;

            LoadFromXml(datasetNode);
        }

        public Dataset(SsisTestSuite ssisTestSuite, string name, ConnectionRef connectionReference, bool isResultsStored, string query)
        {
            if (ssisTestSuite == null)
                throw new ArgumentNullException("ssisTestSuite");

            Name = name;
            ConnectionRef = connectionReference;
            IsResultsStored = isResultsStored;
            Query = query;
            TestSuite = ssisTestSuite;
        }

        public Dataset(SsisTestSuite ssisTestSuite, string name, ConnectionRef connectionReference, bool isResultsStored, string query, DataTable results)
            : this(ssisTestSuite, name, connectionReference, isResultsStored, query)
        {
            Results = results;
        }

        public void LoadFromXml(string packageXml)
        {
            LoadFromXml(Helper.GetXmlNodeFromString(packageXml));
        }

        private void LoadFromXml(XmlNode datasetNode)
        {
            if (datasetNode.Name != DatasetElementName)
                throw new ArgumentException(string.Format("The Xml does not contain the correct type ({0}).", "Dataset"));

            if (datasetNode.Attributes == null)
                throw new ArgumentException("The Xml does not contain any attributes.");

            Name = datasetNode.Attributes["name"].Value;
            IsResultsStored = datasetNode.Attributes["isResultsStored"].Value != null && datasetNode.Attributes["isResultsStored"].Value.ToLowerInvariant() == "true";

            string connectionReferenceName = datasetNode.Attributes["connection"] != null ? datasetNode.Attributes["connection"].Value : null;

            if (!string.IsNullOrEmpty(connectionReferenceName) && TestSuite.ConnectionRefs != null)
            {
                foreach (KeyValuePair<string, ConnectionRef> connectionRef in TestSuite.ConnectionRefs)
                {
                    if (connectionRef.Value.ReferenceName != connectionReferenceName)
                        continue;

                    ConnectionRef = connectionRef.Value;

                    break;
                }
            }

            if (!datasetNode.HasChildNodes)
            {
                Results = null;

                return;
            }

            XmlNode queryNode = null;
            XmlNode resultsNode = null;

            foreach (XmlNode childNode in datasetNode.ChildNodes)
            {
                if (childNode.Name != "results" && childNode.Name != "query")
                    continue;

                if (childNode.Name == "query")
                    queryNode = childNode;

                if (childNode.Name == "results")
                    resultsNode = childNode;
            }

            if (resultsNode == null || !resultsNode.HasChildNodes || resultsNode.FirstChild == null || resultsNode.FirstChild.NodeType != XmlNodeType.CDATA || string.IsNullOrEmpty(resultsNode.FirstChild.InnerText))
                Results = null;
            else
            {
                string rawDataStream = resultsNode.FirstChild.InnerText.Trim();

                if (string.IsNullOrEmpty(rawDataStream))
                    Results = null;
                else
                {
                    DataTable deserializedDataTable = new DataTable(ResultsDataTableName);

                    deserializedDataTable.ReadXml(new StringReader(rawDataStream));

                    Results = deserializedDataTable;
                }
            }

            if (queryNode == null || !queryNode.HasChildNodes || queryNode.FirstChild == null || queryNode.FirstChild.NodeType != XmlNodeType.CDATA || string.IsNullOrEmpty(queryNode.FirstChild.InnerText))
                Query = null;
            else
            {
                string queryText = queryNode.FirstChild.InnerText;

                Query = string.IsNullOrEmpty(queryText) ? null : queryText;
            }
        }

        public string PersistToXml()
        {
            StringBuilder xml = new StringBuilder();

            XmlWriterSettings writerSettings = new XmlWriterSettings { ConformanceLevel = ConformanceLevel.Fragment, OmitXmlDeclaration = true };

            using (XmlWriter xmlWriter = XmlWriter.Create(xml, writerSettings))
            {
                xmlWriter.WriteStartElement(DatasetElementName);
                xmlWriter.WriteAttributeString("name", Name);
                xmlWriter.WriteAttributeString("connection", ConnectionRef == null ? string.Empty : ConnectionRef.ReferenceName);
                xmlWriter.WriteAttributeString("isResultsStored", IsResultsStored.ToString().ToLowerInvariant());

                if (!string.IsNullOrEmpty(Query))
                {
                    xmlWriter.WriteStartElement("query");
                    xmlWriter.WriteCData(Query);
                    xmlWriter.WriteEndElement();
                }

                if (Results != null)
                {
                    xmlWriter.WriteStartElement("results");

                    using (StringWriter stringWriter = new StringWriter())
                    {
                        Results.WriteXml(stringWriter, XmlWriteMode.WriteSchema, true);

                        xmlWriter.WriteCData(stringWriter.ToString());
                    }

                    xmlWriter.WriteEndElement();
                }

                xmlWriter.WriteEndElement();
                xmlWriter.Close();
            }

            return xml.ToString();
        }

#if SQL2005
        [Description("The Connection that the SQLCommand will use"),
         TypeConverter("SsisUnit.Design.ConnectionRefConverter, SsisUnit.Design, Version=1.0.0.0, Culture=neutral, PublicKeyToken=6fbed22cbef36cab")]
#elif SQL2008
        [Description("The Connection that the SQLCommand will use"),
         TypeConverter("SsisUnit.Design.ConnectionRefConverter, SsisUnit2008.Design, Version=1.0.0.0, Culture=neutral, PublicKeyToken=6fbed22cbef36cab")]
#elif SQL2012
        [Description("The Connection that the SQLCommand will use"),
         TypeConverter("SsisUnit.Design.ConnectionRefConverter, SsisUnit.Design.2012, Version=1.0.0.0, Culture=neutral, PublicKeyToken=6fbed22cbef36cab")]
#elif SQL2014
        [Description("The Connection that the SQLCommand will use"),
         TypeConverter("SsisUnit.Design.ConnectionRefConverter, SsisUnit.Design.2014, Version=1.0.0.0, Culture=neutral, PublicKeyToken=6fbed22cbef36cab")]
#endif
        public ConnectionRef ConnectionRef { get; set; }

        public string Name { get; set; }

        public bool IsResultsStored { get; set; }

        [Browsable(false)]
        public DataTable Results { get; internal set; }

#if SQL2005
        [Description("The SQL statement to be executed by the SQLCommand"),
         Editor("SsisUnit.Design.QueryEditor, SsisUnit.Design, Version=1.0.0.0, Culture=neutral, PublicKeyToken=6fbed22cbef36cab", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
#elif SQL2008
        [Description("The SQL statement to be executed by the SQLCommand"),
         Editor("SsisUnit.Design.QueryEditor, SsisUnit2008.Design, Version=1.0.0.0, Culture=neutral, PublicKeyToken=6fbed22cbef36cab", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
#elif SQL2012
        [Description("The SQL statement to be executed by the SQLCommand"),
         Editor("SsisUnit.Design.QueryEditor, SsisUnit.Design.2012, Version=1.0.0.0, Culture=neutral, PublicKeyToken=6fbed22cbef36cab", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
#elif SQL2014
        [Description("The SQL statement to be executed by the SQLCommand"),
         Editor("SsisUnit.Design.QueryEditor, SsisUnit.Design.2014, Version=1.0.0.0, Culture=neutral, PublicKeyToken=6fbed22cbef36cab", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
#endif
        public string Query { get; set; }

        [Browsable(false)]
        public SsisTestSuite TestSuite { get; private set; }

        internal DataTable RetrieveDataTable()
        {
            if (!IsResultsStored)
            {
                using (var command = Helper.GetCommand(ConnectionRef, Query))
                {
                    command.Connection.Open();
                    using (IDataReader expectedReader = command.ExecuteReader())
                    {
                        var ds = new DataSet();

                        ds.Load(expectedReader, LoadOption.OverwriteChanges, new[] { "Results" });

                        if (ds.Tables.Count < 1)
                        {
                            throw new ApplicationException(
                                string.Format(
                                    CultureInfo.CurrentCulture, "The dataset (\"{0}\") did not retrieve any data.", Name));
                        }


                        return ds.Tables[0];
                    }
                }
            }

            if (Results == null || Results.Columns.Count < 1)
            {
                throw new ApplicationException(
                    string.Format(
                        CultureInfo.CurrentCulture,
                        "The expected dataset's (\"{0}\") stored results does not contain data. Populate the Results data table before executing.",
                        Name));
            }

            return Results;
        }
    }
}