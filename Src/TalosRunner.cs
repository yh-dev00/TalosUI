using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;

namespace TalosCore
{
    public enum RunResultStatus
    {
        NotStarted,
        Running,
        Passed,
        Failed
    }

    public enum TestRunnerProgressKind
    {
        SuiteStarted,
        SuiteCompleted,
        TestStarted,
        TestCompleted,
        StepStarted,
        StepCompleted,
        Message
    }

    public class TestSuiteRunResult
    {
        public TestSuiteRunResult()
        {
            SuiteName = string.Empty;
            TargetAppPath = string.Empty;
            Status = RunResultStatus.NotStarted;
            Tests = new List<TestCaseRunResult>();
            FailureReason = string.Empty;
            OutputDirectory = string.Empty;
            LogPath = string.Empty;
            TextReportPath = string.Empty;
            JsonReportPath = string.Empty;
        }

        public string SuiteName { get; set; }
        public string TargetAppPath { get; set; }
        public RunResultStatus Status { get; set; }
        public DateTime StartedUtc { get; set; }
        public DateTime EndedUtc { get; set; }
        public List<TestCaseRunResult> Tests { get; set; }
        public string FailureReason { get; set; }
        public string OutputDirectory { get; set; }
        public string LogPath { get; set; }
        public string TextReportPath { get; set; }
        public string JsonReportPath { get; set; }

        public int PassedCount
        {
            get { return CountTestsWithStatus(RunResultStatus.Passed); }
        }

        public int FailedCount
        {
            get { return CountTestsWithStatus(RunResultStatus.Failed); }
        }

        private int CountTestsWithStatus(RunResultStatus status)
        {
            int count = 0;

            foreach (TestCaseRunResult test in Tests)
            {
                if (test != null && test.Status == status)
                {
                    count++;
                }
            }

            return count;
        }
    }

    public class TestCaseRunResult
    {
        public TestCaseRunResult()
        {
            TestName = string.Empty;
            Status = RunResultStatus.NotStarted;
            FailureReason = string.Empty;
            ScreenshotPath = string.Empty;
            Steps = new List<StepRunResult>();
        }

        public string TestName { get; set; }
        public RunResultStatus Status { get; set; }
        public DateTime StartedUtc { get; set; }
        public DateTime EndedUtc { get; set; }
        public string FailureReason { get; set; }
        public string ScreenshotPath { get; set; }
        public List<StepRunResult> Steps { get; set; }
    }

    public class StepRunResult
    {
        public StepRunResult()
        {
            Action = StepAction.Invoke;
            Status = RunResultStatus.NotStarted;
            Message = string.Empty;
            ScreenshotPath = string.Empty;
        }

        public int StepId { get; set; }
        public StepAction Action { get; set; }
        public RunResultStatus Status { get; set; }
        public DateTime StartedUtc { get; set; }
        public DateTime EndedUtc { get; set; }
        public string Message { get; set; }
        public string ScreenshotPath { get; set; }
    }

    public class TestRunnerProgressEventArgs : EventArgs
    {
        public TestRunnerProgressEventArgs(
            TestRunnerProgressKind kind,
            TestSuiteRunResult suiteResult,
            TestCaseRunResult testResult,
            StepRunResult stepResult,
            string message)
        {
            Kind = kind;
            SuiteResult = suiteResult;
            TestResult = testResult;
            StepResult = stepResult;
            Message = message ?? string.Empty;
        }

        public TestRunnerProgressKind Kind { get; private set; }
        public TestSuiteRunResult SuiteResult { get; private set; }
        public TestCaseRunResult TestResult { get; private set; }
        public StepRunResult StepResult { get; private set; }
        public string Message { get; private set; }
    }

    public class TestRunner
    {
        private const int GracefulCloseTimeoutMs = 5000;

        private readonly IUiAutomationService uiAutomationService;
        private readonly ITargetProcessManager targetProcessManager;
        private readonly CTalosLog talosLog;

