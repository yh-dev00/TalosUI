using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web.Script.Serialization;

namespace TalosCore
{
    public class CTalosMemory
    {
        public const int CurrentSchemaVersion = TestSuite.CurrentSchemaVersion;
        public const string SuiteFileExtension = ".talos.json";

        public TestSuite LoadSuite(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException("Suite file path is required.", "path");
            }

            if (!File.Exists(path))
            {
                throw new FileNotFoundException("Suite file does not exist.", path);
            }

            string json = File.ReadAllText(path, Encoding.UTF8);

            if (string.IsNullOrWhiteSpace(json))
            {
                throw new CTalosMemoryException("Suite file is empty.");
            }

            JavaScriptSerializer serializer = CreateSerializer();
            int schemaVersion = ReadSchemaVersion(serializer, json);
            ValidateSchemaVersion(schemaVersion);

            TestSuite suite;

            try
            {
                suite = serializer.Deserialize<TestSuite>(json);
            }
            catch (Exception ex)
            {
                throw new CTalosMemoryException("Suite file is not valid TalosUI JSON: " + ex.Message, ex);
            }

            if (suite == null)
            {
                throw new CTalosMemoryException("Suite file did not contain a test suite.");
            }

            NormalizeSuite(suite);
            return suite;
        }

        public void SaveSuite(TestSuite suite, string path)
        {
            if (suite == null)
            {
                throw new ArgumentNullException("suite");
            }

            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException("Suite file path is required.", "path");
            }

            string directory = Path.GetDirectoryName(Path.GetFullPath(path));

            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            suite.SchemaVersion = CurrentSchemaVersion;
            NormalizeSuite(suite);

