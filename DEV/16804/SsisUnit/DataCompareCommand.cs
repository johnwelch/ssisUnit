using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Xml;

using Microsoft.SqlServer.Dts.Runtime;

using SsisUnitBase.Enums;
using SsisUnitBase.EventArgs;

#if SQL2012 || SQL2008
using IDTSComponentMetaData = Microsoft.SqlServer.Dts.Pipeline.Wrapper.IDTSComponentMetaData100;
#elif SQL2005
using IDTSComponentMetaData = Microsoft.SqlServer.Dts.Pipeline.Wrapper.IDTSComponentMetaData90;
#endif

namespace SsisUnit
{
    public class DataCompareCommand : CommandBase
    {
        private const string PropName = "name";
        private const string PropExpectedDataset = "expected";
        private const string PropActualDataset = "actual";

        private Dataset _actualDataset;
        private Dataset _expectedDataset;

        public DataCompareCommand(SsisTestSuite testSuite)
            : base(testSuite)
        {
            InitializeProperties();
        }

        public DataCompareCommand(SsisTestSuite testSuite, object parent)
            : base(testSuite, parent)
        {
            InitializeProperties();
        }

        public DataCompareCommand(SsisTestSuite testSuite, string commandXml)
            : base(testSuite, commandXml)
        {
            InitializeProperties();
        }

        public DataCompareCommand(SsisTestSuite testSuite, object parent, string commandXml)
            : base(testSuite, parent, commandXml)
        {
            InitializeProperties();
        }

        public DataCompareCommand(SsisTestSuite testSuite, XmlNode commandXml)
            : base(testSuite, commandXml)
        {
            InitializeProperties();

            AssignPropertiesFromXmlNode(testSuite, commandXml);
        }

        public DataCompareCommand(SsisTestSuite testSuite, object parent, XmlNode commandXml)
            : base(testSuite, parent, commandXml)
        {
            InitializeProperties();

            AssignPropertiesFromXmlNode(testSuite, commandXml);
        }

        public DataCompareCommand(SsisTestSuite testSuite, string name, Dataset expectedDataset, Dataset actualDataset)
            : this(testSuite)
        {
            Name = name;
            ExpectedDataset = expectedDataset;
            ActualDataset = actualDataset;
        }

        public DataCompareCommand(SsisTestSuite testSuite, object parent, string name, Dataset expectedDataset, Dataset actualDataset)
            : this(testSuite, parent)
        {
            Name = name;
            ExpectedDataset = expectedDataset;
            ActualDataset = actualDataset;
        }

        private void AssignPropertiesFromXmlNode(SsisTestSuite testSuite, XmlNode commandXml)
        {
            if (commandXml == null || commandXml.Attributes == null || testSuite == null || testSuite.Datasets == null)
                return;

            string actualDatasetName = commandXml.Attributes[PropActualDataset] != null ? commandXml.Attributes[PropActualDataset].Value : null;
            string expectedDatasetName = commandXml.Attributes[PropExpectedDataset] != null ? commandXml.Attributes[PropExpectedDataset].Value : null;

            foreach (Dataset dataset in testSuite.Datasets.Values)
            {
                if (!string.IsNullOrEmpty(actualDatasetName) && string.Compare(dataset.Name, actualDatasetName, StringComparison.Ordinal) == 0)
                    ActualDataset = dataset;

                if (!string.IsNullOrEmpty(expectedDatasetName) && string.Compare(dataset.Name, expectedDatasetName, StringComparison.Ordinal) == 0)
                    ExpectedDataset = dataset;
            }
        }