        public event EventHandler<TestRunnerProgressEventArgs> ProgressChanged;

        public TestRunner(
            IUiAutomationService uiAutomationService,
            ITargetProcessManager targetProcessManager,
            CTalosLog talosLog)
        {
            if (uiAutomationService == null)
            {
                throw new ArgumentNullException("uiAutomationService");
            }

            if (targetProcessManager == null)
            {
                throw new ArgumentNullException("targetProcessManager");
            }

            this.uiAutomationService = uiAutomationService;
            this.targetProcessManager = targetProcessManager;
            this.talosLog = talosLog ?? new CTalosLog();
        }

        public TestSuiteRunResult RunSuite(TestSuite suite)
        {
            if (suite == null)
            {
                throw new ArgumentNullException("suite");
            }

            return RunTests(suite, suite.Tests);
        }

        public TestSuiteRunResult RunSingleTest(TestSuite suite, TestCase testCase)
        {
            if (testCase == null)
            {
                throw new ArgumentNullException("testCase");
            }

            return RunTests(suite, new TestCase[] { testCase });
        }

        public TestSuiteRunResult RunTests(TestSuite suite, IEnumerable<TestCase> testsToRun)
        {
            if (suite == null)
            {
                throw new ArgumentNullException("suite");
            }

            List<TestCase> tests = ToRunnableTestList(testsToRun);
            TestSuiteRunResult suiteResult = CreateSuiteResult(suite);
            suiteResult.Status = RunResultStatus.Running;
            suiteResult.StartedUtc = DateTime.UtcNow;
            Report(TestRunnerProgressKind.SuiteStarted, suiteResult, null, null, "Suite run started.");
            talosLog.StartRun(suiteResult);

            try
            {
                for (int i = 0; i < tests.Count; i++)
                {
                    TestCaseRunResult testResult = RunTestCaseInternal(suite, suiteResult, tests[i]);
                    suiteResult.Tests.Add(testResult);
                    talosLog.LogTestResult(testResult);
                    Report(TestRunnerProgressKind.TestCompleted, suiteResult, testResult, null, testResult.FailureReason);

                    if (suite.AutoRelaunchBetweenTests && i < tests.Count - 1)
                    {
                        targetProcessManager.Relaunch(
                            suite.TargetAppPath,
                            suite.LaunchParams,
                            GracefulCloseTimeoutMs);
                    }
                }

                suiteResult.Status = suiteResult.FailedCount == 0 ? RunResultStatus.Passed : RunResultStatus.Failed;
            }
            catch (Exception ex)
            {
                suiteResult.Status = RunResultStatus.Failed;
                suiteResult.FailureReason = ex.Message;
                Report(TestRunnerProgressKind.Message, suiteResult, null, null, ex.Message);
            }
            finally
            {
                suiteResult.EndedUtc = DateTime.UtcNow;
                talosLog.EndRun(suiteResult);
                Report(TestRunnerProgressKind.SuiteCompleted, suiteResult, null, null, suiteResult.FailureReason);
            }

            return suiteResult;
        }

