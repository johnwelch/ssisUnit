using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using SsisUnit;
using Microsoft.SqlServer.Dts.Runtime;
using SsisUnit.Design;

namespace ssisUnitTestRunnerUI
{
    public partial class TestBrowser : UserControl
    {
        private SsisTestSuite _testSuite;

        private object _originalItem; //Will hold the originally selected node when the node is changed
        private Dictionary<string, SsisTestSuite> _testSuites = new Dictionary<string, SsisTestSuite>();
        private Microsoft.SqlServer.Dts.Runtime.Application _ssisApp = new Microsoft.SqlServer.Dts.Runtime.Application();

        public TestBrowser()
        {
            InitializeComponent();
            dlgFileOpen.FileName = string.Empty;
            dlgFileOpen.Filter = "Test Files(*.ssisUnit)|*.ssisUnit|All files (*.*)|*.*";
            treeTest.ShowNodeToolTips = true;
            LoadCommands();
        }

        //TODO: Get rid of file dialogs and use UIHelper instead

        #region Events

        public event EventHandler<NodeSelectedEventArgs> NodeSelected;
        public event EventHandler<TestSuiteSelectedEventArgs> TestSuiteSelected;

        internal virtual void OnRaiseNodeSelected(NodeSelectedEventArgs e)
        {
            EventHandler<NodeSelectedEventArgs> handler = NodeSelected;

            // Event will be null if there are no subscribers
            if (handler != null)
            {
                handler(this, e);
            }
        }

        internal virtual void OnRaiseTestSuiteSelected(TestSuiteSelectedEventArgs e)
        {
            EventHandler<TestSuiteSelectedEventArgs> handler = TestSuiteSelected;

            // Event will be null if there are no subscribers
            if (handler != null)
            {
                handler(this, e);
            }
        }

        #endregion

        public object SelectedItem
        {
            get { return treeTest.SelectedNode.Tag; }
        }

        public SsisTestSuite TestSuite
        {
            get { return _testSuite; }
        }

        public void CreateTest()
        {
            SsisTestSuite newTs = new SsisTestSuite();
            
            LoadTest(newTs, GetUnusedName());
        }

        private string GetUnusedName()
        {
            int counter = 1;
            while (cboTests.Items.Contains("TestSuite " + counter.ToString()))
            {
                counter++;
            }

            return "TestSuite " + counter.ToString();
        }

        public void AddTestSuite(string fileName)
        {
            _testSuite = new SsisTestSuite(fileName);
            LoadTest(_testSuite, fileName);
        }

        public void SaveTestSuite(string fileName)
        {
            _testSuite.Save(fileName);
            if (fileName!=cboTests.SelectedItem.ToString())
            {
                cboTests.Items.Remove(cboTests.SelectedItem);
                UpdateTestSelection(fileName);
                treeTest.Nodes[0].Text = fileName;
            }
        }

        public void AddCommand(string commandType)
        {
            CommandBase cb = CommandBase.CreateCommand(_testSuite, commandType);

            TreeNode commandNode = new TreeNode(cb.CommandName);
            commandNode.Tag = cb;

            if (treeTest.SelectedNode.Tag is CommandSet)
            {
                CommandSet cs = (CommandSet)treeTest.SelectedNode.Tag;
                cs.Commands.Add(cb);
                treeTest.SelectedNode.Nodes.Add(commandNode);
            }
            else if (treeTest.SelectedNode.Tag is SsisAssert)
            {
                SsisAssert assert = (SsisAssert)treeTest.SelectedNode.Tag;
                if (assert.Command != null)
                {
                    throw new ArgumentException("There can only be one Command for an Assert. Please delete the existing Command before adding a new one.");
                }
                assert.Command = cb;
                treeTest.SelectedNode.Nodes.Add(commandNode);
            }
            commandNode.EnsureVisible();
            treeTest.SelectedNode = commandNode;
        }