        private void InitializeProperties()
        {
            if (!Properties.ContainsKey(PropName))
                Properties.Add(PropName, new CommandProperty(PropName, string.Empty));

            if (!Properties.ContainsKey(PropExpectedDataset))
                Properties.Add(PropExpectedDataset, new CommandProperty(PropExpectedDataset, string.Empty));

            if (!Properties.ContainsKey(PropActualDataset))
                Properties.Add(PropActualDataset, new CommandProperty(PropActualDataset, string.Empty));
        }

#if SQL2005
        [Description("The Connection that the SQLCommand will use"),
         TypeConverter("SsisUnit.Design.DatasetConverter, SsisUnit.Design, Version=1.0.0.0, Culture=neutral, PublicKeyToken=6fbed22cbef36cab")]
#elif SQL2008
        [Description("The Connection that the SQLCommand will use"),
         TypeConverter("SsisUnit.Design.DatasetConverter, SsisUnit2008.Design, Version=1.0.0.0, Culture=neutral, PublicKeyToken=6fbed22cbef36cab")]
#elif SQL2012
        [Description("The Actual Dataset that the DataCompareCommmand will use"),
         TypeConverter("SsisUnit.Design.DatasetConverter, SsisUnit.Design.2012, Version=1.0.0.0, Culture=neutral, PublicKeyToken=6fbed22cbef36cab")]
#endif
        public Dataset ActualDataset
        {
            get
            {
                return _actualDataset;
            }
            set
            {
                _actualDataset = value;

                Properties[PropActualDataset].Value = value != null ? value.Name : string.Empty;
            }
        }

#if SQL2005
        [Description("The Connection that the SQLCommand will use"),
         TypeConverter("SsisUnit.Design.DatasetConverter, SsisUnit.Design, Version=1.0.0.0, Culture=neutral, PublicKeyToken=6fbed22cbef36cab")]
#elif SQL2008
        [Description("The Connection that the SQLCommand will use"),
         TypeConverter("SsisUnit.Design.DatasetConverter, SsisUnit2008.Design, Version=1.0.0.0, Culture=neutral, PublicKeyToken=6fbed22cbef36cab")]
#elif SQL2012
        [Description("The Expected Dataset that the DataCompareCommand will use"),
         TypeConverter("SsisUnit.Design.DatasetConverter, SsisUnit.Design.2012, Version=1.0.0.0, Culture=neutral, PublicKeyToken=6fbed22cbef36cab")]
#endif
        public Dataset ExpectedDataset
        {
            get
            {
                return _expectedDataset;
            }
            set
            {
                _expectedDataset = value;

                Properties[PropExpectedDataset].Value = value != null ? value.Name : string.Empty;
            }
        }

        public override object Execute(object project, Package package, DtsContainer container)
        {
            return Execute(package, container);
        }

