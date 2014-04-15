using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Microsoft.SqlServer.Dts.Pipeline;
using Microsoft.SqlServer.Dts.Pipeline.Wrapper;
using Microsoft.SqlServer.Dts.Runtime.Wrapper;

#if SQL2014 || SQL2008 || SQL2012
using IDTSCustomProperty = Microsoft.SqlServer.Dts.Pipeline.Wrapper.IDTSCustomProperty100;
using IDTSOutputColumn = Microsoft.SqlServer.Dts.Pipeline.Wrapper.IDTSOutputColumn100;
#elif SQL2005
using IDTSCustomProperty = Microsoft.SqlServer.Dts.Pipeline.Wrapper.IDTSCustomProperty90;
using IDTSOutputColumn = Microsoft.SqlServer.Dts.Pipeline.Wrapper.IDTSOutputColumn90;
#endif

namespace SsisUnit.TestComponents
{
    [DtsPipelineComponent(
        ComponentType = ComponentType.SourceAdapter,
        DisplayName = "SsisUnit Source",
        CurrentVersion = 0,
        Description = "Source used with SsisUnit tests to provide static data to the data flow.")]
    public class SsisUnitSource : PipelineComponent
    {
        private const string StartingDataSet =
            @"<NewDataSet>
  <xs:schema id=""NewDataSet"" xmlns="""" xmlns:xs=""http://www.w3.org/2001/XMLSchema"" xmlns:msdata=""urn:schemas-microsoft-com:xml-msdata"" xmlns:msprop=""urn:schemas-microsoft-com:xml-msprop"">
    <xs:element name=""NewDataSet"" msdata:IsDataSet=""true"" msdata:MainDataTable=""SsisUnitSource"" msdata:UseCurrentLocale=""true"">
      <xs:complexType>
        <xs:choice minOccurs=""0"" maxOccurs=""unbounded"">
          <xs:element name=""SsisUnit"">
            <xs:complexType>
              <xs:sequence>
                <xs:element name=""Column1"" msprop:codePage=""1252"" minOccurs=""0"">
                  <xs:simpleType>
                    <xs:restriction base=""xs:string"">
                      <xs:maxLength value=""50"" />
                    </xs:restriction>
                  </xs:simpleType>
                </xs:element>
                <xs:element name=""Column2"" type=""xs:dateTime"" minOccurs=""0"" />
                <xs:element name=""Column3"" type=""xs:int"" minOccurs=""0"" />
                <xs:element name=""Column4"" msprop:scale=""5"" msprop:precision=""15"" type=""xs:decimal"" minOccurs=""0"" />
              </xs:sequence>
            </xs:complexType>
          </xs:element>
        </xs:choice>
      </xs:complexType>
    </xs:element>
  </xs:schema>
</NewDataSet>";

        private const string DataTablePropertyName = "TestData";
        private Dictionary<string, int> _bufferColumnMapping;

        public override IDTSCustomProperty SetComponentProperty(string propertyName, object propertyValue)
        {
            if (propertyName == DataTablePropertyName)
            {
                try
                {
                    var dataTable = LoadDataTable(propertyValue.ToString());
                    CreateOutputColumns(dataTable);
                }
                catch (Exception ex)
                {
                    throw new ArgumentException("The TestData value does not contain a valid XML representation of a data table.", "propertyValue", ex);
                }
            }

            return base.SetComponentProperty(propertyName, propertyValue);
        }

        public override void ProvideComponentProperties()
        {
            RemoveAllInputsOutputsAndCustomProperties();

            var output = ComponentMetaData.OutputCollection.New();
            output.Name = "Default Output";
            output.Description = "Outputs the test data provided by this component";

            var dataTableProperty = ComponentMetaData.CustomPropertyCollection.New();
            dataTableProperty.Name = DataTablePropertyName;
            dataTableProperty.Description = "Contains a persisted data table that will be used as the test data.";
            dataTableProperty.ExpressionType = DTSCustomPropertyExpressionType.CPET_NOTIFY;
            dataTableProperty.Value = StartingDataSet;
        }

        public override void ReinitializeMetaData()
        {
            var dataTable = LoadDataTable(DataTableXml);

            CreateOutputColumns(dataTable);
        }

        private void CreateOutputColumns(DataTable dataTable)
        {
            // Need to sync the columns
            // ComponentMetaData.OutputCollection[0].OutputColumnCollection.RemoveAll();

            var existingColumns = new List<string>();
            foreach (IDTSOutputColumn outputColumn in ComponentMetaData.OutputCollection[0].OutputColumnCollection)
            {
                if (!dataTable.Columns.Contains(outputColumn.Name))
                {
                    ComponentMetaData.OutputCollection[0].OutputColumnCollection.RemoveObjectByID(outputColumn.ID);
                }
                else
                {
                    var dataColumn = dataTable.Columns[outputColumn.Name];
                    DataType dataType = DataRecordTypeToBufferType(dataColumn.DataType);
                    int length = dataColumn.MaxLength < 0 ? 0 : dataColumn.MaxLength;
                    int precision = TryGetExtendedProperty(dataColumn, "precision", 0);
                    int scale = TryGetExtendedProperty(dataColumn, "scale", 0);
                    int codePage = TryGetExtendedProperty(dataColumn, "codePage", 0);
                    if (codePage != 0 && dataType == DataType.DT_WSTR)
                    {
                        dataType = DataType.DT_STR;
                    }

                    outputColumn.SetDataTypeProperties(dataType, length, precision, scale, codePage);
                    existingColumns.Add(outputColumn.Name);
                }
            }

            foreach (DataColumn dataColumn in dataTable.Columns)
            {
                if (existingColumns.Contains(dataColumn.ColumnName))
                {
                    continue;
                }

                var column = ComponentMetaData.OutputCollection[0].OutputColumnCollection.New();
                column.Name = dataColumn.ColumnName;

                DataType dataType = DataRecordTypeToBufferType(dataColumn.DataType);
                int length = dataColumn.MaxLength < 0 ? 0 : dataColumn.MaxLength;
                int precision = TryGetExtendedProperty(dataColumn, "precision", 0);
                int scale = TryGetExtendedProperty(dataColumn, "scale", 0);
                int codePage = TryGetExtendedProperty(dataColumn, "codePage", 0);
                if (codePage != 0 && dataType == DataType.DT_WSTR)
                {
                    dataType = DataType.DT_STR;
                }

                column.SetDataTypeProperties(dataType, length, precision, scale, codePage);
            }
        }

        private string DataTableXml
        {
            get
            {
                var dataTableProperty = ComponentMetaData.CustomPropertyCollection[DataTablePropertyName];
                return dataTableProperty.Value.ToString();
            }
        }

        private DataTable LoadDataTable(string dataTableXml)
        {
            //var dataSet = new DataSet();
            var dataTable = new DataTable();
            
            try
            {
                var reader = new StringReader(dataTableXml);
                dataTable.ReadXml(reader);
            }
            catch (Exception)
            {
                bool cancel;
                ComponentMetaData.FireError(0, ComponentMetaData.Name,
                                            "The TestData property does not contain a valid XML representation of a data table",
                                            string.Empty, 0, out cancel);
            }

            return dataTable;
        }

        private static T TryGetExtendedProperty<T>(DataColumn dataColumn, string name, T defaultValue)
        {
            return dataColumn.ExtendedProperties.ContainsKey(name)
                       ? (T)Convert.ChangeType(dataColumn.ExtendedProperties[name], typeof(T))
                       : defaultValue;
        }

        public override void PreExecute()
        {
            if (_bufferColumnMapping == null)
            {
                _bufferColumnMapping = new Dictionary<string, int>();
            }
            else
            {
                _bufferColumnMapping.Clear();
            }

            foreach (IDTSOutputColumn column in ComponentMetaData.OutputCollection[0].OutputColumnCollection)
            {
                _bufferColumnMapping.Add(column.Name, BufferManager.FindColumnByLineageID(ComponentMetaData.OutputCollection[0].Buffer, column.LineageID));
            }
        }

        public override void PrimeOutput(int outputs, int[] outputIDs, PipelineBuffer[] buffers)
        {
            base.PrimeOutput(outputs, outputIDs, buffers);

            PipelineBuffer buffer = buffers[0];
            var dataTable = LoadDataTable(DataTableXml);
            foreach (DataRow dataRow in dataTable.Rows)
            {
                buffer.AddRow();

                foreach (var columnMapping in _bufferColumnMapping)
                {
                    // TODO: Update to use the correct data type methods
                    if (dataRow[columnMapping.Key] == DBNull.Value)
                    {
                        buffer.SetNull(columnMapping.Value);
                    }
                    else
                    {
                        buffer[columnMapping.Value] = dataRow[columnMapping.Key];
                    }
                }
            }

            buffer.SetEndOfRowset();
        }

        public override DTSValidationStatus Validate()
        {
            try
            {
                LoadDataTable(DataTableXml);
            }
            catch (Exception)
            {
                bool cancel;
                ComponentMetaData.FireError(0, ComponentMetaData.Name,
                                            "The TestData property does not contain a valid XML representation of a data table",
                                            string.Empty, 0, out cancel);
                return DTSValidationStatus.VS_NEEDSNEWMETADATA;
            }

            return base.Validate();
        }
    }
}
