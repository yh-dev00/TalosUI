using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Text;
using System.Web.Script.Serialization;
using System.Windows.Forms;

namespace TalosCore
{
    public class CTalosLog
    {
        private const string TimestampFileFormat = "yyyyMMdd_HHmmss";
        private const string TimestampDisplayFormat = "yyyy-MM-dd HH:mm:ss 'UTC'";

        private readonly string outputRoot;
        private string runDirectory;
        private string logPath;
        private string textReportPath;
        private string jsonReportPath;

        public CTalosLog()
            : this(null)
        {
        }

        public CTalosLog(string outputRoot)
        {
            this.outputRoot = string.IsNullOrWhiteSpace(outputRoot)
                ? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Reports")
                : outputRoot;
            runDirectory = string.Empty;
            logPath = string.Empty;
            textReportPath = string.Empty;
            jsonReportPath = string.Empty;
        }

        public string RunDirectory
        {
            get { return runDirectory; }
        }

        public string LogPath
        {
            get { return logPath; }
        }

        public string TextReportPath
        {
            get { return textReportPath; }
        }

        public string JsonReportPath
        {
            get { return jsonReportPath; }
        }

        public virtual void StartRun(TestSuiteRunResult suiteResult)
        {
            if (suiteResult == null)
            {
                throw new ArgumentNullException("suiteResult");
            }

            DateTime startedUtc = suiteResult.StartedUtc == DateTime.MinValue
                ? DateTime.UtcNow
                : suiteResult.StartedUtc;
            suiteResult.StartedUtc = startedUtc;

            runDirectory = CreateRunDirectory(suiteResult, startedUtc);
            logPath = Path.Combine(runDirectory, "run.log");
            textReportPath = Path.Combine(runDirectory, "report.txt");
            jsonReportPath = Path.Combine(runDirectory, "report.json");

            suiteResult.OutputDirectory = runDirectory;
            suiteResult.LogPath = logPath;
            suiteResult.TextReportPath = textReportPath;
            suiteResult.JsonReportPath = jsonReportPath;

            AppendLogLine("Run started: " + FormatTimestamp(startedUtc));
            AppendLogLine("Suite: " + NullToEmpty(suiteResult.SuiteName));
            AppendLogLine("Target: " + NullToEmpty(suiteResult.TargetAppPath));
        }

        public virtual void EndRun(TestSuiteRunResult suiteResult)
        {
            if (suiteResult == null)
            {
                throw new ArgumentNullException("suiteResult");
            }

            EnsureRunPaths(suiteResult);

            if (suiteResult.EndedUtc == DateTime.MinValue)
            {
                suiteResult.EndedUtc = DateTime.UtcNow;
            }

            AppendLogLine("Run ended: " + FormatTimestamp(suiteResult.EndedUtc));
            AppendLogLine("Status: " + suiteResult.Status);
            WriteTextReport(suiteResult);
            WriteJsonReport(suiteResult);
        }

        public virtual void LogTestResult(TestCaseRunResult testResult)
        {
            if (testResult == null)
            {
                return;
            }

            AppendLogLine(
                "Test " + Quote(testResult.TestName) + " " + testResult.Status +
                FormatFailureReason(testResult.FailureReason));
        }

        public virtual void LogStepResult(TestCaseRunResult testResult, StepRunResult stepResult)
        {
            if (stepResult == null)
            {
                return;
            }

            string testName = testResult == null ? string.Empty : testResult.TestName;
            AppendLogLine(
                "Step " + stepResult.StepId + " (" + stepResult.Action + ") in " +
                Quote(testName) + " " + stepResult.Status + ": " + NullToEmpty(stepResult.Message));
        }

        public virtual string CaptureFailureScreenshot(TestCaseRunResult testResult, StepRunResult stepResult)
        {
            try
            {
                EnsureRunPaths(null);

                string screenshotsDirectory = Path.Combine(runDirectory, "screenshots");
                Directory.CreateDirectory(screenshotsDirectory);

                string testName = testResult == null ? "test" : testResult.TestName;
                int stepId = stepResult == null ? 0 : stepResult.StepId;
                string fileName = BuildScreenshotFileName(testName, stepId);
                string path = Path.Combine(screenshotsDirectory, fileName);

                Rectangle bounds = GetVirtualScreenBounds();

                using (Bitmap bitmap = new Bitmap(bounds.Width, bounds.Height))
                using (Graphics graphics = Graphics.FromImage(bitmap))
                {
                    graphics.CopyFromScreen(bounds.Left, bounds.Top, 0, 0, bounds.Size);
                    bitmap.Save(path, ImageFormat.Png);
                }

                AppendLogLine("Captured failure screenshot: " + path);
                return path;
            }
            catch (Exception ex)
            {
                AppendLogLine("Screenshot capture failed: " + ex.Message);
                return string.Empty;
            }
        }