        public override object Execute(Package package, DtsContainer container)
        {
            if (ExpectedDataset == null)
                throw new ApplicationException(string.Format(CultureInfo.CurrentCulture, "The expectedDataset attribute is {0}, which does not reference a valid dataset.", Properties[PropExpectedDataset].Value));

            if (ExpectedDataset.ConnectionRef == null)
                throw new ApplicationException(string.Format(CultureInfo.CurrentCulture, "The expected dataset's (\"{0}\") connection does not reference a valid Connection Reference.", Properties[PropExpectedDataset].Value));

            if (string.IsNullOrEmpty(ExpectedDataset.Query))
                throw new ApplicationException(string.Format(CultureInfo.CurrentCulture, "The expected dataset's (\"{0}\") query is not specified.", Properties[PropExpectedDataset].Value));

            if (ActualDataset == null)
                throw new ApplicationException(string.Format(CultureInfo.CurrentCulture, "The actualDataset attribute is {0}, which does not reference a valid dataset.", Properties[PropActualDataset].Value));

            if (ActualDataset.ConnectionRef == null)
                throw new ApplicationException(string.Format(CultureInfo.CurrentCulture, "The actual dataset's (\"{0}\") connection does not reference a valid Connection Reference.", Properties[PropActualDataset].Value));

            if (string.IsNullOrEmpty(ActualDataset.Query))
                throw new ApplicationException(string.Format(CultureInfo.CurrentCulture, "The actual dataset's (\"{0}\") query is not specified.", Properties[PropActualDataset].Value));

            DbCommand expectedDbCommand = null;
            DbCommand actualDbCommand = null;

            DataTable expectedDataTable;
            DataTable actualDataTable;

            CommandParentType commandParentType = GetCommandParentType();

            try
            {
                OnCommandStarted(new CommandStartedEventArgs(DateTime.Now, Name, null, null, commandParentType));

                if (!ExpectedDataset.IsResultsStored)
                {
                    expectedDbCommand = Helper.GetCommand(ExpectedDataset.ConnectionRef, ExpectedDataset.Query);
                    expectedDbCommand.Connection.Open();

                    using (IDataReader expectedReader = expectedDbCommand.ExecuteReader())
                    {
                        var ds = new DataSet();

                        ds.Load(expectedReader, LoadOption.OverwriteChanges, new[] { "Results" });

                        if (ds.Tables.Count < 1)
                            throw new ApplicationException(string.Format(CultureInfo.CurrentCulture, "The expected dataset (\"{0}\") did not retrieve any data.", Properties[PropExpectedDataset].Value));

                        expectedDataTable = ds.Tables[0];
                    }
                }
                else
                {
                    if (ExpectedDataset.Results == null || ExpectedDataset.Results.Columns.Count < 1)
                        throw new ApplicationException(string.Format(CultureInfo.CurrentCulture, "The expected dataset's (\"{0}\") stored results does not contain data. Populate the Results data table before executing.", Properties[PropExpectedDataset].Value));

                    expectedDataTable = ExpectedDataset.Results;
                }
            }
            catch (Exception ex)
            {
                OnCommandFailed(new CommandFailedEventArgs(DateTime.Now, Name, null, null, ex.Message, commandParentType));

                throw;
            }
            finally
            {
                if (expectedDbCommand != null)
                {
                    if (expectedDbCommand.Connection != null)
                        expectedDbCommand.Connection.Close();

                    expectedDbCommand.Dispose();
                }
            }

            try
            {
                if (!ActualDataset.IsResultsStored)
                {
                    actualDbCommand = Helper.GetCommand(ActualDataset.ConnectionRef, ActualDataset.Query);
                    actualDbCommand.Connection.Open();

                    using (IDataReader actualReader = actualDbCommand.ExecuteReader())
                    {
                        var ds = new DataSet();

                        ds.Load(actualReader, LoadOption.OverwriteChanges, new[] { "Results" });

                        if (ds.Tables.Count < 1)
                            throw new ApplicationException(string.Format(CultureInfo.CurrentCulture, "The actual dataset (\"{0}\") did not retrieve any data.", Properties[PropActualDataset].Value));

                        actualDataTable = ds.Tables[0];
                    }
                }
                else
                {
                    if (ActualDataset.Results == null || ActualDataset.Results.Columns.Count < 1)
                        throw new ApplicationException(string.Format(CultureInfo.CurrentCulture, "The actual dataset's (\"{0}\") stored results does not contain data. Populate the Results data table before executing.", Properties[PropActualDataset].Value));

                    actualDataTable = ActualDataset.Results;
                }
            }
            catch (Exception ex)
            {
                OnCommandFailed(new CommandFailedEventArgs(DateTime.Now, Name, null, null, ex.Message, commandParentType));

                throw;
            }
            finally
            {
                if (actualDbCommand != null)
                {
                    if (actualDbCommand.Connection != null)
                        actualDbCommand.Connection.Close();

                    actualDbCommand.Dispose();
                }
            }

            bool isComparisonPossible = true;
            var expectedDatasetMessages = new List<string>();
            var actualDatasetMessages = new List<string>();

            if (expectedDataTable == null || expectedDataTable.Columns.Count < 1)
            {
                isComparisonPossible = false;

                expectedDatasetMessages.Add(string.Format(CultureInfo.CurrentCulture, "The expected dataset (\"{0}\") did not retrieve any data.", Properties[PropExpectedDataset].Value));
            }
            else
            {
                expectedDataTable.AcceptChanges();
            }

            if (actualDataTable == null || actualDataTable.Columns.Count < 1)
            {
                isComparisonPossible = false;

                actualDatasetMessages.Add(string.Format(CultureInfo.CurrentCulture, "The actual dataset (\"{0}\") did not retrieve any data.", Properties[PropActualDataset].Value));
            }
            else
            {
                actualDataTable.AcceptChanges();
            }

            bool isSchemasCompatible;

            if (isComparisonPossible)
            {
                if (expectedDataTable.Columns.Count == actualDataTable.Columns.Count)
                {
                    isSchemasCompatible = true;

                    // Compare each table's columns and ensure that they're the same datatypes at each position.
                    for (int columnIndex = 0; columnIndex < expectedDataTable.Columns.Count; columnIndex++)
                    {
                        if (expectedDataTable.Columns[columnIndex].DataType == actualDataTable.Columns[columnIndex].DataType)
                            continue;

                        isSchemasCompatible = false;

                        break;
                    }
                }
                else
                {
                    isSchemasCompatible = false;

                    actualDatasetMessages.Add(string.Format("The expected \"{0}\" and actual \"{1}\" datasets do not have the same columns and/or data types for each column.  Ensure that both datasets return the same number of columns and that each column is the same data type.", Properties[PropExpectedDataset].Value, Properties[PropActualDataset].Value));
                }
            }
            else
                isSchemasCompatible = false;

            var expectedDatasetErrorIndices = new Dictionary<int, IEnumerable<int>>(expectedDataTable != null ? expectedDataTable.Rows.Count : 0);
            var actualDatasetErrorIndices = new Dictionary<int, IEnumerable<int>>(actualDataTable != null ? actualDataTable.Rows.Count : 0);
            int rowIndex = 0;

            if (expectedDataTable != null && actualDataTable != null)
            {
                for (; rowIndex < expectedDataTable.Rows.Count; rowIndex++)
                {
                    if (rowIndex < actualDataTable.Rows.Count)
                    {
                        DataRow expectedRow = expectedDataTable.Rows[rowIndex];
                        DataRow actualRow = actualDataTable.Rows[rowIndex];

                        var expectedColumnsDifferent = new List<int>();
                        var actualColumnsDifferent = new List<int>();

                        for (int columnIndex = 0; columnIndex < expectedRow.ItemArray.Length; columnIndex++)
                        {
                            if (columnIndex < actualRow.ItemArray.Length)
                            {
                                if (!Equals(expectedRow[columnIndex], actualRow[columnIndex]))
                                    actualColumnsDifferent.Add(columnIndex);
                            }
                            else
                                expectedColumnsDifferent.Add(columnIndex);
                        }

                        // If there are rows that aren't found in the actual dataset or there are additional columns in the the expected dataset.
                        if (expectedColumnsDifferent.Count > 0)
                            expectedDatasetErrorIndices.Add(rowIndex, expectedColumnsDifferent.Count >= expectedRow.ItemArray.Length ? null : expectedColumnsDifferent);

                        if (actualColumnsDifferent.Count > 0)
                            actualDatasetErrorIndices.Add(rowIndex, actualColumnsDifferent.Count >= expectedRow.ItemArray.Length ? null : actualColumnsDifferent);
                    }
                    else
                        expectedDatasetErrorIndices.Add(rowIndex, null);
                }
            }

            if (actualDataTable != null)
            {
                for (; rowIndex < actualDataTable.Rows.Count; rowIndex++)
                {
                    actualDatasetErrorIndices.Add(rowIndex, null);
                }
            }

            bool isResultsSame = actualDatasetErrorIndices.Count < 1 && expectedDatasetErrorIndices.Count < 1;

            var results = new DataCompareCommandResults(ExpectedDataset, ActualDataset, expectedDataTable, actualDataTable, expectedDatasetErrorIndices, actualDatasetErrorIndices, isSchemasCompatible, isResultsSame, expectedDatasetMessages, actualDatasetMessages);

            string resultMessage = isResultsSame ?
                string.Format("The datasets \"{0}\" and \"{1}\" are the same.", ExpectedDataset.Name, ActualDataset.Name)
                :
                string.Format("{0} row{1} differ{2} between the expected \"{3}\" and actual \"{4}\" datasets.",
                              (expectedDatasetErrorIndices.Count + actualDatasetErrorIndices.Count).ToString("N0"),
                              actualDatasetErrorIndices.Count == 1 ? string.Empty : "s",
                              actualDatasetErrorIndices.Count == 1 ? "s" : string.Empty,
                              ExpectedDataset.Name,
                              ActualDataset.Name);

            var completedEventArgs = new DataCompareCommandCompletedEventArgs(DateTime.Now, Name, null, null, resultMessage, results, commandParentType);

            if (isResultsSame)
                OnCommandCompleted(completedEventArgs);
            else
                OnCommandFailed(new CommandFailedEventArgs(DateTime.Now, Name, null, null, resultMessage, results, commandParentType));

            return results;
        }
    }
}