        private TestCaseRunResult RunTestCaseInternal(
            TestSuite suite,
            TestSuiteRunResult suiteResult,
            TestCase testCase)
        {
            TestCaseRunResult testResult = CreateTestResult(testCase);
            testResult.Status = RunResultStatus.Running;
            testResult.StartedUtc = DateTime.UtcNow;
            Report(TestRunnerProgressKind.TestStarted, suiteResult, testResult, null, "Test started.");

            HashSet<string> expectedWindowsSeen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            try
            {
                targetProcessManager.EnsureRunning(suite.TargetAppPath, suite.LaunchParams);
                ObserveExpectedWindows(testCase, expectedWindowsSeen);

                if (HasTargetCrashed())
                {
                    return FailTest(testResult, null, "Target process is not running before test steps.");
                }

                foreach (Step step in SafeSteps(testCase))
                {
                    StepRunResult stepResult = RunStep(suite, suiteResult, testResult, testCase, step, expectedWindowsSeen);
                    testResult.Steps.Add(stepResult);
                    talosLog.LogStepResult(testResult, stepResult);
                    Report(TestRunnerProgressKind.StepCompleted, suiteResult, testResult, stepResult, stepResult.Message);

                    if (stepResult.Status == RunResultStatus.Failed)
                    {
                        return FailTest(testResult, stepResult, stepResult.Message);
                    }
                }

                string conditionFailure = EvaluateEndOfTestConditions(testCase, expectedWindowsSeen);

                if (!string.IsNullOrEmpty(conditionFailure))
                {
                    return FailTest(testResult, null, conditionFailure);
                }

                testResult.Status = RunResultStatus.Passed;
                testResult.EndedUtc = DateTime.UtcNow;
                return testResult;
            }
            catch (Exception ex)
            {
                return FailTest(testResult, null, ex.Message);
            }
        }

        private StepRunResult RunStep(
            TestSuite suite,
            TestSuiteRunResult suiteResult,
            TestCaseRunResult testResult,
            TestCase testCase,
            Step step,
            HashSet<string> expectedWindowsSeen)
        {
            StepRunResult stepResult = CreateStepResult(step);
            stepResult.Status = RunResultStatus.Running;
            stepResult.StartedUtc = DateTime.UtcNow;
            Report(TestRunnerProgressKind.StepStarted, suiteResult, testResult, stepResult, "Step started.");

            if (HasTargetCrashed())
            {
                return FailStep(stepResult, "Target process crashed before step " + step.Id + ".");
            }

            ElementLocator locator = CopyLocatorForCurrentProcess(step.Locator);
            UiElementInfo element = uiAutomationService.FindElement(locator);

            if (element == null)
            {
                return FailStep(stepResult, "Step " + step.Id + " element could not be found.");
            }

            string actionText = GetActionText(step);

            if (!uiAutomationService.PerformAction(element, step.Action, actionText))
            {
                return FailStep(stepResult, "Step " + step.Id + " action failed: " + step.Action + ".");
            }

            int delayMs = GetDelayMs(suite, step);

            if (delayMs > 0)
            {
                Thread.Sleep(delayMs);
            }

            if (HasTargetCrashed())
            {
                return FailStep(stepResult, "Target process crashed after step " + step.Id + ".");
            }

            ObserveExpectedWindows(testCase, expectedWindowsSeen);

            string forbiddenFailure = FindForbiddenWindowFailure(testCase);

            if (!string.IsNullOrEmpty(forbiddenFailure))
            {
                return FailStep(stepResult, forbiddenFailure);
            }

            stepResult.Status = RunResultStatus.Passed;
            stepResult.Message = "Step " + step.Id + " passed.";
            stepResult.EndedUtc = DateTime.UtcNow;
            return stepResult;
        }

        private string EvaluateEndOfTestConditions(TestCase testCase, HashSet<string> expectedWindowsSeen)
        {
            ObserveExpectedWindows(testCase, expectedWindowsSeen);

            string valueFailure = FindValueCheckFailure(testCase);

            if (!string.IsNullOrEmpty(valueFailure))
            {
                return valueFailure;
            }

            string forbiddenFailure = FindForbiddenWindowFailure(testCase);

            if (!string.IsNullOrEmpty(forbiddenFailure))
            {
                return forbiddenFailure;
            }

            return FindExpectedWindowFailure(testCase, expectedWindowsSeen);
        }