        public void AddConnectionRef()
        {
            TreeNode crNode = treeTest.Nodes[0].Nodes.Find("Connection List", false)[0];
            int counter = 1;
            while (_testSuite.ConnectionRefs.ContainsKey("ConnectionRef" + counter.ToString()))
            {
                counter++;
            }
            ConnectionRef cr = new ConnectionRef("ConnectionRef" + counter.ToString(), string.Empty, ConnectionRef.ConnectionTypeEnum.ConnectionString);
            _testSuite.ConnectionRefs.Add(cr.ReferenceName, cr);
            TreeNode tn = new TreeNode(cr.ReferenceName);
            tn.Tag = cr;
            crNode.Nodes.Add(tn);
            tn.EnsureVisible();
            treeTest.SelectedNode = tn;
        }

        public void AddPackageRef()
        {
            TreeNode prNode = treeTest.Nodes[0].Nodes.Find("Package List", false)[0];
            int counter = 1;
            while (_testSuite.PackageRefs.ContainsKey("PackageRef" + counter.ToString()))
            {
                counter++;
            }
            PackageRef pr = new PackageRef("PackageRef" + counter.ToString(), string.Empty, PackageRef.PackageStorageType.FileSystem);
            _testSuite.PackageRefs.Add(pr.Name, pr);
            TreeNode tn = new TreeNode(pr.Name);
            tn.Tag = pr;
            prNode.Nodes.Add(tn);
            tn.EnsureVisible();
            treeTest.SelectedNode = tn;
        }

        public void DeleteItem()
        {
//            if (treeTest.SelectedNode.Nodes.Count > 0) return;
            if (treeTest.SelectedNode.Name == "Connection List" ||
                treeTest.SelectedNode.Name == "Package List" ||
                treeTest.SelectedNode.Name == "Test Suite Setup" ||
                treeTest.SelectedNode.Name == "Setup" ||
                treeTest.SelectedNode.Name == "Test" ||
                treeTest.SelectedNode.Name == "Teardown" ||
                treeTest.SelectedNode.Name == "Test Suite Teardown")
            {
                return;
            }
            if (treeTest.SelectedNode.Parent.Tag is CommandSet)
            {
                CommandSet cs = (CommandSet)treeTest.SelectedNode.Parent.Tag;
                cs.Commands.Remove((CommandBase)treeTest.SelectedNode.Tag);
                treeTest.SelectedNode.Remove();
            }
            if (treeTest.SelectedNode.Tag is ConnectionRef)
            {
                ConnectionRef cref = (ConnectionRef) treeTest.SelectedNode.Tag;
                _testSuite.ConnectionRefs.Remove(cref.ReferenceName);
                //Dictionary<string, ConnectionRef> crefs = (Dictionary<string, ConnectionRef>)treeTest.SelectedNode.Parent.Tag;
                //crefs.Remove(((ConnectionRef)treeTest.SelectedNode.Tag).ReferenceName);
                treeTest.SelectedNode.Remove();
            }
            if (treeTest.SelectedNode.Tag is PackageRef)
            {
                PackageRef cref = (PackageRef)treeTest.SelectedNode.Tag;
                _testSuite.PackageRefs.Remove(cref.Name);
                //Dictionary<string, PackageRef> prefs = (Dictionary<string, PackageRef>)treeTest.SelectedNode.Parent.Tag;
                //prefs.Remove(((PackageRef)treeTest.SelectedNode.Tag).Name);
                treeTest.SelectedNode.Remove();
            }
            if (treeTest.SelectedNode.Tag is Test)
            {
                Test test = (Test)treeTest.SelectedNode.Tag;
                _testSuite.Tests.Remove(test.Name);
                //Dictionary<string, Test> tests = (Dictionary<string, Test>)treeTest.SelectedNode.Parent.Tag;
                //tests.Remove(((Test)treeTest.SelectedNode.Tag).Name);
                treeTest.SelectedNode.Remove();
            }
            if (treeTest.SelectedNode.Parent.Tag is Test)
            {
                Test cs = (Test)treeTest.SelectedNode.Parent.Tag;
                cs.Asserts.Remove(((SsisAssert)treeTest.SelectedNode.Tag).Name);
                treeTest.SelectedNode.Remove();
            }
            if (treeTest.SelectedNode.Parent.Tag is SsisAssert)
            {
                SsisAssert cs = (SsisAssert)treeTest.SelectedNode.Parent.Tag;
                cs.Command = null;
                treeTest.SelectedNode.Remove();
            }
        }

