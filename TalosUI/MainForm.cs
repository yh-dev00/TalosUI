using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
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
        private readonly CTalosMemory talosMemory;
        private readonly Timer inspectHoverTimer;
        private readonly LowLevelMouseProc mouseHookCallback;
        private TestSuite currentSuite;
        private string currentSuitePath;
        private TalosUiState currentState;
        private Point lastMousePosition;
        private Point lastResolvedMousePosition;
        private DateTime mouseStableSinceUtc;
        private UiElementInfo currentHoverElement;
        private HighlighterWindow highlighterWindow;
        private TestCase currentTestCase;
        private IntPtr mouseHookHandle;
        private bool refreshingStepGrid;
        private bool refreshingTestCaseList;
        private bool refreshingTestCaseDetails;
        private bool refreshingConditionGrids;
        private bool suppressDirtyTracking;
        private bool isSuiteDirty;

        public MainForm()
        {
            InitializeComponent();
            targetProcessManager = new TargetProcessManager();
            uiAutomationService = new UiAutomationService();
            talosMemory = new CTalosMemory();
            currentSuite = CreateDefaultSuite();
            currentTestCase = currentSuite.Tests[0];
            mouseHookCallback = MouseHookProc;
            inspectHoverTimer = new Timer();
            inspectHoverTimer.Interval = InspectPollIntervalMs;
            inspectHoverTimer.Tick += inspectHoverTimer_Tick;
            currentState = TalosUiState.Idle;
            lastMousePosition = Cursor.Position;
            lastResolvedMousePosition = Point.Empty;
            mouseStableSinceUtc = DateTime.UtcNow;
            UpdateUiState();
            ClearInspectedElementDetails();
            RefreshSuiteMetadataEditor();
            RefreshTestCaseList();
            RefreshTestCaseDetails();
            RefreshStepGrid();
            RefreshConditionGrids();
            MarkSuiteClean();
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
                currentSuite.TargetAppPath = sTargetPath;
                MarkSuiteDirty();
            }
        }

        private void btnRecord_Click(object sender, EventArgs e)
        {
            SetUiState(TalosUiState.Idle);

            if (!ValidateSuiteForSaveOrRun(true, true))
            {
                return;
            }

            sTargetPath = currentSuite.TargetAppPath;

            try
            {
                targetProcessManager.EnsureRunning(sTargetPath, currentSuite.LaunchParams);
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

            ApplySuiteMetadataFromEditor();
            SetUiState(TalosUiState.Record);
        }

        private void TsBtnSave_Click(object sender, EventArgs e)
        {
            SaveCurrentSuite();
        }

        private void TsBtnSaveAs_Click(object sender, EventArgs e)
        {
            SaveCurrentSuiteAs();
        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!ConfirmDiscardUnsavedChanges())
            {
                return;
            }

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "TalosUI suites (*.talos.json)|*.talos.json|JSON files (*.json)|*.json|All files (*.*)|*.*";
            openFileDialog.Title = "Open TalosUI Suite";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                LoadSuiteFromFile(openFileDialog.FileName);
            }
        }

        private void SuiteMetadataChanged(object sender, EventArgs e)
        {
            ApplySuiteMetadataFromEditor();
            MarkSuiteDirty();
        }

        private void btnStopRecord_Click(object sender, EventArgs e)
        {
            SetUiState(TalosUiState.Idle);
        }

        private void listBoxTestCase_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (refreshingTestCaseList)
            {
                return;
            }

            SelectTestCaseByIndex(listBoxTestCase.SelectedIndex);
        }

        private void btnAddTestCase_Click(object sender, EventArgs e)
        {
            string requestedName = txtTestCaseName.Text.Trim();
            TestCase testCase = new TestCase();
            testCase.Name = string.IsNullOrWhiteSpace(requestedName) ?
                GenerateDefaultTestCaseName() :
                EnsureUniqueTestCaseName(requestedName, null);

            currentSuite.Tests.Add(testCase);
            currentTestCase = testCase;
            MarkSuiteDirty();
            RefreshTestCaseList();
            RefreshTestCaseDetails();
            RefreshStepGrid();
            RefreshConditionGrids();
        }

        private void btnRemoveTestCase_Click(object sender, EventArgs e)
        {
            int index = listBoxTestCase.SelectedIndex;

            if (index < 0 || index >= currentSuite.Tests.Count)
            {
                return;
            }

            currentSuite.Tests.RemoveAt(index);

            if (currentSuite.Tests.Count == 0)
            {
                currentTestCase = null;
            }
            else
            {
                currentTestCase = currentSuite.Tests[Math.Min(index, currentSuite.Tests.Count - 1)];
            }

            MarkSuiteDirty();
            RefreshTestCaseList();
            RefreshTestCaseDetails();
            RefreshStepGrid();
            RefreshConditionGrids();
        }

        private void btnSaveTestCase_Click(object sender, EventArgs e)
        {
            if (currentTestCase == null)
            {
                return;
            }

            string newName = txtTestCaseName.Text.Trim();

            if (string.IsNullOrWhiteSpace(newName))
            {
                MessageBox.Show(
                    "Enter a test case name before saving.",
                    "TalosUI",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            currentTestCase.Name = EnsureUniqueTestCaseName(newName, currentTestCase);
            MarkSuiteDirty();
            RefreshTestCaseList();
            RefreshTestCaseDetails();
        }

        private void btnDuplicateTestCase_Click(object sender, EventArgs e)
        {
            if (currentTestCase == null)
            {
                return;
            }

            TestCase duplicate = CopyTestCase(currentTestCase);
            duplicate.Name = EnsureUniqueTestCaseName(currentTestCase.Name + " Copy", null);
            currentSuite.Tests.Insert(currentSuite.Tests.IndexOf(currentTestCase) + 1, duplicate);
            currentTestCase = duplicate;
            MarkSuiteDirty();
            RefreshTestCaseList();
            RefreshTestCaseDetails();
            RefreshStepGrid();
            RefreshConditionGrids();
        }

        private void txtTestCaseDescription_TextChanged(object sender, EventArgs e)
        {
            if (refreshingTestCaseDetails || currentTestCase == null)
            {
                return;
            }

            currentTestCase.Description = txtTestCaseDescription.Text;
            MarkSuiteDirty();
        }

        private void txtTestCaseName_TextChanged(object sender, EventArgs e)
        {
            if (refreshingTestCaseDetails || currentTestCase == null)
            {
                return;
            }

            MarkSuiteDirty();
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
            MarkSuiteDirty();
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

        private void btnAddValueCheck_Click(object sender, EventArgs e)
        {
            if (currentTestCase == null)
            {
                return;
            }

            if (currentHoverElement == null)
            {
                MessageBox.Show(
                    "Inspect an element first, then add it as a value check.",
                    "TalosUI",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            EnsureConditions(currentTestCase);
            ValueCheck valueCheck = new ValueCheck();
            valueCheck.Id = GenerateConditionId("value", currentTestCase.Conditions.ValueChecks.Count + 1);
            valueCheck.Locator = currentHoverElement.ToElementLocator();
            valueCheck.ExpectedValue = string.Empty;
            currentTestCase.Conditions.ValueChecks.Add(valueCheck);
            MarkSuiteDirty();
            RefreshConditionGridsPreservingSelection(grdValueChecks, currentTestCase.Conditions.ValueChecks.Count - 1);
        }

        private void btnRemoveValueCheck_Click(object sender, EventArgs e)
        {
            if (currentTestCase == null || grdValueChecks.CurrentRow == null)
            {
                return;
            }

            int index = grdValueChecks.CurrentRow.Index;
            EnsureConditions(currentTestCase);

            if (index >= 0 && index < currentTestCase.Conditions.ValueChecks.Count)
            {
                currentTestCase.Conditions.ValueChecks.RemoveAt(index);
                MarkSuiteDirty();
                RefreshConditionGrids();
            }
        }

        private void btnAddExpectedWindow_Click(object sender, EventArgs e)
        {
            if (currentTestCase == null)
            {
                return;
            }

            EnsureConditions(currentTestCase);
            ExpectedWindowCondition condition = new ExpectedWindowCondition();
            condition.Id = GenerateConditionId("expectedWindow", currentTestCase.Conditions.ExpectedWindows.Count + 1);
            currentTestCase.Conditions.ExpectedWindows.Add(condition);
            MarkSuiteDirty();
            RefreshConditionGridsPreservingSelection(grdExpectedWindows, currentTestCase.Conditions.ExpectedWindows.Count - 1);
        }

        private void btnRemoveExpectedWindow_Click(object sender, EventArgs e)
        {
            if (currentTestCase == null || grdExpectedWindows.CurrentRow == null)
            {
                return;
            }

            int index = grdExpectedWindows.CurrentRow.Index;
            EnsureConditions(currentTestCase);

            if (index >= 0 && index < currentTestCase.Conditions.ExpectedWindows.Count)
            {
                currentTestCase.Conditions.ExpectedWindows.RemoveAt(index);
                MarkSuiteDirty();
                RefreshConditionGrids();
            }
        }

        private void btnAddForbiddenWindow_Click(object sender, EventArgs e)
        {
            if (currentTestCase == null)
            {
                return;
            }

            EnsureConditions(currentTestCase);
            ForbiddenWindowCondition condition = new ForbiddenWindowCondition();
            condition.Id = GenerateConditionId("forbiddenWindow", currentTestCase.Conditions.ForbiddenWindows.Count + 1);
            currentTestCase.Conditions.ForbiddenWindows.Add(condition);
            MarkSuiteDirty();
            RefreshConditionGridsPreservingSelection(grdForbiddenWindows, currentTestCase.Conditions.ForbiddenWindows.Count - 1);
        }

        private void btnRemoveForbiddenWindow_Click(object sender, EventArgs e)
        {
            if (currentTestCase == null || grdForbiddenWindows.CurrentRow == null)
            {
                return;
            }

            int index = grdForbiddenWindows.CurrentRow.Index;
            EnsureConditions(currentTestCase);

            if (index >= 0 && index < currentTestCase.Conditions.ForbiddenWindows.Count)
            {
                currentTestCase.Conditions.ForbiddenWindows.RemoveAt(index);
                MarkSuiteDirty();
                RefreshConditionGrids();
            }
        }

        private void grdValueChecks_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (refreshingConditionGrids || currentTestCase == null || e.RowIndex < 0)
            {
                return;
            }

            EnsureConditions(currentTestCase);

            if (e.RowIndex >= currentTestCase.Conditions.ValueChecks.Count)
            {
                return;
            }

            ValueCheck valueCheck = currentTestCase.Conditions.ValueChecks[e.RowIndex];
            DataGridViewRow row = grdValueChecks.Rows[e.RowIndex];
            valueCheck.Id = Convert.ToString(row.Cells["ValueCheckId"].Value) ?? string.Empty;
            valueCheck.ExpectedValue = Convert.ToString(row.Cells["ExpectedValue"].Value) ?? string.Empty;
            MarkSuiteDirty();
        }

        private void grdExpectedWindows_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (refreshingConditionGrids || currentTestCase == null || e.RowIndex < 0)
            {
                return;
            }

            EnsureConditions(currentTestCase);

            if (e.RowIndex >= currentTestCase.Conditions.ExpectedWindows.Count)
            {
                return;
            }

            ExpectedWindowCondition condition = currentTestCase.Conditions.ExpectedWindows[e.RowIndex];
            DataGridViewRow row = grdExpectedWindows.Rows[e.RowIndex];
            condition.Id = Convert.ToString(row.Cells["ExpectedWindowId"].Value) ?? string.Empty;
            condition.Pattern = Convert.ToString(row.Cells["ExpectedPattern"].Value) ?? string.Empty;
            condition.MatchType = ParseWindowMatchType(Convert.ToString(row.Cells["ExpectedMatchType"].Value));
            condition.MustAppearAtLeastOnce = Convert.ToBoolean(row.Cells["MustAppearAtLeastOnce"].Value ?? false);
            MarkSuiteDirty();
        }

        private void grdForbiddenWindows_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (refreshingConditionGrids || currentTestCase == null || e.RowIndex < 0)
            {
                return;
            }

            EnsureConditions(currentTestCase);

            if (e.RowIndex >= currentTestCase.Conditions.ForbiddenWindows.Count)
            {
                return;
            }

            ForbiddenWindowCondition condition = currentTestCase.Conditions.ForbiddenWindows[e.RowIndex];
            DataGridViewRow row = grdForbiddenWindows.Rows[e.RowIndex];
            condition.Id = Convert.ToString(row.Cells["ForbiddenWindowId"].Value) ?? string.Empty;
            condition.Pattern = Convert.ToString(row.Cells["ForbiddenPattern"].Value) ?? string.Empty;
            condition.MatchType = ParseWindowMatchType(Convert.ToString(row.Cells["ForbiddenMatchType"].Value));
            MarkSuiteDirty();
        }

        private void conditionGrid_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            DataGridView grid = sender as DataGridView;

            if (grid != null && grid.IsCurrentCellDirty)
            {
                grid.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        private void conditionGrid_DataError(object sender, DataGridViewDataErrorEventArgs e)
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

        private bool SaveCurrentSuite()
        {
            if (string.IsNullOrWhiteSpace(currentSuitePath))
            {
                return SaveCurrentSuiteAs();
            }

            return SaveSuiteToFile(currentSuitePath);
        }

        private bool SaveCurrentSuiteAs()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "TalosUI suites (*.talos.json)|*.talos.json|JSON files (*.json)|*.json|All files (*.*)|*.*";
            saveFileDialog.Title = "Save TalosUI Suite";
            saveFileDialog.DefaultExt = "talos.json";
            saveFileDialog.AddExtension = true;

            if (!string.IsNullOrWhiteSpace(currentSuitePath))
            {
                saveFileDialog.FileName = Path.GetFileName(currentSuitePath);
                saveFileDialog.InitialDirectory = Path.GetDirectoryName(currentSuitePath);
            }
            else
            {
                saveFileDialog.FileName = CreateSafeSuiteFileName(currentSuite.SuiteName);
            }

            if (saveFileDialog.ShowDialog() != DialogResult.OK)
            {
                return false;
            }

            return SaveSuiteToFile(saveFileDialog.FileName);
        }

        private bool SaveSuiteToFile(string path)
        {
            ApplyCurrentTestCaseDetailsFromEditor();

            if (!ValidateSuiteForSaveOrRun(true, true))
            {
                return false;
            }

            try
            {
                talosMemory.SaveSuite(currentSuite, path);
                currentSuitePath = path;
                suppressDirtyTracking = true;
                RefreshTestCaseList();
                RefreshTestCaseDetails();
                suppressDirtyTracking = false;
                MarkSuiteClean();
                MessageBox.Show(
                    "Suite saved to " + path,
                    "TalosUI",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Save failed",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return false;
            }
        }

        private void LoadSuiteFromFile(string path)
        {
            try
            {
                TestSuite loadedSuite = talosMemory.LoadSuite(path);
                SetUiState(TalosUiState.Idle);
                currentSuite = loadedSuite;
                currentSuitePath = path;
                currentTestCase = currentSuite.Tests.Count > 0 ? currentSuite.Tests[0] : null;
                sTargetPath = currentSuite.TargetAppPath ?? string.Empty;
                RefreshAllSuiteEditors();
                MarkSuiteClean();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Load failed",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void RefreshAllSuiteEditors()
        {
            suppressDirtyTracking = true;
            RefreshSuiteMetadataEditor();
            RefreshTestCaseList();
            RefreshTestCaseDetails();
            RefreshStepGrid();
            RefreshConditionGrids();
            suppressDirtyTracking = false;
            UpdateUiState();
        }

        private string CreateSafeSuiteFileName(string suiteName)
        {
            string baseName = string.IsNullOrWhiteSpace(suiteName) ? "Untitled Suite" : suiteName.Trim();

            foreach (char invalidChar in Path.GetInvalidFileNameChars())
            {
                baseName = baseName.Replace(invalidChar, '_');
            }

            return baseName + CTalosMemory.SuiteFileExtension;
        }

        private bool ConfirmDiscardUnsavedChanges()
        {
            if (!isSuiteDirty)
            {
                return true;
            }

            DialogResult result = MessageBox.Show(
                "Save changes to the current suite before continuing?",
                "Unsaved suite",
                MessageBoxButtons.YesNoCancel,
                MessageBoxIcon.Question);

            if (result == DialogResult.Cancel)
            {
                return false;
            }

            if (result == DialogResult.Yes)
            {
                return SaveCurrentSuite();
            }

            return true;
        }

        private void MarkSuiteDirty()
        {
            if (suppressDirtyTracking)
            {
                return;
            }

            isSuiteDirty = true;
            UpdateWindowTitle();
        }

        private void MarkSuiteClean()
        {
            isSuiteDirty = false;
            UpdateWindowTitle();
        }

        private void UpdateWindowTitle()
        {
            string suiteDisplayName = string.IsNullOrWhiteSpace(currentSuitePath) ?
                currentSuite.SuiteName :
                Path.GetFileName(currentSuitePath);

            if (string.IsNullOrWhiteSpace(suiteDisplayName))
            {
                suiteDisplayName = "Untitled Suite";
            }

            Text = "T.A.L.O.S - " + suiteDisplayName + (isSuiteDirty ? " *" : string.Empty);
        }

        private void RefreshSuiteMetadataEditor()
        {
            txtSuiteName.Text = currentSuite.SuiteName ?? string.Empty;
            edtTargetPath.Text = currentSuite.TargetAppPath ?? string.Empty;
            txtLaunchParams.Text = currentSuite.LaunchParams ?? string.Empty;
            numDefaultFixedDelay.Value = Math.Min(
                numDefaultFixedDelay.Maximum,
                Math.Max(numDefaultFixedDelay.Minimum, currentSuite.DefaultFixedDelayMs));
            chkAutoRelaunch.Checked = currentSuite.AutoRelaunchBetweenTests;
        }

        private void ApplySuiteMetadataFromEditor()
        {
            if (txtSuiteName == null)
            {
                return;
            }

            currentSuite.SuiteName = txtSuiteName.Text.Trim();
            currentSuite.TargetAppPath = edtTargetPath.Text.Trim();
            currentSuite.LaunchParams = txtLaunchParams.Text;
            currentSuite.DefaultFixedDelayMs = Convert.ToInt32(numDefaultFixedDelay.Value);
            currentSuite.AutoRelaunchBetweenTests = chkAutoRelaunch.Checked;
            UpdateWindowTitle();
        }

        private void ApplyCurrentTestCaseDetailsFromEditor()
        {
            if (currentTestCase == null || txtTestCaseName == null)
            {
                return;
            }

            string newName = txtTestCaseName.Text.Trim();

            if (!string.IsNullOrWhiteSpace(newName))
            {
                currentTestCase.Name = EnsureUniqueTestCaseName(newName, currentTestCase);
            }

            currentTestCase.Description = txtTestCaseDescription.Text ?? string.Empty;
        }

        private void RefreshTestCaseList()
        {
            refreshingTestCaseList = true;
            listBoxTestCase.Items.Clear();

            foreach (TestCase test in currentSuite.Tests)
            {
                listBoxTestCase.Items.Add(test.Name);
            }

            int selectedIndex = -1;

            if (currentTestCase != null)
            {
                selectedIndex = currentSuite.Tests.IndexOf(currentTestCase);
            }

            if (selectedIndex >= 0)
            {
                listBoxTestCase.SelectedIndex = selectedIndex;
            }
            else if (listBoxTestCase.Items.Count > 0)
            {
                listBoxTestCase.SelectedIndex = 0;
                currentTestCase = currentSuite.Tests[0];
            }
            else
            {
                currentTestCase = null;
            }

            txtTestCaseName.Text = currentTestCase == null ? string.Empty : currentTestCase.Name;
            refreshingTestCaseList = false;
            UpdateTestCaseButtons();
        }

        private void RefreshTestCaseDetails()
        {
            refreshingTestCaseDetails = true;

            if (currentTestCase == null)
            {
                txtTestCaseName.Text = string.Empty;
                txtTestCaseDescription.Text = string.Empty;
                txtTestCaseDescription.Enabled = false;
            }
            else
            {
                txtTestCaseName.Text = currentTestCase.Name ?? string.Empty;
                txtTestCaseDescription.Text = currentTestCase.Description ?? string.Empty;
                txtTestCaseDescription.Enabled = currentState != TalosUiState.Running;
            }

            refreshingTestCaseDetails = false;
        }

        private void SelectTestCaseByIndex(int index)
        {
            if (index >= 0 && index < currentSuite.Tests.Count)
            {
                currentTestCase = currentSuite.Tests[index];
            }
            else
            {
                currentTestCase = null;
            }

            RefreshTestCaseDetails();
            RefreshStepGrid();
            RefreshConditionGrids();
            UpdateTestCaseButtons();
        }

        private string GenerateDefaultTestCaseName()
        {
            int index = currentSuite.Tests.Count + 1;

            while (TestCaseNameExists("Recorded Test " + index, null))
            {
                index++;
            }

            return "Recorded Test " + index;
        }

        private string EnsureUniqueTestCaseName(string requestedName, TestCase ignoredTestCase)
        {
            if (!TestCaseNameExists(requestedName, ignoredTestCase))
            {
                return requestedName;
            }

            int suffix = 2;
            string candidate = requestedName + " " + suffix;

            while (TestCaseNameExists(candidate, ignoredTestCase))
            {
                suffix++;
                candidate = requestedName + " " + suffix;
            }

            return candidate;
        }

        private bool TestCaseNameExists(string name, TestCase ignoredTestCase)
        {
            foreach (TestCase test in currentSuite.Tests)
            {
                if (!object.ReferenceEquals(test, ignoredTestCase) &&
                    string.Equals(test.Name, name, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
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

        private void RefreshConditionGrids()
        {
            RefreshConditionGridsPreservingSelection(null, -1);
        }

        private void RefreshConditionGridsPreservingSelection(DataGridView selectedGrid, int selectedRowIndex)
        {
            refreshingConditionGrids = true;
            grdValueChecks.Rows.Clear();
            grdExpectedWindows.Rows.Clear();
            grdForbiddenWindows.Rows.Clear();

            if (currentTestCase != null)
            {
                EnsureConditions(currentTestCase);

                foreach (ValueCheck valueCheck in currentTestCase.Conditions.ValueChecks)
                {
                    grdValueChecks.Rows.Add(
                        valueCheck.Id,
                        FormatLocatorTarget(valueCheck.Locator),
                        valueCheck.ExpectedValue,
                        valueCheck.Locator == null ? string.Empty : valueCheck.Locator.ControlType);
                }

                foreach (ExpectedWindowCondition condition in currentTestCase.Conditions.ExpectedWindows)
                {
                    grdExpectedWindows.Rows.Add(
                        condition.Id,
                        condition.Pattern,
                        condition.MatchType.ToString(),
                        condition.MustAppearAtLeastOnce);
                }

                foreach (ForbiddenWindowCondition condition in currentTestCase.Conditions.ForbiddenWindows)
                {
                    grdForbiddenWindows.Rows.Add(
                        condition.Id,
                        condition.Pattern,
                        condition.MatchType.ToString());
                }
            }

            refreshingConditionGrids = false;
            SelectConditionGridRow(selectedGrid, selectedRowIndex);
            UpdateConditionButtons();
        }

        private void SelectConditionGridRow(DataGridView grid, int selectedRowIndex)
        {
            if (grid == null || selectedRowIndex < 0 || selectedRowIndex >= grid.Rows.Count)
            {
                return;
            }

            grid.ClearSelection();
            grid.Rows[selectedRowIndex].Selected = true;
            grid.CurrentCell = grid.Rows[selectedRowIndex].Cells[0];
        }

        private void UpdateConditionButtons()
        {
            bool canEditConditions = currentState != TalosUiState.Running && currentTestCase != null;
            btnAddValueCheck.Enabled = canEditConditions;
            btnRemoveValueCheck.Enabled = canEditConditions && grdValueChecks.Rows.Count > 0;
            btnAddExpectedWindow.Enabled = canEditConditions;
            btnRemoveExpectedWindow.Enabled = canEditConditions && grdExpectedWindows.Rows.Count > 0;
            btnAddForbiddenWindow.Enabled = canEditConditions;
            btnRemoveForbiddenWindow.Enabled = canEditConditions && grdForbiddenWindows.Rows.Count > 0;
        }

        private void EnsureConditions(TestCase testCase)
        {
            if (testCase.Conditions == null)
            {
                testCase.Conditions = new TestConditions();
            }

            if (testCase.Conditions.ValueChecks == null)
            {
                testCase.Conditions.ValueChecks = new List<ValueCheck>();
            }

            if (testCase.Conditions.ExpectedWindows == null)
            {
                testCase.Conditions.ExpectedWindows = new List<ExpectedWindowCondition>();
            }

            if (testCase.Conditions.ForbiddenWindows == null)
            {
                testCase.Conditions.ForbiddenWindows = new List<ForbiddenWindowCondition>();
            }
        }

        private string GenerateConditionId(string prefix, int startIndex)
        {
            int index = Math.Max(1, startIndex);
            string candidate = prefix + index;

            while (ConditionIdExists(candidate))
            {
                index++;
                candidate = prefix + index;
            }

            return candidate;
        }

        private bool ConditionIdExists(string id)
        {
            if (currentTestCase == null || string.IsNullOrEmpty(id))
            {
                return false;
            }

            EnsureConditions(currentTestCase);

            foreach (ValueCheck valueCheck in currentTestCase.Conditions.ValueChecks)
            {
                if (string.Equals(valueCheck.Id, id, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            foreach (ExpectedWindowCondition condition in currentTestCase.Conditions.ExpectedWindows)
            {
                if (string.Equals(condition.Id, id, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            foreach (ForbiddenWindowCondition condition in currentTestCase.Conditions.ForbiddenWindows)
            {
                if (string.Equals(condition.Id, id, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        private WindowPatternMatchType ParseWindowMatchType(string value)
        {
            WindowPatternMatchType parsed;

            if (Enum.TryParse(value, out parsed))
            {
                return parsed;
            }

            return WindowPatternMatchType.Substring;
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

            step.Parameters.Text = Convert.ToString(row.Cells["SetValueText"].Value) ?? string.Empty;
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
            MarkSuiteDirty();
            RefreshStepGridPreservingSelection(newIndex);
        }

        private void HandleRecordedLeftClick(Point screenPoint)
        {
            if (currentState != TalosUiState.Record || IsPointInsideTalosUi(screenPoint))
            {
                return;
            }

            UiElementInfo hoveredElement = currentHoverElement;
            HideHighlightOverlay();

            UiElementInfo clickedElement = GetElementAtPointOrNull(screenPoint);

            if (!IsRecordableElement(clickedElement))
            {
                clickedElement = GetHoveredElementRecordingFallback(hoveredElement, screenPoint);
            }

            if (!IsRecordableElement(clickedElement))
            {
                return;
            }

            lastMousePosition = screenPoint;
            lastResolvedMousePosition = screenPoint;
            mouseStableSinceUtc = DateTime.UtcNow;
            OnHoveredElementChanged(clickedElement);
            AppendRecordedStep(clickedElement);
            ShowInvokedHighlightFeedback(clickedElement);
        }

        private UiElementInfo GetElementAtPointOrNull(Point screenPoint)
        {
            try
            {
                return uiAutomationService.GetElementAtPoint(screenPoint.X, screenPoint.Y);
            }
            catch (Exception)
            {
                return null;
            }
        }

        private UiElementInfo GetHoveredElementRecordingFallback(UiElementInfo hoveredElement, Point screenPoint)
        {
            if (!IsRecordableElement(hoveredElement) || !ElementBoundsContainPoint(hoveredElement, screenPoint))
            {
                return null;
            }

            return hoveredElement;
        }

        private bool IsRecordableElement(UiElementInfo element)
        {
            return element != null && element.ProcessId != Process.GetCurrentProcess().Id;
        }

        private bool ElementBoundsContainPoint(UiElementInfo element, Point screenPoint)
        {
            if (element == null || element.BoundingRectangle == null || element.BoundingRectangle.IsEmpty)
            {
                return false;
            }

            PersistedRectangle bounds = element.BoundingRectangle;
            return screenPoint.X >= bounds.X &&
                screenPoint.X <= bounds.X + bounds.Width &&
                screenPoint.Y >= bounds.Y &&
                screenPoint.Y <= bounds.Y + bounds.Height;
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
            MarkSuiteDirty();
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

            return FormatLocatorTarget(step.Locator);
        }

        private string FormatLocatorTarget(ElementLocator locator)
        {
            if (locator == null)
            {
                return string.Empty;
            }

            List<string> parts = new List<string>();

            if (!string.IsNullOrEmpty(locator.AutomationId))
            {
                parts.Add("AutomationId=" + locator.AutomationId);
            }

            if (!string.IsNullOrEmpty(locator.Name))
            {
                parts.Add("Name=" + locator.Name);
            }

            if (parts.Count == 0 && !string.IsNullOrEmpty(locator.ClassName))
            {
                parts.Add("Class=" + locator.ClassName);
            }

            return parts.Count == 0 ? locator.ControlType : string.Join(" | ", parts.ToArray());
        }

        private TestCase CopyTestCase(TestCase source)
        {
            TestCase copy = new TestCase();
            copy.Name = source.Name ?? string.Empty;
            copy.Description = source.Description ?? string.Empty;
            copy.Steps = new List<Step>();

            if (source.Steps != null)
            {
                foreach (Step step in source.Steps)
                {
                    copy.Steps.Add(CopyStep(step));
                }
            }

            copy.Conditions = CopyConditions(source.Conditions);
            return copy;
        }

        private Step CopyStep(Step source)
        {
            Step copy = new Step();

            if (source == null)
            {
                return copy;
            }

            copy.Id = source.Id;
            copy.Action = source.Action;
            copy.FixedDelayMs = source.FixedDelayMs;
            copy.Locator = CopyLocator(source.Locator);
            copy.Parameters = CopyStepParameters(source.Parameters);
            return copy;
        }

        private StepParameters CopyStepParameters(StepParameters source)
        {
            StepParameters copy = new StepParameters();

            if (source == null)
            {
                return copy;
            }

            copy.Text = source.Text ?? string.Empty;
            copy.SendKeysText = source.SendKeysText ?? string.Empty;
            copy.SelectedItem = source.SelectedItem ?? string.Empty;
            return copy;
        }

        private TestConditions CopyConditions(TestConditions source)
        {
            TestConditions copy = new TestConditions();

            if (source == null)
            {
                return copy;
            }

            if (source.ValueChecks != null)
            {
                foreach (ValueCheck valueCheck in source.ValueChecks)
                {
                    ValueCheck valueCopy = new ValueCheck();
                    valueCopy.Id = valueCheck.Id ?? string.Empty;
                    valueCopy.ExpectedValue = valueCheck.ExpectedValue ?? string.Empty;
                    valueCopy.EvaluationPoint = valueCheck.EvaluationPoint;
                    valueCopy.Locator = CopyLocator(valueCheck.Locator);
                    copy.ValueChecks.Add(valueCopy);
                }
            }

            if (source.ExpectedWindows != null)
            {
                foreach (ExpectedWindowCondition condition in source.ExpectedWindows)
                {
                    ExpectedWindowCondition conditionCopy = new ExpectedWindowCondition();
                    CopyWindowCondition(condition, conditionCopy);
                    conditionCopy.MustAppearAtLeastOnce = condition.MustAppearAtLeastOnce;
                    copy.ExpectedWindows.Add(conditionCopy);
                }
            }

            if (source.ForbiddenWindows != null)
            {
                foreach (ForbiddenWindowCondition condition in source.ForbiddenWindows)
                {
                    ForbiddenWindowCondition conditionCopy = new ForbiddenWindowCondition();
                    CopyWindowCondition(condition, conditionCopy);
                    copy.ForbiddenWindows.Add(conditionCopy);
                }
            }

            return copy;
        }

        private void CopyWindowCondition(WindowConditionBase source, WindowConditionBase target)
        {
            target.Id = source.Id ?? string.Empty;
            target.Pattern = source.Pattern ?? string.Empty;
            target.MatchType = source.MatchType;
        }

        private ElementLocator CopyLocator(ElementLocator source)
        {
            ElementLocator copy = new ElementLocator();

            if (source == null)
            {
                return copy;
            }

            copy.AutomationId = source.AutomationId ?? string.Empty;
            copy.Name = source.Name ?? string.Empty;
            copy.ControlType = source.ControlType ?? string.Empty;
            copy.ClassName = source.ClassName ?? string.Empty;
            copy.ProcessId = source.ProcessId;
            copy.BoundingRectangle = PersistedRectangle.Copy(source.BoundingRectangle);
            copy.AncestorPath = AncestorDescriptor.CopyList(source.AncestorPath);
            return copy;
        }

        private bool ValidateSuiteForSaveOrRun(bool requireTargetPath, bool showMessage)
        {
            ApplySuiteMetadataFromEditor();

            List<string> errors = new List<string>();

            if (string.IsNullOrWhiteSpace(currentSuite.SuiteName))
            {
                errors.Add("Suite name is required.");
            }

            if (requireTargetPath)
            {
                if (string.IsNullOrWhiteSpace(currentSuite.TargetAppPath))
                {
                    errors.Add("Target exe is required.");
                }
                else if (!File.Exists(currentSuite.TargetAppPath))
                {
                    errors.Add("Target exe does not exist.");
                }
                else if (!string.Equals(Path.GetExtension(currentSuite.TargetAppPath), ".exe", StringComparison.OrdinalIgnoreCase))
                {
                    errors.Add("Target path must point to an .exe file.");
                }
            }

            if (currentSuite.Tests == null || currentSuite.Tests.Count == 0)
            {
                errors.Add("At least one test case is required.");
            }
            else
            {
                ValidateTestCases(errors);
            }

            if (errors.Count > 0 && showMessage)
            {
                MessageBox.Show(
                    string.Join(Environment.NewLine, errors.ToArray()),
                    "Suite validation",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }

            return errors.Count == 0;
        }

        private void ValidateTestCases(List<string> errors)
        {
            HashSet<string> names = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (TestCase testCase in currentSuite.Tests)
            {
                if (testCase == null || string.IsNullOrWhiteSpace(testCase.Name))
                {
                    errors.Add("Every test case needs a name.");
                    continue;
                }

                if (!names.Add(testCase.Name))
                {
                    errors.Add("Test case names must be unique: " + testCase.Name);
                }

                EnsureConditions(testCase);
                ValidateWindowConditions(errors, testCase.Name, testCase.Conditions.ExpectedWindows, "expected");
                ValidateWindowConditions(errors, testCase.Name, testCase.Conditions.ForbiddenWindows, "forbidden");
            }
        }

        private void ValidateWindowConditions<TCondition>(
            List<string> errors,
            string testCaseName,
            IEnumerable<TCondition> conditions,
            string conditionType)
            where TCondition : WindowConditionBase
        {
            if (conditions == null)
            {
                return;
            }

            foreach (TCondition condition in conditions)
            {
                if (string.IsNullOrWhiteSpace(condition.Pattern))
                {
                    errors.Add("The " + conditionType + " window condition in " + testCaseName + " needs a pattern.");
                }
            }
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

        private void ShowInvokedHighlightFeedback(UiElementInfo element)
        {
            if (element == null || element.BoundingRectangle == null || element.BoundingRectangle.IsEmpty)
            {
                return;
            }

            if (highlighterWindow == null)
            {
                highlighterWindow = new HighlighterWindow();
            }

            highlighterWindow.ShowInvokedFeedback(element.BoundingRectangle);
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
            bool canEdit = currentState != TalosUiState.Running;
            btnStartInspect.Enabled = currentState == TalosUiState.Idle;
            btnStopInspect.Enabled = currentState == TalosUiState.Inspect || currentState == TalosUiState.Record;
            btnRecord.Enabled = currentState != TalosUiState.Running;
            btnStartRecord.Enabled = currentState == TalosUiState.Idle || currentState == TalosUiState.Inspect;
            btnStopRecord.Enabled = currentState == TalosUiState.Record;
            edtTargetPath.Enabled = canEdit;
            btnSelectTarget.Enabled = canEdit;
            txtSuiteName.Enabled = canEdit;
            txtLaunchParams.Enabled = canEdit;
            numDefaultFixedDelay.Enabled = canEdit;
            chkAutoRelaunch.Enabled = canEdit;
            listBoxTestCase.Enabled = canEdit;
            txtTestCaseName.Enabled = canEdit && currentTestCase != null;
            txtTestCaseDescription.Enabled = canEdit && currentTestCase != null;
            grdRecordedSteps.Enabled = canEdit && currentTestCase != null;
            grdValueChecks.Enabled = canEdit && currentTestCase != null;
            grdExpectedWindows.Enabled = canEdit && currentTestCase != null;
            grdForbiddenWindows.Enabled = canEdit && currentTestCase != null;
            UpdateTestCaseButtons();
            UpdateConditionButtons();
        }

        private void UpdateTestCaseButtons()
        {
            bool canEditTestCases = currentState != TalosUiState.Running;
            btnAddTestCase.Enabled = canEditTestCases;
            btnRemoveTestCase.Enabled = canEditTestCases && currentTestCase != null;
            btnSaveTestCase.Enabled = canEditTestCases && currentTestCase != null;
            btnDuplicateTestCase.Enabled = canEditTestCases && currentTestCase != null;
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
            if (!ConfirmDiscardUnsavedChanges())
            {
                e.Cancel = true;
                return;
            }

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
