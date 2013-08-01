using System;
using System.Collections.Generic;
using System.Data;

namespace SsisUnit
{
    public sealed class DataCompareCommandResults : CommandResultsBase
    {
        internal DataCompareCommandResults(
            Dataset expectedDataset, 
            Dataset actualDataset,
            DataTable expectedResults,
            DataTable actualResults,
            IDictionary<int, IEnumerable<int>> expectedDatasetErrorIndices,
            IDictionary<int, IEnumerable<int>> actualDatasetErrorIndices,
            bool isSchemasCompatible,
            bool isDatasetsSame,
            IEnumerable<string> expectedDatasetMessages,
            IEnumerable<string> actualDatasetMessages)
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
            ExpectedDatasetErrorIndices = expectedDatasetErrorIndices;
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
        /// Positive Integer Key = Row in error
        ///      Value == NULL : entire row differs from expected dataset. (red)
        ///      Value != NULL : collection of column indices that differ from the expected dataset.
        ///          Column Indices Collection
        ///              Positive Integer : actual column value differs from expected column value. (red)
        ///              Negative Integer : actual column exists in actual data table row but not in the expected data table row. (green)
        /// </summary>
        public IDictionary<int, IEnumerable<int>> ActualDatasetErrorIndices { get; private set; }
        public IEnumerable<string> ActualDatasetMessages { get; private set; }
        public Dataset ExpectedDataset { get; private set; }
        /// <summary>
        /// In order to reduce memory pressure, a collection of integers have been used, see below for what the values mean.
        /// 
        /// Legend to values within the datasetError dictionary:
        /// 
        /// Positive Integer Key = Row is extra because it does not exist in actual dataset.
        ///      Value == NULL : entire row does not exist in actual dataset. (green)
        ///      Value != NULL : collection of column indices that are are additional to the actual dataset columns for this row. (Maybe future support for jagged datasets???)
        ///          Column Indices Collection
        ///              Positive Integer : column is additional to the actual dataset. (green)
        /// </summary>
        public IDictionary<int, IEnumerable<int>> ExpectedDatasetErrorIndices { get; private set; }
        public DataTable ExpectedResults { get; private set; }
        public IEnumerable<string> ExpectedDatasetMessages { get; private set; }
        public bool IsDatasetsSame { get; private set; }
        public bool IsSchemasCompatible { get; private set; }
    }
}