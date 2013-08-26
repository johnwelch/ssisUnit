namespace ssisUnitTestRunnerUI
{
    partial class TestResults
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.testSuiteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.runTestSuiteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.keepResultsToolStripItem = new System.Windows.Forms.ToolStripMenuItem();
            this.refreshToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.ResultType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ExecutionTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PackageName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TaskName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TestName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.AssertResult = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TestPassed = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.lblTestCount = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblTestPassed = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblTestsFailed = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblAssertCount = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblAssertPassed = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblAssertsFailed = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.statusStrip1.SuspendLayout();
            this.toolStripContainer1.BottomToolStripPanel.SuspendLayout();
            this.toolStripContainer1.ContentPanel.SuspendLayout();
            this.toolStripContainer1.TopToolStripPanel.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.testSuiteToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(934, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveToolStripMenuItem,
            this.toolStripMenuItem1,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(98, 22);
            this.saveToolStripMenuItem.Text = "&Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.SaveToolStripMenuItemClick);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(95, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(98, 22);
            this.exitToolStripMenuItem.Text = "E&xit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.ExitToolStripMenuItemClick);
            // 
            // testSuiteToolStripMenuItem
            // 
            this.testSuiteToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.runTestSuiteToolStripMenuItem,
            this.keepResultsToolStripItem,
            this.refreshToolStripMenuItem});
            this.testSuiteToolStripMenuItem.Name = "testSuiteToolStripMenuItem";
            this.testSuiteToolStripMenuItem.Size = new System.Drawing.Size(70, 20);
            this.testSuiteToolStripMenuItem.Text = "Test Suite";
            // 
            // runTestSuiteToolStripMenuItem
            // 
            this.runTestSuiteToolStripMenuItem.Name = "runTestSuiteToolStripMenuItem";
            this.runTestSuiteToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
            this.runTestSuiteToolStripMenuItem.Text = "Run Test Suite";
            this.runTestSuiteToolStripMenuItem.Click += new System.EventHandler(this.RunTestSuiteToolStripMenuItemClick);
            // 
            // keepResultsToolStripItem
            // 
            this.keepResultsToolStripItem.Checked = true;
            this.keepResultsToolStripItem.CheckOnClick = true;
            this.keepResultsToolStripItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.keepResultsToolStripItem.Name = "keepResultsToolStripItem";
            this.keepResultsToolStripItem.Size = new System.Drawing.Size(188, 22);
            this.keepResultsToolStripItem.Text = "Keep Previous Results";
            // 
            // refreshToolStripMenuItem
            // 
            this.refreshToolStripMenuItem.Name = "refreshToolStripMenuItem";
            this.refreshToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F5;
            this.refreshToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
            this.refreshToolStripMenuItem.Text = "Refresh Test Case";
            this.refreshToolStripMenuItem.Click += new System.EventHandler(this.refreshToolStripMenuItem_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCellsExceptHeaders;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ResultType,
            this.ExecutionTime,
            this.PackageName,
            this.TaskName,
            this.TestName,
            this.AssertResult,
            this.TestPassed});
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView1.DefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(0, 0);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.Size = new System.Drawing.Size(934, 556);
            this.dataGridView1.TabIndex = 1;
            // 
            // ResultType
            // 
            this.ResultType.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.ResultType.HeaderText = "Type";
            this.ResultType.Name = "ResultType";
            this.ResultType.ReadOnly = true;
            this.ResultType.Width = 56;
            // 
            // ExecutionTime
            // 
            this.ExecutionTime.FillWeight = 75F;
            this.ExecutionTime.HeaderText = "Executed";
            this.ExecutionTime.Name = "ExecutionTime";
            this.ExecutionTime.ReadOnly = true;
            this.ExecutionTime.Width = 120;
            // 
            // PackageName
            // 
            this.PackageName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.PackageName.FillWeight = 75F;
            this.PackageName.HeaderText = "Package Name";
            this.PackageName.Name = "PackageName";
            this.PackageName.ReadOnly = true;
            // 
            // TaskName
            // 
            this.TaskName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.TaskName.FillWeight = 75F;
            this.TaskName.HeaderText = "Task Name";
            this.TaskName.Name = "TaskName";
            this.TaskName.ReadOnly = true;
            // 
            // TestName
            // 
            this.TestName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.TestName.FillWeight = 75F;
            this.TestName.HeaderText = "Test Name";
            this.TestName.Name = "TestName";
            this.TestName.ReadOnly = true;
            // 
            // AssertResult
            // 
            this.AssertResult.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.AssertResult.FillWeight = 150F;
            this.AssertResult.HeaderText = "Result";
            this.AssertResult.Name = "AssertResult";
            this.AssertResult.ReadOnly = true;
            // 
            // TestPassed
            // 
            this.TestPassed.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.TestPassed.HeaderText = "Passed";
            this.TestPassed.Name = "TestPassed";
            this.TestPassed.ReadOnly = true;
            this.TestPassed.Width = 48;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblTestCount,
            this.lblTestPassed,
            this.lblTestsFailed,
            this.lblAssertCount,
            this.lblAssertPassed,
            this.lblAssertsFailed});
            this.statusStrip1.Location = new System.Drawing.Point(0, 0);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(934, 22);
            this.statusStrip1.TabIndex = 2;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // lblTestCount
            // 
            this.lblTestCount.Name = "lblTestCount";
            this.lblTestCount.Size = new System.Drawing.Size(77, 17);
            this.lblTestCount.Text = "Test Count: 0";
            // 
            // lblTestPassed
            // 
            this.lblTestPassed.Name = "lblTestPassed";
            this.lblTestPassed.Size = new System.Drawing.Size(85, 17);
            this.lblTestPassed.Text = "Tests Passed: 0";
            // 
            // lblTestsFailed
            // 
            this.lblTestsFailed.Name = "lblTestsFailed";
            this.lblTestsFailed.Size = new System.Drawing.Size(80, 17);
            this.lblTestsFailed.Text = "Tests Failed: 0";
            // 
            // lblAssertCount
            // 
            this.lblAssertCount.Name = "lblAssertCount";
            this.lblAssertCount.Size = new System.Drawing.Size(87, 17);
            this.lblAssertCount.Text = "Assert Count: 0";
            // 
            // lblAssertPassed
            // 
            this.lblAssertPassed.Name = "lblAssertPassed";
            this.lblAssertPassed.Size = new System.Drawing.Size(95, 17);
            this.lblAssertPassed.Text = "Asserts Passed: 0";
            // 
            // lblAssertsFailed
            // 
            this.lblAssertsFailed.Name = "lblAssertsFailed";
            this.lblAssertsFailed.Size = new System.Drawing.Size(90, 17);
            this.lblAssertsFailed.Text = "Asserts Failed: 0";
            // 
            // toolStripContainer1
            // 
            // 
            // toolStripContainer1.BottomToolStripPanel
            // 
            this.toolStripContainer1.BottomToolStripPanel.Controls.Add(this.statusStrip1);
            // 
            // toolStripContainer1.ContentPanel
            // 
            this.toolStripContainer1.ContentPanel.AutoScroll = true;
            this.toolStripContainer1.ContentPanel.Controls.Add(this.dataGridView1);
            this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(934, 556);
            this.toolStripContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStripContainer1.LeftToolStripPanelVisible = false;
            this.toolStripContainer1.Location = new System.Drawing.Point(0, 0);
            this.toolStripContainer1.Name = "toolStripContainer1";
            this.toolStripContainer1.RightToolStripPanelVisible = false;
            this.toolStripContainer1.Size = new System.Drawing.Size(934, 602);
            this.toolStripContainer1.TabIndex = 3;
            this.toolStripContainer1.Text = "toolStripContainer1";
            // 
            // toolStripContainer1.TopToolStripPanel
            // 
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.menuStrip1);
            // 
            // TestResults
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(934, 602);
            this.Controls.Add(this.toolStripContainer1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "TestResults";
            this.Text = "TestResults";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.TestResultsFormClosing);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.toolStripContainer1.BottomToolStripPanel.ResumeLayout(false);
            this.toolStripContainer1.BottomToolStripPanel.PerformLayout();
            this.toolStripContainer1.ContentPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.PerformLayout();
            this.toolStripContainer1.ResumeLayout(false);
            this.toolStripContainer1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel lblTestCount;
        private System.Windows.Forms.ToolStripStatusLabel lblTestPassed;
        private System.Windows.Forms.ToolStripStatusLabel lblTestsFailed;
        private System.Windows.Forms.ToolStripStatusLabel lblAssertCount;
        private System.Windows.Forms.ToolStripStatusLabel lblAssertPassed;
        private System.Windows.Forms.ToolStripStatusLabel lblAssertsFailed;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem testSuiteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem runTestSuiteToolStripMenuItem;
        private System.Windows.Forms.ToolStripContainer toolStripContainer1;
        private System.Windows.Forms.ToolStripMenuItem keepResultsToolStripItem;
        private System.Windows.Forms.DataGridViewTextBoxColumn ResultType;
        private System.Windows.Forms.DataGridViewTextBoxColumn ExecutionTime;
        private System.Windows.Forms.DataGridViewTextBoxColumn PackageName;
        private System.Windows.Forms.DataGridViewTextBoxColumn TaskName;
        private System.Windows.Forms.DataGridViewTextBoxColumn TestName;
        private System.Windows.Forms.DataGridViewTextBoxColumn AssertResult;
        private System.Windows.Forms.DataGridViewCheckBoxColumn TestPassed;
        private System.Windows.Forms.ToolStripMenuItem refreshToolStripMenuItem;
    }
}