using System;
using System.ComponentModel;

namespace SsisUnit.Design
{
    class DatasetConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type t)
        {
            return t == typeof(string) || base.CanConvertFrom(context, t);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo info, object value)
        {
            var key = value as string;
            
            if (key != null)
            {
                if (key == string.Empty)
                    return null;

                try
                {
                    CommandBase commandBase = context.Instance as CommandBase;

                    if (commandBase != null)
                        return commandBase.TestSuite.Datasets[key];

                    Dataset dataset = context.Instance as Dataset;

                    if (dataset != null)
                        return dataset.TestSuite.Datasets[key];
                }
                catch
                {
                    throw new ArgumentException("Can not convert '" + key + "' to type Dataset");
                }
            }
            
            return base.ConvertFrom(context, info, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destType)
        {
            if (destType == typeof(string) && value is Dataset)
            {
                var cRef = (Dataset)value;

                return cRef.Name;
            }

            return base.ConvertTo(context, culture, value, destType);
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            SsisTestSuite testSuite = null;

            CommandBase commandBase = context.Instance as CommandBase;

            if (commandBase != null)
                testSuite = commandBase.TestSuite;

            Dataset dataset = context.Instance as Dataset;

            if (commandBase == null && dataset != null)
                testSuite = dataset.TestSuite;

            if (testSuite == null || testSuite.Datasets == null)
                return new StandardValuesCollection(new Dataset[0]);

            Dataset[] datasets = new Dataset[testSuite.Datasets.Count];

            testSuite.Datasets.Values.CopyTo(datasets, 0);

            return new StandardValuesCollection(datasets);
        }

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return true;
        }
    }
}