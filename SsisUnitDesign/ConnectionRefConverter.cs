using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace SsisUnit.Design
{
    class ConnectionRefConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type t)
        {

            if (t == typeof(string))
            {
                return true;
            }
            return base.CanConvertFrom(context, t);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo info, object value)
        {
            var key = value as string;
            if (key != null)
            {
                if ((key) == string.Empty) return null;
                try
                {
                    var command = (CommandBase)context.Instance;
                    ConnectionRef cRef = command.TestSuite.ConnectionRefs[key];
                    return cRef;
                }
                catch
                {
                    throw new ArgumentException("Can not convert '" + key + "' to type ConnectionRef");
                }
            }
            return base.ConvertFrom(context, info, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destType)
        {
            if (destType == typeof(string) && value is ConnectionRef)
            {
                var cRef = (ConnectionRef)value;

                return cRef.ReferenceName;
            }
            return base.ConvertTo(context, culture, value, destType);
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            //Get the TestSuite reference
            var command = (CommandBase)context.Instance;
            var refs = new string[command.TestSuite.ConnectionRefs.Count];
            int count = 0;
            foreach (ConnectionRef cRef in command.TestSuite.ConnectionRefs.Values)
            {
                refs[count] = cRef.ReferenceName;
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
