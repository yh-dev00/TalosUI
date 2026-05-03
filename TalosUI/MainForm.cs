using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using TalosCore;

namespace TalosUI
{
    public partial class MainForm : Form
    {
        private const int GracefulCloseTimeoutMs = 5000;
        private const int InspectPollIntervalMs = 100;
        private const int InspectHoverDwellMs = 1000;
        private const int WhMouseLl = 14;
        private const int WmLButtonDown = 0x0201;
        private string sTargetPath = string.Empty;

        private readonly ITargetProcessManager targetProcessManager;
        private readonly IUiAutomationService uiAutomationService;
        private readonly Timer inspectHoverTimer;
        private readonly TestSuite currentSuite;
        private readonly LowLevelMouseProc mouseHookCallback;
        private TalosUiState currentState;
        private Point lastMousePosition;
        private Point lastResolvedMousePosition;
        private DateTime mouseStableSinceUtc;
        private UiElementInfo currentHoverElement;
        private HighlighterWindow highlighterWindow;
        private TestCase currentTestCase;
        private IntPtr mouseHookHandle;
        private bool refreshingStepGrid;
        private GroupBox grpRecordedSteps;
        private Label lblCurrentTestCase;
        private ComboBox cmbTestCases;
        private Button btnStartRecord;
        private Button btnStopRecord;
        private Button btnMoveStepUp;
        private Button btnMoveStepDown;
        private DataGridView grdRecordedSteps;

        public MainForm()
        {
            InitializeComponent();
            targetProcessManager = new TargetProcessManager();
            uiAutomationService = new UiAutomationService();
            currentSuite = CreateDefaultSuite();
            currentTestCase = currentSuite.Tests[0];
            mouseHookCallback = MouseHookProc;
            inspectHoverTimer = new Timer(components);
            inspectHoverTimer.Interval = InspectPollIntervalMs;
            inspectHoverTimer.Tick += inspectHoverTimer_Tick;
            currentState = TalosUiState.Idle;
            lastMousePosition = Cursor.Position;
            lastResolvedMousePosition = Point.Empty;
            mouseStableSinceUtc = DateTime.UtcNow;
            InitializeRecordModeUi();
            UpdateUiState();
            ClearInspectedElementDetails();
            RefreshTestCaseList();
            RefreshStepGrid();
        }

