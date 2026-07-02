using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Automation;
using System.Windows.Forms;
using WindowsPoint = System.Windows.Point;
using WindowsRect = System.Windows.Rect;

namespace TalosCore
{
    public interface IUiAutomationService
    {
        UiElementInfo GetElementAtPoint(int x, int y);
        List<UiElementInfo> GetTopLevelWindowsForProcess(int processId);
        UiElementInfo FindElement(ElementLocator locator);
        bool PerformAction(UiElementInfo element, StepAction action, string text);
        string GetElementValue(UiElementInfo element);
    }

    public class UiAutomationService : IUiAutomationService
    {
        private const double BoundingRectangleTolerance = 8.0;
        private const double DpiScaleTolerance = 0.01;
        private const int NativeUiAutomationNamePropertyId = 30005;
        private const uint MonitorDefaultToNearest = 2;
        private const uint GetAncestorRoot = 2;
        private const int MdtEffectiveDpi = 0;

        public UiElementInfo GetElementAtPoint(int x, int y)
        {
            AutomationElement element = GetAutomationElementFromPhysicalPoint(x, y);
            return ToUiElementInfo(element, x, y);
        }

        public List<UiElementInfo> GetTopLevelWindowsForProcess(int processId)
        {
            List<UiElementInfo> windows = new List<UiElementInfo>();

            if (processId <= 0)
            {
                return windows;
            }

            foreach (AutomationElement child in GetTopLevelWindowElementsForProcess(processId))
            {
                UiElementInfo info = ToUiElementInfo(child);

                if (info != null)
                {
                    windows.Add(info);
                }
            }

            return windows;
        }

        public UiElementInfo FindElement(ElementLocator locator)
        {
            AutomationElement element = FindAutomationElement(locator);
            return ToUiElementInfo(element);
        }

        public bool PerformAction(UiElementInfo element, StepAction action, string text)
        {
            AutomationElement automationElement = ResolveElement(element);

            if (automationElement == null)
            {
                return false;
            }

            try
            {
                switch (action)
                {
                    case StepAction.Invoke:
                        return InvokeElement(automationElement);

                    case StepAction.SetValue:
                        return SetElementValue(automationElement, text);

                    case StepAction.SelectItem:
                        return SelectElement(automationElement);

                    case StepAction.SendKeys:
                        return SendKeysToElement(automationElement, text);

                    default:
                        return false;
                }
            }
            catch (ElementNotAvailableException)
            {
                return false;
            }
            catch (InvalidOperationException)
            {
                return false;
            }
        }

        public string GetElementValue(UiElementInfo element)
        {
            AutomationElement automationElement = ResolveElement(element);

            if (automationElement == null)
            {
                return string.Empty;
            }

            object pattern;

            if (automationElement.TryGetCurrentPattern(ValuePattern.Pattern, out pattern))
            {
                return ((ValuePattern)pattern).Current.Value ?? string.Empty;
            }

            return GetStringProperty(automationElement, AutomationElement.NameProperty);
        }

        private AutomationElement FindAutomationElement(ElementLocator locator)
        {
            if (locator == null)
            {
                return null;
            }

            AutomationElement scopeRoot = ResolveScopeRoot(locator);

            if (scopeRoot == null)
            {
                return null;
            }

            AutomationElement byAutomationId = FindByAutomationId(scopeRoot, locator.AutomationId);

            if (byAutomationId != null)
            {
                return byAutomationId;
            }

            AutomationElement parent = ResolveParentFromPath(scopeRoot, locator.AncestorPath);
            AutomationElement byControlTypeAndName = FindByControlTypeAndName(parent ?? scopeRoot, locator.ControlType, locator.Name);

            if (byControlTypeAndName != null)
            {
                return byControlTypeAndName;
            }

            AutomationElement byAncestorPath = ResolvePath(scopeRoot, locator.AncestorPath, false);

            if (byAncestorPath != null && MatchesLocator(byAncestorPath, locator))
            {
                return byAncestorPath;
            }

            return FindByBoundingRectangle(locator);
        }

        private AutomationElement ResolveScopeRoot(ElementLocator locator)
        {
            if (locator.ProcessId > 0)
            {
                List<AutomationElement> windows = GetTopLevelWindowElementsForProcess(locator.ProcessId);

                if (windows.Count > 0)
                {
                    return windows[0];
                }
            }

            return AutomationElement.RootElement;
        }

        private List<AutomationElement> GetTopLevelWindowElementsForProcess(int processId)
        {
            List<AutomationElement> windows = new List<AutomationElement>();

            if (processId <= 0)
            {
                return windows;
            }

            AutomationElementCollection children = AutomationElement.RootElement.FindAll(
                TreeScope.Children,
                new PropertyCondition(AutomationElement.ProcessIdProperty, processId));

            foreach (AutomationElement child in children)
            {
                windows.Add(child);
            }

            return windows;
        }

