using System;
using System.Collections.Generic;

namespace TalosCore
{
    public class TestSuite
    {
        public const int CurrentSchemaVersion = 1;

        public TestSuite()
        {
            SchemaVersion = CurrentSchemaVersion;
            SuiteName = "Untitled Suite";
            TargetAppPath = string.Empty;
            LaunchParams = string.Empty;
            DefaultFixedDelayMs = 500;
            AutoRelaunchBetweenTests = false;
            Tests = new List<TestCase>();
        }

        public int SchemaVersion { get; set; }
        public string SuiteName { get; set; }
        public string TargetAppPath { get; set; }
        public string LaunchParams { get; set; }
        public int DefaultFixedDelayMs { get; set; }
        public bool AutoRelaunchBetweenTests { get; set; }
        public List<TestCase> Tests { get; set; }
    }

    public class TestCase
    {
        public TestCase()
        {
            Name = "New Test";
            Description = string.Empty;
            Steps = new List<Step>();
            Conditions = new TestConditions();
        }

        public string Name { get; set; }
        public string Description { get; set; }
        public List<Step> Steps { get; set; }
        public TestConditions Conditions { get; set; }
    }

    public class Step
    {
        public Step()
        {
            Id = 0;
            Locator = new ElementLocator();
            Action = StepAction.Invoke;
            Parameters = new StepParameters();
            FixedDelayMs = null;
        }

        public int Id { get; set; }
        public ElementLocator Locator { get; set; }
        public StepAction Action { get; set; }
        public StepParameters Parameters { get; set; }
        public int? FixedDelayMs { get; set; }
    }

    public enum StepAction
    {
        Invoke,
        SetValue,
        SelectItem,
        SendKeys
    }

    public class StepParameters
    {
        public StepParameters()
        {
            Text = string.Empty;
            SendKeysText = string.Empty;
            SelectedItem = string.Empty;
        }

        public string Text { get; set; }
        public string SendKeysText { get; set; }
        public string SelectedItem { get; set; }
    }

    public class UiElementInfo
    {
        public UiElementInfo()
        {
            HowFound = string.Empty;
            AutomationId = string.Empty;
            Name = string.Empty;
            ControlType = string.Empty;
            ClassName = string.Empty;
            ProcessId = 0;
            NativeWindowHandle = 0;
            BoundingRectangle = new PersistedRectangle();
            AncestorPath = new List<AncestorDescriptor>();
        }

        public string HowFound { get; set; }
        public string AutomationId { get; set; }
        public string Name { get; set; }
        public string ControlType { get; set; }
        public string ClassName { get; set; }
        public int ProcessId { get; set; }
        public int NativeWindowHandle { get; set; }
        public PersistedRectangle BoundingRectangle { get; set; }
        public List<AncestorDescriptor> AncestorPath { get; set; }

        public ElementLocator ToElementLocator()
        {
            return ElementLocator.FromUiElementInfo(this);
        }
    }

    public class ElementLocator
    {
        public ElementLocator()
        {
            AutomationId = string.Empty;
            Name = string.Empty;
            ControlType = string.Empty;
            ClassName = string.Empty;
            ProcessId = 0;
            BoundingRectangle = new PersistedRectangle();
            AncestorPath = new List<AncestorDescriptor>();
        }

        public string AutomationId { get; set; }
        public string Name { get; set; }
        public string ControlType { get; set; }
        public string ClassName { get; set; }
        public int ProcessId { get; set; }
        public PersistedRectangle BoundingRectangle { get; set; }
        public List<AncestorDescriptor> AncestorPath { get; set; }

        public static ElementLocator FromUiElementInfo(UiElementInfo element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            ElementLocator locator = new ElementLocator();
            locator.AutomationId = element.AutomationId ?? string.Empty;
            locator.Name = element.Name ?? string.Empty;
            locator.ControlType = element.ControlType ?? string.Empty;
            locator.ClassName = element.ClassName ?? string.Empty;
            locator.ProcessId = element.ProcessId;
            locator.BoundingRectangle = PersistedRectangle.Copy(element.BoundingRectangle);
            locator.AncestorPath = AncestorDescriptor.CopyList(element.AncestorPath);
            return locator;
        }
    }