        private string CreateRunDirectory(TestSuiteRunResult suiteResult, DateTime startedUtc)
        {
            string suiteName = suiteResult == null ? "Suite" : suiteResult.SuiteName;
            string directoryName = startedUtc.ToString(TimestampFileFormat, CultureInfo.InvariantCulture) +
                "Z_" + SanitizeFileName(suiteName);
            string directory = Path.Combine(outputRoot, directoryName);

            int suffix = 2;
            string uniqueDirectory = directory;

            while (Directory.Exists(uniqueDirectory))
            {
                uniqueDirectory = directory + "_" + suffix.ToString(CultureInfo.InvariantCulture);
                suffix++;
            }

            Directory.CreateDirectory(uniqueDirectory);
            return uniqueDirectory;
        }

        private void EnsureRunPaths(TestSuiteRunResult suiteResult)
        {
            if (string.IsNullOrEmpty(runDirectory))
            {
                DateTime startedUtc = suiteResult == null || suiteResult.StartedUtc == DateTime.MinValue
                    ? DateTime.UtcNow
                    : suiteResult.StartedUtc;
                runDirectory = CreateRunDirectory(suiteResult, startedUtc);
            }

            if (string.IsNullOrEmpty(logPath))
            {
                logPath = Path.Combine(runDirectory, "run.log");
            }

            if (string.IsNullOrEmpty(textReportPath))
            {
                textReportPath = Path.Combine(runDirectory, "report.txt");
            }

            if (string.IsNullOrEmpty(jsonReportPath))
            {
                jsonReportPath = Path.Combine(runDirectory, "report.json");
            }

            if (suiteResult != null)
            {
                suiteResult.OutputDirectory = runDirectory;
                suiteResult.LogPath = logPath;
                suiteResult.TextReportPath = textReportPath;
                suiteResult.JsonReportPath = jsonReportPath;
            }
        }

        private void AppendLogLine(string line)
        {
            try
            {
                if (string.IsNullOrEmpty(logPath))
                {
                    EnsureRunPaths(null);
                }

                string entry = FormatTimestamp(DateTime.UtcNow) + "  " + NullToEmpty(line) + Environment.NewLine;
                File.AppendAllText(logPath, entry, Encoding.UTF8);
            }
            catch
            {
                // Logging must not change test execution results.
            }
        }

        private void WriteTextReport(TestSuiteRunResult suiteResult)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("TalosUI Test Run Report");
            builder.AppendLine("=======================");
            builder.AppendLine("Suite: " + NullToEmpty(suiteResult.SuiteName));
            builder.AppendLine("Target: " + NullToEmpty(suiteResult.TargetAppPath));
            builder.AppendLine("Status: " + suiteResult.Status);
            builder.AppendLine("Started: " + FormatTimestamp(suiteResult.StartedUtc));
            builder.AppendLine("Ended: " + FormatTimestamp(suiteResult.EndedUtc));
            builder.AppendLine("Passed: " + suiteResult.PassedCount.ToString(CultureInfo.InvariantCulture));
            builder.AppendLine("Failed: " + suiteResult.FailedCount.ToString(CultureInfo.InvariantCulture));

            if (!string.IsNullOrEmpty(suiteResult.FailureReason))
            {
                builder.AppendLine("Run failure: " + suiteResult.FailureReason);
            }

            builder.AppendLine();
            builder.AppendLine("Tests");
            builder.AppendLine("-----");

