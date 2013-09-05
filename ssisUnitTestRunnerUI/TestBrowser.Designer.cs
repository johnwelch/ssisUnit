namespace ssisUnitTestRunnerUI
{
    partial class TestBrowser
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.treeTest = new System.Windows.Forms.TreeView();
            this.mnuContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addAssertToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addCommandToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addTestToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addConnectionRefToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addPackageRefToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addDatasetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.deleteItemToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cboTests = new System.Windows.Forms.ComboBox();
            this.dlgFileOpen = new System.Windows.Forms.OpenFileDialog();
            this.btnFileOpen = new System.Windows.Forms.Button();
            this.imgList = new System.Windows.Forms.ImageList(this.components);
            this.mnuContextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // treeTest
            // 
            this.treeTest.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.treeTest.ContextMenuStrip = this.mnuContextMenuStrip;
            this.treeTest.Location = new System.Drawing.Point(4, 29);
            this.treeTest.Name = "treeTest";
            this.treeTest.Size = new System.Drawing.Size(178, 366);
            this.treeTest.TabIndex = 0;
            this.treeTest.BeforeSelect += new System.Windows.Forms.TreeViewCancelEventHandler(this.TreeTestBeforeSelect);
            this.treeTest.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.TreeTestAfterSelect);
            // 
            // mnuContextMenuStrip
            // 
            this.mnuContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addAssertToolStripMenuItem,
            this.addCommandToolStripMenuItem,
            this.addTestToolStripMenuItem,
            this.addConnectionRefToolStripMenuItem,
            this.addPackageRefToolStripMenuItem,
            this.addDatasetToolStripMenuItem,
            this.toolStripMenuItem1,
            this.deleteItemToolStripMenuItem});
            this.mnuContextMenuStrip.Name = "mnuContextMenuStrip";
            this.mnuContextMenuStrip.Size = new System.Drawing.Size(182, 208);
            // 
            // addAssertToolStripMenuItem
            // 
            this.addAssertToolStripMenuItem.Name = "addAssertToolStripMenuItem";
            this.addAssertToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            this.addAssertToolStripMenuItem.Text = "Add Assert";
            this.addAssertToolStripMenuItem.Click += new System.EventHandler(this.AddAssertToolStripMenuItemClick);
            // 
            // addCommandToolStripMenuItem
            // 
            this.addCommandToolStripMenuItem.Name = "addCommandToolStripMenuItem";
            this.addCommandToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            this.addCommandToolStripMenuItem.Text = "Add Command";
            // 
            // addTestToolStripMenuItem
            // 
            this.addTestToolStripMenuItem.Name = "addTestToolStripMenuItem";
            this.addTestToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            this.addTestToolStripMenuItem.Text = "Add Test";
            this.addTestToolStripMenuItem.Click += new System.EventHandler(this.AddTestToolStripMenuItemClick);
            // 
            // addConnectionRefToolStripMenuItem
            // 
            this.addConnectionRefToolStripMenuItem.Name = "addConnectionRefToolStripMenuItem";
            this.addConnectionRefToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            this.addConnectionRefToolStripMenuItem.Text = "Add Connection Ref";
            this.addConnectionRefToolStripMenuItem.Click += new System.EventHandler(this.AddConnectionRefToolStripMenuItemClick);
            // 
            // addPackageRefToolStripMenuItem
            // 
            this.addPackageRefToolStripMenuItem.Name = "addPackageRefToolStripMenuItem";
            this.addPackageRefToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            this.addPackageRefToolStripMenuItem.Text = "Add Package Ref";
            this.addPackageRefToolStripMenuItem.Click += new System.EventHandler(this.AddPackageRefToolStripMenuItemClick);
            // 
            // addDatasetToolStripMenuItem
            // 
            this.addDatasetToolStripMenuItem.Name = "addDatasetToolStripMenuItem";
            this.addDatasetToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            this.addDatasetToolStripMenuItem.Text = "Add Dataset";
            this.addDatasetToolStripMenuItem.Click += new System.EventHandler(this.AddDatasetToolStripMenuItemClick);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(178, 6);
            // 
            // deleteItemToolStripMenuItem
            // 
            this.deleteItemToolStripMenuItem.Name = "deleteItemToolStripMenuItem";
            this.deleteItemToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            this.deleteItemToolStripMenuItem.Text = "Delete Item";
            this.deleteItemToolStripMenuItem.Click += new System.EventHandler(this.DeleteItemToolStripMenuItemClick);
            // 
            // cboTests
            // 
            this.cboTests.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboTests.FormattingEnabled = true;
            this.cboTests.Location = new System.Drawing.Point(4, 3);
            this.cboTests.Name = "cboTests";
            this.cboTests.Size = new System.Drawing.Size(152, 21);
            this.cboTests.TabIndex = 1;
            this.cboTests.SelectionChangeCommitted += new System.EventHandler(this.CboTestsSelectionChangeCommitted);
            // 
            // dlgFileOpen
            // 
            this.dlgFileOpen.FileName = "openFileDialog1";
            // 
            // btnFileOpen
            // 
            this.btnFileOpen.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnFileOpen.Location = new System.Drawing.Point(159, 4);
            this.btnFileOpen.Name = "btnFileOpen";
            this.btnFileOpen.Size = new System.Drawing.Size(24, 20);
            this.btnFileOpen.TabIndex = 2;
            this.btnFileOpen.Text = "...";
            this.btnFileOpen.UseVisualStyleBackColor = true;
            this.btnFileOpen.Click += new System.EventHandler(this.BtnFileOpenClick);
            // 
            // imgList
            // 
            this.imgList.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.imgList.ImageSize = new System.Drawing.Size(16, 16);
            this.imgList.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // TestBrowser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnFileOpen);
            this.Controls.Add(this.cboTests);
            this.Controls.Add(this.treeTest);
            this.MinimumSize = new System.Drawing.Size(50, 50);
            this.Name = "TestBrowser";
            this.Size = new System.Drawing.Size(185, 398);
            this.mnuContextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView treeTest;
        private System.Windows.Forms.ComboBox cboTests;
        private System.Windows.Forms.OpenFileDialog dlgFileOpen;
        private System.Windows.Forms.Button btnFileOpen;
        private System.Windows.Forms.ImageList imgList;
        private System.Windows.Forms.ContextMenuStrip mnuContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem addAssertToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addCommandToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addTestToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addConnectionRefToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addPackageRefToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem deleteItemToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addDatasetToolStripMenuItem;
    }
}
