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
            this.cboTests = new System.Windows.Forms.ComboBox();
            this.dlgFileOpen = new System.Windows.Forms.OpenFileDialog();
            this.btnFileOpen = new System.Windows.Forms.Button();
            this.imgList = new System.Windows.Forms.ImageList(this.components);
            this.SuspendLayout();
            // 
            // treeTest
            // 
            this.treeTest.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.treeTest.Location = new System.Drawing.Point(4, 29);
            this.treeTest.Name = "treeTest";
            this.treeTest.Size = new System.Drawing.Size(178, 366);
            this.treeTest.TabIndex = 0;
            this.treeTest.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeTest_AfterSelect);
            this.treeTest.BeforeSelect += new System.Windows.Forms.TreeViewCancelEventHandler(this.treeTest_BeforeSelect);
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
            this.cboTests.SelectionChangeCommitted += new System.EventHandler(this.cboTests_SelectionChangeCommitted);
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
            this.btnFileOpen.Click += new System.EventHandler(this.btnFileOpen_Click);
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
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView treeTest;
        private System.Windows.Forms.ComboBox cboTests;
        private System.Windows.Forms.OpenFileDialog dlgFileOpen;
        private System.Windows.Forms.Button btnFileOpen;
        private System.Windows.Forms.ImageList imgList;
    }
}
