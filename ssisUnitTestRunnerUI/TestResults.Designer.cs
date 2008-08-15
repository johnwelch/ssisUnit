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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
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
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(746, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ResultType,
            this.ExecutionTime,
            this.PackageName,
            this.TaskName,
            this.TestName,
            this.AssertResult,
            this.TestPassed});
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(0, 24);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.Size = new System.Drawing.Size(746, 257);
            this.dataGridView1.TabIndex = 1;
            this.dataGridView1.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellContentClick);
            // 
            // ResultType
            // 
            this.ResultType.HeaderText = "Type";
            this.ResultType.Name = "ResultType";
            this.ResultType.ReadOnly = true;
            // 
            // ExecutionTime
            // 
            this.ExecutionTime.HeaderText = "Executed";
            this.ExecutionTime.Name = "ExecutionTime";
            this.ExecutionTime.ReadOnly = true;
            // 
            // PackageName
            // 
            this.PackageName.HeaderText = "Package Name";
            this.PackageName.Name = "PackageName";
            this.PackageName.ReadOnly = true;
            // 
            // TaskName
            // 
            this.TaskName.HeaderText = "Task Name";
            this.TaskName.Name = "TaskName";
            this.TaskName.ReadOnly = true;
            // 
            // TestName
            // 
            this.TestName.HeaderText = "Test Name";
            this.TestName.Name = "TestName";
            this.TestName.ReadOnly = true;
            // 
            // AssertResult
            // 
            this.AssertResult.HeaderText = "Result";
            this.AssertResult.Name = "AssertResult";
            this.AssertResult.ReadOnly = true;
            // 
            // TestPassed
            // 
            this.TestPassed.HeaderText = "Passed";
            this.TestPassed.Name = "TestPassed";
            this.TestPassed.ReadOnly = true;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblTestCount,
            this.lblTestPassed,
            this.lblTestsFailed,
            this.lblAssertCount,
            this.lblAssertPassed,
            this.lblAssertsFailed});
            this.statusStrip1.Location = new System.Drawing.Point(0, 259);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(746, 22);
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
            // TestResults
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(746, 281);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "TestResults";
            this.Text = "TestResults";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DataGridViewTextBoxColumn ResultType;
        private System.Windows.Forms.DataGridViewTextBoxColumn ExecutionTime;
        private System.Windows.Forms.DataGridViewTextBoxColumn PackageName;
        private System.Windows.Forms.DataGridViewTextBoxColumn TaskName;
        private System.Windows.Forms.DataGridViewTextBoxColumn TestName;
        private System.Windows.Forms.DataGridViewTextBoxColumn AssertResult;
        private System.Windows.Forms.DataGridViewCheckBoxColumn TestPassed;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel lblTestCount;
        private System.Windows.Forms.ToolStripStatusLabel lblTestPassed;
        private System.Windows.Forms.ToolStripStatusLabel lblTestsFailed;
        private System.Windows.Forms.ToolStripStatusLabel lblAssertCount;
        private System.Windows.Forms.ToolStripStatusLabel lblAssertPassed;
        private System.Windows.Forms.ToolStripStatusLabel lblAssertsFailed;
    }
}