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

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo info, object value)
        {

            if (value is string)
            {
                if (((string)value) == string.Empty) return null;
                try
                {
                    CommandBase command = (CommandBase)context.Instance;
                    ConnectionRef cRef = command.TestSuite.ConnectionRefs[(string)value];
                    return cRef;
                }
                catch
                {
                    throw new ArgumentException("Can not convert '" + (string)value + "' to type ConnectionRef");
                }
            }
            return base.ConvertFrom(context, info, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destType)
        {
            if (destType == typeof(string) && value is ConnectionRef)
            {
                ConnectionRef cRef = (ConnectionRef)value;

                return cRef.ReferenceName;
            }
            return base.ConvertTo(context, culture, value, destType);
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            //Get the TestSuite reference
            CommandBase command = (CommandBase)context.Instance;
            string[] refs = new string[command.TestSuite.ConnectionRefs.Count];
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
