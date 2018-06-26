using Microsoft.SqlServer.Dts.Runtime;
using System.Drawing.Design;
using System.ComponentModel;
using System;
using System.Data;
using System.Globalization;

namespace SsisUnit.Design
{
    class DatasetBrowserEditor : UITypeEditor
    {
        
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            Dataset ds = context.Instance as Dataset;
            DataTable dt;

            bool isStored = ds.IsResultsStored;
            System.Diagnostics.Debug.Print(isStored.ToString());

            if(value == null)
            {
                if(ds.Query != null)
                {
                    dt = ds.RetrieveDataTable();
                }
                else
                {
                    dt = new DataTable("Results");
                }
            }
            else
            {
                dt = value as DataTable;
            }

            DatasetBrowser dsb = new DatasetBrowser();
            dsb.ResultsDatatable = dt;
            dsb.FormIsResultsStored = isStored;
            dsb.OpenedDataset = ds;

            dsb.ShowDialog();
            return dsb.ResultsDatatable;

            //return this.EditValue(dt);
        }


        public DataTable EditValue(DataTable value)
        {
            DatasetBrowser dsb = new DatasetBrowser();
            dsb.ResultsDatatable = value;
            dsb.ShowDialog();
            return dsb.ResultsDatatable;
        }
    }

    public class DataTableConverter : ExpandableObjectConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if(destinationType == typeof(DataTable))
            {
                return true;
            }

            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if(destinationType == typeof(System.String) && value is DataTable)
            {
                DataTable dt = (DataTable)value;
                return "DataTable: " + dt.TableName;
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if(sourceType == typeof(string))
            {
                return true;
            }
            return base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if(value is string)
            {
                try
                {
                    string s = (string)value;

                    DataTable dt = new DataTable("NewOne");
                    dt.Columns.Add("Col1", typeof(System.Byte));
                    return dt;
                }
                catch
                {
                    throw new ArgumentException("Hey! I can not convert '" + (string)value + "' to type DataTable");
                }
            }
            return base.ConvertFrom(context, culture, value);
        }
    }
}
