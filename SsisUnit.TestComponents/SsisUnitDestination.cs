using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using Microsoft.SqlServer.Dts.Pipeline;
using Microsoft.SqlServer.Dts.Pipeline.Wrapper;
using Microsoft.SqlServer.Dts.Runtime.Wrapper;

namespace SsisUnit.TestComponents
{
    [DtsPipelineComponent(
        ComponentType = ComponentType.DestinationAdapter,
        DisplayName = "SsisUnit Destination",
        CurrentVersion = 0,
        Description = "Destination used with SsisUnit tests to retrieve data from the data flow.")]
    public class SsisUnitDestination : PipelineComponent
    {
        private const string DataTablePropertyName = "DataTableXml";
        private const string VariablePropertyName = "VariableName";

        private Dictionary<string, int> _bufferColumnMapping;

        public DataTable DataTable { get; set; }

        public override void ProvideComponentProperties()
        {
            RemoveAllInputsOutputsAndCustomProperties();

            var input = ComponentMetaData.InputCollection.New();
            input.Name = "Default Input";
            input.Description = "Receives the test data provided by an upstream component";
            input.HasSideEffects = true;

            var dataTableProperty = ComponentMetaData.CustomPropertyCollection.New();
            dataTableProperty.Name = DataTablePropertyName;
            dataTableProperty.Description = "Contains an XML representation of the data table.";
            dataTableProperty.Value = string.Empty;

            var variableProperty = ComponentMetaData.CustomPropertyCollection.New();
            variableProperty.Name = VariablePropertyName;
            variableProperty.Description = "References a variable to store the data table results.";
            variableProperty.Value = string.Empty;
        }

        public override void OnInputPathAttached(int inputID)
        {
            var virtualInput = ComponentMetaData.InputCollection[0].GetVirtualInput();

            CreateInputColumns(virtualInput);
        }

        public override void OnInputPathDetached(int inputID)
        {
            var virtualInput = ComponentMetaData.InputCollection[0].GetVirtualInput();

            CreateInputColumns(virtualInput);
        }

        public override void ReinitializeMetaData()
        {
            var virtualInput = ComponentMetaData.InputCollection[0].GetVirtualInput();

            CreateInputColumns(virtualInput);
        }

        private void CreateInputColumns(IDTSVirtualInput100 virtualInput)
        {
            ComponentMetaData.InputCollection[0].InputColumnCollection.RemoveAll();
            foreach (IDTSVirtualInputColumn100 virtualInputColumn in virtualInput.VirtualInputColumnCollection)
            {
                var inputColumn = ComponentMetaData.InputCollection[0].InputColumnCollection.New();
                inputColumn.LineageID = virtualInputColumn.LineageID;
                inputColumn.UsageType = DTSUsageType.UT_READONLY;
            }
        }

        public override void PreExecute()
        {
            if (DataTable == null)
            {
                DataTable = new DataTable("SsisUnit");   
            }

            if (_bufferColumnMapping == null)
            {
                _bufferColumnMapping = new Dictionary<string, int>();
            }
            else
            {
                _bufferColumnMapping.Clear();
            }

            foreach (IDTSInputColumn100 column in ComponentMetaData.InputCollection[0].InputColumnCollection)
            {
                bool isLong = false;
                _bufferColumnMapping.Add(column.Name, BufferManager.FindColumnByLineageID(ComponentMetaData.InputCollection[0].Buffer, column.LineageID));
                var dataColumn = new DataColumn(column.Name);
                dataColumn.DataType = BufferTypeToDataRecordType(ConvertBufferDataTypeToFitManaged(column.DataType, ref isLong));
                dataColumn.MaxLength = column.Length == 0 ? -1 : column.Length;
                if (column.CodePage != 0)
                {
                    dataColumn.ExtendedProperties.Add("codePage", column.CodePage);
                }

                if (column.Scale != 0)
                {
                    dataColumn.ExtendedProperties.Add("scale", column.Scale);
                }

                if (column.Precision != 0)
                {
                    dataColumn.ExtendedProperties.Add("precision", column.Precision);
                }

                DataTable.Columns.Add(dataColumn);
            }
        }

        public override void ProcessInput(int inputID, PipelineBuffer buffer)
        {
            base.ProcessInput(inputID, buffer);

            while (buffer.NextRow())
            {
                var dataRow = DataTable.NewRow();
                foreach (var columnMapping in _bufferColumnMapping)
                {
                    // TODO: Update to use the correct data type methods
                    if (buffer.IsNull(columnMapping.Value))
                    {
                        dataRow[columnMapping.Key] = DBNull.Value;
                    }
                    else
                    {
                        dataRow[columnMapping.Key] = buffer[columnMapping.Value];    
                    }
                }
                DataTable.Rows.Add(dataRow);
            }

            if (buffer.EndOfRowset)
            {
                var xml = SaveDataTableToXml(DataTable);
                var property = ComponentMetaData.CustomPropertyCollection[DataTablePropertyName];
                property.Value = xml;

                string variableName = ComponentMetaData.CustomPropertyCollection[VariablePropertyName].Value.ToString();
                if (!string.IsNullOrEmpty(variableName))
                {
                    IDTSVariables100 variables = null;
                    VariableDispenser.LockOneForWrite(variableName, ref variables);
                    variables[variableName].Value = xml;
                    variables.Unlock();                    
                }
            }
        }

        public override void PostExecute()
        {
            ////var logFile = Path.Combine(Path.GetTempPath(), "SsisUnitDestination.log");
            ////File.WriteAllText(logFile, SaveDataTableToXml(DataTable));
        }

        public override DTSValidationStatus Validate()
        {
            if (ComponentMetaData.InputCollection[0].InputColumnCollection.Count != ComponentMetaData.InputCollection[0].GetVirtualInput().VirtualInputColumnCollection.Count)
            {
                return DTSValidationStatus.VS_NEEDSNEWMETADATA;
            }

            return base.Validate();
        }

        private static string SaveDataTableToXml(DataTable dataTable)
        {
            using (var writer = new StringWriter())
            {
                dataTable.WriteXml(writer, XmlWriteMode.WriteSchema);
                return writer.ToString();
            }
        }
    }
}
