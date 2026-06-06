using System;

namespace TalosCore
{
    public class CTalosLog
    {
        public virtual void StartRun(TestSuiteRunResult suiteResult)
        {
        }

        public virtual void EndRun(TestSuiteRunResult suiteResult)
        {
        }

        public virtual void LogTestResult(TestCaseRunResult testResult)
        {
        }

        public virtual void LogStepResult(TestCaseRunResult testResult, StepRunResult stepResult)
        {
        }

        public virtual string CaptureFailureScreenshot(TestCaseRunResult testResult, StepRunResult stepResult)
        {
            return string.Empty;
        }
    }
}