        private void UpdateTestSelection(string fileName)
        {
            if (!cboTests.Items.Contains(fileName))
            {
                cboTests.Items.Add(fileName);
            }
            cboTests.SelectedItem = fileName;
        }

        private void btnFileOpen_Click(object sender, EventArgs e)
        {
            if (dlgFileOpen.ShowDialog() == DialogResult.OK)
            {
                LoadTest(new SsisTestSuite(dlgFileOpen.FileName), dlgFileOpen.FileName);
            }
        }

        private void LoadTest(SsisTestSuite testSuite, string name)
        {
            treeTest.Nodes.Clear();
            TreeNode testSuiteNode = new TreeNode(name);

            treeTest.Nodes.Add(testSuiteNode);
            testSuiteNode.Nodes.Add(CreateConnectionListNode(testSuite));
            testSuiteNode.Nodes.Add(CreatePackageListNode(testSuite));
            testSuiteNode.Nodes.Add(CreateCommandSetNode("Test Suite Setup", testSuite.TestSuiteSetup));
            testSuiteNode.Nodes.Add(CreateCommandSetNode("Setup", testSuite.SetupCommands));
            testSuiteNode.Nodes.Add(CreateTestsNode(testSuite));
            testSuiteNode.Nodes.Add(CreateCommandSetNode("Teardown", testSuite.TeardownCommands));
            testSuiteNode.Nodes.Add(CreateCommandSetNode("Test Suite Teardown", testSuite.TestSuiteTeardown));
            testSuiteNode.Expand();
            testSuiteNode.EnsureVisible();
            treeTest.SelectedNode = testSuiteNode;

            if (cboTests.SelectedItem == null)
            {
                OnRaiseTestSuiteSelected(new TestSuiteSelectedEventArgs(string.Empty, name));
            }
            else
            {
                OnRaiseTestSuiteSelected(new TestSuiteSelectedEventArgs(cboTests.SelectedItem.ToString(), name));
            }
            _testSuite = testSuite;

            
            if (_testSuites.ContainsKey(name))
            {
                _testSuites.Remove(name);
            }
            _testSuites.Add(name, testSuite);
            UpdateTestSelection(name);
        }

        private TreeNode CreateTestsNode(SsisTestSuite testSuite)
        {
            TreeNode testsNode = new TreeNode("Tests");
            testsNode.Name = "Tests";
            foreach (Test test in testSuite.Tests.Values)
            {
                testsNode.Nodes.Add(CreateTestNode(test));
            }
            return testsNode;
        }

        private TreeNode CreateTestNode(Test test)
        {
            TreeNode testNode = new TreeNode(test.Name);
            testNode.Tag = test;
            testNode.Nodes.Add(CreateCommandSetNode("Test Setup", test.TestSetup));

            if (! test.Validate())
            {
                testNode.ForeColor = Color.Red;
                testNode.ToolTipText = test.ValidationMessages;
            }

            foreach (SsisAssert assert in test.Asserts.Values)
            {
                testNode.Nodes.Add(CreateAssertNode(assert));
            }
            testNode.Nodes.Add(CreateCommandSetNode("Test Teardown", test.TestTeardown));
            return testNode;
        }

