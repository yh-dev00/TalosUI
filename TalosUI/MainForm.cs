using System;
using System.Drawing;
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
        private string sTargetPath = string.Empty;

        private readonly ITargetProcessManager targetProcessManager;
        private readonly IUiAutomationService uiAutomationService;
        private readonly Timer inspectHoverTimer;
        private TalosUiState currentState;
        private Point lastMousePosition;
        private Point lastResolvedMousePosition;
        private DateTime mouseStableSinceUtc;
        private UiElementInfo currentHoverElement;
        private HighlighterWindow highlighterWindow;

        public MainForm()
        {
            InitializeComponent();
            targetProcessManager = new TargetProcessManager();
            uiAutomationService = new UiAutomationService();
            inspectHoverTimer = new Timer(components);
            inspectHoverTimer.Interval = InspectPollIntervalMs;
            inspectHoverTimer.Tick += inspectHoverTimer_Tick;
            currentState = TalosUiState.Idle;
            lastMousePosition = Cursor.Position;
            lastResolvedMousePosition = Point.Empty;
            mouseStableSinceUtc = DateTime.UtcNow;
            UpdateUiState();
            ClearInspectedElementDetails();
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
            }
            else
            {
                inspectHoverTimer.Stop();
                currentHoverElement = null;
                ClearInspectedElementDetails();
                HideHighlightOverlay();
            }

            UpdateUiState();
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
    }

    internal enum TalosUiState
    {
        Idle,
        Inspect,
        Record,
        Running
    }
}