        private AutomationElement ResolveElement(UiElementInfo element)
        {
            if (element == null)
            {
                return null;
            }

            if (element.NativeWindowHandle != 0)
            {
                AutomationElement byHandle = AutomationElement.FromHandle(new IntPtr(element.NativeWindowHandle));

                if (byHandle != null && MatchesInfo(byHandle, element))
                {
                    return byHandle;
                }
            }

            return FindAutomationElement(element.ToElementLocator());
        }

        private AutomationElement FindByAutomationId(AutomationElement root, string automationId)
        {
            if (root == null || string.IsNullOrEmpty(automationId))
            {
                return null;
            }

            return root.FindFirst(
                TreeScope.Descendants,
                new PropertyCondition(AutomationElement.AutomationIdProperty, automationId));
        }

        private AutomationElement FindByControlTypeAndName(AutomationElement root, string controlTypeName, string name)
        {
            if (root == null || string.IsNullOrEmpty(controlTypeName) || string.IsNullOrEmpty(name))
            {
                return null;
            }

            System.Windows.Automation.Condition condition = new AndCondition(
                new PropertyCondition(AutomationElement.ControlTypeProperty, ControlTypeFromName(controlTypeName)),
                new PropertyCondition(AutomationElement.NameProperty, name));

            return root.FindFirst(TreeScope.Descendants, condition);
        }

        private AutomationElement ResolveParentFromPath(AutomationElement root, List<AncestorDescriptor> path)
        {
            if (path == null || path.Count <= 1)
            {
                return root;
            }

            return ResolvePath(root, path.Take(path.Count - 1).ToList(), true);
        }

        private AutomationElement ResolvePath(AutomationElement root, List<AncestorDescriptor> path, bool allowRootMatch)
        {
            if (root == null || path == null || path.Count == 0)
            {
                return null;
            }

            int startIndex = 0;
            AutomationElement current = root;

            if (DescriptorMatchesElement(path[0], root))
            {
                startIndex = 1;
            }

            for (int i = startIndex; i < path.Count; i++)
            {
                current = FindChildByDescriptor(current, path[i]);

                if (current == null)
                {
                    return null;
                }
            }

            return current;
        }

        private AutomationElement FindChildByDescriptor(AutomationElement parent, AncestorDescriptor descriptor)
        {
            if (parent == null || descriptor == null)
            {
                return null;
            }

            AutomationElementCollection children = parent.FindAll(
                TreeScope.Children,
                System.Windows.Automation.Condition.TrueCondition);
            List<AutomationElement> matches = new List<AutomationElement>();

            foreach (AutomationElement child in children)
            {
                if (DescriptorMatchesElement(descriptor, child))
                {
                    matches.Add(child);
                }
            }

            if (matches.Count == 0)
            {
                return null;
            }

            if (descriptor.IndexWithinParent.HasValue &&
                descriptor.IndexWithinParent.Value >= 0 &&
                descriptor.IndexWithinParent.Value < matches.Count)
            {
                return matches[descriptor.IndexWithinParent.Value];
            }

            return matches[0];
        }

        private AutomationElement FindByBoundingRectangle(ElementLocator locator)
        {
            if (locator == null || locator.BoundingRectangle == null || locator.BoundingRectangle.IsEmpty)
            {
                return null;
            }

            AutomationElement element = GetAutomationElementFromPhysicalPoint(
                locator.BoundingRectangle.CenterX,
                locator.BoundingRectangle.CenterY);

            if (element == null)
            {
                return null;
            }

            WindowsRect rectangle = GetBoundingRectangle(element);

            if (RectangleWithinTolerance(rectangle, locator.BoundingRectangle))
            {
                return element;
            }

            return null;
        }

        private AutomationElement GetAutomationElementFromPhysicalPoint(double physicalX, double physicalY)
        {
            DpiVirtualizationContext context = GetDpiVirtualizationContextFromPhysicalPoint(physicalX, physicalY);
            WindowsPoint providerPoint = PhysicalToProviderPoint(physicalX, physicalY, context);
            AutomationElement element = AutomationElement.FromPoint(providerPoint);

            if (HasDpiScale(context) && !ElementMatchesProcess(element, context.ProcessId))
            {
                element = AutomationElement.FromPoint(new WindowsPoint(physicalX, physicalY));
            }

            return element;
        }

        private WindowsPoint PhysicalToProviderPoint(double physicalX, double physicalY, DpiVirtualizationContext context)
        {
            if (!HasDpiScale(context))
            {
                return new WindowsPoint(physicalX, physicalY);
            }

            return new WindowsPoint(
                context.MonitorLeft + ((physicalX - context.MonitorLeft) / context.ScaleX),
                context.MonitorTop + ((physicalY - context.MonitorTop) / context.ScaleY));
        }

        private NativePoint PhysicalToNativeProviderPoint(int physicalX, int physicalY)
        {
            DpiVirtualizationContext context = GetDpiVirtualizationContextFromPhysicalPoint(physicalX, physicalY);
            WindowsPoint providerPoint = PhysicalToProviderPoint(physicalX, physicalY, context);
            return new NativePoint(RoundCoordinate(providerPoint.X), RoundCoordinate(providerPoint.Y));
        }

