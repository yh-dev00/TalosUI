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
            this.components = new System.ComponentModel.Container();
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
            this.btnStartInspect = new System.Windows.Forms.Button();
            this.btnStopInspect = new System.Windows.Forms.Button();
            this.lblUiState = new System.Windows.Forms.Label();
            this.lblUiStateValue = new System.Windows.Forms.Label();
            this.grpInspectedElement = new System.Windows.Forms.GroupBox();
            this.txtHowFound = new System.Windows.Forms.TextBox();
            this.lblHowFound = new System.Windows.Forms.Label();
            this.txtAncestorPath = new System.Windows.Forms.TextBox();
            this.lblAncestorPath = new System.Windows.Forms.Label();
            this.txtBoundingRectangle = new System.Windows.Forms.TextBox();
            this.lblBoundingRectangle = new System.Windows.Forms.Label();
            this.txtProcessId = new System.Windows.Forms.TextBox();
            this.lblProcessId = new System.Windows.Forms.Label();
            this.txtClassName = new System.Windows.Forms.TextBox();
            this.lblClassName = new System.Windows.Forms.Label();
            this.txtControlType = new System.Windows.Forms.TextBox();
            this.lblControlType = new System.Windows.Forms.Label();
            this.txtElementName = new System.Windows.Forms.TextBox();
            this.lblElementName = new System.Windows.Forms.Label();
            this.txtAutomationId = new System.Windows.Forms.TextBox();
            this.lblAutomationId = new System.Windows.Forms.Label();
            this.toolStrip1.SuspendLayout();
            this.grpInspectedElement.SuspendLayout();
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
            this.toolStrip1.Size = new System.Drawing.Size(664, 27);
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
            this.edtTargetPath.Size = new System.Drawing.Size(504, 22);
            this.edtTargetPath.TabIndex = 2;
            // 
            // btnSelectTarget
            // 
            this.btnSelectTarget.Location = new System.Drawing.Point(608, 47);
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
            this.btnRecord.Size = new System.Drawing.Size(192, 28);
            this.btnRecord.TabIndex = 4;
            this.btnRecord.Text = "Launch Target";
            this.btnRecord.UseVisualStyleBackColor = true;
            this.btnRecord.Click += new System.EventHandler(this.btnRecord_Click);
            // 
            // btnStartInspect
            // 
            this.btnStartInspect.Location = new System.Drawing.Point(224, 80);
            this.btnStartInspect.Name = "btnStartInspect";
            this.btnStartInspect.Size = new System.Drawing.Size(144, 28);
            this.btnStartInspect.TabIndex = 5;
            this.btnStartInspect.Text = "Start Inspect";
            this.btnStartInspect.UseVisualStyleBackColor = true;
            this.btnStartInspect.Click += new System.EventHandler(this.btnStartInspect_Click);
            // 
            // btnStopInspect
            // 
            this.btnStopInspect.Location = new System.Drawing.Point(376, 80);
            this.btnStopInspect.Name = "btnStopInspect";
            this.btnStopInspect.Size = new System.Drawing.Size(144, 28);
            this.btnStopInspect.TabIndex = 6;
            this.btnStopInspect.Text = "Stop Inspect";
            this.btnStopInspect.UseVisualStyleBackColor = true;
            this.btnStopInspect.Click += new System.EventHandler(this.btnStopInspect_Click);
            // 
            // lblUiState
            // 
            this.lblUiState.AutoSize = true;
            this.lblUiState.Location = new System.Drawing.Point(24, 124);
            this.lblUiState.Name = "lblUiState";
            this.lblUiState.Size = new System.Drawing.Size(41, 17);
            this.lblUiState.TabIndex = 7;
            this.lblUiState.Text = "State";
            // 
            // lblUiStateValue
            // 
            this.lblUiStateValue.AutoSize = true;
            this.lblUiStateValue.Location = new System.Drawing.Point(104, 124);
            this.lblUiStateValue.Name = "lblUiStateValue";
            this.lblUiStateValue.Size = new System.Drawing.Size(30, 17);
            this.lblUiStateValue.TabIndex = 8;
            this.lblUiStateValue.Text = "Idle";
            // 
            // grpInspectedElement
            // 
            this.grpInspectedElement.Controls.Add(this.txtHowFound);
            this.grpInspectedElement.Controls.Add(this.lblHowFound);
            this.grpInspectedElement.Controls.Add(this.txtAncestorPath);
            this.grpInspectedElement.Controls.Add(this.lblAncestorPath);
            this.grpInspectedElement.Controls.Add(this.txtBoundingRectangle);
            this.grpInspectedElement.Controls.Add(this.lblBoundingRectangle);
            this.grpInspectedElement.Controls.Add(this.txtProcessId);
            this.grpInspectedElement.Controls.Add(this.lblProcessId);
            this.grpInspectedElement.Controls.Add(this.txtClassName);
            this.grpInspectedElement.Controls.Add(this.lblClassName);
            this.grpInspectedElement.Controls.Add(this.txtControlType);
            this.grpInspectedElement.Controls.Add(this.lblControlType);
            this.grpInspectedElement.Controls.Add(this.txtElementName);
            this.grpInspectedElement.Controls.Add(this.lblElementName);
            this.grpInspectedElement.Controls.Add(this.txtAutomationId);
            this.grpInspectedElement.Controls.Add(this.lblAutomationId);
            this.grpInspectedElement.Location = new System.Drawing.Point(24, 152);
            this.grpInspectedElement.Name = "grpInspectedElement";
            this.grpInspectedElement.Size = new System.Drawing.Size(616, 392);
            this.grpInspectedElement.TabIndex = 9;
            this.grpInspectedElement.TabStop = false;
            this.grpInspectedElement.Text = "Inspected Element";
            // 
            // txtHowFound
            // 
            this.txtHowFound.Location = new System.Drawing.Point(144, 32);
            this.txtHowFound.Name = "txtHowFound";
            this.txtHowFound.ReadOnly = true;
            this.txtHowFound.Size = new System.Drawing.Size(448, 22);
            this.txtHowFound.TabIndex = 1;
            // 
            // lblHowFound
            // 
            this.lblHowFound.AutoSize = true;
            this.lblHowFound.Location = new System.Drawing.Point(16, 32);
            this.lblHowFound.Name = "lblHowFound";
            this.lblHowFound.Size = new System.Drawing.Size(75, 17);
            this.lblHowFound.TabIndex = 0;
            this.lblHowFound.Text = "How found";
            // 
            // txtAncestorPath
            // 
            this.txtAncestorPath.Location = new System.Drawing.Point(144, 256);
            this.txtAncestorPath.Multiline = true;
            this.txtAncestorPath.Name = "txtAncestorPath";
            this.txtAncestorPath.ReadOnly = true;
            this.txtAncestorPath.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtAncestorPath.Size = new System.Drawing.Size(448, 112);
            this.txtAncestorPath.TabIndex = 15;
            // 
            // lblAncestorPath
            // 
            this.lblAncestorPath.AutoSize = true;
            this.lblAncestorPath.Location = new System.Drawing.Point(16, 256);
            this.lblAncestorPath.Name = "lblAncestorPath";
            this.lblAncestorPath.Size = new System.Drawing.Size(93, 17);
            this.lblAncestorPath.TabIndex = 14;
            this.lblAncestorPath.Text = "AncestorPath";
            // 
            // txtBoundingRectangle
            // 
            this.txtBoundingRectangle.Location = new System.Drawing.Point(144, 224);
            this.txtBoundingRectangle.Name = "txtBoundingRectangle";
            this.txtBoundingRectangle.ReadOnly = true;
            this.txtBoundingRectangle.Size = new System.Drawing.Size(448, 22);
            this.txtBoundingRectangle.TabIndex = 13;
            // 
            // lblBoundingRectangle
            // 
            this.lblBoundingRectangle.AutoSize = true;
            this.lblBoundingRectangle.Location = new System.Drawing.Point(16, 224);
            this.lblBoundingRectangle.Name = "lblBoundingRectangle";
            this.lblBoundingRectangle.Size = new System.Drawing.Size(132, 17);
            this.lblBoundingRectangle.TabIndex = 12;
            this.lblBoundingRectangle.Text = "BoundingRectangle";
            // 
            // txtProcessId
            // 
            this.txtProcessId.Location = new System.Drawing.Point(144, 192);
            this.txtProcessId.Name = "txtProcessId";
            this.txtProcessId.ReadOnly = true;
            this.txtProcessId.Size = new System.Drawing.Size(448, 22);
            this.txtProcessId.TabIndex = 11;
            // 
            // lblProcessId
            // 
            this.lblProcessId.AutoSize = true;
            this.lblProcessId.Location = new System.Drawing.Point(16, 192);
            this.lblProcessId.Name = "lblProcessId";
            this.lblProcessId.Size = new System.Drawing.Size(70, 17);
            this.lblProcessId.TabIndex = 10;
            this.lblProcessId.Text = "ProcessId";
            // 
            // txtClassName
            // 
            this.txtClassName.Location = new System.Drawing.Point(144, 160);
            this.txtClassName.Name = "txtClassName";
            this.txtClassName.ReadOnly = true;
            this.txtClassName.Size = new System.Drawing.Size(448, 22);
            this.txtClassName.TabIndex = 9;
            // 
            // lblClassName
            // 
            this.lblClassName.AutoSize = true;
            this.lblClassName.Location = new System.Drawing.Point(16, 160);
            this.lblClassName.Name = "lblClassName";
            this.lblClassName.Size = new System.Drawing.Size(79, 17);
            this.lblClassName.TabIndex = 8;
            this.lblClassName.Text = "ClassName";
            // 
            // txtControlType
            // 
            this.txtControlType.Location = new System.Drawing.Point(144, 128);
            this.txtControlType.Name = "txtControlType";
            this.txtControlType.ReadOnly = true;
            this.txtControlType.Size = new System.Drawing.Size(448, 22);
            this.txtControlType.TabIndex = 7;
            // 
            // lblControlType
            // 
            this.lblControlType.AutoSize = true;
            this.lblControlType.Location = new System.Drawing.Point(16, 128);
            this.lblControlType.Name = "lblControlType";
            this.lblControlType.Size = new System.Drawing.Size(85, 17);
            this.lblControlType.TabIndex = 6;
            this.lblControlType.Text = "ControlType";
            // 
            // txtElementName
            // 
            this.txtElementName.Location = new System.Drawing.Point(144, 96);
            this.txtElementName.Name = "txtElementName";
            this.txtElementName.ReadOnly = true;
            this.txtElementName.Size = new System.Drawing.Size(448, 22);
            this.txtElementName.TabIndex = 5;
            // 
            // lblElementName
            // 
            this.lblElementName.AutoSize = true;
            this.lblElementName.Location = new System.Drawing.Point(16, 96);
            this.lblElementName.Name = "lblElementName";
            this.lblElementName.Size = new System.Drawing.Size(45, 17);
            this.lblElementName.TabIndex = 4;
            this.lblElementName.Text = "Name";
            // 
            // txtAutomationId
            // 
            this.txtAutomationId.Location = new System.Drawing.Point(144, 64);
            this.txtAutomationId.Name = "txtAutomationId";
            this.txtAutomationId.ReadOnly = true;
            this.txtAutomationId.Size = new System.Drawing.Size(448, 22);
            this.txtAutomationId.TabIndex = 3;
            // 
            // lblAutomationId
            // 
            this.lblAutomationId.AutoSize = true;
            this.lblAutomationId.Location = new System.Drawing.Point(16, 64);
            this.lblAutomationId.Name = "lblAutomationId";
            this.lblAutomationId.Size = new System.Drawing.Size(90, 17);
            this.lblAutomationId.TabIndex = 2;
            this.lblAutomationId.Text = "AutomationId";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(664, 567);
            this.Controls.Add(this.grpInspectedElement);
            this.Controls.Add(this.lblUiStateValue);
            this.Controls.Add(this.lblUiState);
            this.Controls.Add(this.btnStopInspect);
            this.Controls.Add(this.btnStartInspect);
            this.Controls.Add(this.btnRecord);
            this.Controls.Add(this.btnSelectTarget);
            this.Controls.Add(this.edtTargetPath);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.toolStrip1);
            this.Name = "MainForm";
            this.Text = "T.A.L.O.S";
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.grpInspectedElement.ResumeLayout(false);
            this.grpInspectedElement.PerformLayout();
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
        private System.Windows.Forms.Button btnStartInspect;
        private System.Windows.Forms.Button btnStopInspect;
        private System.Windows.Forms.Label lblUiState;
        private System.Windows.Forms.Label lblUiStateValue;
        private System.Windows.Forms.GroupBox grpInspectedElement;
        private System.Windows.Forms.TextBox txtHowFound;
        private System.Windows.Forms.Label lblHowFound;
        private System.Windows.Forms.TextBox txtAutomationId;
        private System.Windows.Forms.Label lblAutomationId;
        private System.Windows.Forms.TextBox txtAncestorPath;
        private System.Windows.Forms.Label lblAncestorPath;
        private System.Windows.Forms.TextBox txtBoundingRectangle;
        private System.Windows.Forms.Label lblBoundingRectangle;
        private System.Windows.Forms.TextBox txtProcessId;
        private System.Windows.Forms.Label lblProcessId;
        private System.Windows.Forms.TextBox txtClassName;
        private System.Windows.Forms.Label lblClassName;
        private System.Windows.Forms.TextBox txtControlType;
        private System.Windows.Forms.Label lblControlType;
        private System.Windows.Forms.TextBox txtElementName;
        private System.Windows.Forms.Label lblElementName;
    }
}