            foreach (TestCaseRunResult test in SafeTests(suiteResult))
            {
                builder.AppendLine(NullToEmpty(test.TestName) + ": " + test.Status);

                if (!string.IsNullOrEmpty(test.FailureReason))
                {
                    builder.AppendLine("  Failure: " + test.FailureReason);
                }

                if (!string.IsNullOrEmpty(test.ScreenshotPath))
                {
                    builder.AppendLine("  Screenshot: " + test.ScreenshotPath);
                }

                foreach (StepRunResult step in SafeSteps(test))
                {
                    builder.AppendLine(
                        "  Step " + step.StepId.ToString(CultureInfo.InvariantCulture) +
                        " [" + step.Action + "]: " + step.Status +
                        (string.IsNullOrEmpty(step.Message) ? string.Empty : " - " + step.Message));

                    if (!string.IsNullOrEmpty(step.ScreenshotPath))
                    {
                        builder.AppendLine("    Screenshot: " + step.ScreenshotPath);
                    }
                }
            }

            File.WriteAllText(textReportPath, builder.ToString(), Encoding.UTF8);
        }

        private void WriteJsonReport(TestSuiteRunResult suiteResult)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            serializer.MaxJsonLength = int.MaxValue;

            Dictionary<string, object> report = new Dictionary<string, object>();
            report["GeneratedUtc"] = ToIsoUtc(DateTime.UtcNow);
            report["MachineName"] = Environment.MachineName;
            report["SuiteName"] = NullToEmpty(suiteResult.SuiteName);
            report["TargetAppPath"] = NullToEmpty(suiteResult.TargetAppPath);
            report["Status"] = suiteResult.Status.ToString();
            report["StartedUtc"] = ToIsoUtc(suiteResult.StartedUtc);
            report["EndedUtc"] = ToIsoUtc(suiteResult.EndedUtc);
            report["PassedCount"] = suiteResult.PassedCount;
            report["FailedCount"] = suiteResult.FailedCount;
            report["FailureReason"] = NullToEmpty(suiteResult.FailureReason);
            report["OutputDirectory"] = NullToEmpty(suiteResult.OutputDirectory);
            report["LogPath"] = NullToEmpty(suiteResult.LogPath);
            report["TextReportPath"] = NullToEmpty(suiteResult.TextReportPath);
            report["JsonReportPath"] = NullToEmpty(suiteResult.JsonReportPath);
            report["Tests"] = BuildTestReports(suiteResult);

