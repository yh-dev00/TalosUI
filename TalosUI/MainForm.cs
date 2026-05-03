using System;
using System.Windows.Forms;
using System.Windows.Automation;
using TalosCore;

namespace TalosUI
{
    public partial class MainForm : Form
    {
        private const int GracefulCloseTimeoutMs = 5000;
        private string sTargetPath = string.Empty;

        private readonly ITargetProcessManager targetProcessManager;
        public MainForm()
        {
            InitializeComponent();
            targetProcessManager = new TargetProcessManager();
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

        private void OnHoveredElementChanged(AutomationElement elementFound)
        {
          
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (targetProcessManager != null && targetProcessManager.IsRunning)
            {
                targetProcessManager.Close(GracefulCloseTimeoutMs);
            }

            base.OnFormClosing(e);
        }
    }
}
