using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using SsisUnit;

namespace ssisUnitTestRunnerUI
{
    public partial class TestBrowser : UserControl
    {
        private const string OpenFileDialogFilter = "Test Files(*.ssisUnit)|*.ssisUnit|All files (*.*)|*.*";

        private readonly Dictionary<string, SsisTestSuite> _testSuites = new Dictionary<string, SsisTestSuite>();

        private object _originalItem; // Will hold the originally selected node when the node is changed
        private SsisTestSuite _testSuite;

        public TestBrowser()
        {
            InitializeComponent();
            dlgFileOpen.FileName = string.Empty;
            dlgFileOpen.Filter = OpenFileDialogFilter;
            treeTest.ShowNodeToolTips = true;
            LoadCommands();
        }

        // TODO: Get rid of file dialogs and use UIHelper instead

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
            while (cboTests.Items.Contains("TestSuite " + counter.ToString(CultureInfo.InvariantCulture)))
            {
                counter++;
            }

            return "TestSuite " + counter.ToString(CultureInfo.InvariantCulture);
        }

        public void AddTestSuite(string fileName)
        {
            _testSuite = new SsisTestSuite(fileName);
            LoadTest(_testSuite, fileName);
        }

        public void SaveTestSuite(string fileName)
        {
            _testSuite.Save(fileName);
            if (fileName != cboTests.SelectedItem.ToString())
            {
                cboTests.Items.Remove(cboTests.SelectedItem);
                UpdateTestSelection(fileName);
                treeTest.Nodes[0].Text = fileName;
            }
        }

        public void AddCommand(string commandType)
        {
            CommandBase cb = CommandBase.CreateCommand(_testSuite, commandType);

            TreeNode commandNode = new TreeNode(cb.CommandName) { Tag = cb };

            CommandSet commandSet = treeTest.SelectedNode.Tag as CommandSet;

            if (commandSet != null)
            {
                CommandSet cs = commandSet;
                cs.Commands.Add(cb);
                treeTest.SelectedNode.Nodes.Add(commandNode);
            }
            else
            {
                SsisAssert ssisAssert = treeTest.SelectedNode.Tag as SsisAssert;
                if (ssisAssert != null)
                {
                    if (ssisAssert.Command != null)
                        throw new ArgumentException("There can only be one Command for an Assert. Please delete the existing Command before adding a new one.");

                    ssisAssert.Command = cb;
                    treeTest.SelectedNode.Nodes.Add(commandNode);
                }
            }
            commandNode.EnsureVisible();
            treeTest.SelectedNode = commandNode;
        }

        public void AddConnectionRef()
        {
            TreeNode crNode = treeTest.Nodes[0].Nodes.Find("Connection List", false)[0];
            int counter = 1;

            while (_testSuite.ConnectionRefs.ContainsKey("ConnectionRef" + counter.ToString(CultureInfo.InvariantCulture)))
            {
                counter++;
            }
            
            ConnectionRef cr = new ConnectionRef("ConnectionRef" + counter.ToString(CultureInfo.InvariantCulture), string.Empty, ConnectionRef.ConnectionTypeEnum.ConnectionString);
            _testSuite.ConnectionRefs.Add(cr.ReferenceName, cr);
            TreeNode tn = new TreeNode(cr.ReferenceName) { Tag = cr };
            crNode.Nodes.Add(tn);
            tn.EnsureVisible();
            treeTest.SelectedNode = tn;
        }

        public void AddPackageRef()
        {
            TreeNode prNode = treeTest.Nodes[0].Nodes.Find("Package List", false)[0];
            int counter = 1;
            while (_testSuite.PackageRefs.ContainsKey("PackageRef" + counter.ToString(CultureInfo.InvariantCulture)))
            {
                counter++;
            }
            PackageRef pr = new PackageRef("PackageRef" + counter.ToString(CultureInfo.InvariantCulture), string.Empty, PackageRef.PackageStorageType.FileSystem);
            _testSuite.PackageRefs.Add(pr.Name, pr);
            TreeNode tn = new TreeNode(pr.Name) { Tag = pr };
            prNode.Nodes.Add(tn);
            tn.EnsureVisible();
            treeTest.SelectedNode = tn;
        }

