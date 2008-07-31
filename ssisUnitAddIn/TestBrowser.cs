using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using SsisUnit;

namespace ssisUnitAddIn
{
    public partial class TestBrowser : UserControl
    {
        private SsisTestSuite _testSuite;

        public TestBrowser()
        {
            InitializeComponent();
            dlgFileOpen.Filter = "Test Files(*.ssisUnit)|*.ssisUnit|All files (*.*)|*.*";
        }

        private void btnFileOpen_Click(object sender, EventArgs e)
        {
            if (dlgFileOpen.ShowDialog() == DialogResult.OK)
            {
                LoadTest(dlgFileOpen.FileName);
                if (!cboTests.Items.Contains(dlgFileOpen.FileName))
                {
                    cboTests.Items.Add(dlgFileOpen.FileName);
                }
                cboTests.SelectedItem = dlgFileOpen.FileName;
            }
        }

        private void LoadTest(string fileName)
        {
            _testSuite = new SsisTestSuite(fileName);
            TreeNode testSuiteNode = new TreeNode(fileName);
            treeTest.Nodes.Add(testSuiteNode);
            testSuiteNode.Nodes.Add(CreateConnectionListNode());
            testSuiteNode.Nodes.Add(CreatePackageListNode());
            testSuiteNode.Nodes.Add(CreateCommandSetNode("Test Suite Setup", _testSuite.TestSuiteSetup));
            testSuiteNode.Nodes.Add(CreateCommandSetNode("Setup", _testSuite.SetupCommands));
            testSuiteNode.Nodes.Add(CreateTestsNode());
            testSuiteNode.Nodes.Add(CreateCommandSetNode("Teardown", _testSuite.TeardownCommands));
            testSuiteNode.Nodes.Add(CreateCommandSetNode("Test Suite Teardown", _testSuite.TestSuiteTeardown));
        }

        private TreeNode CreateTestsNode()
        {
            TreeNode testsNode = new TreeNode("Tests");
            foreach (Test test in _testSuite.Tests.Values)
            {
                testsNode.Nodes.Add(CreateTestNode(test));
            }
            return testsNode;
        }

        private TreeNode CreateTestNode(Test test)
        {
            TreeNode testNode = new TreeNode(test.Name);
            foreach (SsisAssert assert in test.Asserts.Values)
            {
                testNode.Nodes.Add(CreateAssertNode(assert));
            }
            return testNode;
        }

        private TreeNode CreateAssertNode(SsisAssert assert)
        {
            TreeNode assertNode = new TreeNode(assert.Name);

            return assertNode;
        }

        private TreeNode CreateCommandSetNode(string rootNodeName, CommandSet commandSet)
        {
            TreeNode commandSetNode = new TreeNode(rootNodeName);
            foreach (CommandBase comm in commandSet.Commands)
            {
                commandSetNode.Nodes.Add(comm.CommandName);
            }
            return commandSetNode;
        }

        private TreeNode CreatePackageListNode()
        {
            TreeNode packageList = new TreeNode("Package List");
            foreach (PackageRef pkg in _testSuite.PackageRefs.Values)
            {
                packageList.Nodes.Add(pkg.Name);
            }
            return packageList;
        }

        private TreeNode CreateConnectionListNode()
        {
            TreeNode connectionList = new TreeNode("Connection List");
            foreach (ConnectionRef conn in _testSuite.ConnectionRefs.Values)
            {
                connectionList.Nodes.Add(conn.ReferenceName);
            }
            return connectionList;
        }
    }
}
