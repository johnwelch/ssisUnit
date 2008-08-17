using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using Microsoft.SqlServer.Dts.Runtime;
using System.Globalization;

namespace SsisUnit.Design
{
    class TaskConverter : TypeConverter
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

        //Called to get the value to set on the underlying object
        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo info, object value)
        {
            if (value is string)
            {
                if (((string)value) == string.Empty) return null;

                if (IsGuid(value)) return value;

                //If we made it this far, it's not a GUID - it's a task name
                try
                {
                    Test test = (Test)context.Instance;
                    Package pkg = Helper.LoadPackage(test.TestSuite, test.PackageLocation);
                    DtsContainer con = Helper.FindExecutable(pkg, value.ToString());
                    return con.ID;
                }
                catch
                {
                    throw new ArgumentException("Can not convert '" + (string)value + "' to GUID");
                }
            }
            return base.ConvertFrom(context, info, value);
        }

        private static bool IsGuid(object value)
        {
            try
            {
                Guid testGuid = new Guid(value.ToString());
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        //Called to get the display value for the property grid
        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destType)
        {
            if (destType == typeof(string) && value is string)
            {
                if (!IsGuid(value)) return value;

                Test test = (Test)context.Instance;
                Package pkg = Helper.LoadPackage(test.TestSuite, test.PackageLocation);
                DtsContainer con = Helper.FindExecutable(pkg, value.ToString());
                return con.Name;
            }
            return base.ConvertTo(context, culture, value, destType);
        }

        
    }
}
