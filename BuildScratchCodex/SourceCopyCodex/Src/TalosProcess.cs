using System;
using System.Diagnostics;
using System.IO;

namespace TalosCore
{
    public interface ITargetProcessManager
    {
        bool IsRunning { get; }
        bool HasCrashed { get; }
        int ProcessId { get; }
        string TargetPath { get; }
        string LaunchArguments { get; }

        Process Launch(string targetPath, string launchArguments);
        Process EnsureRunning(string targetPath, string launchArguments);
        Process Relaunch(string targetPath, string launchArguments, int gracefulCloseTimeoutMs);
        bool Close(int gracefulCloseTimeoutMs);
        void Kill();
        void Refresh();
    }

    public class TargetProcessManager : ITargetProcessManager
    {
        private Process targetProcess;
        private bool closeRequested;
        private bool hasCrashed;

        public bool IsRunning
        {
            get
            {
                Refresh();
                return targetProcess != null && !targetProcess.HasExited;
            }
        }

        public bool HasCrashed
        {
            get
            {
                Refresh();
                return hasCrashed;
            }
        }

        public int ProcessId
        {
            get
            {
                Refresh();

                if (targetProcess == null || targetProcess.HasExited)
                {
                    return 0;
                }

                return targetProcess.Id;
            }
        }

        public string TargetPath { get; private set; }
        public string LaunchArguments { get; private set; }

        public TargetProcessManager()
        {
            TargetPath = string.Empty;
            LaunchArguments = string.Empty;
        }

        public Process Launch(string targetPath, string launchArguments)
        {
            ValidateTargetPath(targetPath);

            if (IsRunning)
            {
                throw new InvalidOperationException("A target process is already running. Close or relaunch it before launching another target.");
            }

            TargetPath = targetPath;
            LaunchArguments = launchArguments ?? string.Empty;
            closeRequested = false;
            hasCrashed = false;

            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = TargetPath;
            startInfo.Arguments = LaunchArguments;
            startInfo.WorkingDirectory = Path.GetDirectoryName(TargetPath);
            startInfo.UseShellExecute = false;

            targetProcess = Process.Start(startInfo);

            if (targetProcess == null)
            {
                throw new InvalidOperationException("The target process could not be started.");
            }

            return targetProcess;
        }

        public Process EnsureRunning(string targetPath, string launchArguments)
        {
            if (IsRunning)
            {
                return targetProcess;
            }

            return Launch(targetPath, launchArguments);
        }

        public Process Relaunch(string targetPath, string launchArguments, int gracefulCloseTimeoutMs)
        {
            Close(gracefulCloseTimeoutMs);
            return Launch(targetPath, launchArguments);
        }

        public bool Close(int gracefulCloseTimeoutMs)
        {
            Refresh();

            if (targetProcess == null || targetProcess.HasExited)
            {
                closeRequested = true;
                return true;
            }

            closeRequested = true;

            try
            {
                bool closeMessageSent = false;

                if (targetProcess.MainWindowHandle != IntPtr.Zero)
                {
                    closeMessageSent = targetProcess.CloseMainWindow();
                }

                if (closeMessageSent && targetProcess.WaitForExit(Math.Max(0, gracefulCloseTimeoutMs)))
                {
                    return true;
                }

                Kill();
                return false;
            }
            catch (InvalidOperationException)
            {
                return true;
            }
        }

        public void Kill()
        {
            Refresh();

            if (targetProcess == null || targetProcess.HasExited)
            {
                return;
            }

            closeRequested = true;
            targetProcess.Kill();
            targetProcess.WaitForExit();
        }

        public void Refresh()
        {
            if (targetProcess == null)
            {
                return;
            }

            try
            {
                targetProcess.Refresh();

                if (targetProcess.HasExited && !closeRequested)
                {
                    hasCrashed = true;
                }
            }
            catch (InvalidOperationException)
            {
                if (!closeRequested)
                {
                    hasCrashed = true;
                }
            }
        }

        private static void ValidateTargetPath(string targetPath)
        {
            if (string.IsNullOrWhiteSpace(targetPath))
            {
                throw new ArgumentException("Target application path is required.", "targetPath");
            }

            if (!Path.IsPathRooted(targetPath))
            {
                throw new ArgumentException("Target application path must be an absolute path.", "targetPath");
            }

            if (!File.Exists(targetPath))
            {
                throw new FileNotFoundException("Target application path does not exist.", targetPath);
            }

            if (!string.Equals(Path.GetExtension(targetPath), ".exe", StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException("Target application path must point to an .exe file.", "targetPath");
            }
        }
    }
}