        private string FindValueCheckFailure(TestCase testCase)
        {
            foreach (ValueCheck valueCheck in SafeValueChecks(testCase))
            {
                ElementLocator locator = CopyLocatorForCurrentProcess(valueCheck.Locator);
                UiElementInfo element = uiAutomationService.FindElement(locator);

                if (element == null)
                {
                    return "Value check '" + valueCheck.Id + "' element could not be found.";
                }

                string actualValue = uiAutomationService.GetElementValue(element) ?? string.Empty;
                string expectedValue = valueCheck.ExpectedValue ?? string.Empty;

                if (!string.Equals(actualValue, expectedValue, StringComparison.Ordinal))
                {
                    return "Value check '" + valueCheck.Id + "' expected '" + expectedValue + "' but found '" + actualValue + "'.";
                }
            }

            return string.Empty;
        }

        private string FindForbiddenWindowFailure(TestCase testCase)
        {
            List<UiElementInfo> windows = GetTargetWindows();

            foreach (ForbiddenWindowCondition condition in SafeForbiddenWindows(testCase))
            {
                string validationFailure = ValidateWindowCondition(condition);

                if (!string.IsNullOrEmpty(validationFailure))
                {
                    return validationFailure;
                }

                string matchError;

                foreach (UiElementInfo window in windows)
                {
                    if (MatchesWindowCondition(window, condition, out matchError))
                    {
                        return "Forbidden window condition '" + FormatConditionName(condition) +
                            "' matched window '" + FormatWindowName(window) + "'.";
                    }

                    if (!string.IsNullOrEmpty(matchError))
                    {
                        return matchError;
                    }
                }
            }

            return string.Empty;
        }

        private string FindExpectedWindowFailure(TestCase testCase, HashSet<string> expectedWindowsSeen)
        {
            List<UiElementInfo> windows = GetTargetWindows();

            foreach (ExpectedWindowCondition condition in SafeExpectedWindows(testCase))
            {
                string validationFailure = ValidateWindowCondition(condition);

                if (!string.IsNullOrEmpty(validationFailure))
                {
                    return validationFailure;
                }

                if (condition.MustAppearAtLeastOnce && expectedWindowsSeen.Contains(GetConditionKey(condition)))
                {
                    continue;
                }

                bool found = false;
                string matchError;

                foreach (UiElementInfo window in windows)
                {
                    if (MatchesWindowCondition(window, condition, out matchError))
                    {
                        found = true;
                        break;
                    }

                    if (!string.IsNullOrEmpty(matchError))
                    {
                        return matchError;
                    }
                }

                if (!found)
                {
                    return condition.MustAppearAtLeastOnce
                        ? "Expected window condition '" + FormatConditionName(condition) + "' did not appear during the test."
                        : "Expected window condition '" + FormatConditionName(condition) + "' was not found at the end of the test.";
                }
            }

            return string.Empty;
        }

        private void ObserveExpectedWindows(TestCase testCase, HashSet<string> expectedWindowsSeen)
        {
            if (expectedWindowsSeen == null)
            {
                return;
            }

            List<UiElementInfo> windows = GetTargetWindows();

            foreach (ExpectedWindowCondition condition in SafeExpectedWindows(testCase))
            {
                string matchError;

                foreach (UiElementInfo window in windows)
                {
                    if (MatchesWindowCondition(window, condition, out matchError))
                    {
                        expectedWindowsSeen.Add(GetConditionKey(condition));
                        break;
                    }
                }
            }
        }