        private WindowsRect ProviderToPhysicalRectangle(WindowsRect rectangle, DpiVirtualizationContext context)
        {
            if (rectangle.IsEmpty || !HasDpiScale(context))
            {
                return rectangle;
            }

            double left = context.MonitorLeft + ((rectangle.X - context.MonitorLeft) * context.ScaleX);
            double top = context.MonitorTop + ((rectangle.Y - context.MonitorTop) * context.ScaleY);
            double right = context.MonitorLeft + (((rectangle.X + rectangle.Width) - context.MonitorLeft) * context.ScaleX);
            double bottom = context.MonitorTop + (((rectangle.Y + rectangle.Height) - context.MonitorTop) * context.ScaleY);

            return new WindowsRect(left, top, Math.Max(0, right - left), Math.Max(0, bottom - top));
        }

        private bool ElementMatchesProcess(AutomationElement element, int processId)
        {
            if (element == null)
            {
                return false;
            }

            if (processId <= 0)
            {
                return true;
            }

            return GetIntProperty(element, AutomationElement.ProcessIdProperty) == processId;
        }

        private bool HasDpiScale(DpiVirtualizationContext context)
        {
            return context != null &&
                context.IsValid &&
                (Math.Abs(context.ScaleX - 1.0) > DpiScaleTolerance ||
                Math.Abs(context.ScaleY - 1.0) > DpiScaleTolerance);
        }

        private int RoundCoordinate(double value)
        {
            return (int)Math.Round(value, MidpointRounding.AwayFromZero);
        }

        private DpiVirtualizationContext GetDpiVirtualizationContextFromPhysicalPoint(double physicalX, double physicalY)
        {
            NativePoint point = new NativePoint(RoundCoordinate(physicalX), RoundCoordinate(physicalY));
            IntPtr hwnd = WindowFromPoint(point);

            if (hwnd == IntPtr.Zero)
            {
                return new DpiVirtualizationContext();
            }

            IntPtr monitor = MonitorFromPoint(point, MonitorDefaultToNearest);
            return CreateDpiVirtualizationContext(hwnd, monitor);
        }

        private DpiVirtualizationContext GetDpiVirtualizationContextFromWindow(IntPtr hwnd)
        {
            if (hwnd == IntPtr.Zero)
            {
                return new DpiVirtualizationContext();
            }

            IntPtr rootWindow = GetRootWindowHandle(hwnd);
            IntPtr monitor = MonitorFromWindow(rootWindow, MonitorDefaultToNearest);
            return CreateDpiVirtualizationContext(rootWindow, monitor);
        }

        private DpiVirtualizationContext CreateDpiVirtualizationContext(IntPtr hwnd, IntPtr monitor)
        {
            DpiVirtualizationContext context = new DpiVirtualizationContext();

            if (hwnd == IntPtr.Zero)
            {
                return context;
            }

            IntPtr rootWindow = GetRootWindowHandle(hwnd);
            context.WindowHandle = rootWindow == IntPtr.Zero ? hwnd : rootWindow;

            uint processId;
            GetWindowThreadProcessId(context.WindowHandle, out processId);
            context.ProcessId = processId > int.MaxValue ? 0 : Convert.ToInt32(processId);

            if (monitor == IntPtr.Zero)
            {
                monitor = MonitorFromWindow(context.WindowHandle, MonitorDefaultToNearest);
            }

            if (monitor == IntPtr.Zero)
            {
                return context;
            }

            MonitorInfo monitorInfo = new MonitorInfo();
            monitorInfo.Size = Marshal.SizeOf(typeof(MonitorInfo));

            if (!GetMonitorInfo(monitor, ref monitorInfo))
            {
                return context;
            }

            uint windowDpi;
            uint monitorDpiX;
            uint monitorDpiY;

            if (!TryGetDpiForWindow(context.WindowHandle, out windowDpi) ||
                !TryGetDpiForMonitor(monitor, out monitorDpiX, out monitorDpiY) ||
                windowDpi == 0 ||
                monitorDpiX == 0 ||
                monitorDpiY == 0)
            {
                return context;
            }

            context.MonitorHandle = monitor;
            context.MonitorLeft = monitorInfo.Monitor.Left;
            context.MonitorTop = monitorInfo.Monitor.Top;
            context.ScaleX = (double)monitorDpiX / (double)windowDpi;
            context.ScaleY = (double)monitorDpiY / (double)windowDpi;
            context.IsValid = true;
            return context;
        }

        private bool TryGetDpiForWindow(IntPtr hwnd, out uint dpi)
        {
            dpi = 0;

            if (hwnd == IntPtr.Zero)
            {
                return false;
            }

            try
            {
                dpi = GetDpiForWindowNative(hwnd);
                return dpi > 0;
            }
            catch (EntryPointNotFoundException)
            {
                return false;
            }
            catch (DllNotFoundException)
            {
                return false;
            }
        }

