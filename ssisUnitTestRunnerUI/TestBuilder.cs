using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using SsisUnit;
using System.Globalization;
using Microsoft.SqlServer.Dts.Runtime;
using SsisUnit.Design;

namespace ssisUnitTestRunnerUI
{
    public partial class testSuiteUI : Form
    {

        private string _currentFileName = string.Empty;
        private Microsoft.SqlServer.Dts.Runtime.Application _ssisApp = new Microsoft.SqlServer.Dts.Runtime.Application();

        public testSuiteUI()
        {
            InitializeComponent();

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

            if (e.NewItem is ISsisUnitPersist)
            {
                txtXML.Text = ((ISsisUnitPersist)e.NewItem).PersistToXml();
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
            if (p is Test)
            {
                addAssertToolStripMenuItem.Enabled = true;
            }
            else
            {
                addAssertToolStripMenuItem.Enabled = false;
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string fileName = string.Empty;

            if (UIHelper.ShowOpen(ref fileName, UIHelper.FileFilter.SsisUnit, true) == DialogResult.OK)
            {
                _currentFileName = fileName;
                testBrowser1.AddTestSuite(fileName);
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (System.IO.File.Exists(_currentFileName))
            {
                testBrowser1.SaveTestSuite(_currentFileName);
            }
            else
            {
                if (UIHelper.ShowSaveAs(ref _currentFileName, UIHelper.FileFilter.SsisUnit, false) == DialogResult.OK)
                {
                    testBrowser1.SaveTestSuite(_currentFileName);
                }
            }

        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //dlgSaveFile.FileName = _currentFileName;
            //if (dlgSaveFile.ShowDialog() == DialogResult.OK)
            //{
            _currentFileName = string.Empty;
            testBrowser1.CreateTest();
            //}

        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (UIHelper.ShowSaveAs(ref _currentFileName, UIHelper.FileFilter.SsisUnit, false) == DialogResult.OK)
            {
                testBrowser1.SaveTestSuite(_currentFileName);
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
            SsisTestSuite ts = null;
            string packageFileName = string.Empty;
            if (UIHelper.ShowOpen(ref packageFileName, UIHelper.FileFilter.DTSX, true) == DialogResult.OK)
            {
                ts = CreateTestFromPackage(packageFileName);
                if (ts != null)
                {
                    testBrowser1.AddTestSuite(ts);
                }
            }
        }

        private SsisTestSuite CreateTestFromPackage(string fileName)
        {
            SsisTestSuite ts = null;

            Package package = _ssisApp.LoadPackage(fileName, null);
            PackageBrowser pb = new PackageBrowser();
            pb.MultiSelect = true;
            if (pb.ShowDialog(package) == DialogResult.OK)
            {
                ts = new SsisTestSuite();
                AddConnectionRefs(ts, package);
                AddPackageRefs(ts, package, fileName);
                AddTests(ts, package, pb.SelectedTasks);
            }
            return ts;
        }

        private void AddTests(SsisTestSuite ts, Package package, List<TaskItem> taskItemList)
        {
            foreach (TaskItem item in taskItemList)
            {
                int counter = 1;
                string testName = item.Name;

                while (ts.Tests.ContainsKey(testName))
                {
                    testName = item.Name + counter.ToString();
                    counter++;
                }
                ts.Tests.Add(testName, new Test(ts, testName, package.Name, item.ID));
            }

        }

        private void AddPackageRefs(SsisTestSuite ts, Package package, string location)
        {
            if (!ts.PackageRefs.ContainsKey(package.Name))
            {
                ts.PackageRefs.Add(package.Name, new PackageRef(package.Name, location, PackageRef.PackageStorageType.FileSystem));
            }
        }

        private void AddConnectionRefs(SsisTestSuite ts, Package package)
        {
            foreach (ConnectionManager cm in package.Connections)
            {
                if (cm.CreationName == "OLEDB" || cm.CreationName.StartsWith("ADO.NET"))
                {
                    if (!ts.ConnectionRefs.ContainsKey(cm.Name))
                    {
                        ts.ConnectionRefs.Add(cm.Name, new ConnectionRef(cm.Name, cm.ConnectionString, ConnectionRef.ConnectionTypeEnum.ConnectionString));
                    }

                }
            }
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

        private void addTestFromPackageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string packageFileName = string.Empty;
            if (UIHelper.ShowOpen(ref packageFileName, UIHelper.FileFilter.DTSX, true) == DialogResult.OK)
            {
                AddTestFromPackage(packageFileName);
            }
        }



        private void AddTestFromPackage(string fileName)
        {
            Package package = _ssisApp.LoadPackage(fileName, null);
            PackageBrowser pb = new PackageBrowser();
            pb.MultiSelect = true;
            if (pb.ShowDialog(package) == DialogResult.OK)
            {
                AddConnectionRefs(testBrowser1.TestSuite, package);
                AddPackageRefs(testBrowser1.TestSuite, package, fileName);
                AddTests(testBrowser1.TestSuite, package, pb.SelectedTasks);
            }

            testBrowser1.RefreshTestSuite();
        }

    }

}
