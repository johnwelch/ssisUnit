using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.SqlServer.Dts.Runtime;

namespace SsisUnit.Design
{
    public partial class PackageBrowser : Form
    {
        private Package _SSISPackage;
        private bool _multiSelect = false;

        public PackageBrowser()
        {
            InitializeComponent();
        }

        public bool MultiSelect
        {
            get { return _multiSelect; }
            set { _multiSelect = value; }
        }

        public Package SSISPackage
        {
            get { return _SSISPackage; }
            set { _SSISPackage = value; }
        }

        public string SelectedTask
        {
            get { return treePackage.SelectedNode.Name; }
        }

        public string SelectedTaskID
        {
            get { return treePackage.SelectedNode.Tag.ToString(); }
        }

        public DialogResult ShowDialog(Package package)
        {
            _SSISPackage = package;
            LoadTree(package);
            return this.ShowDialog();
        }

        public DialogResult ShowDialog(Package package, string task)
        {
            _SSISPackage = package;
            LoadTree(package);
            treePackage.SelectedNode = FindNodeByNameOrTag(treePackage.Nodes, task);
            return this.ShowDialog();
        }

        private TreeNode FindNodeByNameOrTag(TreeNodeCollection nodes, string task)
        {
            TreeNode returnValue = null;
            
            TreeNode[] tn = nodes.Find(task, false);
            if (tn.Length > 0)
            {
                returnValue = tn[0];
            }
            else
            {

                foreach (TreeNode node in nodes)
                {
                    if (node.Tag!=null && node.Tag.ToString() == task)
                    {
                        returnValue = node;
                        break;
                    }

                    returnValue = FindNodeByNameOrTag(node.Nodes, task);
                    if (returnValue!=null)
                    {
                        break;
                    }
                }
            }
            return returnValue;
        }

        private void LoadTree(Package package)
        {
            treePackage.CheckBoxes = _multiSelect;
            btnDeselectAll.Visible = _multiSelect;
            btnSelectAll.Visible = _multiSelect;

            treePackage.Nodes.Clear();
            IterateContainers(package, treePackage.Nodes);
        }

        public List<TaskItem> SelectedTasks
        {
            get
            { return GetCheckedNodes(treePackage.Nodes); }
        }

        private List<TaskItem> GetCheckedNodes(TreeNodeCollection nodes)
        {
            List<TaskItem> checkedNodes = new List<TaskItem>();

            if (!_multiSelect)
            {
                checkedNodes.Add(new TaskItem(treePackage.SelectedNode.Tag.ToString(), treePackage.SelectedNode.Name));
            }

            foreach (TreeNode node in nodes)
            {
                if (node.Text != "Event Handlers" && node.Checked)
                {
                    checkedNodes.Add(new TaskItem(node.Tag.ToString(), node.Name));
                }

                checkedNodes.AddRange(GetCheckedNodes(node.Nodes));
            }
            return checkedNodes;
        }

        private void IterateContainers(DtsContainer parent, TreeNodeCollection nodes)
        {
            TreeNode node = new TreeNode();
            node.Name = parent.Name;
            node.Text = parent.Name;
            node.Tag = parent.ID;
            node.Checked = true;
            node.Expand();

            if (parent is EventsProvider)
            {
                EventsProvider ep = (EventsProvider)parent;

                if (ep.EventHandlers.Count > 0)
                {
                    TreeNode eventNode = new TreeNode("Event Handlers");
                    foreach (DtsEventHandler eh in ep.EventHandlers)
                    {
                        IterateContainers((DtsContainer)eh, eventNode.Nodes);
                    }
                    node.Nodes.Add(eventNode);
                }


            }

            nodes.Add(node);

            IDTSSequence seq = (IDTSSequence)parent;

            foreach (Executable e in seq.Executables)
            {
                if (e is IDTSSequence)
                {
                    IterateContainers((DtsContainer)e, node.Nodes);
                }
                else
                {
                    DtsContainer task = (DtsContainer)e;
                    TreeNode childNode = new TreeNode();
                    childNode.Name = task.Name;
                    childNode.Text = task.Name;
                    childNode.Tag = task.ID;
                    childNode.Checked = true;

                    if (task is EventsProvider)
                    {
                        EventsProvider ep = (EventsProvider)task;

                        if (ep.EventHandlers.Count > 0)
                        {
                            TreeNode eventNode = new TreeNode("Event Handlers");
                            foreach (DtsEventHandler eh in ep.EventHandlers)
                            {
                                IterateContainers((DtsContainer)eh, eventNode.Nodes);
                            }
                            node.Nodes.Add(eventNode);
                        }
                    }

                    node.Nodes.Add(childNode);
                }

            }

            return;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {

        }

        private void btnSelectAll_Click(object sender, EventArgs e)
        {
            CheckNodes(treePackage.Nodes, true);
        }

        private void CheckNodes(TreeNodeCollection nodes, bool nodeChecked)
        {
            foreach (TreeNode node in nodes)
            {
                if (node.Text == "Event Handlers")
                {
                    node.Checked = false;
                }
                else
                {
                    node.Checked = nodeChecked;
                }

                CheckNodes(node.Nodes, nodeChecked);
            }
        }

        private void btnDeselectAll_Click(object sender, EventArgs e)
        {
            CheckNodes(treePackage.Nodes, false);
        }

        private void PackageBrowser_Load(object sender, EventArgs e)
        {

        }
    }

    public struct TaskItem
    {
        private string _id;
        private string _name;

        public TaskItem(string id, string name)
        {
            _id = id;
            _name = name;
        }

        public string ID
        {
            get { return _id; }
        }
        public string Name
        {
            get { return _name; }
        }
    }
}
