using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Design;
using System.ComponentModel;

namespace SsisUnit.Design
{
    class QueryEditor:UITypeEditor
    {
        public QueryEditor()
        {
        }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            return this.EditValue(value as string);
        }

        public string EditValue()
        {
            return this.EditValue(string.Empty);
        }

        public string EditValue(string value)
        {
            QueryBuilder builder = new QueryBuilder();
            builder.Query = value;
            builder.ShowDialog();
            return builder.Query;
        }
    }
}
