namespace TalosUI
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.TsBtnAbout = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.TsDropDownBtnFile = new System.Windows.Forms.ToolStripDropDownButton();
            this.loadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.TsBtnSave = new System.Windows.Forms.ToolStripMenuItem();
            this.TsBtnSaveAs = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.label1 = new System.Windows.Forms.Label();
            this.edtTargetPath = new System.Windows.Forms.TextBox();
            this.btnSelectTarget = new System.Windows.Forms.Button();
            this.btnRecord = new System.Windows.Forms.Button();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripSeparator1,
            this.TsBtnAbout,
            this.toolStripSeparator2,
            this.TsDropDownBtnFile,
            this.toolStripSeparator3});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(411, 27);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 27);
            // 
            // TsBtnAbout
            // 
            this.TsBtnAbout.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.TsBtnAbout.Image = ((System.Drawing.Image)(resources.GetObject("TsBtnAbout.Image")));
            this.TsBtnAbout.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.TsBtnAbout.Name = "TsBtnAbout";
            this.TsBtnAbout.Size = new System.Drawing.Size(54, 24);
            this.TsBtnAbout.Text = "About";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 27);
            // 
            // TsDropDownBtnFile
            // 
            this.TsDropDownBtnFile.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.TsDropDownBtnFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadToolStripMenuItem,
            this.TsBtnSave,
            this.TsBtnSaveAs});
            this.TsDropDownBtnFile.Image = ((System.Drawing.Image)(resources.GetObject("TsDropDownBtnFile.Image")));
            this.TsDropDownBtnFile.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.TsDropDownBtnFile.Name = "TsDropDownBtnFile";
            this.TsDropDownBtnFile.Size = new System.Drawing.Size(46, 24);
            this.TsDropDownBtnFile.Text = "File";
            // 
            // loadToolStripMenuItem
            // 
            this.loadToolStripMenuItem.Name = "loadToolStripMenuItem";
            this.loadToolStripMenuItem.Size = new System.Drawing.Size(144, 26);
            this.loadToolStripMenuItem.Text = "Load";
            // 
            // TsBtnSave
            // 
            this.TsBtnSave.Name = "TsBtnSave";
            this.TsBtnSave.Size = new System.Drawing.Size(144, 26);
            this.TsBtnSave.Text = "Save";
            // 
            // TsBtnSaveAs
            // 
            this.TsBtnSaveAs.Name = "TsBtnSaveAs";
            this.TsBtnSaveAs.Size = new System.Drawing.Size(144, 26);
            this.TsBtnSaveAs.Text = "Save As...";
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 27);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 48);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(77, 17);
            this.label1.TabIndex = 1;
            this.label1.Text = "Target Exe";
            // 
            // edtTargetPath
            // 
            this.edtTargetPath.Location = new System.Drawing.Point(104, 48);
            this.edtTargetPath.Name = "edtTargetPath";
            this.edtTargetPath.ReadOnly = true;
            this.edtTargetPath.Size = new System.Drawing.Size(248, 22);
            this.edtTargetPath.TabIndex = 2;
            // 
            // btnSelectTarget
            // 
            this.btnSelectTarget.Location = new System.Drawing.Point(352, 48);
            this.btnSelectTarget.Name = "btnSelectTarget";
            this.btnSelectTarget.Size = new System.Drawing.Size(32, 26);
            this.btnSelectTarget.TabIndex = 3;
            this.btnSelectTarget.Text = "...";
            this.btnSelectTarget.UseVisualStyleBackColor = true;
            this.btnSelectTarget.Click += new System.EventHandler(this.btnSelectTarget_Click);
            // 
            // btnRecord
            // 
            this.btnRecord.Location = new System.Drawing.Point(24, 80);
            this.btnRecord.Name = "btnRecord";
            this.btnRecord.Size = new System.Drawing.Size(360, 24);
            this.btnRecord.TabIndex = 4;
            this.btnRecord.Text = "Record";
            this.btnRecord.UseVisualStyleBackColor = true;
            this.btnRecord.Click += new System.EventHandler(this.btnRecord_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(411, 141);
            this.Controls.Add(this.btnRecord);
            this.Controls.Add(this.btnSelectTarget);
            this.Controls.Add(this.edtTargetPath);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.toolStrip1);
            this.Name = "MainForm";
            this.Text = "T.A.L.O.S";
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton TsBtnAbout;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripDropDownButton TsDropDownBtnFile;
        private System.Windows.Forms.ToolStripMenuItem loadToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem TsBtnSave;
        private System.Windows.Forms.ToolStripMenuItem TsBtnSaveAs;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox edtTargetPath;
        private System.Windows.Forms.Button btnSelectTarget;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.Button btnRecord;
    }
}