        public void AddDataset()
        {
            TreeNode prNode = treeTest.Nodes[0].Nodes.Find("Dataset List", false)[0];
            int counter = 1;

            while (_testSuite.Datasets.ContainsKey("Dataset" + counter.ToString(CultureInfo.InvariantCulture)))
            {
                counter++;
            }
            
            Dataset dataset = new Dataset(_testSuite, "Dataset" + counter.ToString(CultureInfo.InvariantCulture), null, false, string.Empty);
            _testSuite.Datasets.Add(dataset.Name, dataset);
            TreeNode tn = new TreeNode(dataset.Name) { Tag = dataset };
            prNode.Nodes.Add(tn);
            tn.EnsureVisible();
            treeTest.SelectedNode = tn;
        }

        public void DeleteItem()
        {
            if (treeTest.SelectedNode.Name == "Connection List" ||
                treeTest.SelectedNode.Name == "Package List" ||
                treeTest.SelectedNode.Name == "Dataset List" ||
                treeTest.SelectedNode.Name == "Test Suite Setup" ||
                treeTest.SelectedNode.Name == "Setup" ||
                treeTest.SelectedNode.Name == "Test" ||
                treeTest.SelectedNode.Name == "Teardown" ||
                treeTest.SelectedNode.Name == "Test Suite Teardown")
            {
                return;
            }

            CommandSet commandSet = treeTest.SelectedNode.Parent == null ? null : treeTest.SelectedNode.Parent.Tag as CommandSet;

            if (commandSet != null)
            {
                commandSet.Commands.Remove((CommandBase)treeTest.SelectedNode.Tag);
                treeTest.SelectedNode.Remove();
                return;
            }

            ConnectionRef connectionRef = treeTest.SelectedNode.Tag as ConnectionRef;
            
            if (connectionRef != null)
            {
                ConnectionRef cref = connectionRef;
                _testSuite.ConnectionRefs.Remove(cref.ReferenceName);
                treeTest.SelectedNode.Remove();
                return;
            }

            PackageRef packageRef = treeTest.SelectedNode.Tag as PackageRef;

            if (packageRef != null)
            {
                _testSuite.PackageRefs.Remove(packageRef.Name);
                treeTest.SelectedNode.Remove();
                return;
            }

            Dataset dataset = treeTest.SelectedNode.Tag as Dataset;

            if (dataset != null)
            {
                _testSuite.Datasets.Remove(dataset.Name);
                treeTest.SelectedNode.Remove();
                return;
            }

            Test unitTest = treeTest.SelectedNode.Tag as Test;

            if (unitTest != null)
            {
                _testSuite.Tests.Remove(unitTest.Name);
                treeTest.SelectedNode.Remove();
                return;
            }

            SsisAssert ssisAssert = treeTest.SelectedNode.Tag as SsisAssert;
            
            if (ssisAssert != null)
            {
                if (treeTest.SelectedNode.Parent == null)
                    return;

                Test cs = (Test)treeTest.SelectedNode.Parent.Tag;
                cs.Asserts.Remove(ssisAssert.Name);
                treeTest.SelectedNode.Remove();
                return;
            }

            if (treeTest.SelectedNode.Parent == null || !(treeTest.SelectedNode.Parent.Tag is SsisAssert))
                return;

            SsisAssert assert = (SsisAssert)treeTest.SelectedNode.Parent.Tag;
            assert.Command = null;
            treeTest.SelectedNode.Remove();
        }

        private void UpdateTestSelection(string fileName)
        {
            if (!cboTests.Items.Contains(fileName))
                cboTests.Items.Add(fileName);

            cboTests.SelectedItem = fileName;
        }

        private void BtnFileOpenClick(object sender, EventArgs e)
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
            testSuiteNode.Nodes.Add(CreateDatasetsListNode(testSuite));
            testSuiteNode.Nodes.Add(CreateCommandSetNode("Test Suite Setup", testSuite.TestSuiteSetup));
            testSuiteNode.Nodes.Add(CreateCommandSetNode("Setup", testSuite.SetupCommands));
            testSuiteNode.Nodes.Add(CreateTestsNode(testSuite));
            testSuiteNode.Nodes.Add(CreateCommandSetNode("Teardown", testSuite.TeardownCommands));
            testSuiteNode.Nodes.Add(CreateCommandSetNode("Test Suite Teardown", testSuite.TestSuiteTeardown));
            testSuiteNode.Expand();
            testSuiteNode.EnsureVisible();
            treeTest.SelectedNode = testSuiteNode;