            JavaScriptSerializer serializer = CreateSerializer();
            string json = FormatJson(serializer.Serialize(suite));
            File.WriteAllText(path, json, Encoding.UTF8);
        }

        private static JavaScriptSerializer CreateSerializer()
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            serializer.MaxJsonLength = int.MaxValue;
            serializer.RecursionLimit = 100;
            return serializer;
        }

        private static int ReadSchemaVersion(JavaScriptSerializer serializer, string json)
        {
            object parsed;

            try
            {
                parsed = serializer.DeserializeObject(json);
            }
            catch (Exception ex)
            {
                throw new CTalosMemoryException("Suite file is not valid JSON: " + ex.Message, ex);
            }

            Dictionary<string, object> root = parsed as Dictionary<string, object>;

            if (root == null || !root.ContainsKey("SchemaVersion"))
            {
                throw new CTalosMemoryException("Suite file is missing SchemaVersion.");
            }

            object versionValue = root["SchemaVersion"];

            try
            {
                return Convert.ToInt32(versionValue);
            }
            catch (Exception ex)
            {
                throw new CTalosMemoryException("Suite file has an invalid SchemaVersion.", ex);
            }
        }

        private static void ValidateSchemaVersion(int schemaVersion)
        {
            if (schemaVersion > CurrentSchemaVersion)
            {
                throw new CTalosMemoryException(
                    "Suite file uses schema version " + schemaVersion +
                    ", but this TalosUI build supports version " + CurrentSchemaVersion + ".");
            }

            if (schemaVersion < CurrentSchemaVersion)
            {
                throw new CTalosMemoryException(
                    "Suite file uses older schema version " + schemaVersion +
                    ". Migration is not implemented in v1.");
            }
        }

        private static void NormalizeSuite(TestSuite suite)
        {
            suite.SuiteName = suite.SuiteName ?? string.Empty;
            suite.TargetAppPath = suite.TargetAppPath ?? string.Empty;
            suite.LaunchParams = suite.LaunchParams ?? string.Empty;

            if (suite.Tests == null)
            {
                suite.Tests = new List<TestCase>();
            }

            foreach (TestCase testCase in suite.Tests)
            {
                NormalizeTestCase(testCase);
            }
        }

        private static void NormalizeTestCase(TestCase testCase)
        {
            if (testCase == null)
            {
                return;
            }

            testCase.Name = testCase.Name ?? string.Empty;
            testCase.Description = testCase.Description ?? string.Empty;

            if (testCase.Steps == null)
            {
                testCase.Steps = new List<Step>();
            }

            foreach (Step step in testCase.Steps)
            {
                NormalizeStep(step);
            }

            if (testCase.Conditions == null)
            {
                testCase.Conditions = new TestConditions();
            }

            NormalizeConditions(testCase.Conditions);
        }

        private static void NormalizeStep(Step step)
        {
            if (step == null)
            {
                return;
            }

            if (step.Locator == null)
            {
                step.Locator = new ElementLocator();
            }

            NormalizeLocator(step.Locator);

            if (step.Parameters == null)
            {
                step.Parameters = new StepParameters();
            }

            step.Parameters.Text = step.Parameters.Text ?? string.Empty;
            step.Parameters.SendKeysText = step.Parameters.SendKeysText ?? string.Empty;
            step.Parameters.SelectedItem = step.Parameters.SelectedItem ?? string.Empty;
        }

        private static void NormalizeConditions(TestConditions conditions)
        {
            if (conditions.ValueChecks == null)
            {
                conditions.ValueChecks = new List<ValueCheck>();
            }

            foreach (ValueCheck valueCheck in conditions.ValueChecks)
            {
                NormalizeValueCheck(valueCheck);
            }

            if (conditions.ExpectedWindows == null)
            {
                conditions.ExpectedWindows = new List<ExpectedWindowCondition>();
            }

            foreach (ExpectedWindowCondition condition in conditions.ExpectedWindows)
            {
                NormalizeWindowCondition(condition);
            }

            if (conditions.ForbiddenWindows == null)
            {
                conditions.ForbiddenWindows = new List<ForbiddenWindowCondition>();
            }

            foreach (ForbiddenWindowCondition condition in conditions.ForbiddenWindows)
            {
                NormalizeWindowCondition(condition);
            }
        }

        private static void NormalizeValueCheck(ValueCheck valueCheck)
        {
            if (valueCheck == null)
            {
                return;
            }

            valueCheck.Id = valueCheck.Id ?? string.Empty;
            valueCheck.ExpectedValue = valueCheck.ExpectedValue ?? string.Empty;

            if (valueCheck.Locator == null)
            {
                valueCheck.Locator = new ElementLocator();
            }

            NormalizeLocator(valueCheck.Locator);
        }

        private static void NormalizeWindowCondition(WindowConditionBase condition)
        {
            if (condition == null)
            {
                return;
            }

            condition.Id = condition.Id ?? string.Empty;
            condition.Pattern = condition.Pattern ?? string.Empty;
        }

        private static void NormalizeLocator(ElementLocator locator)
        {
            locator.AutomationId = locator.AutomationId ?? string.Empty;
            locator.Name = locator.Name ?? string.Empty;
            locator.ControlType = locator.ControlType ?? string.Empty;
            locator.ClassName = locator.ClassName ?? string.Empty;

            if (locator.BoundingRectangle == null)
            {
                locator.BoundingRectangle = new PersistedRectangle();
            }

            if (locator.AncestorPath == null)
            {
                locator.AncestorPath = new List<AncestorDescriptor>();
            }

            foreach (AncestorDescriptor ancestor in locator.AncestorPath)
            {
                if (ancestor == null)
                {
                    continue;
                }

                ancestor.AutomationId = ancestor.AutomationId ?? string.Empty;
                ancestor.Name = ancestor.Name ?? string.Empty;
                ancestor.ControlType = ancestor.ControlType ?? string.Empty;
            }
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

    public class CTalosMemoryException : Exception
    {
        public CTalosMemoryException(string message)
            : base(message)
        {
        }

        public CTalosMemoryException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
