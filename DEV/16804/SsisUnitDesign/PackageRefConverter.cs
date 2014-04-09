using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace SsisUnit.Design
{
    class PackageRefConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type t)
        {

            if (t == typeof(string))
            {
                return true;
            }
            return base.CanConvertFrom(context, t);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo info, object value)
        {

            //if (value is string)
            //{
            //    if (((string)value) == string.Empty) return null;
            //    try
            //    {
            //        Test test = (Test)context.Instance;
            //        PackageRef pRef = test.TestSuite.PackageRefs[(string)value];
            //        return pRef;
            //    }
            //    catch
            //    {
            //        throw new ArgumentException("Can not convert '" + (string)value + "' to type PackageRef");
            //    }
            //}
            if (value is string)
            {
                return (string)value;
            }
            return base.ConvertFrom(context, info, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destType)
        {
            if (destType == typeof(string) && value is PackageRef)
            {
                PackageRef pRef = (PackageRef)value;

                return pRef.Name;
            }
            if (destType == typeof(string) && value is string)
            {
                //Test test = (Test)context.Instance;
                //PackageRef pRef = test.TestSuite.PackageRefs[(string)value];

                return value;
            }
            return base.ConvertTo(context, culture, value, destType);
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            //Get the TestSuite reference
            Test test = (Test)context.Instance;
            string[] refs = new string[test.TestSuite.PackageRefs.Count];
            int count = 0;
            foreach (PackageRef pRef in test.TestSuite.PackageRefs.Values)
            {
                refs[count] = pRef.Name;
                count++;
            }
            return new StandardValuesCollection(refs);
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
