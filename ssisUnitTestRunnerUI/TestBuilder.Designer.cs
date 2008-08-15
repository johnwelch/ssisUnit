namespace ssisUnitTestRunnerUI
{
    partial class testSuiteUI
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.testBrowser1 = new ssisUnitTestRunnerUI.TestBrowser();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabProperties = new System.Windows.Forms.TabPage();
            this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
            this.tabXML = new System.Windows.Forms.TabPage();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.txtXML = new System.Windows.Forms.TextBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newFromPackageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.addAssertToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addCommandToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addTestToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addConnectionRefToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addPackageRefToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.deleteSelectedItemToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.testSuiteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.runSuiteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.runSelectedTestToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dlgOpenFile = new System.Windows.Forms.OpenFileDialog();
            this.dlgSaveFile = new System.Windows.Forms.SaveFileDialog();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabProperties.SuspendLayout();
            this.tabXML.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 24);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.testBrowser1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.tabControl1);
            this.splitContainer1.Size = new System.Drawing.Size(615, 464);
            this.splitContainer1.SplitterDistance = 205;
            this.splitContainer1.TabIndex = 0;
            // 
            // testBrowser1
            // 
            this.testBrowser1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.testBrowser1.Location = new System.Drawing.Point(0, 0);
            this.testBrowser1.MinimumSize = new System.Drawing.Size(50, 50);
            this.testBrowser1.Name = "testBrowser1";
            this.testBrowser1.Size = new System.Drawing.Size(205, 464);
            this.testBrowser1.TabIndex = 0;
            this.testBrowser1.TestSuiteSelected += new System.EventHandler<ssisUnitTestRunnerUI.TestSuiteSelectedEventArgs>(this.testBrowser1_TestSuiteSelected);
            this.testBrowser1.NodeSelected += new System.EventHandler<ssisUnitTestRunnerUI.NodeSelectedEventArgs>(this.testBrowser1_NodeSelected);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabProperties);
            this.tabControl1.Controls.Add(this.tabXML);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(406, 464);
            this.tabControl1.TabIndex = 0;
            // 
            // tabProperties
            // 
            this.tabProperties.Controls.Add(this.propertyGrid1);
            this.tabProperties.Location = new System.Drawing.Point(4, 22);
            this.tabProperties.Name = "tabProperties";
            this.tabProperties.Padding = new System.Windows.Forms.Padding(3);
            this.tabProperties.Size = new System.Drawing.Size(398, 438);
            this.tabProperties.TabIndex = 0;
            this.tabProperties.Text = "Properties";
            this.tabProperties.UseVisualStyleBackColor = true;
            // 
            // propertyGrid1
            // 
            this.propertyGrid1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGrid1.Location = new System.Drawing.Point(3, 3);
            this.propertyGrid1.Name = "propertyGrid1";
            this.propertyGrid1.Size = new System.Drawing.Size(392, 432);
            this.propertyGrid1.TabIndex = 1;
            // 
            // tabXML
            // 
            this.tabXML.Controls.Add(this.btnRefresh);
            this.tabXML.Controls.Add(this.btnSave);
            this.tabXML.Controls.Add(this.txtXML);
            this.tabXML.Location = new System.Drawing.Point(4, 22);
            this.tabXML.Name = "tabXML";
            this.tabXML.Padding = new System.Windows.Forms.Padding(3);
            this.tabXML.Size = new System.Drawing.Size(398, 438);
            this.tabXML.TabIndex = 1;
            this.tabXML.Text = "XML";
            this.tabXML.UseVisualStyleBackColor = true;
            // 
            // btnRefresh
            // 
            this.btnRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRefresh.Location = new System.Drawing.Point(315, 407);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(75, 23);
            this.btnRefresh.TabIndex = 2;
            this.btnRefresh.Text = "&Refresh";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.Location = new System.Drawing.Point(234, 407);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 1;
            this.btnSave.Text = "&Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Visible = false;
            // 
            // txtXML
            // 
            this.txtXML.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtXML.Location = new System.Drawing.Point(6, 6);
            this.txtXML.Multiline = true;
            this.txtXML.Name = "txtXML";
            this.txtXML.ReadOnly = true;
            this.txtXML.Size = new System.Drawing.Size(386, 395);
            this.txtXML.TabIndex = 0;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.toolStripMenuItem1,
            this.testSuiteToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(615, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.newFromPackageToolStripMenuItem,
            this.openToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.saveAsToolStripMenuItem,
            this.toolStripMenuItem2,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
            this.newToolStripMenuItem.Text = "&New...";
            this.newToolStripMenuItem.Click += new System.EventHandler(this.newToolStripMenuItem_Click);
            // 
            // newFromPackageToolStripMenuItem
            // 
            this.newFromPackageToolStripMenuItem.Name = "newFromPackageToolStripMenuItem";
            this.newFromPackageToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
            this.newFromPackageToolStripMenuItem.Text = "New From &Package...";
            this.newFromPackageToolStripMenuItem.Visible = false;
            this.newFromPackageToolStripMenuItem.Click += new System.EventHandler(this.newFromPackageToolStripMenuItem_Click);
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
            this.openToolStripMenuItem.Text = "&Open...";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
            this.saveToolStripMenuItem.Text = "&Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
            this.saveAsToolStripMenuItem.Text = "Save As...";
            this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.saveAsToolStripMenuItem_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(182, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
            this.exitToolStripMenuItem.Text = "E&xit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addAssertToolStripMenuItem,
            this.addCommandToolStripMenuItem,
            this.addTestToolStripMenuItem,
            this.addConnectionRefToolStripMenuItem,
            this.addPackageRefToolStripMenuItem,
            this.toolStripSeparator1,
            this.deleteSelectedItemToolStripMenuItem});
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(39, 20);
            this.toolStripMenuItem1.Text = "&Edit";
            // 
            // addAssertToolStripMenuItem
            // 
            this.addAssertToolStripMenuItem.Name = "addAssertToolStripMenuItem";
            this.addAssertToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            this.addAssertToolStripMenuItem.Text = "Add &Assert";
            // 
            // addCommandToolStripMenuItem
            // 
            this.addCommandToolStripMenuItem.Name = "addCommandToolStripMenuItem";
            this.addCommandToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            this.addCommandToolStripMenuItem.Text = "Add &Command";
            // 
            // addTestToolStripMenuItem
            // 
            this.addTestToolStripMenuItem.Name = "addTestToolStripMenuItem";
            this.addTestToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            this.addTestToolStripMenuItem.Text = "Add &Test";
            // 
            // addConnectionRefToolStripMenuItem
            // 
            this.addConnectionRefToolStripMenuItem.Name = "addConnectionRefToolStripMenuItem";
            this.addConnectionRefToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            this.addConnectionRefToolStripMenuItem.Text = "Add Connection &Ref";
            // 
            // addPackageRefToolStripMenuItem
            // 
            this.addPackageRefToolStripMenuItem.Name = "addPackageRefToolStripMenuItem";
            this.addPackageRefToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            this.addPackageRefToolStripMenuItem.Text = "Add Pa&ckage Ref";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(178, 6);
            // 
            // deleteSelectedItemToolStripMenuItem
            // 
            this.deleteSelectedItemToolStripMenuItem.Name = "deleteSelectedItemToolStripMenuItem";
            this.deleteSelectedItemToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            this.deleteSelectedItemToolStripMenuItem.Text = "&Delete Selected Item";
            this.deleteSelectedItemToolStripMenuItem.Click += new System.EventHandler(this.deleteSelectedItemToolStripMenuItem_Click);
            // 
            // testSuiteToolStripMenuItem
            // 
            this.testSuiteToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.runSuiteToolStripMenuItem,
            this.runSelectedTestToolStripMenuItem});
            this.testSuiteToolStripMenuItem.Name = "testSuiteToolStripMenuItem";
            this.testSuiteToolStripMenuItem.Size = new System.Drawing.Size(70, 20);
            this.testSuiteToolStripMenuItem.Text = "&Test Suite";
            // 
            // runSuiteToolStripMenuItem
            // 
            this.runSuiteToolStripMenuItem.Name = "runSuiteToolStripMenuItem";
            this.runSuiteToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
            this.runSuiteToolStripMenuItem.Text = "Run Suite";
            this.runSuiteToolStripMenuItem.Click += new System.EventHandler(this.runSuiteToolStripMenuItem_Click);
            // 
            // runSelectedTestToolStripMenuItem
            // 
            this.runSelectedTestToolStripMenuItem.Name = "runSelectedTestToolStripMenuItem";
            this.runSelectedTestToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
            this.runSelectedTestToolStripMenuItem.Text = "Run Selected Test";
            this.runSelectedTestToolStripMenuItem.Click += new System.EventHandler(this.runSelectedTestToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // dlgOpenFile
            // 
            this.dlgOpenFile.FileName = "openFileDialog1";
            // 
            // testSuiteUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(615, 488);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.menuStrip1);
            this.Name = "testSuiteUI";
            this.Text = "Test Suite Browser";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabProperties.ResumeLayout(false);
            this.tabXML.ResumeLayout(false);
            this.tabXML.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private TestBrowser testBrowser1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.OpenFileDialog dlgOpenFile;
        private System.Windows.Forms.SaveFileDialog dlgSaveFile;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem testSuiteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newFromPackageToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem deleteSelectedItemToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addAssertToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addCommandToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addTestToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addConnectionRefToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addPackageRefToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem runSuiteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem runSelectedTestToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabProperties;
        private System.Windows.Forms.PropertyGrid propertyGrid1;
        private System.Windows.Forms.TabPage tabXML;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.TextBox txtXML;
    }
}

