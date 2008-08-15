using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using SsisUnit;
using System.Globalization;

namespace ssisUnitTestRunnerUI
{
    public partial class testSuiteUI : Form
    {
        string _currentFileName = string.Empty;

        public testSuiteUI()
        {
            InitializeComponent();
            dlgOpenFile.FileName = string.Empty;
            dlgOpenFile.Filter = "Test Files(*.ssisUnit)|*.ssisUnit|All files (*.*)|*.*";

            dlgSaveFile.Filter = "Test Files(*.ssisUnit)|*.ssisUnit|All files (*.*)|*.*";
            dlgSaveFile.CheckPathExists = true;

            LoadCommands();
        }

        private void LoadCommands()
        {
            foreach (Type t in System.Reflection.Assembly.GetAssembly(typeof(CommandBase)).GetTypes())
            {
                if (typeof(CommandBase).IsAssignableFrom(t)
                    && (!object.ReferenceEquals(t, typeof(CommandBase)))
                    && (!t.IsAbstract))
                {
                    ToolStripMenuItem addGenericCommandToolStripMenuItem = new ToolStripMenuItem(t.Name);
                    addGenericCommandToolStripMenuItem.Name = t.Name;
                    addGenericCommandToolStripMenuItem.Click += new EventHandler(addGenericCommandToolStripMenuItem_Click);
                    addCommandToolStripMenuItem.DropDownItems.Add(addGenericCommandToolStripMenuItem);
                }
            }
        }



        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void testBrowser1_NodeSelected(object sender, NodeSelectedEventArgs e)
        {
            propertyGrid1.SelectedObject = e.NewItem;
            EnableMenuItems(e.NewItem);

            if (e.NewItem is CommandBase)
            {
                txtXML.Text = ((CommandBase)e.NewItem).PersistToXml();
            }
        }

        private void EnableMenuItems(object p)
        {
            if (p is CommandSet || p is SsisAssert)
            {
                addCommandToolStripMenuItem.Enabled = true;
            }
            else
            {
                addCommandToolStripMenuItem.Enabled = false;
            }
            //TODO: Add handling for other types
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dlgOpenFile.CheckFileExists = true;
            if (dlgOpenFile.ShowDialog() == DialogResult.OK)
            {
                _currentFileName = dlgOpenFile.FileName;
                testBrowser1.AddTestSuite(dlgOpenFile.FileName);
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            testBrowser1.SaveTest(_currentFileName);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dlgSaveFile.FileName = _currentFileName;
            if (dlgSaveFile.ShowDialog() == DialogResult.OK)
            {
                testBrowser1.CreateTest(dlgSaveFile.FileName);
            }

        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dlgSaveFile.FileName = _currentFileName;
            if (dlgSaveFile.ShowDialog() == DialogResult.OK)
            {
                testBrowser1.SaveTest(dlgSaveFile.FileName);
            }

        }

        private void testBrowser1_TestSuiteSelected(object sender, TestSuiteSelectedEventArgs e)
        {
            _currentFileName = e.CurrentFile;
        }

        private void addTestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            testBrowser1.AddTest();
        }

        private void addGenericCommandToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem tsItem = (ToolStripMenuItem)sender;
            testBrowser1.AddCommand(tsItem.Name);
        }

        private void addConnectionRefToolStripMenuItem_Click(object sender, EventArgs e)
        {
            testBrowser1.AddConnectionRef();
        }

        private void deleteSelectedItemToolStripMenuItem_Click(object sender, EventArgs e)
        {
            testBrowser1.DeleteItem();
        }

        private void addAssertToolStripMenuItem_Click(object sender, EventArgs e)
        {
            testBrowser1.AddAssert();
        }

        private void addPackageRefToolStripMenuItem_Click(object sender, EventArgs e)
        {
            testBrowser1.AddPackageRef();
        }

        private void newFromPackageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dlgOpenFile.ShowDialog() == DialogResult.OK)
            {
                CreateTestFromPackage(dlgOpenFile.FileName);
            }
        }

        private void CreateTestFromPackage(string p)
        {
            throw new NotImplementedException();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowHelp();
        }

        private void ShowHelp()
        {
            throw new NotImplementedException();
        }

        private void runSuiteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            testBrowser1.RunSuite();
        }

        private void runSelectedTestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            testBrowser1.RunSelectedTest();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            if (testBrowser1.SelectedItem is CommandBase)
            {
                txtXML.Text = ((CommandBase)testBrowser1.SelectedItem).PersistToXml();
            }
        }

    }

}