            string json = FormatJson(serializer.Serialize(report));
            File.WriteAllText(jsonReportPath, json, Encoding.UTF8);
        }

        private List<object> BuildTestReports(TestSuiteRunResult suiteResult)
        {
            List<object> tests = new List<object>();

            foreach (TestCaseRunResult test in SafeTests(suiteResult))
            {
                Dictionary<string, object> testReport = new Dictionary<string, object>();
                testReport["TestName"] = NullToEmpty(test.TestName);
                testReport["Status"] = test.Status.ToString();
                testReport["StartedUtc"] = ToIsoUtc(test.StartedUtc);
                testReport["EndedUtc"] = ToIsoUtc(test.EndedUtc);
                testReport["FailureReason"] = NullToEmpty(test.FailureReason);
                testReport["ScreenshotPath"] = NullToEmpty(test.ScreenshotPath);
                testReport["Steps"] = BuildStepReports(test);
                tests.Add(testReport);
            }

            return tests;
        }

        private List<object> BuildStepReports(TestCaseRunResult test)
        {
            List<object> steps = new List<object>();

            foreach (StepRunResult step in SafeSteps(test))
            {
                Dictionary<string, object> stepReport = new Dictionary<string, object>();
                stepReport["StepId"] = step.StepId;
                stepReport["Action"] = step.Action.ToString();
                stepReport["Status"] = step.Status.ToString();
                stepReport["StartedUtc"] = ToIsoUtc(step.StartedUtc);
                stepReport["EndedUtc"] = ToIsoUtc(step.EndedUtc);
                stepReport["Message"] = NullToEmpty(step.Message);
                stepReport["ScreenshotPath"] = NullToEmpty(step.ScreenshotPath);
                steps.Add(stepReport);
            }

            return steps;
        }

        private string BuildScreenshotFileName(string testName, int stepId)
        {
            string stepPart = stepId > 0
                ? "_step-" + stepId.ToString(CultureInfo.InvariantCulture)
                : string.Empty;
            return DateTime.UtcNow.ToString(TimestampFileFormat, CultureInfo.InvariantCulture) +
                "Z_" + SanitizeFileName(testName) + stepPart + ".png";
        }

        private static Rectangle GetVirtualScreenBounds()
        {
            Rectangle bounds = Screen.PrimaryScreen == null
                ? new Rectangle(0, 0, 1, 1)
                : Screen.PrimaryScreen.Bounds;

            foreach (Screen screen in Screen.AllScreens)
            {
                bounds = Rectangle.Union(bounds, screen.Bounds);
            }

            return bounds.Width <= 0 || bounds.Height <= 0
                ? new Rectangle(0, 0, 1, 1)
                : bounds;
        }

        private static string SanitizeFileName(string value)
        {
            string text = string.IsNullOrWhiteSpace(value) ? "Suite" : value.Trim();
            char[] invalidCharacters = Path.GetInvalidFileNameChars();
            StringBuilder builder = new StringBuilder();

            foreach (char character in text)
            {
                bool invalid = false;

                for (int i = 0; i < invalidCharacters.Length; i++)
                {
                    if (character == invalidCharacters[i])
                    {
                        invalid = true;
                        break;
                    }
                }

                if (invalid || char.IsWhiteSpace(character))
                {
                    builder.Append('_');
                }
                else
                {
                    builder.Append(character);
                }
            }

            string sanitized = builder.ToString().Trim('_');

            if (sanitized.Length == 0)
            {
                sanitized = "Suite";
            }

            return sanitized.Length > 80 ? sanitized.Substring(0, 80) : sanitized;
        }

        private static string FormatTimestamp(DateTime value)
        {
            if (value == DateTime.MinValue)
            {
                return string.Empty;
            }

            return value.ToUniversalTime().ToString(TimestampDisplayFormat, CultureInfo.InvariantCulture);
        }

        private static string ToIsoUtc(DateTime value)
        {
            if (value == DateTime.MinValue)
            {
                return string.Empty;
            }

            return value.ToUniversalTime().ToString("o", CultureInfo.InvariantCulture);
        }

        private static string Quote(string value)
        {
            return "'" + NullToEmpty(value) + "'";
        }

        private static string FormatFailureReason(string reason)
        {
            return string.IsNullOrEmpty(reason) ? string.Empty : " - " + reason;
        }

        private static string NullToEmpty(string value)
        {
            return value ?? string.Empty;
        }

        private static IEnumerable<TestCaseRunResult> SafeTests(TestSuiteRunResult suiteResult)
        {
            if (suiteResult == null || suiteResult.Tests == null)
            {
                return new List<TestCaseRunResult>();
            }

            return suiteResult.Tests;
        }

        private static IEnumerable<StepRunResult> SafeSteps(TestCaseRunResult test)
        {
            if (test == null || test.Steps == null)
            {
                return new List<StepRunResult>();
            }

            return test.Steps;
        }

        private static string FormatJson(string json)
        {
            StringBuilder builder = new StringBuilder();
            int indent = 0;
            bool inString = false;
            bool escaped = false;

            for (int i = 0; i < json.Length; i++)
            {
                char current = json[i];

                if (inString)
                {
                    builder.Append(current);

                    if (escaped)
                    {
                        escaped = false;
                    }
                    else if (current == '\\')
                    {
                        escaped = true;
                    }
                    else if (current == '"')
                    {
                        inString = false;
                    }

                    continue;
                }

                switch (current)
                {
                    case '"':
                        inString = true;
                        builder.Append(current);
                        break;
                    case '{':
                    case '[':
                        builder.Append(current);
                        builder.AppendLine();
                        indent++;
                        AppendIndent(builder, indent);
                        break;
                    case '}':
                    case ']':
                        builder.AppendLine();
                        indent--;
                        AppendIndent(builder, indent);
                        builder.Append(current);
                        break;
                    case ',':
                        builder.Append(current);
                        builder.AppendLine();
                        AppendIndent(builder, indent);
                        break;
                    case ':':
                        builder.Append(": ");
                        break;
                    default:
                        builder.Append(current);
                        break;
                }
            }

            return builder.ToString();
        }

        private static void AppendIndent(StringBuilder builder, int indent)
        {
            for (int i = 0; i < indent; i++)
            {
                builder.Append("  ");
            }
        }
    }
}