        private bool TryGetDpiForMonitor(IntPtr monitor, out uint dpiX, out uint dpiY)
        {
            dpiX = 0;
            dpiY = 0;

            if (monitor == IntPtr.Zero)
            {
                return false;
            }

            try
            {
                int result = GetDpiForMonitorNative(monitor, MdtEffectiveDpi, out dpiX, out dpiY);
                return result == 0 && dpiX > 0 && dpiY > 0;
            }
            catch (EntryPointNotFoundException)
            {
                return false;
            }
            catch (DllNotFoundException)
            {
                return false;
            }
        }

        private IntPtr GetRootWindowHandle(IntPtr hwnd)
        {
            if (hwnd == IntPtr.Zero)
            {
                return IntPtr.Zero;
            }

            IntPtr rootWindow = GetAncestor(hwnd, GetAncestorRoot);
            return rootWindow == IntPtr.Zero ? hwnd : rootWindow;
        }

        private bool InvokeElement(AutomationElement element)
        {
            object pattern;

            if (!element.TryGetCurrentPattern(InvokePattern.Pattern, out pattern))
            {
                return false;
            }

            ((InvokePattern)pattern).Invoke();
            return true;
        }

        private bool SetElementValue(AutomationElement element, string text)
        {
            object pattern;

            if (!element.TryGetCurrentPattern(ValuePattern.Pattern, out pattern))
            {
                return false;
            }

            ValuePattern valuePattern = (ValuePattern)pattern;

            if (valuePattern.Current.IsReadOnly)
            {
                return false;
            }

            valuePattern.SetValue(text ?? string.Empty);
            return true;
        }

        private bool SelectElement(AutomationElement element)
        {
            object pattern;

            if (!element.TryGetCurrentPattern(SelectionItemPattern.Pattern, out pattern))
            {
                return false;
            }

            ((SelectionItemPattern)pattern).Select();
            return true;
        }

        private bool SendKeysToElement(AutomationElement element, string text)
        {
            try
            {
                element.SetFocus();
                SendKeys.SendWait(text ?? string.Empty);
                return true;
            }
            catch (InvalidOperationException)
            {
                return false;
            }
        }

        private UiElementInfo ToUiElementInfo(AutomationElement element)
        {
            return ToUiElementInfo(element, null, null);
        }

        private UiElementInfo ToUiElementInfo(AutomationElement element, int? screenX, int? screenY)
        {
            if (element == null)
            {
                return null;
            }

            UiElementInfo info = new UiElementInfo();
            info.HowFound = FormatHowFound(screenX, screenY);
            info.AutomationId = GetStringProperty(element, AutomationElement.AutomationIdProperty);
            info.Name = GetInspectName(element, screenX, screenY);
            info.ControlType = ControlTypeToName(GetControlType(element));
            info.ClassName = GetStringProperty(element, AutomationElement.ClassNameProperty);
            info.ProcessId = GetIntProperty(element, AutomationElement.ProcessIdProperty);
            info.NativeWindowHandle = GetIntProperty(element, AutomationElement.NativeWindowHandleProperty);
            info.BoundingRectangle = ToPersistedRectangle(GetBoundingRectangle(element));
            info.AncestorPath = BuildAncestorPath(element);
            return info;
        }

        private string FormatHowFound(int? screenX, int? screenY)
        {
            if (!screenX.HasValue || !screenY.HasValue)
            {
                return string.Empty;
            }

            return string.Format("Mouse move ({0},{1})", screenX.Value, screenY.Value);
        }

        private List<AncestorDescriptor> BuildAncestorPath(AutomationElement element)
        {
            List<AutomationElement> elements = new List<AutomationElement>();
            TreeWalker walker = TreeWalker.ControlViewWalker;
            AutomationElement current = element;

            while (current != null && !AutomationElement.RootElement.Equals(current))
            {
                elements.Insert(0, current);
                current = walker.GetParent(current);
            }

            List<AncestorDescriptor> path = new List<AncestorDescriptor>();

            foreach (AutomationElement item in elements)
            {
                AncestorDescriptor descriptor = new AncestorDescriptor();
                descriptor.AutomationId = GetStringProperty(item, AutomationElement.AutomationIdProperty);
                descriptor.Name = GetStringProperty(item, AutomationElement.NameProperty);
                descriptor.ControlType = ControlTypeToName(GetControlType(item));
                descriptor.IndexWithinParent = GetIndexWithinMatchingSiblings(item);
                path.Add(descriptor);
            }

            return path;
        }

