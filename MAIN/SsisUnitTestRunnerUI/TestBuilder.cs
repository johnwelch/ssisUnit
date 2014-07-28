using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Forms;
using SsisUnit;
using Microsoft.SqlServer.Dts.Runtime;
using SsisUnit.Design;
using System.IO;
using System.Xml;

using SsisUnit.Enums;

using SsisUnitBase;

namespace ssisUnitTestRunnerUI
{
    public partial class testSuiteUI : Form
    {
        private readonly Microsoft.SqlServer.Dts.Runtime.Application _ssisApp = new Microsoft.SqlServer.Dts.Runtime.Application();

        private string _currentFileName = string.Empty;

        public testSuiteUI(string testCaseFile)
        {
            InitializeComponent();

            LoadCommands();

            if (testCaseFile == string.Empty || !File.Exists(testCaseFile))
                return;

            _currentFileName = testCaseFile;
            testBrowser1.AddTestSuite(testCaseFile);
        }

        private void LoadCommands()
        {
            foreach (Type t in System.Reflection.Assembly.GetAssembly(typeof(CommandBase)).GetTypes())
            {
                if (!typeof(CommandBase).IsAssignableFrom(t) || ReferenceEquals(t, typeof(CommandBase)) || t.IsAbstract)
                    continue;

                ToolStripMenuItem addGenericCommandToolStripMenuItem = new ToolStripMenuItem(t.Name);
                addGenericCommandToolStripMenuItem.Name = t.Name;
                addGenericCommandToolStripMenuItem.Click += addGenericCommandToolStripMenuItem_Click;
                addCommandToolStripMenuItem.DropDownItems.Add(addGenericCommandToolStripMenuItem);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string titleText = "Test Suite Builder";

#if SQL2012
            titleText += " (SSIS 2012)";
#elif SQL2014
            titleText += " (SSIS 2014)";
#elif SQL2008
            titleText += " (SSIS 2008)";
#elif SQL2005
            titleText += " (SSIS 2005)";
#endif

            Text = titleText;
        }

        private void testBrowser1_NodeSelected(object sender, NodeSelectedEventArgs e)
        {
            propertyGrid1.SelectedObject = e.NewItem;
            EnableMenuItems(e.NewItem);

            ISsisUnitPersist item = e.NewItem as ISsisUnitPersist;
            
            txtXML.Text = item != null ? FormatXml(item.PersistToXml()) : string.Empty;

            // ConnectionRef connectionRef = e.NewItem as ConnectionRef;

            // if (connectionRef != null)
            //     connectionRef.RefreshInvariantTypeAccessiblity();
        }

        public static string FormatXml(string xml)
        {
            XmlDocument doc = new XmlDocument();
            XmlDocumentFragment frag = doc.CreateDocumentFragment();
            frag.InnerXml = xml;

            XmlWriterSettings settings = new XmlWriterSettings { OmitXmlDeclaration = true, ConformanceLevel = ConformanceLevel.Fragment, Indent = true };
            MemoryStream memoryStream = new MemoryStream();
            XmlWriter xw = XmlWriter.Create(memoryStream, settings);

            frag.WriteTo(xw);
            xw.Flush();
            xw.Close();

            memoryStream.Position = 0;
            StreamReader streamReader = new StreamReader(memoryStream);
            
            return streamReader.ReadToEnd();
        }

        private void EnableMenuItems(object p)
        {
            if (p is CommandSet || p is SsisAssert)
                addCommandToolStripMenuItem.Enabled = true;
            else
                addCommandToolStripMenuItem.Enabled = false;

            if (p is Test)
                addAssertToolStripMenuItem.Enabled = true;
            else
                addAssertToolStripMenuItem.Enabled = false;
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenTestSuite();
        }

        private void OpenTestSuite()
        {
            string fileName = string.Empty;

            if (UIHelper.ShowOpen(ref fileName, UIHelper.FileFilter.SsisUnit, true) != DialogResult.OK)
                return;

            _currentFileName = fileName;
            testBrowser1.AddTestSuite(fileName);
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveTestSuite();
        }

        private void SaveTestSuite()
        {
            if (File.Exists(_currentFileName))
                testBrowser1.SaveTestSuite(_currentFileName);
            else if (UIHelper.ShowSaveAs(ref _currentFileName, UIHelper.FileFilter.SsisUnit, false) == DialogResult.OK)
                testBrowser1.SaveTestSuite(_currentFileName);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CreateTestSuite();
        }

        private void CreateTestSuite()
        {
            _currentFileName = string.Empty;
            testBrowser1.CreateTest();
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (UIHelper.ShowSaveAs(ref _currentFileName, UIHelper.FileFilter.SsisUnit, false) == DialogResult.OK)
                testBrowser1.SaveTestSuite(_currentFileName);
        }

        private void testBrowser1_TestSuiteSelected(object sender, TestSuiteSelectedEventArgs e)
        {
            _currentFileName = e.CurrentFile;

            string titleText = "Test Suite Builder";

#if SQL2012
            titleText += " (SSIS 2012)";
#elif SQL2014
            titleText += " (SSIS 2014)";
#elif SQL2008
            titleText += " (SSIS 2008)";
#elif SQL2005
            titleText += " (SSIS 2005)";
#endif

            Text = titleText + " - " + _currentFileName;
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
            string packageFileName = string.Empty;

            if (UIHelper.ShowOpen(ref packageFileName, UIHelper.FileFilter.DTSX, true) != DialogResult.OK)
                return;

            SsisTestSuite ts = CreateTestFromPackage(packageFileName);

            if (ts != null)
                testBrowser1.AddTestSuite(ts);
        }

        private SsisTestSuite CreateTestFromPackage(string fileName)
        {
            SsisTestSuite ts = null;

            Package package = _ssisApp.LoadPackage(fileName, null);
            PackageBrowser pb = new PackageBrowser { MultiSelect = true };

            if (pb.ShowDialog(package) == DialogResult.OK)
            {
                ts = new SsisTestSuite();

                AddConnectionRefs(ts, package);
                AddPackageRefs(ts, package, fileName);
                AddTests(ts, package, pb.SelectedTasks);
            }
            
            return ts;
        }

        private void AddTests(SsisTestSuite ts, IDTSName package, IEnumerable<TaskItem> taskItemList)
        {
            foreach (TaskItem item in taskItemList)
            {
                int counter = 1;
                string testName = item.Name;

                while (ts.Tests.ContainsKey(testName))
                {
                    testName = item.Name + counter.ToString(CultureInfo.InvariantCulture);
                    counter++;
                }

                ts.Tests.Add(testName, new Test(ts, testName, package.Name, null, item.ID));
            }
        }

        private void AddPackageRefs(SsisTestSuite ts, IDTSName package, string location)
        {
            if (!ts.PackageList.ContainsKey(package.Name))
                ts.PackageList.Add(package.Name, new PackageRef(package.Name, location, PackageStorageType.FileSystem));
        }

        private void AddConnectionRefs(SsisTestSuite ts, Package package)
        {
            foreach (ConnectionManager cm in package.Connections)
            {
                if ((cm.CreationName == "OLEDB" || cm.CreationName.StartsWith("ADO.NET")) && !ts.ConnectionList.ContainsKey(cm.Name))
                    ts.ConnectionList.Add(cm.Name, new ConnectionRef(cm.Name, cm.ConnectionString, ConnectionRef.ConnectionTypeEnum.ConnectionString));
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowHelp();
        }

        private void ShowHelp()
        {
            System.Diagnostics.Process.Start("http://www.codeplex.com/ssisUnit");
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
            CommandBase commandBase = testBrowser1.SelectedItem as CommandBase;

            if (commandBase != null)
                txtXML.Text = commandBase.PersistToXml();
        }

        private void addTestFromPackageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string packageFileName = string.Empty;

            if (UIHelper.ShowOpen(ref packageFileName, UIHelper.FileFilter.DTSX, true) == DialogResult.OK)
                AddTestFromPackage(packageFileName);
        }

        private void AddTestFromPackage(string fileName)
        {
            Package package = _ssisApp.LoadPackage(fileName, null);
            PackageBrowser pb = new PackageBrowser { MultiSelect = true };

            if (pb.ShowDialog(package) == DialogResult.OK)
            {
                AddConnectionRefs(testBrowser1.TestSuite, package);
                AddPackageRefs(testBrowser1.TestSuite, package, fileName);
                AddTests(testBrowser1.TestSuite, package, pb.SelectedTasks);
            }

            testBrowser1.RefreshTestSuite();
        }

        private void newToolStripButton_Click(object sender, EventArgs e)
        {
            CreateTestSuite();
        }

        private void openToolStripButton_Click(object sender, EventArgs e)
        {
            OpenTestSuite();
        }

        private void saveToolStripButton_Click(object sender, EventArgs e)
        {
            SaveTestSuite();
        }

        private void runTestSuiteTtoolStripButton_Click(object sender, EventArgs e)
        {
            try
            {
                testBrowser1.RunSuite();
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred when performing this operation. " + ex.Message);
            }
        }

        private void addDatasetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            testBrowser1.AddDataset();
        }
    }
}