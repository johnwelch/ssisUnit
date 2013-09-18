using System;
using System.ComponentModel;

namespace SsisUnit.Design
{
    class ProjectRefConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type t)
        {
            return t == typeof(string) || base.CanConvertFrom(context, t);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo info, object value)
        {
            return value as string ?? base.ConvertFrom(context, info, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destType)
        {
            ProjectRef pRef = value as ProjectRef;
            string projectReferenceValue = value as string;

            if (destType == typeof(string))
            {
                if (pRef != null)
                    return pRef.Name;

                if (projectReferenceValue != null)
                    return value;
            }

            return base.ConvertTo(context, culture, value, destType);
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            // Get the TestSuite reference
            Test test = context.Instance as Test;

            if (test == null)
                return new StandardValuesCollection(new string[0]);

            string[] refs = new string[test.TestSuite.PackageRefs.Count];
            int count = 0;
            
            foreach (ProjectRef pRef in test.TestSuite.ProjectRefs.Values)
            {
                refs[count++] = pRef.Name;
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