        private TreeNode CreateAssertNode(SsisAssert assert)
        {
            TreeNode assertNode = new TreeNode(assert.Name);
            assertNode.Name = assert.Name;
            assertNode.Tag = assert;
            if (!assert.Validate())
            {
                assertNode.ForeColor = Color.Red;
                assertNode.ToolTipText = assert.ValidationMessages;
            }
            TreeNode commandNode = new TreeNode(assert.Command.CommandName);
            commandNode.Tag = assert.Command;
            assertNode.Nodes.Add(commandNode);
            return assertNode;
        }

        private TreeNode CreateCommandSetNode(string rootNodeName, CommandSet commandSet)
        {
            TreeNode commandSetNode = new TreeNode(rootNodeName);
            commandSetNode.Name = rootNodeName;
            commandSetNode.Tag = commandSet;
            foreach (CommandBase comm in commandSet.Commands)
            {
                TreeNode commandNode = new TreeNode(comm.CommandName);
                commandNode.Tag = comm;
                commandSetNode.Nodes.Add(commandNode);
            }
            return commandSetNode;
        }

        private TreeNode CreatePackageListNode(SsisTestSuite testSuite)
        {
            TreeNode packageList = new TreeNode("Package List");
            packageList.Name = "Package List";
            foreach (PackageRef pkg in testSuite.PackageRefs.Values)
            {
                TreeNode pkgNode = new TreeNode(pkg.Name);
                pkgNode.Tag = pkg;
                packageList.Nodes.Add(pkgNode);
            }
            return packageList;
        }

        private TreeNode CreateConnectionListNode(SsisTestSuite testSuite)
        {
            TreeNode connectionList = new TreeNode("Connection List");
            connectionList.Name = "Connection List";
            foreach (ConnectionRef conn in testSuite.ConnectionRefs.Values)
            {
                TreeNode connNode = new TreeNode(conn.ReferenceName);
                connNode.Tag = conn;
                connectionList.Nodes.Add(connNode);
            }
            return connectionList;
        }

        private void treeTest_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            if (treeTest.SelectedNode == null) return;

            _originalItem = treeTest.SelectedNode.Tag;

            //Update Tree for label changes
            if (_originalItem is ConnectionRef)
            {
                treeTest.SelectedNode.Text = ((ConnectionRef)_originalItem).ReferenceName;
            }
            if (_originalItem is PackageRef)
            {
                treeTest.SelectedNode.Text = ((PackageRef)_originalItem).Name;
            }
            if (_originalItem is Test)
            {
                Test test = (Test)_originalItem;
                treeTest.SelectedNode.Text = test.Name;
            }
            if (_originalItem is SsisAssert)
            {
                treeTest.SelectedNode.Text = ((SsisAssert)_originalItem).Name;
            }
            if (_originalItem is IValidate)
            {
                Validate((IValidate)_originalItem, treeTest.SelectedNode);
            }
        }

        private void Validate(IValidate item, TreeNode node)
        {
            if (item.Validate())
            {
                node.ForeColor = Color.Black;
                node.ToolTipText = string.Empty;
            }
            else
            {
                node.ForeColor = Color.Red;
                node.ToolTipText = item.ValidationMessages;
            }
        }

