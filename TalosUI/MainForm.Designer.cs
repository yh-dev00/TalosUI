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
            this.btnStartRecord = new System.Windows.Forms.Button();
            this.btnStopRecord = new System.Windows.Forms.Button();
            this.grpRecordedSteps = new System.Windows.Forms.GroupBox();
            this.txtTestCaseDescription = new System.Windows.Forms.TextBox();
            this.lblTestDescription = new System.Windows.Forms.Label();
            this.btnDuplicateTestCase = new System.Windows.Forms.Button();
            this.btnSaveTestCase = new System.Windows.Forms.Button();
            this.btnRemoveTestCase = new System.Windows.Forms.Button();
            this.btnAddTestCase = new System.Windows.Forms.Button();
            this.txtTestCaseName = new System.Windows.Forms.TextBox();
            this.listBoxTestCase = new System.Windows.Forms.ListBox();
            this.lblCurrentTestCase = new System.Windows.Forms.Label();
            this.btnMoveStepUp = new System.Windows.Forms.Button();
            this.btnMoveStepDown = new System.Windows.Forms.Button();
            this.grdRecordedSteps = new System.Windows.Forms.DataGridView();
            this.Id = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Action = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.Target = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ControlType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.FixedDelayMs = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SetValueText = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SendKeysText = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SelectedItem = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lblSuiteName = new System.Windows.Forms.Label();
            this.txtSuiteName = new System.Windows.Forms.TextBox();
            this.lblLaunchParams = new System.Windows.Forms.Label();
            this.txtLaunchParams = new System.Windows.Forms.TextBox();
            this.lblDefaultDelay = new System.Windows.Forms.Label();
            this.numDefaultFixedDelay = new System.Windows.Forms.NumericUpDown();
            this.lblDefaultDelaySuffix = new System.Windows.Forms.Label();
            this.chkAutoRelaunch = new System.Windows.Forms.CheckBox();
            this.grpConditions = new System.Windows.Forms.GroupBox();
            this.lblValueChecks = new System.Windows.Forms.Label();
            this.btnAddValueCheck = new System.Windows.Forms.Button();
            this.btnRemoveValueCheck = new System.Windows.Forms.Button();
            this.grdValueChecks = new System.Windows.Forms.DataGridView();
            this.ValueCheckId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ValueTarget = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ExpectedValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ValueControlType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lblExpectedWindows = new System.Windows.Forms.Label();
            this.btnAddExpectedWindow = new System.Windows.Forms.Button();
            this.btnRemoveExpectedWindow = new System.Windows.Forms.Button();
            this.grdExpectedWindows = new System.Windows.Forms.DataGridView();
            this.ExpectedWindowId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ExpectedPattern = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ExpectedMatchType = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.MustAppearAtLeastOnce = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.lblForbiddenWindows = new System.Windows.Forms.Label();
            this.btnAddForbiddenWindow = new System.Windows.Forms.Button();
            this.btnRemoveForbiddenWindow = new System.Windows.Forms.Button();
            this.grdForbiddenWindows = new System.Windows.Forms.DataGridView();
            this.ForbiddenWindowId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ForbiddenPattern = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ForbiddenMatchType = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.toolStrip1.SuspendLayout();
            this.grpInspectedElement.SuspendLayout();
            this.grpRecordedSteps.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdRecordedSteps)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numDefaultFixedDelay)).BeginInit();
            this.grpConditions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdValueChecks)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdExpectedWindows)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdForbiddenWindows)).BeginInit();
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
            this.toolStrip1.Size = new System.Drawing.Size(1322, 27);
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
            this.loadToolStripMenuItem.Click += new System.EventHandler(this.loadToolStripMenuItem_Click);
            // 
            // TsBtnSave
            // 
            this.TsBtnSave.Name = "TsBtnSave";
            this.TsBtnSave.Size = new System.Drawing.Size(144, 26);
            this.TsBtnSave.Text = "Save";
            this.TsBtnSave.Click += new System.EventHandler(this.TsBtnSave_Click);
            // 
            // TsBtnSaveAs
            // 
            this.TsBtnSaveAs.Name = "TsBtnSaveAs";
            this.TsBtnSaveAs.Size = new System.Drawing.Size(144, 26);
            this.TsBtnSaveAs.Text = "Save As...";
            this.TsBtnSaveAs.Click += new System.EventHandler(this.TsBtnSaveAs_Click);
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
            this.edtTargetPath.Size = new System.Drawing.Size(504, 22);
            this.edtTargetPath.TabIndex = 2;
            this.edtTargetPath.TextChanged += new System.EventHandler(this.SuiteMetadataChanged);
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
            this.grpInspectedElement.Size = new System.Drawing.Size(1000, 392);
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
            this.txtAncestorPath.Size = new System.Drawing.Size(832, 112);
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
            // btnStartRecord
            // 
            this.btnStartRecord.Location = new System.Drawing.Point(528, 80);
            this.btnStartRecord.Name = "btnStartRecord";
            this.btnStartRecord.Size = new System.Drawing.Size(112, 28);
            this.btnStartRecord.TabIndex = 10;
            this.btnStartRecord.Text = "Start Record";
            this.btnStartRecord.UseVisualStyleBackColor = true;
            this.btnStartRecord.Click += new System.EventHandler(this.btnStartRecord_Click);
            // 
            // btnStopRecord
            // 
            this.btnStopRecord.Location = new System.Drawing.Point(648, 80);
            this.btnStopRecord.Name = "btnStopRecord";
            this.btnStopRecord.Size = new System.Drawing.Size(112, 28);
            this.btnStopRecord.TabIndex = 11;
            this.btnStopRecord.Text = "Stop Record";
            this.btnStopRecord.UseVisualStyleBackColor = true;
            this.btnStopRecord.Click += new System.EventHandler(this.btnStopRecord_Click);
            // 
            // grpRecordedSteps
            // 
            this.grpRecordedSteps.Controls.Add(this.txtTestCaseDescription);
            this.grpRecordedSteps.Controls.Add(this.lblTestDescription);
            this.grpRecordedSteps.Controls.Add(this.btnDuplicateTestCase);
            this.grpRecordedSteps.Controls.Add(this.btnSaveTestCase);
            this.grpRecordedSteps.Controls.Add(this.btnRemoveTestCase);
            this.grpRecordedSteps.Controls.Add(this.btnAddTestCase);
            this.grpRecordedSteps.Controls.Add(this.txtTestCaseName);
            this.grpRecordedSteps.Controls.Add(this.listBoxTestCase);
            this.grpRecordedSteps.Controls.Add(this.lblCurrentTestCase);
            this.grpRecordedSteps.Controls.Add(this.btnMoveStepUp);
            this.grpRecordedSteps.Controls.Add(this.btnMoveStepDown);
            this.grpRecordedSteps.Controls.Add(this.grdRecordedSteps);
            this.grpRecordedSteps.Location = new System.Drawing.Point(24, 552);
            this.grpRecordedSteps.Name = "grpRecordedSteps";
            this.grpRecordedSteps.Size = new System.Drawing.Size(1128, 356);
            this.grpRecordedSteps.TabIndex = 12;
            this.grpRecordedSteps.TabStop = false;
            this.grpRecordedSteps.Text = "Test Cases and Steps";
            // 
            // txtTestCaseDescription
            // 
            this.txtTestCaseDescription.Location = new System.Drawing.Point(288, 56);
            this.txtTestCaseDescription.Multiline = true;
            this.txtTestCaseDescription.Name = "txtTestCaseDescription";
            this.txtTestCaseDescription.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtTestCaseDescription.Size = new System.Drawing.Size(784, 48);
            this.txtTestCaseDescription.TabIndex = 11;
            this.txtTestCaseDescription.TextChanged += new System.EventHandler(this.txtTestCaseDescription_TextChanged);
            // 
            // lblTestDescription
            // 
            this.lblTestDescription.AutoSize = true;
            this.lblTestDescription.Location = new System.Drawing.Point(200, 58);
            this.lblTestDescription.Name = "lblTestDescription";
            this.lblTestDescription.Size = new System.Drawing.Size(79, 17);
            this.lblTestDescription.TabIndex = 10;
            this.lblTestDescription.Text = "Description";
            // 
            // btnDuplicateTestCase
            // 
            this.btnDuplicateTestCase.Location = new System.Drawing.Point(784, 22);
            this.btnDuplicateTestCase.Name = "btnDuplicateTestCase";
            this.btnDuplicateTestCase.Size = new System.Drawing.Size(80, 28);
            this.btnDuplicateTestCase.TabIndex = 9;
            this.btnDuplicateTestCase.Text = "Duplicate";
            this.btnDuplicateTestCase.UseVisualStyleBackColor = true;
            this.btnDuplicateTestCase.Click += new System.EventHandler(this.btnDuplicateTestCase_Click);
            // 
            // btnSaveTestCase
            // 
            this.btnSaveTestCase.Location = new System.Drawing.Point(704, 22);
            this.btnSaveTestCase.Name = "btnSaveTestCase";
            this.btnSaveTestCase.Size = new System.Drawing.Size(72, 28);
            this.btnSaveTestCase.TabIndex = 4;
            this.btnSaveTestCase.Text = "Save";
            this.btnSaveTestCase.UseVisualStyleBackColor = true;
            this.btnSaveTestCase.Click += new System.EventHandler(this.btnSaveTestCase_Click);
            // 
            // btnRemoveTestCase
            // 
            this.btnRemoveTestCase.Location = new System.Drawing.Point(608, 22);
            this.btnRemoveTestCase.Name = "btnRemoveTestCase";
            this.btnRemoveTestCase.Size = new System.Drawing.Size(88, 28);
            this.btnRemoveTestCase.TabIndex = 3;
            this.btnRemoveTestCase.Text = "Remove";
            this.btnRemoveTestCase.UseVisualStyleBackColor = true;
            this.btnRemoveTestCase.Click += new System.EventHandler(this.btnRemoveTestCase_Click);
            // 
            // btnAddTestCase
            // 
            this.btnAddTestCase.Location = new System.Drawing.Point(528, 22);
            this.btnAddTestCase.Name = "btnAddTestCase";
            this.btnAddTestCase.Size = new System.Drawing.Size(72, 28);
            this.btnAddTestCase.TabIndex = 2;
            this.btnAddTestCase.Text = "Add";
            this.btnAddTestCase.UseVisualStyleBackColor = true;
            this.btnAddTestCase.Click += new System.EventHandler(this.btnAddTestCase_Click);
            // 
            // txtTestCaseName
            // 
            this.txtTestCaseName.Location = new System.Drawing.Point(200, 24);
            this.txtTestCaseName.Name = "txtTestCaseName";
            this.txtTestCaseName.Size = new System.Drawing.Size(312, 22);
            this.txtTestCaseName.TabIndex = 1;
            this.txtTestCaseName.TextChanged += new System.EventHandler(this.txtTestCaseName_TextChanged);
            // 
            // listBoxTestCase
            // 
            this.listBoxTestCase.FormattingEnabled = true;
            this.listBoxTestCase.ItemHeight = 16;
            this.listBoxTestCase.Location = new System.Drawing.Point(16, 56);
            this.listBoxTestCase.Name = "listBoxTestCase";
            this.listBoxTestCase.Size = new System.Drawing.Size(168, 276);
            this.listBoxTestCase.TabIndex = 5;
            this.listBoxTestCase.SelectedIndexChanged += new System.EventHandler(this.listBoxTestCase_SelectedIndexChanged);
            // 
            // lblCurrentTestCase
            // 
            this.lblCurrentTestCase.AutoSize = true;
            this.lblCurrentTestCase.Location = new System.Drawing.Point(16, 28);
            this.lblCurrentTestCase.Name = "lblCurrentTestCase";
            this.lblCurrentTestCase.Size = new System.Drawing.Size(79, 17);
            this.lblCurrentTestCase.TabIndex = 0;
            this.lblCurrentTestCase.Text = "Test Cases";
            // 
            // btnMoveStepUp
            // 
            this.btnMoveStepUp.Location = new System.Drawing.Point(872, 22);
            this.btnMoveStepUp.Name = "btnMoveStepUp";
            this.btnMoveStepUp.Size = new System.Drawing.Size(104, 28);
            this.btnMoveStepUp.TabIndex = 6;
            this.btnMoveStepUp.Text = "Move Up";
            this.btnMoveStepUp.UseVisualStyleBackColor = true;
            this.btnMoveStepUp.Click += new System.EventHandler(this.btnMoveStepUp_Click);
            // 
            // btnMoveStepDown
            // 
            this.btnMoveStepDown.Location = new System.Drawing.Point(984, 22);
            this.btnMoveStepDown.Name = "btnMoveStepDown";
            this.btnMoveStepDown.Size = new System.Drawing.Size(112, 28);
            this.btnMoveStepDown.TabIndex = 7;
            this.btnMoveStepDown.Text = "Move Down";
            this.btnMoveStepDown.UseVisualStyleBackColor = true;
            this.btnMoveStepDown.Click += new System.EventHandler(this.btnMoveStepDown_Click);
            // 
            // grdRecordedSteps
            // 
            this.grdRecordedSteps.AllowUserToAddRows = false;
            this.grdRecordedSteps.AllowUserToDeleteRows = false;
            this.grdRecordedSteps.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grdRecordedSteps.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Id,
            this.Action,
            this.Target,
            this.ControlType,
            this.FixedDelayMs,
            this.SetValueText,
            this.SendKeysText,
            this.SelectedItem});
            this.grdRecordedSteps.Location = new System.Drawing.Point(200, 116);
            this.grdRecordedSteps.MultiSelect = false;
            this.grdRecordedSteps.Name = "grdRecordedSteps";
            this.grdRecordedSteps.RowHeadersVisible = false;
            this.grdRecordedSteps.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.grdRecordedSteps.Size = new System.Drawing.Size(872, 216);
            this.grdRecordedSteps.TabIndex = 8;
            this.grdRecordedSteps.CellValidating += new System.Windows.Forms.DataGridViewCellValidatingEventHandler(this.grdRecordedSteps_CellValidating);
            this.grdRecordedSteps.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.grdRecordedSteps_CellValueChanged);
            this.grdRecordedSteps.CurrentCellDirtyStateChanged += new System.EventHandler(this.grdRecordedSteps_CurrentCellDirtyStateChanged);
            this.grdRecordedSteps.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.grdRecordedSteps_DataError);
            // 
            // Id
            // 
            this.Id.HeaderText = "Id";
            this.Id.Name = "Id";
            this.Id.ReadOnly = true;
            this.Id.Width = 42;
            // 
            // Action
            // 
            this.Action.HeaderText = "Action";
            this.Action.Items.AddRange(new object[] {
            "Invoke",
            "SetValue",
            "SelectItem",
            "SendKeys"});
            this.Action.Name = "Action";
            this.Action.Width = 96;
            // 
            // Target
            // 
            this.Target.HeaderText = "Target";
            this.Target.Name = "Target";
            this.Target.ReadOnly = true;
            this.Target.Width = 210;
            // 
            // ControlType
            // 
            this.ControlType.HeaderText = "Control";
            this.ControlType.Name = "ControlType";
            this.ControlType.ReadOnly = true;
            this.ControlType.Width = 90;
            // 
            // FixedDelayMs
            // 
            this.FixedDelayMs.HeaderText = "Delay ms";
            this.FixedDelayMs.Name = "FixedDelayMs";
            this.FixedDelayMs.Width = 76;
            // 
            // SetValueText
            // 
            this.SetValueText.HeaderText = "SetValue Text";
            this.SetValueText.Name = "SetValueText";
            this.SetValueText.Width = 130;
            // 
            // SendKeysText
            // 
            this.SendKeysText.HeaderText = "SendKeys";
            this.SendKeysText.Name = "SendKeysText";
            this.SendKeysText.Width = 120;
            // 
            // SelectedItem
            // 
            this.SelectedItem.HeaderText = "Selected Item";
            this.SelectedItem.Name = "SelectedItem";
            this.SelectedItem.Width = 136;
            // 
            // lblSuiteName
            // 
            this.lblSuiteName.AutoSize = true;
            this.lblSuiteName.Location = new System.Drawing.Point(776, 48);
            this.lblSuiteName.Name = "lblSuiteName";
            this.lblSuiteName.Size = new System.Drawing.Size(81, 17);
            this.lblSuiteName.TabIndex = 13;
            this.lblSuiteName.Text = "Suite Name";
            // 
            // txtSuiteName
            // 
            this.txtSuiteName.Location = new System.Drawing.Point(880, 45);
            this.txtSuiteName.Name = "txtSuiteName";
            this.txtSuiteName.Size = new System.Drawing.Size(272, 22);
            this.txtSuiteName.TabIndex = 14;
            this.txtSuiteName.TextChanged += new System.EventHandler(this.SuiteMetadataChanged);
            // 
            // lblLaunchParams
            // 
            this.lblLaunchParams.AutoSize = true;
            this.lblLaunchParams.Location = new System.Drawing.Point(776, 80);
            this.lblLaunchParams.Name = "lblLaunchParams";
            this.lblLaunchParams.Size = new System.Drawing.Size(88, 17);
            this.lblLaunchParams.TabIndex = 15;
            this.lblLaunchParams.Text = "Launch Args";
            // 
            // txtLaunchParams
            // 
            this.txtLaunchParams.Location = new System.Drawing.Point(880, 77);
            this.txtLaunchParams.Name = "txtLaunchParams";
            this.txtLaunchParams.Size = new System.Drawing.Size(272, 22);
            this.txtLaunchParams.TabIndex = 16;
            this.txtLaunchParams.TextChanged += new System.EventHandler(this.SuiteMetadataChanged);
            // 
            // lblDefaultDelay
            // 
            this.lblDefaultDelay.AutoSize = true;
            this.lblDefaultDelay.Location = new System.Drawing.Point(776, 112);
            this.lblDefaultDelay.Name = "lblDefaultDelay";
            this.lblDefaultDelay.Size = new System.Drawing.Size(93, 17);
            this.lblDefaultDelay.TabIndex = 17;
            this.lblDefaultDelay.Text = "Default Delay";
            // 
            // numDefaultFixedDelay
            // 
            this.numDefaultFixedDelay.Location = new System.Drawing.Point(880, 109);
            this.numDefaultFixedDelay.Maximum = new decimal(new int[] {
            600000,
            0,
            0,
            0});
            this.numDefaultFixedDelay.Name = "numDefaultFixedDelay";
            this.numDefaultFixedDelay.Size = new System.Drawing.Size(96, 22);
            this.numDefaultFixedDelay.TabIndex = 18;
            this.numDefaultFixedDelay.ValueChanged += new System.EventHandler(this.SuiteMetadataChanged);
            // 
            // lblDefaultDelaySuffix
            // 
            this.lblDefaultDelaySuffix.AutoSize = true;
            this.lblDefaultDelaySuffix.Location = new System.Drawing.Point(984, 112);
            this.lblDefaultDelaySuffix.Name = "lblDefaultDelaySuffix";
            this.lblDefaultDelaySuffix.Size = new System.Drawing.Size(26, 17);
            this.lblDefaultDelaySuffix.TabIndex = 19;
            this.lblDefaultDelaySuffix.Text = "ms";
            // 
            // chkAutoRelaunch
            // 
            this.chkAutoRelaunch.AutoSize = true;
            this.chkAutoRelaunch.Location = new System.Drawing.Point(1032, 111);
            this.chkAutoRelaunch.Name = "chkAutoRelaunch";
            this.chkAutoRelaunch.Size = new System.Drawing.Size(90, 21);
            this.chkAutoRelaunch.TabIndex = 20;
            this.chkAutoRelaunch.Text = "Relaunch";
            this.chkAutoRelaunch.UseVisualStyleBackColor = true;
            this.chkAutoRelaunch.CheckedChanged += new System.EventHandler(this.SuiteMetadataChanged);
            // 
            // grpConditions
            // 
            this.grpConditions.Controls.Add(this.lblValueChecks);
            this.grpConditions.Controls.Add(this.btnAddValueCheck);
            this.grpConditions.Controls.Add(this.btnRemoveValueCheck);
            this.grpConditions.Controls.Add(this.grdValueChecks);
            this.grpConditions.Controls.Add(this.lblExpectedWindows);
            this.grpConditions.Controls.Add(this.btnAddExpectedWindow);
            this.grpConditions.Controls.Add(this.btnRemoveExpectedWindow);
            this.grpConditions.Controls.Add(this.grdExpectedWindows);
            this.grpConditions.Controls.Add(this.lblForbiddenWindows);
            this.grpConditions.Controls.Add(this.btnAddForbiddenWindow);
            this.grpConditions.Controls.Add(this.btnRemoveForbiddenWindow);
            this.grpConditions.Controls.Add(this.grdForbiddenWindows);
            this.grpConditions.Location = new System.Drawing.Point(24, 916);
            this.grpConditions.Name = "grpConditions";
            this.grpConditions.Size = new System.Drawing.Size(1128, 280);
            this.grpConditions.TabIndex = 21;
            this.grpConditions.TabStop = false;
            this.grpConditions.Text = "Conditions";
            // 
            // lblValueChecks
            // 
            this.lblValueChecks.AutoSize = true;
            this.lblValueChecks.Location = new System.Drawing.Point(16, 28);
            this.lblValueChecks.Name = "lblValueChecks";
            this.lblValueChecks.Size = new System.Drawing.Size(94, 17);
            this.lblValueChecks.TabIndex = 0;
            this.lblValueChecks.Text = "Value Checks";
            // 
            // btnAddValueCheck
            // 
            this.btnAddValueCheck.Location = new System.Drawing.Point(120, 23);
            this.btnAddValueCheck.Name = "btnAddValueCheck";
            this.btnAddValueCheck.Size = new System.Drawing.Size(112, 28);
            this.btnAddValueCheck.TabIndex = 1;
            this.btnAddValueCheck.Text = "Add Inspected";
            this.btnAddValueCheck.UseVisualStyleBackColor = true;
            this.btnAddValueCheck.Click += new System.EventHandler(this.btnAddValueCheck_Click);
            // 
            // btnRemoveValueCheck
            // 
            this.btnRemoveValueCheck.Location = new System.Drawing.Point(240, 23);
            this.btnRemoveValueCheck.Name = "btnRemoveValueCheck";
            this.btnRemoveValueCheck.Size = new System.Drawing.Size(72, 28);
            this.btnRemoveValueCheck.TabIndex = 2;
            this.btnRemoveValueCheck.Text = "Remove";
            this.btnRemoveValueCheck.UseVisualStyleBackColor = true;
            this.btnRemoveValueCheck.Click += new System.EventHandler(this.btnRemoveValueCheck_Click);
            // 
            // grdValueChecks
            // 
            this.grdValueChecks.AllowUserToAddRows = false;
            this.grdValueChecks.AllowUserToDeleteRows = false;
            this.grdValueChecks.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grdValueChecks.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ValueCheckId,
            this.ValueTarget,
            this.ExpectedValue,
            this.ValueControlType});
            this.grdValueChecks.Location = new System.Drawing.Point(16, 56);
            this.grdValueChecks.MultiSelect = false;
            this.grdValueChecks.Name = "grdValueChecks";
            this.grdValueChecks.RowHeadersVisible = false;
            this.grdValueChecks.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.grdValueChecks.Size = new System.Drawing.Size(344, 200);
            this.grdValueChecks.TabIndex = 3;
            this.grdValueChecks.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.grdValueChecks_CellValueChanged);
            this.grdValueChecks.CurrentCellDirtyStateChanged += new System.EventHandler(this.conditionGrid_CurrentCellDirtyStateChanged);
            this.grdValueChecks.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.conditionGrid_DataError);
            // 
            // ValueCheckId
            // 
            this.ValueCheckId.HeaderText = "Id";
            this.ValueCheckId.Name = "ValueCheckId";
            this.ValueCheckId.Width = 70;
            // 
            // ValueTarget
            // 
            this.ValueTarget.HeaderText = "Target";
            this.ValueTarget.Name = "ValueTarget";
            this.ValueTarget.ReadOnly = true;
            this.ValueTarget.Width = 120;
            // 
            // ExpectedValue
            // 
            this.ExpectedValue.HeaderText = "Expected";
            this.ExpectedValue.Name = "ExpectedValue";
            // 
            // ValueControlType
            // 
            this.ValueControlType.HeaderText = "Control";
            this.ValueControlType.Name = "ValueControlType";
            this.ValueControlType.ReadOnly = true;
            this.ValueControlType.Width = 70;
            // 
            // lblExpectedWindows
            // 
            this.lblExpectedWindows.AutoSize = true;
            this.lblExpectedWindows.Location = new System.Drawing.Point(384, 28);
            this.lblExpectedWindows.Name = "lblExpectedWindows";
            this.lblExpectedWindows.Size = new System.Drawing.Size(126, 17);
            this.lblExpectedWindows.TabIndex = 4;
            this.lblExpectedWindows.Text = "Expected Windows";
            // 
            // btnAddExpectedWindow
            // 
            this.btnAddExpectedWindow.Location = new System.Drawing.Point(512, 23);
            this.btnAddExpectedWindow.Name = "btnAddExpectedWindow";
            this.btnAddExpectedWindow.Size = new System.Drawing.Size(56, 28);
            this.btnAddExpectedWindow.TabIndex = 5;
            this.btnAddExpectedWindow.Text = "Add";
            this.btnAddExpectedWindow.UseVisualStyleBackColor = true;
            this.btnAddExpectedWindow.Click += new System.EventHandler(this.btnAddExpectedWindow_Click);
            // 
            // btnRemoveExpectedWindow
            // 
            this.btnRemoveExpectedWindow.Location = new System.Drawing.Point(576, 23);
            this.btnRemoveExpectedWindow.Name = "btnRemoveExpectedWindow";
            this.btnRemoveExpectedWindow.Size = new System.Drawing.Size(72, 28);
            this.btnRemoveExpectedWindow.TabIndex = 6;
            this.btnRemoveExpectedWindow.Text = "Remove";
            this.btnRemoveExpectedWindow.UseVisualStyleBackColor = true;
            this.btnRemoveExpectedWindow.Click += new System.EventHandler(this.btnRemoveExpectedWindow_Click);
            // 
            // grdExpectedWindows
            // 
            this.grdExpectedWindows.AllowUserToAddRows = false;
            this.grdExpectedWindows.AllowUserToDeleteRows = false;
            this.grdExpectedWindows.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grdExpectedWindows.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ExpectedWindowId,
            this.ExpectedPattern,
            this.ExpectedMatchType,
            this.MustAppearAtLeastOnce});
            this.grdExpectedWindows.Location = new System.Drawing.Point(384, 56);
            this.grdExpectedWindows.MultiSelect = false;
            this.grdExpectedWindows.Name = "grdExpectedWindows";
            this.grdExpectedWindows.RowHeadersVisible = false;
            this.grdExpectedWindows.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.grdExpectedWindows.Size = new System.Drawing.Size(344, 200);
            this.grdExpectedWindows.TabIndex = 7;
            this.grdExpectedWindows.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.grdExpectedWindows_CellValueChanged);
            this.grdExpectedWindows.CurrentCellDirtyStateChanged += new System.EventHandler(this.conditionGrid_CurrentCellDirtyStateChanged);
            this.grdExpectedWindows.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.conditionGrid_DataError);
            // 
            // ExpectedWindowId
            // 
            this.ExpectedWindowId.HeaderText = "Id";
            this.ExpectedWindowId.Name = "ExpectedWindowId";
            this.ExpectedWindowId.Width = 70;
            // 
            // ExpectedPattern
            // 
            this.ExpectedPattern.HeaderText = "Pattern";
            this.ExpectedPattern.Name = "ExpectedPattern";
            this.ExpectedPattern.Width = 128;
            // 
            // ExpectedMatchType
            // 
            this.ExpectedMatchType.HeaderText = "Match";
            this.ExpectedMatchType.Items.AddRange(new object[] {
            "Substring",
            "Regex"});
            this.ExpectedMatchType.Name = "ExpectedMatchType";
            this.ExpectedMatchType.Width = 82;
            // 
            // MustAppearAtLeastOnce
            // 
            this.MustAppearAtLeastOnce.HeaderText = "Once";
            this.MustAppearAtLeastOnce.Name = "MustAppearAtLeastOnce";
            this.MustAppearAtLeastOnce.Width = 48;
            // 
            // lblForbiddenWindows
            // 
            this.lblForbiddenWindows.AutoSize = true;
            this.lblForbiddenWindows.Location = new System.Drawing.Point(752, 28);
            this.lblForbiddenWindows.Name = "lblForbiddenWindows";
            this.lblForbiddenWindows.Size = new System.Drawing.Size(132, 17);
            this.lblForbiddenWindows.TabIndex = 8;
            this.lblForbiddenWindows.Text = "Forbidden Windows";
            // 
            // btnAddForbiddenWindow
            // 
            this.btnAddForbiddenWindow.Location = new System.Drawing.Point(880, 23);
            this.btnAddForbiddenWindow.Name = "btnAddForbiddenWindow";
            this.btnAddForbiddenWindow.Size = new System.Drawing.Size(56, 28);
            this.btnAddForbiddenWindow.TabIndex = 9;
            this.btnAddForbiddenWindow.Text = "Add";
            this.btnAddForbiddenWindow.UseVisualStyleBackColor = true;
            this.btnAddForbiddenWindow.Click += new System.EventHandler(this.btnAddForbiddenWindow_Click);
            // 
            // btnRemoveForbiddenWindow
            // 
            this.btnRemoveForbiddenWindow.Location = new System.Drawing.Point(944, 23);
            this.btnRemoveForbiddenWindow.Name = "btnRemoveForbiddenWindow";
            this.btnRemoveForbiddenWindow.Size = new System.Drawing.Size(72, 28);
            this.btnRemoveForbiddenWindow.TabIndex = 10;
            this.btnRemoveForbiddenWindow.Text = "Remove";
            this.btnRemoveForbiddenWindow.UseVisualStyleBackColor = true;
            this.btnRemoveForbiddenWindow.Click += new System.EventHandler(this.btnRemoveForbiddenWindow_Click);
            // 
            // grdForbiddenWindows
            // 
            this.grdForbiddenWindows.AllowUserToAddRows = false;
            this.grdForbiddenWindows.AllowUserToDeleteRows = false;
            this.grdForbiddenWindows.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grdForbiddenWindows.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ForbiddenWindowId,
            this.ForbiddenPattern,
            this.ForbiddenMatchType});
            this.grdForbiddenWindows.Location = new System.Drawing.Point(752, 56);
            this.grdForbiddenWindows.MultiSelect = false;
            this.grdForbiddenWindows.Name = "grdForbiddenWindows";
            this.grdForbiddenWindows.RowHeadersVisible = false;
            this.grdForbiddenWindows.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.grdForbiddenWindows.Size = new System.Drawing.Size(344, 200);
            this.grdForbiddenWindows.TabIndex = 11;
            this.grdForbiddenWindows.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.grdForbiddenWindows_CellValueChanged);
            this.grdForbiddenWindows.CurrentCellDirtyStateChanged += new System.EventHandler(this.conditionGrid_CurrentCellDirtyStateChanged);
            this.grdForbiddenWindows.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.conditionGrid_DataError);
            // 
            // ForbiddenWindowId
            // 
            this.ForbiddenWindowId.HeaderText = "Id";
            this.ForbiddenWindowId.Name = "ForbiddenWindowId";
            this.ForbiddenWindowId.Width = 70;
            // 
            // ForbiddenPattern
            // 
            this.ForbiddenPattern.HeaderText = "Pattern";
            this.ForbiddenPattern.Name = "ForbiddenPattern";
            this.ForbiddenPattern.Width = 168;
            // 
            // ForbiddenMatchType
            // 
            this.ForbiddenMatchType.HeaderText = "Match";
            this.ForbiddenMatchType.Items.AddRange(new object[] {
            "Substring",
            "Regex"});
            this.ForbiddenMatchType.Name = "ForbiddenMatchType";
            this.ForbiddenMatchType.Width = 82;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(1343, 1055);
            this.Controls.Add(this.grpConditions);
            this.Controls.Add(this.chkAutoRelaunch);
            this.Controls.Add(this.lblDefaultDelaySuffix);
            this.Controls.Add(this.numDefaultFixedDelay);
            this.Controls.Add(this.lblDefaultDelay);
            this.Controls.Add(this.txtLaunchParams);
            this.Controls.Add(this.lblLaunchParams);
            this.Controls.Add(this.txtSuiteName);
            this.Controls.Add(this.lblSuiteName);
            this.Controls.Add(this.grpRecordedSteps);
            this.Controls.Add(this.btnStopRecord);
            this.Controls.Add(this.btnStartRecord);
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
            this.MinimumSize = new System.Drawing.Size(850, 650);
            this.Name = "MainForm";
            this.Text = "T.A.L.O.S";
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.grpInspectedElement.ResumeLayout(false);
            this.grpInspectedElement.PerformLayout();
            this.grpRecordedSteps.ResumeLayout(false);
            this.grpRecordedSteps.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdRecordedSteps)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numDefaultFixedDelay)).EndInit();
            this.grpConditions.ResumeLayout(false);
            this.grpConditions.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdValueChecks)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdExpectedWindows)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdForbiddenWindows)).EndInit();
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
        private System.Windows.Forms.Button btnStartRecord;
        private System.Windows.Forms.Button btnStopRecord;
        private System.Windows.Forms.GroupBox grpRecordedSteps;
        private System.Windows.Forms.Label lblCurrentTestCase;
        private System.Windows.Forms.TextBox txtTestCaseName;
        private System.Windows.Forms.Button btnAddTestCase;
        private System.Windows.Forms.Button btnRemoveTestCase;
        private System.Windows.Forms.Button btnSaveTestCase;
        private System.Windows.Forms.Button btnMoveStepUp;
        private System.Windows.Forms.Button btnMoveStepDown;
        private System.Windows.Forms.DataGridView grdRecordedSteps;
        private System.Windows.Forms.ListBox listBoxTestCase;
        private System.Windows.Forms.DataGridViewTextBoxColumn Id;
        private System.Windows.Forms.DataGridViewComboBoxColumn Action;
        private System.Windows.Forms.DataGridViewTextBoxColumn Target;
        private System.Windows.Forms.DataGridViewTextBoxColumn ControlType;
        private System.Windows.Forms.DataGridViewTextBoxColumn FixedDelayMs;
        private System.Windows.Forms.DataGridViewTextBoxColumn SetValueText;
        private System.Windows.Forms.DataGridViewTextBoxColumn SendKeysText;
        private System.Windows.Forms.DataGridViewTextBoxColumn SelectedItem;
        private System.Windows.Forms.Label lblSuiteName;
        private System.Windows.Forms.TextBox txtSuiteName;
        private System.Windows.Forms.Label lblLaunchParams;
        private System.Windows.Forms.TextBox txtLaunchParams;
        private System.Windows.Forms.Label lblDefaultDelay;
        private System.Windows.Forms.NumericUpDown numDefaultFixedDelay;
        private System.Windows.Forms.Label lblDefaultDelaySuffix;
        private System.Windows.Forms.CheckBox chkAutoRelaunch;
        private System.Windows.Forms.Button btnDuplicateTestCase;
        private System.Windows.Forms.Label lblTestDescription;
        private System.Windows.Forms.TextBox txtTestCaseDescription;
        private System.Windows.Forms.GroupBox grpConditions;
        private System.Windows.Forms.Label lblValueChecks;
        private System.Windows.Forms.Button btnAddValueCheck;
        private System.Windows.Forms.Button btnRemoveValueCheck;
        private System.Windows.Forms.DataGridView grdValueChecks;
        private System.Windows.Forms.DataGridViewTextBoxColumn ValueCheckId;
        private System.Windows.Forms.DataGridViewTextBoxColumn ValueTarget;
        private System.Windows.Forms.DataGridViewTextBoxColumn ExpectedValue;
        private System.Windows.Forms.DataGridViewTextBoxColumn ValueControlType;
        private System.Windows.Forms.Label lblExpectedWindows;
        private System.Windows.Forms.Button btnAddExpectedWindow;
        private System.Windows.Forms.Button btnRemoveExpectedWindow;
        private System.Windows.Forms.DataGridView grdExpectedWindows;
        private System.Windows.Forms.DataGridViewTextBoxColumn ExpectedWindowId;
        private System.Windows.Forms.DataGridViewTextBoxColumn ExpectedPattern;
        private System.Windows.Forms.DataGridViewComboBoxColumn ExpectedMatchType;
        private System.Windows.Forms.DataGridViewCheckBoxColumn MustAppearAtLeastOnce;
        private System.Windows.Forms.Label lblForbiddenWindows;
        private System.Windows.Forms.Button btnAddForbiddenWindow;
        private System.Windows.Forms.Button btnRemoveForbiddenWindow;
        private System.Windows.Forms.DataGridView grdForbiddenWindows;
        private System.Windows.Forms.DataGridViewTextBoxColumn ForbiddenWindowId;
        private System.Windows.Forms.DataGridViewTextBoxColumn ForbiddenPattern;
        private System.Windows.Forms.DataGridViewComboBoxColumn ForbiddenMatchType;
    }
}
