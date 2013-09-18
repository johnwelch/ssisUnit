using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;

namespace SsisUnit.Design
{
    public class ConnectionRefInvariantTypeConverter : StringConverter
    {
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            DataTable providersDataTable = DbProviderFactories.GetFactoryClasses();

            List<string> providerInvariantNames = new List<string>();

            foreach (DataRow providerRow in providersDataTable.Rows)
            {
                providerInvariantNames.Add(providerRow[2].ToString());
            }

            return new StandardValuesCollection(providerInvariantNames.ToArray());
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return false;
        }

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }
    }
}