        private int? GetIndexWithinMatchingSiblings(AutomationElement element)
        {
            TreeWalker walker = TreeWalker.ControlViewWalker;
            AutomationElement parent = walker.GetParent(element);

            if (parent == null)
            {
                return null;
            }

            AutomationElementCollection siblings = parent.FindAll(
                TreeScope.Children,
                System.Windows.Automation.Condition.TrueCondition);
            int matchIndex = 0;
            AncestorDescriptor descriptor = new AncestorDescriptor();
            descriptor.AutomationId = GetStringProperty(element, AutomationElement.AutomationIdProperty);
            descriptor.Name = GetStringProperty(element, AutomationElement.NameProperty);
            descriptor.ControlType = ControlTypeToName(GetControlType(element));

            foreach (AutomationElement sibling in siblings)
            {
                if (!DescriptorMatchesElement(descriptor, sibling))
                {
                    continue;
                }

                if (AutomationElement.Equals(sibling, element))
                {
                    return matchIndex;
                }

                matchIndex++;
            }

            return null;
        }

        private bool MatchesLocator(AutomationElement element, ElementLocator locator)
        {
            if (element == null || locator == null)
            {
                return false;
            }

            if (!string.IsNullOrEmpty(locator.AutomationId) &&
                !string.Equals(GetStringProperty(element, AutomationElement.AutomationIdProperty), locator.AutomationId, StringComparison.Ordinal))
            {
                return false;
            }

            if (!string.IsNullOrEmpty(locator.ControlType) &&
                !string.Equals(ControlTypeToName(GetControlType(element)), locator.ControlType, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            if (!string.IsNullOrEmpty(locator.Name) &&
                !string.Equals(GetStringProperty(element, AutomationElement.NameProperty), locator.Name, StringComparison.Ordinal))
            {
                return false;
            }

            return true;
        }

        private bool MatchesInfo(AutomationElement element, UiElementInfo info)
        {
            if (element == null || info == null)
            {
                return false;
            }

            if (info.ProcessId > 0 && GetIntProperty(element, AutomationElement.ProcessIdProperty) != info.ProcessId)
            {
                return false;
            }

            if (!string.IsNullOrEmpty(info.AutomationId) &&
                !string.Equals(GetStringProperty(element, AutomationElement.AutomationIdProperty), info.AutomationId, StringComparison.Ordinal))
            {
                return false;
            }

            return true;
        }

        private bool DescriptorMatchesElement(AncestorDescriptor descriptor, AutomationElement element)
        {
            if (descriptor == null || element == null)
            {
                return false;
            }

            if (!string.IsNullOrEmpty(descriptor.AutomationId))
            {
                return string.Equals(
                    descriptor.AutomationId,
                    GetStringProperty(element, AutomationElement.AutomationIdProperty),
                    StringComparison.Ordinal);
            }

            bool controlTypeMatches = string.IsNullOrEmpty(descriptor.ControlType) ||
                string.Equals(descriptor.ControlType, ControlTypeToName(GetControlType(element)), StringComparison.OrdinalIgnoreCase);

            bool nameMatches = string.IsNullOrEmpty(descriptor.Name) ||
                string.Equals(descriptor.Name, GetStringProperty(element, AutomationElement.NameProperty), StringComparison.Ordinal);

            return controlTypeMatches && nameMatches;
        }

        private bool RectangleWithinTolerance(WindowsRect actual, PersistedRectangle expected)
        {
            return Math.Abs(actual.X - expected.X) <= BoundingRectangleTolerance &&
                Math.Abs(actual.Y - expected.Y) <= BoundingRectangleTolerance &&
                Math.Abs(actual.Width - expected.Width) <= BoundingRectangleTolerance &&
                Math.Abs(actual.Height - expected.Height) <= BoundingRectangleTolerance;
        }

        private string GetStringProperty(AutomationElement element, AutomationProperty property)
        {
            object value = element.GetCurrentPropertyValue(property, true);
            return value == AutomationElement.NotSupported || value == null ? string.Empty : value.ToString();
        }

        private string GetInspectName(AutomationElement element, int? screenX, int? screenY)
        {
            string nativeName;

            if (screenX.HasValue && screenY.HasValue &&
                TryGetNativeNameFromPoint(screenX.Value, screenY.Value, out nativeName))
            {
                return CorrectWinFormsEditName(element, nativeName);
            }

            if (TryGetNativeNameFromHandle(element, out nativeName))
            {
                return CorrectWinFormsEditName(element, nativeName);
            }

            return CorrectWinFormsEditName(element, GetStringProperty(element, AutomationElement.NameProperty));
        }

        private string CorrectWinFormsEditName(AutomationElement element, string providerName)
        {
            if (element == null || string.IsNullOrEmpty(providerName) || !IsWinFormsEdit(element))
            {
                return providerName ?? string.Empty;
            }

            string nearestLabelName;

            if (!TryGetNearestLeftTextSiblingName(element, out nearestLabelName) ||
                string.IsNullOrEmpty(nearestLabelName) ||
                NamesMatch(providerName, nearestLabelName))
            {
                return providerName;
            }

            return HasSiblingTextName(element, providerName) ? nearestLabelName : providerName;
        }

        private bool IsWinFormsEdit(AutomationElement element)
        {
            return string.Equals(ControlTypeToName(GetControlType(element)), "Edit", StringComparison.OrdinalIgnoreCase) &&
                (GetStringProperty(element, AutomationElement.ClassNameProperty) ?? string.Empty).StartsWith(
                    "WindowsForms10.EDIT",
                    StringComparison.OrdinalIgnoreCase);
        }

        private bool TryGetNearestLeftTextSiblingName(AutomationElement element, out string name)
        {
            name = string.Empty;
            AutomationElement parent = TreeWalker.ControlViewWalker.GetParent(element);

            if (parent == null)
            {
                return false;
            }

            WindowsRect editRectangle = GetBoundingRectangle(element);

            if (editRectangle.IsEmpty)
            {
                return false;
            }

            AutomationElementCollection siblings = parent.FindAll(
                TreeScope.Children,
                System.Windows.Automation.Condition.TrueCondition);
            double bestDistance = double.MaxValue;
            double bestVerticalDelta = double.MaxValue;

            foreach (AutomationElement sibling in siblings)
            {
                if (AutomationElement.Equals(sibling, element) ||
                    !string.Equals(ControlTypeToName(GetControlType(sibling)), "Text", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                string siblingName = GetStringProperty(sibling, AutomationElement.NameProperty);

                if (string.IsNullOrEmpty(siblingName))
                {
                    continue;
                }

                WindowsRect siblingRectangle = GetBoundingRectangle(sibling);

                if (siblingRectangle.IsEmpty || !IsLeftRowLabel(editRectangle, siblingRectangle))
                {
                    continue;
                }

                double horizontalDistance = editRectangle.X - (siblingRectangle.X + siblingRectangle.Width);
                double verticalDelta = Math.Abs(GetCenterY(editRectangle) - GetCenterY(siblingRectangle));

                if (horizontalDistance < bestDistance ||
                    (Math.Abs(horizontalDistance - bestDistance) < 0.1 && verticalDelta < bestVerticalDelta))
                {
                    bestDistance = horizontalDistance;
                    bestVerticalDelta = verticalDelta;
                    name = siblingName;
                }
            }

            return !string.IsNullOrEmpty(name);
        }

        private bool IsLeftRowLabel(WindowsRect editRectangle, WindowsRect labelRectangle)
        {
            const double LeftTolerance = 4.0;
            double labelRight = labelRectangle.X + labelRectangle.Width;

            if (labelRight > editRectangle.X + LeftTolerance)
            {
                return false;
            }

            return RectanglesOverlapVertically(editRectangle, labelRectangle) ||
                Math.Abs(GetCenterY(editRectangle) - GetCenterY(labelRectangle)) <= Math.Max(editRectangle.Height, labelRectangle.Height);
        }

        private bool RectanglesOverlapVertically(WindowsRect first, WindowsRect second)
        {
            double firstBottom = first.Y + first.Height;
            double secondBottom = second.Y + second.Height;
            return first.Y <= secondBottom && second.Y <= firstBottom;
        }

        private double GetCenterY(WindowsRect rectangle)
        {
            return rectangle.Y + (rectangle.Height / 2.0);
        }

        private bool HasSiblingTextName(AutomationElement element, string expectedName)
        {
            AutomationElement parent = TreeWalker.ControlViewWalker.GetParent(element);

            if (parent == null || string.IsNullOrEmpty(expectedName))
            {
                return false;
            }

            AutomationElementCollection siblings = parent.FindAll(
                TreeScope.Children,
                System.Windows.Automation.Condition.TrueCondition);

            foreach (AutomationElement sibling in siblings)
            {
                if (AutomationElement.Equals(sibling, element) ||
                    !string.Equals(ControlTypeToName(GetControlType(sibling)), "Text", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (NamesMatch(expectedName, GetStringProperty(sibling, AutomationElement.NameProperty)))
                {
                    return true;
                }
            }

            return false;
        }

        private bool NamesMatch(string first, string second)
        {
            return string.Equals(
                NormalizeLabelName(first),
                NormalizeLabelName(second),
                StringComparison.OrdinalIgnoreCase);
        }

        private string NormalizeLabelName(string name)
        {
            return (name ?? string.Empty).Trim().TrimEnd(':').Trim();
        }

        private bool TryGetNativeNameFromPoint(int screenX, int screenY, out string name)
        {
            name = string.Empty;
            IUIAutomation automation = null;
            IUIAutomationElement nativeElement = null;
            NativePoint providerPoint = PhysicalToNativeProviderPoint(screenX, screenY);

            try
            {
                automation = (IUIAutomation)new CUIAutomation();
                nativeElement = automation.ElementFromPoint(providerPoint);
                return TryGetNativeName(nativeElement, out name);
            }
            catch (COMException)
            {
                return false;
            }
            catch (InvalidCastException)
            {
                return false;
            }
            finally
            {
                ReleaseComObject(nativeElement);
                ReleaseComObject(automation);
            }
        }

        private bool TryGetNativeNameFromHandle(AutomationElement element, out string name)
        {
            name = string.Empty;

            if (element == null)
            {
                return false;
            }

            int nativeWindowHandle = GetIntProperty(element, AutomationElement.NativeWindowHandleProperty);

            if (nativeWindowHandle == 0)
            {
                return false;
            }

            IUIAutomation automation = null;
            IUIAutomationElement nativeElement = null;

            try
            {
                automation = (IUIAutomation)new CUIAutomation();
                nativeElement = automation.ElementFromHandle(new IntPtr(nativeWindowHandle));
                return TryGetNativeName(nativeElement, out name);
            }
            catch (COMException)
            {
                return false;
            }
            catch (InvalidCastException)
            {
                return false;
            }
            finally
            {
                ReleaseComObject(nativeElement);
                ReleaseComObject(automation);
            }
        }

        private bool TryGetNativeName(IUIAutomationElement nativeElement, out string name)
        {
            name = string.Empty;

            if (nativeElement == null)
            {
                return false;
            }

            object value = nativeElement.GetCurrentPropertyValueEx(NativeUiAutomationNamePropertyId, true);

            if (value == null || value == DBNull.Value)
            {
                return true;
            }

            name = value.ToString();
            return true;
        }

        private void ReleaseComObject(object comObject)
        {
            if (comObject != null && Marshal.IsComObject(comObject))
            {
                Marshal.ReleaseComObject(comObject);
            }
        }

        private int GetIntProperty(AutomationElement element, AutomationProperty property)
        {
            object value = element.GetCurrentPropertyValue(property, true);
            return value == AutomationElement.NotSupported || value == null ? 0 : Convert.ToInt32(value);
        }

        private WindowsRect GetBoundingRectangle(AutomationElement element)
        {
            return GetPhysicalBoundingRectangle(element);
        }

        private WindowsRect GetPhysicalBoundingRectangle(AutomationElement element)
        {
            if (element == null)
            {
                return WindowsRect.Empty;
            }

            int nativeWindowHandle = GetIntProperty(element, AutomationElement.NativeWindowHandleProperty);

            if (nativeWindowHandle != 0)
            {
                WindowsRect windowRectangle;

                if (TryGetPhysicalWindowRectangle(new IntPtr(nativeWindowHandle), out windowRectangle))
                {
                    return windowRectangle;
                }
            }

            WindowsRect rectangle = GetRawBoundingRectangle(element);

            if (rectangle.IsEmpty || nativeWindowHandle != 0)
            {
                return rectangle;
            }

            // HWND-backed UIA elements already report physical pixels via GetWindowRect.
            // WinForms/MSAA child elements without their own HWND can report coordinates
            // in the DPI-unaware target process' virtual screen space, so scale those
            // provider rectangles back to TalosUI's physical screen coordinate contract.
            IntPtr containingWindowHandle = GetContainingNativeWindowHandle(element);
            DpiVirtualizationContext context = GetDpiVirtualizationContextFromWindow(containingWindowHandle);
            return ProviderToPhysicalRectangle(rectangle, context);
        }

        private WindowsRect GetRawBoundingRectangle(AutomationElement element)
        {
            object value = element.GetCurrentPropertyValue(AutomationElement.BoundingRectangleProperty, true);

            if (value == AutomationElement.NotSupported || value == null)
            {
                return WindowsRect.Empty;
            }

            return (WindowsRect)value;
        }

        private bool TryGetPhysicalWindowRectangle(IntPtr hwnd, out WindowsRect rectangle)
        {
            rectangle = WindowsRect.Empty;

            if (hwnd == IntPtr.Zero)
            {
                return false;
            }

            NativeRect nativeRectangle;

            if (!GetWindowRect(hwnd, out nativeRectangle))
            {
                return false;
            }

            if (nativeRectangle.Right <= nativeRectangle.Left || nativeRectangle.Bottom <= nativeRectangle.Top)
            {
                return false;
            }

            rectangle = new WindowsRect(
                nativeRectangle.Left,
                nativeRectangle.Top,
                nativeRectangle.Right - nativeRectangle.Left,
                nativeRectangle.Bottom - nativeRectangle.Top);
            return true;
        }

        private IntPtr GetContainingNativeWindowHandle(AutomationElement element)
        {
            TreeWalker walker = TreeWalker.ControlViewWalker;
            AutomationElement current = element;

            while (current != null && !AutomationElement.RootElement.Equals(current))
            {
                int nativeWindowHandle = GetIntProperty(current, AutomationElement.NativeWindowHandleProperty);

                if (nativeWindowHandle != 0)
                {
                    return new IntPtr(nativeWindowHandle);
                }

                current = walker.GetParent(current);
            }

            return IntPtr.Zero;
        }

        private PersistedRectangle ToPersistedRectangle(WindowsRect rectangle)
        {
            if (rectangle.IsEmpty)
            {
                return new PersistedRectangle();
            }

            return new PersistedRectangle(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
        }

        private ControlType GetControlType(AutomationElement element)
        {
            object value = element.GetCurrentPropertyValue(AutomationElement.ControlTypeProperty, true);
            return value == AutomationElement.NotSupported || value == null ? null : (ControlType)value;
        }

        private string ControlTypeToName(ControlType controlType)
        {
            if (controlType == null)
            {
                return string.Empty;
            }

            string programmaticName = controlType.ProgrammaticName ?? string.Empty;
            const string prefix = "ControlType.";
            return programmaticName.StartsWith(prefix, StringComparison.Ordinal)
                ? programmaticName.Substring(prefix.Length)
                : programmaticName;
        }

        private ControlType ControlTypeFromName(string controlTypeName)
        {
            if (string.IsNullOrEmpty(controlTypeName))
            {
                return ControlType.Custom;
            }

            string expectedName = controlTypeName.StartsWith("ControlType.", StringComparison.Ordinal)
                ? controlTypeName
                : "ControlType." + controlTypeName;

            foreach (System.Reflection.PropertyInfo property in typeof(ControlType).GetProperties())
            {
                if (property.PropertyType != typeof(ControlType))
                {
                    continue;
                }

                ControlType controlType = (ControlType)property.GetValue(null, null);

                if (string.Equals(controlType.ProgrammaticName, expectedName, StringComparison.OrdinalIgnoreCase))
                {
                    return controlType;
                }
            }

            return ControlType.Custom;
        }

        private class DpiVirtualizationContext
        {
            public DpiVirtualizationContext()
            {
                IsValid = false;
                WindowHandle = IntPtr.Zero;
                MonitorHandle = IntPtr.Zero;
                ProcessId = 0;
                MonitorLeft = 0;
                MonitorTop = 0;
                ScaleX = 1.0;
                ScaleY = 1.0;
            }

            public bool IsValid;
            public IntPtr WindowHandle;
            public IntPtr MonitorHandle;
            public int ProcessId;
            public int MonitorLeft;
            public int MonitorTop;
            public double ScaleX;
            public double ScaleY;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct NativePoint
        {
            public NativePoint(int x, int y)
            {
                X = x;
                Y = y;
            }

            public int X;
            public int Y;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct NativeRect
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MonitorInfo
        {
            public int Size;
            public NativeRect Monitor;
            public NativeRect Work;
            public int Flags;
        }

        [DllImport("user32.dll")]
        private static extern IntPtr WindowFromPoint(NativePoint point);

        [DllImport("user32.dll")]
        private static extern IntPtr MonitorFromPoint(NativePoint point, uint flags);

        [DllImport("user32.dll")]
        private static extern IntPtr MonitorFromWindow(IntPtr hwnd, uint flags);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetMonitorInfo(IntPtr monitor, ref MonitorInfo monitorInfo);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetWindowRect(IntPtr hwnd, out NativeRect rectangle);

        [DllImport("user32.dll")]
        private static extern IntPtr GetAncestor(IntPtr hwnd, uint flags);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint GetWindowThreadProcessId(IntPtr hwnd, out uint processId);

        [DllImport("user32.dll", EntryPoint = "GetDpiForWindow")]
        private static extern uint GetDpiForWindowNative(IntPtr hwnd);

        [DllImport("shcore.dll", EntryPoint = "GetDpiForMonitor")]
        private static extern int GetDpiForMonitorNative(IntPtr monitor, int dpiType, out uint dpiX, out uint dpiY);

        [ComImport]
        [Guid("ff48dba4-60ef-4201-aa87-54103eef594e")]
        private class CUIAutomation
        {
        }

        [ComImport]
        [Guid("30cbe57d-d9d0-452a-ab13-7ac5ac4825ee")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IUIAutomation
        {
            [return: MarshalAs(UnmanagedType.Bool)]
            bool CompareElements(IUIAutomationElement element1, IUIAutomationElement element2);

            [return: MarshalAs(UnmanagedType.Bool)]
            bool CompareRuntimeIds(
                [MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_I4)] int[] runtimeId1,
                [MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_I4)] int[] runtimeId2);

            IUIAutomationElement GetRootElement();

            IUIAutomationElement ElementFromHandle(IntPtr hwnd);

            IUIAutomationElement ElementFromPoint(NativePoint point);
        }

        [ComImport]
        [Guid("d22108aa-8ac5-49a5-837b-37bbb3d7591e")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IUIAutomationElement
        {
            void SetFocus();

            [return: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_I4)]
            int[] GetRuntimeId();

            IUIAutomationElement FindFirst(int scope, IntPtr condition);

            IntPtr FindAll(int scope, IntPtr condition);

            IUIAutomationElement FindFirstBuildCache(int scope, IntPtr condition, IntPtr cacheRequest);

            IntPtr FindAllBuildCache(int scope, IntPtr condition, IntPtr cacheRequest);

            IUIAutomationElement BuildUpdatedCache(IntPtr cacheRequest);

            [return: MarshalAs(UnmanagedType.Struct)]
            object GetCurrentPropertyValue(int propertyId);

            [return: MarshalAs(UnmanagedType.Struct)]
            object GetCurrentPropertyValueEx(int propertyId, [MarshalAs(UnmanagedType.Bool)] bool ignoreDefaultValue);
        }
    }

    public partial class CTalosCore
    {
    }
}
