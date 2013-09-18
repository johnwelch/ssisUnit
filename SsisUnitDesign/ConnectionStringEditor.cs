using System;
using System.Collections.Generic;
using System.Text;

namespace SsisUnit.Design
{
    public class ConnectionStringEditor : System.Drawing.Design.UITypeEditor
    {

        public ConnectionStringEditor()
        {
        }

        public override System.Drawing.Design.UITypeEditorEditStyle GetEditStyle(
          System.ComponentModel.ITypeDescriptorContext context)
        {

            return System.Drawing.Design.UITypeEditorEditStyle.Modal;
        }

        public override object EditValue(
          System.ComponentModel.ITypeDescriptorContext context,
          System.IServiceProvider provider,
          object value)
        {
            return this.EditValue(value as string);
        }

        public string EditValue()
        {
            return this.EditValue(string.Empty);
        }

        public string EditValue(string value)
        {

            string newValue = string.Empty;

            ConnectionStringBuilder csBuilder = new ConnectionStringBuilder();
            csBuilder.ConnectionString = value;
            if (csBuilder.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                return csBuilder.ConnectionString;
            }
            else
            {
                return value;
            }

        }
    }

}
