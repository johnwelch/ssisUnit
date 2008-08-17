using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.SqlServer.Dts.Runtime;
using System.Globalization;

namespace SsisUnit.Design
{
    public class PackageBrowserEditor : System.Drawing.Design.UITypeEditor
    {
        //FIgure out a way to display the name instead of the GUID?
        public PackageBrowserEditor()
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
            //Get the TestSuite reference
            Test test = (Test)context.Instance;
            
            Package pkg = Helper.LoadPackage(test.TestSuite, test.PackageLocation);
            
            PackageBrowser csBuilder = new PackageBrowser();
            if (csBuilder.ShowDialog(pkg, value.ToString()) == System.Windows.Forms.DialogResult.OK)
            {
                return csBuilder.SelectedTaskID;
            }
            else
            {
                return value;
            }
        }

    }

}