            OnRaiseTestSuiteSelected(cboTests.SelectedItem == null ? new TestSuiteSelectedEventArgs(string.Empty, name) : new TestSuiteSelectedEventArgs(cboTests.SelectedItem.ToString(), name));

            _testSuite = testSuite;

            if (_testSuites.ContainsKey(name))
                _testSuites.Remove(name);

            _testSuites.Add(name, testSuite);
            
            UpdateTestSelection(name);
        }

        private TreeNode CreateTestsNode(SsisTestSuite testSuite)
        {
            TreeNode testsNode = new TreeNode("Tests") { Name = "Tests" };
            foreach (Test test in testSuite.Tests.Values)
            {
                testsNode.Nodes.Add(CreateTestNode(test));
            }
            return testsNode;
        }

        private TreeNode CreateTestNode(Test test)
        {
            TreeNode testNode = new TreeNode(test.Name) { Tag = test };
            testNode.Nodes.Add(CreateCommandSetNode("Test Setup", test.TestSetup));

            if (!test.Validate())
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
            TreeNode assertNode = new TreeNode(assert.Name) { Name = assert.Name, Tag = assert };
            if (!assert.Validate())
            {
                assertNode.ForeColor = Color.Red;
                assertNode.ToolTipText = assert.ValidationMessages;
            }

            if (assert.Command != null)
            {
                TreeNode commandNode = new TreeNode(assert.Command.CommandName) { Tag = assert.Command };
                assertNode.Nodes.Add(commandNode);
            }

            return assertNode;
        }

        private TreeNode CreateCommandSetNode(string rootNodeName, CommandSet commandSet)
        {
            TreeNode commandSetNode = new TreeNode(rootNodeName) { Name = rootNodeName, Tag = commandSet };
            foreach (CommandBase comm in commandSet.Commands)
            {
                TreeNode commandNode = new TreeNode(comm.CommandName) { Tag = comm };
                commandSetNode.Nodes.Add(commandNode);
            }
            return commandSetNode;
        }

        private TreeNode CreatePackageListNode(SsisTestSuite testSuite)
        {
            TreeNode packageList = new TreeNode("Package List") { Name = "Package List" };
            foreach (PackageRef pkg in testSuite.PackageRefs.Values)
            {
                TreeNode pkgNode = new TreeNode(pkg.Name) { Tag = pkg };
                packageList.Nodes.Add(pkgNode);
            }
            return packageList;
        }

        private TreeNode CreateConnectionListNode(SsisTestSuite testSuite)
        {
            TreeNode connectionList = new TreeNode("Connection List") { Name = "Connection List" };
            foreach (ConnectionRef conn in testSuite.ConnectionRefs.Values)
            {
                TreeNode connNode = new TreeNode(conn.ReferenceName) { Tag = conn };
                connectionList.Nodes.Add(connNode);
            }
            return connectionList;
        }

        private TreeNode CreateDatasetsListNode(SsisTestSuite testSuite)
        {
            TreeNode connectionList = new TreeNode("Datasets") { Name = "Dataset List" };
            foreach (Dataset dataset in testSuite.Datasets.Values)
            {
                TreeNode connNode = new TreeNode(dataset.Name) { Tag = dataset };
                connectionList.Nodes.Add(connNode);
            }
            return connectionList;
        }

        private void TreeTestBeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            if (treeTest.SelectedNode == null) return;

            _originalItem = treeTest.SelectedNode.Tag;

            IValidate item = _originalItem as IValidate;

            if (item != null)
            {
                Validate(item, treeTest.SelectedNode);
            }

            // Update Tree for label changes
            ConnectionRef connectionRef = _originalItem as ConnectionRef;

            if (connectionRef != null)
            {
                treeTest.SelectedNode.Text = connectionRef.ReferenceName;
                return;
            }

            PackageRef packageRef = _originalItem as PackageRef;

            if (packageRef != null)
            {
                treeTest.SelectedNode.Text = packageRef.Name;
                return;
            }

            Test test = _originalItem as Test;

            if (test != null)
            {
                treeTest.SelectedNode.Text = test.Name;
                return;
            }

            SsisAssert ssisAssert = _originalItem as SsisAssert;

            if (ssisAssert != null)
                treeTest.SelectedNode.Text = ssisAssert.Name;
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

        private void TreeTestAfterSelect(object sender, TreeViewEventArgs e)
        {
            OnRaiseNodeSelected(new NodeSelectedEventArgs(_originalItem, e.Node.Tag));
            EnableMenuItems(e.Node.Tag);
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

        private void CboTestsSelectionChangeCommitted(object sender, EventArgs e)
        {
            LoadTest(_testSuites[cboTests.SelectedItem.ToString()], cboTests.SelectedItem.ToString());
        }

        public void AddAssert()
        {
            if (!(treeTest.SelectedNode.Tag is Test)) return;
            Test test = (Test)treeTest.SelectedNode.Tag;
            int counter = 1;
            while (test.Asserts.ContainsKey("Assert" + counter.ToString(CultureInfo.InvariantCulture)))
            {
                counter++;
            }
            SsisAssert assert = new SsisAssert(_testSuite, "Assert" + counter.ToString(CultureInfo.InvariantCulture), null, false);
            test.Asserts.Add(assert.Name, assert);
            TreeNode tn = new TreeNode(assert.Name) { Tag = assert };
            treeTest.SelectedNode.Nodes.Add(tn);
            tn.EnsureVisible();
            treeTest.SelectedNode = tn;
        }

        public void AddTest()
        {
            TreeNode prNode = treeTest.Nodes[0].Nodes.Find("Tests", false)[0];
            int counter = 1;
            while (_testSuite.Tests.ContainsKey("Test" + counter.ToString(CultureInfo.InvariantCulture)))
            {
                counter++;
            }
            Test test = new Test(_testSuite, "Test" + counter.ToString(CultureInfo.InvariantCulture), string.Empty, string.Empty, string.Empty);
            _testSuite.Tests.Add(test.Name, test);
            TreeNode tn = CreateTestNode(test);
            prNode.Nodes.Add(tn);
            tn.EnsureVisible();
            treeTest.SelectedNode = tn;
        }

        public void RunSuite()
        {
            if (_testSuite == null)
            {
                throw new ApplicationException("No Test Suite is selected.");
            }
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

        private void AddAssertToolStripMenuItemClick(object sender, EventArgs e)
        {
            AddAssert();
        }

        private void AddTestToolStripMenuItemClick(object sender, EventArgs e)
        {
            AddTest();
        }

        private void AddConnectionRefToolStripMenuItemClick(object sender, EventArgs e)
        {
            AddConnectionRef();
        }

        private void AddDatasetToolStripMenuItemClick(object sender, EventArgs e)
        {
            AddDataset();
        }

        private void AddPackageRefToolStripMenuItemClick(object sender, EventArgs e)
        {
            AddPackageRef();
        }

        private void DeleteItemToolStripMenuItemClick(object sender, EventArgs e)
        {
            DeleteItem();
        }

        private void AddGenericCommandToolStripMenuItemClick(object sender, EventArgs e)
        {
            ToolStripMenuItem tsItem = (ToolStripMenuItem)sender;
            AddCommand(tsItem.Name);
        }

        private void LoadCommands()
        {
            foreach (Type t in System.Reflection.Assembly.GetAssembly(typeof(CommandBase)).GetTypes())
            {
                if (!typeof(CommandBase).IsAssignableFrom(t) || ReferenceEquals(t, typeof(CommandBase)) || t.IsAbstract)
                    continue;

                ToolStripMenuItem addGenericCommandToolStripMenuItem = new ToolStripMenuItem(t.Name) { Name = t.Name };
                addGenericCommandToolStripMenuItem.Click += AddGenericCommandToolStripMenuItemClick;
                addCommandToolStripMenuItem.DropDownItems.Add(addGenericCommandToolStripMenuItem);
            }
        }

        public void RefreshTestSuite()
        {
            LoadTest(_testSuite, cboTests.SelectedItem.ToString());
        }
    }
}