        private void treeTest_AfterSelect(object sender, TreeViewEventArgs e)
        {
            OnRaiseNodeSelected(new NodeSelectedEventArgs(_originalItem, e.Node.Tag));
            EnableMenuItems(e.Node.Tag);
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

        private void cboTests_SelectionChangeCommitted(object sender, EventArgs e)
        {
            LoadTest(_testSuites[cboTests.SelectedItem.ToString()],cboTests.SelectedItem.ToString());
        }

        public void AddAssert()
        {
            if (!(treeTest.SelectedNode.Tag is Test)) return;
            Test test = (Test)treeTest.SelectedNode.Tag;
            int counter = 1;
            while (test.Asserts.ContainsKey("Assert" + counter.ToString()))
            {
                counter++;
            }
            SsisAssert assert = new SsisAssert(_testSuite, "Assert" + counter.ToString(), null, false);
            test.Asserts.Add(assert.Name, assert);
            TreeNode tn = new TreeNode(assert.Name);
            tn.Tag = assert;
            treeTest.SelectedNode.Nodes.Add(tn);
            tn.EnsureVisible();
            treeTest.SelectedNode = tn;
        }

        public void AddTest()
        {
            TreeNode prNode = treeTest.Nodes[0].Nodes.Find("Tests", false)[0];
            int counter = 1;
            while (_testSuite.Tests.ContainsKey("Test" + counter.ToString()))
            {
                counter++;
            }
            Test test = new Test(_testSuite, "Test" + counter.ToString(), string.Empty, string.Empty);
            _testSuite.Tests.Add(test.Name, test);
            TreeNode tn = new TreeNode(test.Name);
            tn.Tag = test;
            prNode.Nodes.Add(tn);
            tn.EnsureVisible();
            treeTest.SelectedNode = tn;
        }

        public void RunSuite()
        {
            TestResults ts = new TestResults(_testSuite);
            ts.RunSuite();
        }

        public void RunSelectedTest()
        {
            if (!(treeTest.SelectedNode.Tag is Test)) return;
            TestResults ts = new TestResults(_testSuite);
            ts.RunTest(((Test)treeTest.SelectedNode.Tag).Name);
        }

        internal void AddTestSuite(SsisTestSuite ts)
        {
            LoadTest(ts, GetUnusedName());
        }

        private void addAssertToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.AddAssert();
        }

        private void addTestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.AddTest();
        }

        private void addConnectionRefToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.AddConnectionRef();
        }

        private void addPackageRefToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.AddPackageRef();
        }

        private void deleteItemToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.DeleteItem();
        }
        
        private void addGenericCommandToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem tsItem = (ToolStripMenuItem)sender;
            this.AddCommand(tsItem.Name);
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

        //private void AddTestFromPackagetoolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    string packageFileName = string.Empty;
        //    if (UIHelper.ShowOpen(ref packageFileName, UIHelper.FILE_FILTER_DTSX, true) == DialogResult.OK)
        //    {
        //        AddTestFromPackage(packageFileName);
        //    }
        //}

        //private void AddTestFromPackage(string fileName)
        //{
        //    Package package = _ssisApp.LoadPackage(fileName, null);
        //    PackageBrowser pb = new PackageBrowser();
        //    pb.MultiSelect = true;
        //    if (pb.ShowDialog(package) == DialogResult.OK)
        //    {
        //        if (_testSuite.PackageRefs.ContainsKey()
        //        {
                    
        //        }
        //        AddConnectionRefs(ts, package);
        //        AddPackageRefs(ts, package, fileName);
        //        AddTests(ts, package, pb.SelectedTasks);
        //    }
        //    return ts;
        //}

        public void RefreshTestSuite()
        {
            LoadTest(_testSuite, cboTests.SelectedItem.ToString());
        }
    }

    public class NodeSelectedEventArgs : EventArgs
    {
        public NodeSelectedEventArgs(object oldItem, object newItem)
        {
            this._originalItem = oldItem;
            this._newItem = newItem;
        }

        private object _originalItem;
        private object _newItem;

        public object OriginalItem
        {
            get { return _originalItem; }
        }

        public object NewItem
        {
            get { return _newItem; }
        }
    }

    public class TestSuiteSelectedEventArgs : EventArgs
    {
        public TestSuiteSelectedEventArgs(string oldFile, string newFile)
        {
            this._originalFile = oldFile;
            this._newFile = newFile;
        }

        private string _originalFile;
        private string _newFile;

        public string PreviousFile
        {
            get { return _originalFile; }
        }

        public string CurrentFile
        {
            get { return _newFile; }
        }
    }

}