        private void btnSelectTarget_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Executable files (*.exe)|*.exe|All files (*.*)|*.*";
            openFileDialog.Title = "Select Target Application";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                sTargetPath = openFileDialog.FileName;
                edtTargetPath.Text = sTargetPath;
            }
        }

        private void btnRecord_Click(object sender, EventArgs e)
        {
            SetUiState(TalosUiState.Idle);
            sTargetPath = edtTargetPath.Text;

            try
            {
                targetProcessManager.EnsureRunning(sTargetPath, string.Empty);
                MessageBox.Show(
                    "Target application is running. Process Id: " + targetProcessManager.ProcessId,
                    "TalosUI",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Target launch failed",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void btnStartInspect_Click(object sender, EventArgs e)
        {
            SetUiState(TalosUiState.Inspect);
        }

        private void btnStopInspect_Click(object sender, EventArgs e)
        {
            SetUiState(TalosUiState.Idle);
        }

        private void btnStartRecord_Click(object sender, EventArgs e)
        {
            if (currentTestCase == null)
            {
                MessageBox.Show(
                    "Select a test case before recording.",
                    "TalosUI",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            currentSuite.TargetAppPath = edtTargetPath.Text;
            SetUiState(TalosUiState.Record);
        }

        private void btnStopRecord_Click(object sender, EventArgs e)
        {
            SetUiState(TalosUiState.Idle);
        }

        private void cmbTestCases_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbTestCases.SelectedIndex >= 0 && cmbTestCases.SelectedIndex < currentSuite.Tests.Count)
            {
                currentTestCase = currentSuite.Tests[cmbTestCases.SelectedIndex];
                RefreshStepGrid();
            }
        }

        private void btnMoveStepUp_Click(object sender, EventArgs e)
        {
            MoveSelectedStep(-1);
        }

        private void btnMoveStepDown_Click(object sender, EventArgs e)
        {
            MoveSelectedStep(1);
        }

        private void grdRecordedSteps_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (refreshingStepGrid || e.RowIndex < 0 || currentTestCase == null || e.RowIndex >= currentTestCase.Steps.Count)
            {
                return;
            }

            ApplyGridRowToStep(e.RowIndex);
            RefreshStepGridPreservingSelection(e.RowIndex);
        }

        private void grdRecordedSteps_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (grdRecordedSteps.IsCurrentCellDirty)
            {
                grdRecordedSteps.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        private void grdRecordedSteps_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (e.RowIndex < 0 || grdRecordedSteps.Columns[e.ColumnIndex].Name != "FixedDelayMs")
            {
                return;
            }

            string value = Convert.ToString(e.FormattedValue);
            int parsedDelay;

            if (!string.IsNullOrWhiteSpace(value) && (!int.TryParse(value, out parsedDelay) || parsedDelay < 0))
            {
                e.Cancel = true;
                grdRecordedSteps.Rows[e.RowIndex].ErrorText = "Delay must be blank or a non-negative whole number.";
            }
            else
            {
                grdRecordedSteps.Rows[e.RowIndex].ErrorText = string.Empty;
            }
        }

        private void grdRecordedSteps_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
        }

        private void inspectHoverTimer_Tick(object sender, EventArgs e)
        {
            if (currentState != TalosUiState.Inspect && currentState != TalosUiState.Record)
            {
                return;
            }

            Point currentMousePosition = Cursor.Position;

            if (currentMousePosition != lastMousePosition)
            {
                lastMousePosition = currentMousePosition;
                mouseStableSinceUtc = DateTime.UtcNow;
                lastResolvedMousePosition = Point.Empty;
                currentHoverElement = null;
                ClearInspectedElementDetails();
                HideHighlightOverlay();
                return;
            }

            TimeSpan stableDuration = DateTime.UtcNow - mouseStableSinceUtc;

            if (stableDuration.TotalMilliseconds < InspectHoverDwellMs ||
                currentMousePosition == lastResolvedMousePosition)
            {
                return;
            }

            lastResolvedMousePosition = currentMousePosition;
            ResolveInspectedElement(currentMousePosition);
        }

        private void ResolveInspectedElement(Point screenPoint)
        {
            UiElementInfo elementFound = null;
            HideHighlightOverlay();

            try
            {
                elementFound = uiAutomationService.GetElementAtPoint(screenPoint.X, screenPoint.Y);
            }
            catch (Exception)
            {
                elementFound = null;
            }

            OnHoveredElementChanged(elementFound);
        }

        private void OnHoveredElementChanged(UiElementInfo elementFound)
        {
            currentHoverElement = elementFound;

            if (elementFound == null)
            {
                ClearInspectedElementDetails();
                HideHighlightOverlay();
                return;
            }

            DisplayInspectedElementDetails(elementFound);
            ShowHighlightOverlay(elementFound);
        }

        private void SetUiState(TalosUiState newState)
        {
            currentState = newState;

            if (currentState == TalosUiState.Inspect || currentState == TalosUiState.Record)
            {
                lastMousePosition = Cursor.Position;
                lastResolvedMousePosition = Point.Empty;
                mouseStableSinceUtc = DateTime.UtcNow;
                inspectHoverTimer.Start();

                if (currentState == TalosUiState.Record)
                {
                    InstallMouseHook();
                }
                else
                {
                    RemoveMouseHook();
                }
            }
            else
            {
                inspectHoverTimer.Stop();
                RemoveMouseHook();
                currentHoverElement = null;
                ClearInspectedElementDetails();
                HideHighlightOverlay();
            }

            UpdateUiState();
        }

        private TestSuite CreateDefaultSuite()
        {
            TestSuite suite = new TestSuite();
            TestCase testCase = new TestCase();
            testCase.Name = "Recorded Test 1";
            suite.Tests.Add(testCase);
            return suite;
        }

        private void InitializeRecordModeUi()
        {
            ClientSize = new Size(1050, 870);
            MinimumSize = new Size(850, 650);

            btnStartRecord = new Button();
            btnStartRecord.Location = new Point(528, 80);
            btnStartRecord.Name = "btnStartRecord";
            btnStartRecord.Size = new Size(112, 28);
            btnStartRecord.TabIndex = 10;
            btnStartRecord.Text = "Start Record";
            btnStartRecord.UseVisualStyleBackColor = true;
            btnStartRecord.Click += btnStartRecord_Click;
            Controls.Add(btnStartRecord);

            btnStopRecord = new Button();
            btnStopRecord.Location = new Point(648, 80);
            btnStopRecord.Name = "btnStopRecord";
            btnStopRecord.Size = new Size(112, 28);
            btnStopRecord.TabIndex = 11;
            btnStopRecord.Text = "Stop Record";
            btnStopRecord.UseVisualStyleBackColor = true;
            btnStopRecord.Click += btnStopRecord_Click;
            Controls.Add(btnStopRecord);

            grpRecordedSteps = new GroupBox();
            grpRecordedSteps.Location = new Point(24, 552);
            grpRecordedSteps.Name = "grpRecordedSteps";
            grpRecordedSteps.Size = new Size(1000, 296);
            grpRecordedSteps.TabIndex = 12;
            grpRecordedSteps.TabStop = false;
            grpRecordedSteps.Text = "Recorded Steps";
            Controls.Add(grpRecordedSteps);

            lblCurrentTestCase = new Label();
            lblCurrentTestCase.AutoSize = true;
            lblCurrentTestCase.Location = new Point(16, 28);
            lblCurrentTestCase.Name = "lblCurrentTestCase";
            lblCurrentTestCase.Size = new Size(72, 17);
            lblCurrentTestCase.TabIndex = 0;
            lblCurrentTestCase.Text = "Test Case";
            grpRecordedSteps.Controls.Add(lblCurrentTestCase);

            cmbTestCases = new ComboBox();
            cmbTestCases.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbTestCases.Location = new Point(96, 24);
            cmbTestCases.Name = "cmbTestCases";
            cmbTestCases.Size = new Size(264, 24);
            cmbTestCases.TabIndex = 1;
            cmbTestCases.SelectedIndexChanged += cmbTestCases_SelectedIndexChanged;
            grpRecordedSteps.Controls.Add(cmbTestCases);

            btnMoveStepUp = new Button();
            btnMoveStepUp.Location = new Point(752, 22);
            btnMoveStepUp.Name = "btnMoveStepUp";
            btnMoveStepUp.Size = new Size(104, 28);
            btnMoveStepUp.TabIndex = 2;
            btnMoveStepUp.Text = "Move Up";
            btnMoveStepUp.UseVisualStyleBackColor = true;
            btnMoveStepUp.Click += btnMoveStepUp_Click;
            grpRecordedSteps.Controls.Add(btnMoveStepUp);

            btnMoveStepDown = new Button();
            btnMoveStepDown.Location = new Point(864, 22);
            btnMoveStepDown.Name = "btnMoveStepDown";
            btnMoveStepDown.Size = new Size(112, 28);
            btnMoveStepDown.TabIndex = 3;
            btnMoveStepDown.Text = "Move Down";
            btnMoveStepDown.UseVisualStyleBackColor = true;
            btnMoveStepDown.Click += btnMoveStepDown_Click;
            grpRecordedSteps.Controls.Add(btnMoveStepDown);

            grdRecordedSteps = new DataGridView();
            grdRecordedSteps.AllowUserToAddRows = false;
            grdRecordedSteps.AllowUserToDeleteRows = false;
            grdRecordedSteps.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            grdRecordedSteps.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            grdRecordedSteps.Location = new Point(16, 64);
            grdRecordedSteps.MultiSelect = false;
            grdRecordedSteps.Name = "grdRecordedSteps";
            grdRecordedSteps.RowHeadersVisible = false;
            grdRecordedSteps.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grdRecordedSteps.Size = new Size(960, 208);
            grdRecordedSteps.TabIndex = 4;
            grdRecordedSteps.CellValueChanged += grdRecordedSteps_CellValueChanged;
            grdRecordedSteps.CurrentCellDirtyStateChanged += grdRecordedSteps_CurrentCellDirtyStateChanged;
            grdRecordedSteps.CellValidating += grdRecordedSteps_CellValidating;
            grdRecordedSteps.DataError += grdRecordedSteps_DataError;
            grpRecordedSteps.Controls.Add(grdRecordedSteps);

            ConfigureRecordedStepsGrid();
        }

        private void ConfigureRecordedStepsGrid()
        {
            DataGridViewTextBoxColumn idColumn = new DataGridViewTextBoxColumn();
            idColumn.Name = "Id";
            idColumn.HeaderText = "Id";
            idColumn.ReadOnly = true;
            idColumn.Width = 42;
            grdRecordedSteps.Columns.Add(idColumn);

            DataGridViewComboBoxColumn actionColumn = new DataGridViewComboBoxColumn();
            actionColumn.Name = "Action";
            actionColumn.HeaderText = "Action";
            actionColumn.Width = 96;
            actionColumn.DataSource = Enum.GetNames(typeof(StepAction));
            grdRecordedSteps.Columns.Add(actionColumn);

            DataGridViewTextBoxColumn targetColumn = new DataGridViewTextBoxColumn();
            targetColumn.Name = "Target";
            targetColumn.HeaderText = "Target";
            targetColumn.ReadOnly = true;
            targetColumn.Width = 210;
            grdRecordedSteps.Columns.Add(targetColumn);

            DataGridViewTextBoxColumn controlTypeColumn = new DataGridViewTextBoxColumn();
            controlTypeColumn.Name = "ControlType";
            controlTypeColumn.HeaderText = "Control";
            controlTypeColumn.ReadOnly = true;
            controlTypeColumn.Width = 90;
            grdRecordedSteps.Columns.Add(controlTypeColumn);

            DataGridViewTextBoxColumn delayColumn = new DataGridViewTextBoxColumn();
            delayColumn.Name = "FixedDelayMs";
            delayColumn.HeaderText = "Delay ms";
            delayColumn.Width = 76;
            grdRecordedSteps.Columns.Add(delayColumn);

            DataGridViewTextBoxColumn textColumn = new DataGridViewTextBoxColumn();
            textColumn.Name = "Text";
            textColumn.HeaderText = "SetValue Text";
            textColumn.Width = 130;
            grdRecordedSteps.Columns.Add(textColumn);

            DataGridViewTextBoxColumn sendKeysColumn = new DataGridViewTextBoxColumn();
            sendKeysColumn.Name = "SendKeysText";
            sendKeysColumn.HeaderText = "SendKeys";
            sendKeysColumn.Width = 120;
            grdRecordedSteps.Columns.Add(sendKeysColumn);

            DataGridViewTextBoxColumn selectedItemColumn = new DataGridViewTextBoxColumn();
            selectedItemColumn.Name = "SelectedItem";
            selectedItemColumn.HeaderText = "Selected Item";
            selectedItemColumn.Width = 136;
            grdRecordedSteps.Columns.Add(selectedItemColumn);
        }

        private void RefreshTestCaseList()
        {
            cmbTestCases.Items.Clear();

            foreach (TestCase test in currentSuite.Tests)
            {
                cmbTestCases.Items.Add(test.Name);
            }

            if (currentTestCase != null)
            {
                int index = currentSuite.Tests.IndexOf(currentTestCase);

                if (index >= 0)
                {
                    cmbTestCases.SelectedIndex = index;
                }
            }
            else if (cmbTestCases.Items.Count > 0)
            {
                cmbTestCases.SelectedIndex = 0;
            }
        }

        private void RefreshStepGrid()
        {
            RefreshStepGridPreservingSelection(-1);
        }

        private void RefreshStepGridPreservingSelection(int selectedRowIndex)
        {
            refreshingStepGrid = true;
            grdRecordedSteps.Rows.Clear();

            if (currentTestCase != null)
            {
                foreach (Step step in currentTestCase.Steps)
                {
                    grdRecordedSteps.Rows.Add(
                        step.Id,
                        step.Action.ToString(),
                        FormatStepTarget(step),
                        step.Locator == null ? string.Empty : step.Locator.ControlType,
                        step.FixedDelayMs.HasValue ? step.FixedDelayMs.Value.ToString() : string.Empty,
                        step.Parameters == null ? string.Empty : step.Parameters.Text,
                        step.Parameters == null ? string.Empty : step.Parameters.SendKeysText,
                        step.Parameters == null ? string.Empty : step.Parameters.SelectedItem);
                }
            }

            refreshingStepGrid = false;

            if (selectedRowIndex >= 0 && selectedRowIndex < grdRecordedSteps.Rows.Count)
            {
                grdRecordedSteps.ClearSelection();
                grdRecordedSteps.Rows[selectedRowIndex].Selected = true;
                grdRecordedSteps.CurrentCell = grdRecordedSteps.Rows[selectedRowIndex].Cells[0];
            }

            UpdateRecordedStepButtons();
        }

        private void UpdateRecordedStepButtons()
        {
            bool hasMultipleSteps = currentTestCase != null && currentTestCase.Steps.Count > 1;
            btnMoveStepUp.Enabled = hasMultipleSteps;
            btnMoveStepDown.Enabled = hasMultipleSteps;
        }

        private void ApplyGridRowToStep(int rowIndex)
        {
            Step step = currentTestCase.Steps[rowIndex];
            DataGridViewRow row = grdRecordedSteps.Rows[rowIndex];
            StepAction parsedAction;
            int parsedDelay;

            if (Enum.TryParse(Convert.ToString(row.Cells["Action"].Value), out parsedAction))
            {
                step.Action = parsedAction;
            }

            string delayText = Convert.ToString(row.Cells["FixedDelayMs"].Value);
            step.FixedDelayMs = string.IsNullOrWhiteSpace(delayText) ? (int?)null : int.TryParse(delayText, out parsedDelay) ? parsedDelay : step.FixedDelayMs;

            if (step.Parameters == null)
            {
                step.Parameters = new StepParameters();
            }

            step.Parameters.Text = Convert.ToString(row.Cells["Text"].Value) ?? string.Empty;
            step.Parameters.SendKeysText = Convert.ToString(row.Cells["SendKeysText"].Value) ?? string.Empty;
            step.Parameters.SelectedItem = Convert.ToString(row.Cells["SelectedItem"].Value) ?? string.Empty;
        }

        private void MoveSelectedStep(int direction)
        {
            if (currentTestCase == null || grdRecordedSteps.CurrentRow == null)
            {
                return;
            }

            int oldIndex = grdRecordedSteps.CurrentRow.Index;
            int newIndex = oldIndex + direction;

            if (newIndex < 0 || newIndex >= currentTestCase.Steps.Count)
            {
                return;
            }

            Step step = currentTestCase.Steps[oldIndex];
            currentTestCase.Steps.RemoveAt(oldIndex);
            currentTestCase.Steps.Insert(newIndex, step);
            RenumberSteps(currentTestCase);
            RefreshStepGridPreservingSelection(newIndex);
        }

        private void HandleRecordedLeftClick(Point screenPoint)
        {
            if (currentState != TalosUiState.Record || IsPointInsideTalosUi(screenPoint))
            {
                return;
            }

            UiElementInfo clickedElement = null;

            try
            {
                clickedElement = uiAutomationService.GetElementAtPoint(screenPoint.X, screenPoint.Y);
            }
            catch (Exception)
            {
                clickedElement = null;
            }

            if (clickedElement == null || clickedElement.ProcessId == Process.GetCurrentProcess().Id)
            {
                return;
            }

            OnHoveredElementChanged(clickedElement);
            AppendRecordedStep(clickedElement);
        }

        private void AppendRecordedStep(UiElementInfo element)
        {
            if (currentTestCase == null)
            {
                return;
            }

            Step step = new Step();
            step.Id = GetNextStepId(currentTestCase);
            step.Locator = element.ToElementLocator();
            step.Action = StepAction.Invoke;
            step.Parameters = new StepParameters();
            currentTestCase.Steps.Add(step);
            RefreshStepGridPreservingSelection(currentTestCase.Steps.Count - 1);
        }

        private int GetNextStepId(TestCase testCase)
        {
            int highestId = 0;

            foreach (Step step in testCase.Steps)
            {
                highestId = Math.Max(highestId, step.Id);
            }

            return highestId + 1;
        }

        private void RenumberSteps(TestCase testCase)
        {
            for (int i = 0; i < testCase.Steps.Count; i++)
            {
                testCase.Steps[i].Id = i + 1;
            }
        }

        private string FormatStepTarget(Step step)
        {
            if (step == null || step.Locator == null)
            {
                return string.Empty;
            }

            List<string> parts = new List<string>();

            if (!string.IsNullOrEmpty(step.Locator.AutomationId))
            {
                parts.Add("AutomationId=" + step.Locator.AutomationId);
            }

            if (!string.IsNullOrEmpty(step.Locator.Name))
            {
                parts.Add("Name=" + step.Locator.Name);
            }

            if (parts.Count == 0 && !string.IsNullOrEmpty(step.Locator.ClassName))
            {
                parts.Add("Class=" + step.Locator.ClassName);
            }

            return parts.Count == 0 ? step.Locator.ControlType : string.Join(" | ", parts.ToArray());
        }

        private bool IsPointInsideTalosUi(Point screenPoint)
        {
            foreach (Form form in Application.OpenForms)
            {
                if (form.Visible && form.Bounds.Contains(screenPoint))
                {
                    return true;
                }
            }

            return false;
        }

        private void InstallMouseHook()
        {
            if (mouseHookHandle != IntPtr.Zero)
            {
                return;
            }

            using (Process currentProcess = Process.GetCurrentProcess())
            using (ProcessModule currentModule = currentProcess.MainModule)
            {
                mouseHookHandle = SetWindowsHookEx(
                    WhMouseLl,
                    mouseHookCallback,
                    GetModuleHandle(currentModule.ModuleName),
                    0);
            }
        }

        private void RemoveMouseHook()
        {
            if (mouseHookHandle == IntPtr.Zero)
            {
                return;
            }

            UnhookWindowsHookEx(mouseHookHandle);
            mouseHookHandle = IntPtr.Zero;
        }

        private IntPtr MouseHookProc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)WmLButtonDown && currentState == TalosUiState.Record)
            {
                MouseHookStruct mouseInfo = (MouseHookStruct)Marshal.PtrToStructure(lParam, typeof(MouseHookStruct));
                Point clickPoint = new Point(mouseInfo.Point.X, mouseInfo.Point.Y);

                BeginInvoke(new Action(delegate
                {
                    HandleRecordedLeftClick(clickPoint);
                }));
            }

            return CallNextHookEx(mouseHookHandle, nCode, wParam, lParam);
        }

        private void ShowHighlightOverlay(UiElementInfo element)
        {
            if (element == null || element.BoundingRectangle == null || element.BoundingRectangle.IsEmpty)
            {
                HideHighlightOverlay();
                return;
            }

            if (highlighterWindow == null)
            {
                highlighterWindow = new HighlighterWindow();
            }

            highlighterWindow.ShowHighlight(element.BoundingRectangle);
        }

        private void HideHighlightOverlay()
        {
            if (highlighterWindow != null)
            {
                highlighterWindow.HideHighlight();
            }
        }

        private void UpdateUiState()
        {
            lblUiStateValue.Text = currentState.ToString();
            btnStartInspect.Enabled = currentState == TalosUiState.Idle;
            btnStopInspect.Enabled = currentState == TalosUiState.Inspect || currentState == TalosUiState.Record;
            btnRecord.Enabled = currentState != TalosUiState.Running;
            btnStartRecord.Enabled = currentState == TalosUiState.Idle || currentState == TalosUiState.Inspect;
            btnStopRecord.Enabled = currentState == TalosUiState.Record;
            cmbTestCases.Enabled = currentState != TalosUiState.Running;
        }

        private void ClearInspectedElementDetails()
        {
            txtHowFound.Text = string.Empty;
            txtAutomationId.Text = string.Empty;
            txtElementName.Text = string.Empty;
            txtControlType.Text = string.Empty;
            txtClassName.Text = string.Empty;
            txtProcessId.Text = string.Empty;
            txtBoundingRectangle.Text = string.Empty;
            txtAncestorPath.Text = string.Empty;
        }

        private void DisplayInspectedElementDetails(UiElementInfo element)
        {
            txtHowFound.Text = element.HowFound ?? string.Empty;
            txtAutomationId.Text = element.AutomationId ?? string.Empty;
            txtElementName.Text = element.Name ?? string.Empty;
            txtControlType.Text = element.ControlType ?? string.Empty;
            txtClassName.Text = element.ClassName ?? string.Empty;
            txtProcessId.Text = element.ProcessId.ToString();
            txtBoundingRectangle.Text = FormatRectangle(element.BoundingRectangle);
            txtAncestorPath.Text = FormatAncestorPath(element);
        }

        private string FormatRectangle(PersistedRectangle rectangle)
        {
            if (rectangle == null || rectangle.IsEmpty)
            {
                return string.Empty;
            }

            return string.Format(
                "X={0:0}, Y={1:0}, Width={2:0}, Height={3:0}",
                rectangle.X,
                rectangle.Y,
                rectangle.Width,
                rectangle.Height);
        }

        private string FormatAncestorPath(UiElementInfo element)
        {
            if (element == null || element.AncestorPath == null || element.AncestorPath.Count == 0)
            {
                return string.Empty;
            }

            StringBuilder builder = new StringBuilder();

            for (int i = 0; i < element.AncestorPath.Count; i++)
            {
                AncestorDescriptor ancestor = element.AncestorPath[i];
                builder.Append(i + 1);
                builder.Append(". ");
                builder.Append(ancestor.ControlType);

                if (!string.IsNullOrEmpty(ancestor.Name))
                {
                    builder.Append(" | Name=");
                    builder.Append(ancestor.Name);
                }

                if (!string.IsNullOrEmpty(ancestor.AutomationId))
                {
                    builder.Append(" | AutomationId=");
                    builder.Append(ancestor.AutomationId);
                }

                if (ancestor.IndexWithinParent.HasValue)
                {
                    builder.Append(" | Index=");
                    builder.Append(ancestor.IndexWithinParent.Value);
                }

                if (i < element.AncestorPath.Count - 1)
                {
                    builder.AppendLine();
                }
            }

            return builder.ToString();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            SetUiState(TalosUiState.Idle);
            RemoveMouseHook();
            CloseHighlightOverlay();

            if (targetProcessManager != null && targetProcessManager.IsRunning)
            {
                targetProcessManager.Close(GracefulCloseTimeoutMs);
            }

            base.OnFormClosing(e);
        }

        private void CloseHighlightOverlay()
        {
            if (highlighterWindow != null)
            {
                highlighterWindow.Close();
                highlighterWindow = null;
            }
        }

        private delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);

        [StructLayout(LayoutKind.Sequential)]
        private struct MouseHookStruct
        {
            public NativePoint Point;
            public uint MouseData;
            public uint Flags;
            public uint Time;
            public IntPtr ExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct NativePoint
        {
            public int X;
            public int Y;
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int hookType, LowLevelMouseProc callback, IntPtr moduleHandle, uint threadId);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hookHandle);

        [DllImport("user32.dll")]
        private static extern IntPtr CallNextHookEx(IntPtr hookHandle, int code, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string moduleName);
    }

    internal enum TalosUiState
    {
        Idle,
        Inspect,
        Record,
        Running
    }
}
