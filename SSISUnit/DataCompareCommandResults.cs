using System;
using System.Collections.Generic;
using System.Data;

namespace SsisUnit
{
    public sealed class DataCompareCommandResults
    {
        internal DataCompareCommandResults(Dataset expectedDataset, Dataset actualDataset, DataTable expectedResults, DataTable actualResults, IDictionary<int, IEnumerable<int>> actualDatasetErrorIndices, bool isSchemasCompatible, bool isDatasetsSame, IEnumerable<string> expectedDatasetMessages, IEnumerable<string> actualDatasetMessages)
        {
            if (expectedDataset == null)
                throw new ArgumentNullException("expectedDataset");
            if (actualDataset == null)
                throw new ArgumentNullException("actualDataset");
            if (expectedDatasetMessages == null)
                throw new ArgumentNullException("expectedDatasetMessages");
            if (actualDatasetMessages == null)
                throw new ArgumentNullException("actualDatasetMessages");

            ExpectedDataset = expectedDataset;
            ActualDataset = actualDataset;
            ExpectedResults = expectedResults;
            ActualResults = actualResults;
            ActualDatasetErrorIndices = actualDatasetErrorIndices;
            IsSchemasCompatible = isSchemasCompatible;
            IsDatasetsSame = isDatasetsSame;
            ExpectedDatasetMessages = expectedDatasetMessages;
            ActualDatasetMessages = actualDatasetMessages;
        }

        public Dataset ActualDataset { get; private set; }
        public DataTable ActualResults { get; private set; }
        /// <summary>
        /// In order to reduce memory pressure, a collection of integers have been used, see below for what the values mean.
        /// 
        /// Legend to values within the datasetError dictionary:
        /// 
        /// Positive Integer Key = Actual Row Error
        ///      Value == NULL : entire row is not in expected data table
        ///      Value != NULL : columns are different in actual dataset.
        ///          Value Collection
        ///              Positive Integer : actual column value differs from expected column value.
        ///              Negative Integer : expected column is exists in expected data table row but not in the actual data table row.
        /// 
        /// Negative Integer Key = Expected Row Error
        ///      Value should be ignored due to expected result not appearing in actual data table.
        /// </summary>
        public IDictionary<int, IEnumerable<int>> ActualDatasetErrorIndices { get; private set; }
        public IEnumerable<string> ActualDatasetMessages { get; private set; }
        public Dataset ExpectedDataset { get; private set; }
        public DataTable ExpectedResults { get; private set; }
        public IEnumerable<string> ExpectedDatasetMessages { get; private set; }
        public bool IsDatasetsSame { get; private set; }
        public bool IsSchemasCompatible { get; private set; }
    }
}