    public class AncestorDescriptor
    {
        public AncestorDescriptor()
        {
            AutomationId = string.Empty;
            Name = string.Empty;
            ControlType = string.Empty;
            IndexWithinParent = null;
        }

        public string AutomationId { get; set; }
        public string Name { get; set; }
        public string ControlType { get; set; }
        public int? IndexWithinParent { get; set; }

        public static AncestorDescriptor Copy(AncestorDescriptor source)
        {
            if (source == null)
            {
                return new AncestorDescriptor();
            }

            AncestorDescriptor copy = new AncestorDescriptor();
            copy.AutomationId = source.AutomationId ?? string.Empty;
            copy.Name = source.Name ?? string.Empty;
            copy.ControlType = source.ControlType ?? string.Empty;
            copy.IndexWithinParent = source.IndexWithinParent;
            return copy;
        }

        public static List<AncestorDescriptor> CopyList(IEnumerable<AncestorDescriptor> source)
        {
            List<AncestorDescriptor> copy = new List<AncestorDescriptor>();

            if (source == null)
            {
                return copy;
            }

            foreach (AncestorDescriptor ancestor in source)
            {
                copy.Add(Copy(ancestor));
            }

            return copy;
        }
    }

    public class PersistedRectangle
    {
        public PersistedRectangle()
            : this(0, 0, 0, 0)
        {
        }

        public PersistedRectangle(double x, double y, double width, double height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public double X { get; set; }
        public double Y { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }

        public bool IsEmpty
        {
            get { return Width <= 0 || Height <= 0; }
        }

        public double CenterX
        {
            get { return X + (Width / 2); }
        }

        public double CenterY
        {
            get { return Y + (Height / 2); }
        }

        public static PersistedRectangle Copy(PersistedRectangle source)
        {
            if (source == null)
            {
                return new PersistedRectangle();
            }

            return new PersistedRectangle(source.X, source.Y, source.Width, source.Height);
        }
    }

    public class TestConditions
    {
        public TestConditions()
        {
            ValueChecks = new List<ValueCheck>();
            ExpectedWindows = new List<ExpectedWindowCondition>();
            ForbiddenWindows = new List<ForbiddenWindowCondition>();
        }

        public List<ValueCheck> ValueChecks { get; set; }
        public List<ExpectedWindowCondition> ExpectedWindows { get; set; }
        public List<ForbiddenWindowCondition> ForbiddenWindows { get; set; }
    }

    public class ValueCheck
    {
        public ValueCheck()
        {
            Id = string.Empty;
            Locator = new ElementLocator();
            ExpectedValue = string.Empty;
            EvaluationPoint = ValueCheckEvaluationPoint.EndOfTest;
        }

        public string Id { get; set; }
        public ElementLocator Locator { get; set; }
        public string ExpectedValue { get; set; }
        public ValueCheckEvaluationPoint EvaluationPoint { get; set; }
    }

    public enum ValueCheckEvaluationPoint
    {
        EndOfTest
    }

    public abstract class WindowConditionBase
    {
        protected WindowConditionBase()
        {
            Id = string.Empty;
            Pattern = string.Empty;
            MatchType = WindowPatternMatchType.Substring;
        }

        public string Id { get; set; }
        public string Pattern { get; set; }
        public WindowPatternMatchType MatchType { get; set; }
    }

    public class ExpectedWindowCondition : WindowConditionBase
    {
        public ExpectedWindowCondition()
        {
            MustAppearAtLeastOnce = false;
        }

        public bool MustAppearAtLeastOnce { get; set; }
    }

    public class ForbiddenWindowCondition : WindowConditionBase
    {
    }

    public enum WindowPatternMatchType
    {
        Substring,
        Regex
    }
}
