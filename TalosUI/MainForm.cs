using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

using TalosCore;
using System.Threading;
using System.Windows.Automation;
namespace TalosUI
{
    public partial class MainForm : Form
    {
        //private string sTargetPath = "D:\\Self-Learning\\Cpp\\ViCAT_MASTER_TEST\\x64\\Release\\VICAT_MASTER_LT_TEST.exe";
        private string sTargetPath = "C:\\Program Files (x86)\\ViE Technologies\\Application\\ViCAT-MASTER-LT\\ViCAT-MASTER-LT.exe";

        private Process myProcess = null;
        private CTalosCore talos;
        public MainForm()
        {
            InitializeComponent();
        }

        private void btnSelectTarget_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Executable files (*.exe)|*.exe|All files (*.*)|*.*";
            openFileDialog.Title = "Select Target Application";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                sTargetPath = openFileDialog.FileName;
                //txtTargetPath.Text = sTargetPath;
                //StartTargetApplication();
            }
        }

        private void btnRecord_Click(object sender, EventArgs e)
        {
           
        }

        private void OnHoveredElementChanged(AutomationElement elementFound)
        {
          
        }
    }
}