        private bool MatchesWindowCondition(
            UiElementInfo window,
            WindowConditionBase condition,
            out string matchError)
        {
            matchError = string.Empty;

            if (window == null || condition == null || string.IsNullOrWhiteSpace(condition.Pattern))
            {
                return false;
            }

            string searchableText = BuildWindowSearchText(window);

            if (condition.MatchType == WindowPatternMatchType.Regex)
            {
                try
                {
                    return Regex.IsMatch(searchableText, condition.Pattern, RegexOptions.IgnoreCase);
                }
                catch (ArgumentException ex)
                {
                    matchError = "Window condition '" + condition.Pattern + "' has an invalid regex: " + ex.Message;
                    return false;
                }
            }

            return searchableText.IndexOf(condition.Pattern, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private string ValidateWindowCondition(WindowConditionBase condition)
        {
            if (condition == null || string.IsNullOrWhiteSpace(condition.Pattern))
            {
                return string.Empty;
            }

            if (condition.MatchType != WindowPatternMatchType.Regex)
            {
                return string.Empty;
            }

            try
            {
                Regex.Match(string.Empty, condition.Pattern);
                return string.Empty;
            }
            catch (ArgumentException ex)
            {
                return "Window condition '" + FormatConditionName(condition) + "' has an invalid regex: " + ex.Message;
            }
        }

        private string BuildWindowSearchText(UiElementInfo window)
        {
            return string.Join(
                " ",
                new string[]
                {
                    window.Name ?? string.Empty,
                    window.AutomationId ?? string.Empty,
                    window.ClassName ?? string.Empty,
                    window.ControlType ?? string.Empty
                });
        }

        private string FormatConditionName(WindowConditionBase condition)
        {
            if (condition == null)
            {
                return string.Empty;
            }

            if (!string.IsNullOrWhiteSpace(condition.Id))
            {
                return condition.Id + " (" + condition.Pattern + ")";
            }

            return condition.Pattern ?? string.Empty;
        }

        private string FormatWindowName(UiElementInfo window)
        {
            if (window == null)
            {
                return string.Empty;
            }

            if (!string.IsNullOrWhiteSpace(window.Name))
            {
                return window.Name;
            }

            if (!string.IsNullOrWhiteSpace(window.AutomationId))
            {
                return window.AutomationId;
            }

            if (!string.IsNullOrWhiteSpace(window.ClassName))
            {
                return window.ClassName;
            }

            return window.ControlType ?? string.Empty;
        }

        private TestCaseRunResult FailTest(TestCaseRunResult testResult, StepRunResult stepResult, string reason)
        {
            testResult.Status = RunResultStatus.Failed;
            testResult.FailureReason = reason ?? string.Empty;
            testResult.EndedUtc = DateTime.UtcNow;
            string screenshotPath = talosLog.CaptureFailureScreenshot(testResult, stepResult);
            testResult.ScreenshotPath = screenshotPath ?? string.Empty;

            if (stepResult != null)
            {
                stepResult.ScreenshotPath = testResult.ScreenshotPath;
            }

            return testResult;
        }

        private StepRunResult FailStep(StepRunResult stepResult, string message)
        {
            stepResult.Status = RunResultStatus.Failed;
            stepResult.Message = message ?? string.Empty;
            stepResult.EndedUtc = DateTime.UtcNow;
            return stepResult;
        }

        private bool HasTargetCrashed()
        {
            return targetProcessManager.HasCrashed || !targetProcessManager.IsRunning;
        }

        private List<UiElementInfo> GetTargetWindows()
        {
            int processId = targetProcessManager.ProcessId;

            if (processId <= 0)
            {
                return new List<UiElementInfo>();
            }

            return uiAutomationService.GetTopLevelWindowsForProcess(processId);
        }

        private ElementLocator CopyLocatorForCurrentProcess(ElementLocator source)
        {
            ElementLocator copy = new ElementLocator();

            if (source != null)
            {
                copy.AutomationId = source.AutomationId ?? string.Empty;
                copy.Name = source.Name ?? string.Empty;
                copy.ControlType = source.ControlType ?? string.Empty;
                copy.ClassName = source.ClassName ?? string.Empty;
                copy.ProcessId = source.ProcessId;
                copy.BoundingRectangle = PersistedRectangle.Copy(source.BoundingRectangle);
                copy.AncestorPath = AncestorDescriptor.CopyList(source.AncestorPath);
            }

            int processId = targetProcessManager.ProcessId;

            if (processId > 0)
            {
                copy.ProcessId = processId;
            }

            return copy;
        }

        private string GetActionText(Step step)
        {
            if (step == null || step.Parameters == null)
            {
                return string.Empty;
            }

            switch (step.Action)
            {
                case StepAction.SendKeys:
                    return step.Parameters.SendKeysText ?? string.Empty;
                case StepAction.SelectItem:
                    return step.Parameters.SelectedItem ?? string.Empty;
                default:
                    return step.Parameters.Text ?? string.Empty;
            }
        }

        private int GetDelayMs(TestSuite suite, Step step)
        {
            if (step != null && step.FixedDelayMs.HasValue)
            {
                return Math.Max(0, step.FixedDelayMs.Value);
            }

            return Math.Max(0, suite.DefaultFixedDelayMs);
        }

        private TestSuiteRunResult CreateSuiteResult(TestSuite suite)
        {
            TestSuiteRunResult result = new TestSuiteRunResult();
            result.SuiteName = suite.SuiteName ?? string.Empty;
            result.TargetAppPath = suite.TargetAppPath ?? string.Empty;
            return result;
        }

        private TestCaseRunResult CreateTestResult(TestCase testCase)
        {
            TestCaseRunResult result = new TestCaseRunResult();
            result.TestName = testCase == null ? string.Empty : (testCase.Name ?? string.Empty);
            return result;
        }

        private StepRunResult CreateStepResult(Step step)
        {
            StepRunResult result = new StepRunResult();

            if (step != null)
            {
                result.StepId = step.Id;
                result.Action = step.Action;
            }

            return result;
        }

        private List<TestCase> ToRunnableTestList(IEnumerable<TestCase> testsToRun)
        {
            List<TestCase> tests = new List<TestCase>();

            if (testsToRun == null)
            {
                return tests;
            }

            foreach (TestCase testCase in testsToRun)
            {
                if (testCase != null)
                {
                    tests.Add(testCase);
                }
            }

            return tests;
        }

        private IEnumerable<Step> SafeSteps(TestCase testCase)
        {
            if (testCase == null || testCase.Steps == null)
            {
                return new List<Step>();
            }

            return testCase.Steps;
        }

        private IEnumerable<ValueCheck> SafeValueChecks(TestCase testCase)
        {
            if (testCase == null || testCase.Conditions == null || testCase.Conditions.ValueChecks == null)
            {
                return new List<ValueCheck>();
            }

            return testCase.Conditions.ValueChecks;
        }

        private IEnumerable<ExpectedWindowCondition> SafeExpectedWindows(TestCase testCase)
        {
            if (testCase == null || testCase.Conditions == null || testCase.Conditions.ExpectedWindows == null)
            {
                return new List<ExpectedWindowCondition>();
            }

            return testCase.Conditions.ExpectedWindows;
        }

        private IEnumerable<ForbiddenWindowCondition> SafeForbiddenWindows(TestCase testCase)
        {
            if (testCase == null || testCase.Conditions == null || testCase.Conditions.ForbiddenWindows == null)
            {
                return new List<ForbiddenWindowCondition>();
            }

            return testCase.Conditions.ForbiddenWindows;
        }

        private string GetConditionKey(WindowConditionBase condition)
        {
            if (condition == null)
            {
                return string.Empty;
            }

            if (!string.IsNullOrWhiteSpace(condition.Id))
            {
                return condition.Id;
            }

            return condition.MatchType + ":" + (condition.Pattern ?? string.Empty);
        }

        private void Report(
            TestRunnerProgressKind kind,
            TestSuiteRunResult suiteResult,
            TestCaseRunResult testResult,
            StepRunResult stepResult,
            string message)
        {
            EventHandler<TestRunnerProgressEventArgs> handler = ProgressChanged;

            if (handler != null)
            {
                handler(this, new TestRunnerProgressEventArgs(kind, suiteResult, testResult, stepResult, message));
            }
        }
    }
}
