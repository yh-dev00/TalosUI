# TalosUI – Complete Plan and Requirements

## 1. Project Summary

**Name:** TalosUI  
**Platform:** C# WinForms, Visual Studio 2015  
**Output:** A WinForms `.exe` plus a core `.cs` library (`CTalosCore`)

TalosUI uses Microsoft UI Automation (UIA) to:

- Inspect UI elements of a *target* Windows desktop application.
- Record user interactions with those elements as *steps*.
- Save those steps in files as *test suites* with multiple *test cases*.
- Replay steps to automate the target UI.
- Decide pass/fail based on expected UI behavior (values, windows, crashes, unexpected UI) [4].

The TalosUI core logic is implemented in a C# library (partial class `CTalosCore`), separated from the WinForms UI code [4].

---

## 2. Scope and Constraints

### 2.1. Supported Targets (v1)

- Classic Windows desktop apps:
  - Win32, WinForms, WPF [4].
- Same user session and desktop as TalosUI.
- 32‑bit and 64‑bit targets:
  - TalosUI itself will be 64‑bit.
  - 32‑bit support is best‑effort.

### 2.2. Not In Scope (v1)

- UWP / Store apps.
- Cross‑session / remote desktop automation.
- Web automation through browsers.

### 2.3. Elevation

- TalosUI is expected to be started **as Administrator**.
- This allows automating elevated target apps, but increases risk:
  - Document this clearly in user docs.
  - No automatic elevation prompts in v1.

---

## 3. High‑Level User Flow

### 3.1. Recording Flow

1. User launches TalosUI (as admin).
2. User selects a target `.exe` via file browsing [4].
3. User launches the target app from TalosUI.
4. User starts an **Inspect session**:
   - TalosUI tracks the mouse.
   - When the cursor rests on a UI element for ≥1s:
     - TalosUI retrieves that element via UIA.
     - Displays its info (AutomationId, Name, ControlType, etc.).
     - Highlights it with a translucent overlay [4].
5. User switches to **Record mode**:
   - Same hover inspection as above.
   - When the user clicks on elements in the target UI:
     - TalosUI records steps (element + action).
6. User stops recording when done.
7. User can optionally edit:
   - Step order, per‑step delay.
   - Test cases and conditions.
8. User saves everything into a **test suite file** (`*.talos.json`).

### 3.2. Replay Flow

1. User opens an existing TalosUI suite file [4].
2. TalosUI shows all **test cases** within the suite.
3. User runs:
   - A single test case, or
   - All enabled test cases in order.
4. For each test case:
   - Ensure target app is running:
     - Launch if needed.
   - Execute steps:
     - Locate each element using the stored locator strategy.
     - Perform the recorded action.
     - Wait the configured delay.
     - After each step:
       - Check for app crash.
       - Check for forbidden windows.
   - After steps finish:
     - Evaluate value checks.
     - Evaluate expected windows.
   - Mark test as Pass or Fail.
5. After all tests:
   - Generate a human‑readable `.txt` report [4].
   - Generate a structured JSON report for tools.
   - Screenshots are captured on failures.

---

## 4. Functional Requirements

### 4.1. Target Application Handling

- Browse to select target `.exe` [4].
- Optionally specify launch arguments.
- Launch the target process.
- Detect if target is running or has crashed.
- Optionally close and relaunch target between tests:
  - Controlled by `AutoRelaunchBetweenTests` flag.

### 4.2. Inspect Session

- Behavior similar to Microsoft Inspect.exe [4]:
  - Track current element under the mouse cursor.
  - After ≥1s of stable cursor, resolve that element via UIA.
  - Display key UIA properties:
    - AutomationId.
    - Name.
    - ControlType.
    - ClassName.
    - ProcessId.
    - BoundingRectangle.
    - Ancestor path.
  - Draw a translucent highlight overlay covering the element [4].

### 4.3. Record Mode

- Uses two states: **Inspect** and **Record**.
- In Record mode:
  - Global mouse click capture (left button) while active.
  - For each click:
    - Determine the element under the cursor.
    - Convert it to a `UiElementInfo` and then a `ElementLocator`.
    - Create a `Step` with action (v1 = mainly `Invoke`).
    - Append the Step to the currently selected TestCase.
- Hover inspection continues (overlay + info) to aid accurate clicking.
- Auto‑recording of text (SetValue) is postponed or kept minimal in v1.

### 4.4. Test Suites and Test Cases

- One file (`*.talos.json`) = one **Test Suite** [4].
- A suite contains multiple **Test Cases**:
  - Example: `TestCase1 {step1..4}`, `TestCase2 {step1..6}`, `TestCase3 {step1..2}` [4].
- Suite‑level configuration:
  - `SuiteName`.
  - `TargetAppPath`.
  - `LaunchParams`.
  - `DefaultFixedDelayMs`.
  - `AutoRelaunchBetweenTests`.
- TestCase‑level:
  - `Name`.
  - `Description`.
  - `Steps[]`.
  - `Conditions` (per test).

### 4.5. Steps

- Each Step includes:
  - Unique `Id` within the test.
  - An `ElementLocator`.
  - An `Action` (enum).
  - Optional `Parameters` (e.g., text).
  - Optional per‑step `FixedDelayMs` override.

- v1 actions:
  - `Invoke` (click / press).
  - `SetValue` (enter text; may be manually configured).
  - `SelectItem` (e.g., list/tree item).
  - `SendKeys` (optional, can be minimal in v1).

### 4.6. Replay Behavior

For each Step:

1. Ensure target app is still running:
   - If not, the app is considered crashed -> test fails.
2. Find element using `ElementLocator` and the tiered locator strategy.
3. If element not found:
   - Fail current test.
   - Log and capture screenshot.
4. Perform the action via UI Automation:
   - E.g., InvokePattern / ValuePattern / SelectionItemPattern [1].
5. Wait:
   - Use `Step.FixedDelayMs` if set.
   - Otherwise, use `Suite.DefaultFixedDelayMs`.
6. After delay:
   - Check if app crashed.
   - Check forbidden windows.

### 4.7. Pass/Fail Conditions

Per TestCase:

1. **Value Checks**:
   - For specific elements:
     - Compare the element value (via UIA) with `ExpectedValue`.
   - Evaluated at the end of the test in v1.
   - If any mismatch, test fails.

2. **Window Conditions**:
   - `ExpectedWindows`:
     - Patterns describing windows that must appear at least once (or exist at end).
   - `ForbiddenWindows`:
     - Patterns describing windows that must not appear:
       - If found at any time → test fails.
   - Patterns can be simple substrings or regex.

3. **Global Error – Crash**:
   - If target app crashes during:
     - Execute step: test fails.
     - Conditions evaluation: test fails.
   - If `AutoRelaunchBetweenTests` is enabled:
     - Relaunch app for next test.

4. **Unexpected UI**:
   - Implemented via `ForbiddenWindows`:
     - If an unexpected (forbidden) top‑level window appears after a step, test fails [4].

---

## 5. Element Locator Strategy

### 5.1. Motivation

Using only coordinates is fragile. UI Automation docs recommend using AutomationId, control patterns, and UIA properties to locate elements robustly [1]. AutomationId uniquely identifies an element among its siblings, but not necessarily across the whole tree, so context is important [1].

TalosUI uses a **tiered locator** that combines:

- `AutomationId`.
- `ControlType`.
- `Name`.
- Hierarchical ancestor path.
- `BoundingRectangle` as last resort [4].

### 5.2. Locator Structure

Each `ElementLocator` stores:

- **Direct properties**:
  - `AutomationId`.
  - `Name`.
  - `ControlType` (custom enum: Window, Button, Edit, etc.).
  - `ClassName` (Win32 class, optional).

- **Hierarchy info**:
  - `AncestorPath[]`: list of `AncestorDescriptor` from top‑level window down:
    - `AutomationId`.
    - `Name`.
    - `ControlType`.
    - Optional `IndexWithinParent`.

- **Geometry**:
  - `BoundingRectangle` (`X`, `Y`, `Width`, `Height`).

- **Process context**:
  - `ProcessId` (to scope search to one app instance).

### 5.3. Tiered Lookup Algorithm (v1)

When `IUiAutomationService.FindElement(locator)` is called:

1. **Scope root**:
   - If `locator.ProcessId` present:
     - Enumerate top‑level windows (using UIA) for that process.
     - For v1, pick the first such window as the root.
   - If `ProcessId` missing:
     - Fallback to the desktop root (less efficient).

2. **Tier 1 – Find by AutomationId** [4]:
   - If `AutomationId` is non‑empty:
     - Search descendants of the root window for that AutomationId.
     - If found, return it.
     - Otherwise, proceed to Tier 2.
   - This follows UIA guidance that AutomationId is the preferred identification among siblings but not globally unique [1].

3. **Tier 2 – (ControlType + Name) under parent** [4]:
   - If `AncestorPath` is not empty:
     - Resolve parent container via path (Tier 3 logic).
     - Under that parent, search for child where:
       - `ControlType` matches locator.ControlType.
       - `Name` matches locator.Name.
     - If found, return it.
     - Otherwise, continue to Tier 3.

4. **Tier 3 – AncestorPath only** [4]:
   - From the root:
     - Walk `AncestorPath`:
       - Prefer matches by `AutomationId` when given.
       - Otherwise by `(ControlType + Name)` combination.
       - Use `IndexWithinParent` to disambiguate siblings when necessary.
   - If this walk ends in a valid element:
     - Use that element.
   - If path resolution fails, go to Tier 4.

5. **Tier 4 – BoundingRectangle fallback** [4]:
   - If `BoundingRectangle` is present and non‑empty:
     - Hit‑test at the rectangle’s center point using UI Automation’s `FromPoint`.
     - Compare the returned element’s rectangle to stored rectangle, accepting within small tolerances.
   - If this fails, treat element as not found.

If all tiers fail, `FindElement` returns `null`, and TestRunner fails the step and test.

Multi‑window disambiguation beyond “first window for ProcessId” is deferred to v2.

---

## 6. Data Model (Conceptual)

### 6.1. TestSuite

Represents one suite file (`*.talos.json`):

- `SchemaVersion` (integer).
- `SuiteName`.
- `TargetAppPath`.
- `LaunchParams`.
- `DefaultFixedDelayMs`.
- `AutoRelaunchBetweenTests`.
- `Tests[]`: list of `TestCase`.

### 6.2. TestCase

One logical test within a suite:

- `Name`.
- `Description`.
- `Steps[]`: ordered list of `Step`.
- `Conditions`: `TestConditions`.

### 6.3. Step

Represents one recorded or configured action:

- `Id`: sequential within a test.
- `Locator`: `ElementLocator`.
- `Action`: `StepAction`.
- `Parameters`: `StepParameters` (e.g., `Text`).
- `FixedDelayMs`: optional override for per‑step delay.

### 6.4. UiElementInfo

Runtime representation of a UI element (internal DTO):

- `AutomationId`.
- `Name`.
- `ControlType`.
- `ClassName`.
- `ProcessId`.
- `NativeWindowHandle`.
- `BoundingRectangle`.
- `AncestorPath[]`.

Used at runtime for inspect/record; can be converted to `ElementLocator` for persistence.

### 6.5. TestConditions

Per‑test pass/fail logic:

- `ValueChecks[]`:
  - Each has an `Id`, `ElementLocator`, `ExpectedValue`, and evaluation point (v1: EndOfTest).
- `ExpectedWindows[]`:
  - Patterns describing windows that must appear.
- `ForbiddenWindows[]`:
  - Patterns describing windows that cause failure if they appear.

---

## 7. Architecture and Modules

TalosUI is split between a **WinForms UI** and a **Core Library** (`CTalosCore`) [4].

### 7.1. WinForms UI Layer

Responsibilities:

- Present UI:
  - Target exe selection.
  - Test suite/test case list.
  - Step and conditions editor.
  - Run controls and progress.
- Manage state:
  - Idle / Inspect / Record / Running tests.
- Drive inspect and recording:
  - Mouse hover timer.
  - Global click capture.
  - Calls into `IUiAutomationService` to get elements.
  - Adds/edits/removes Steps and TestCases.

### 7.2. Core Library (`CTalosCore`)

Contains non‑UI logic organized roughly as:

- `IUiAutomationService` and its implementation.
- `ITargetProcessManager` for launch/close/crash detection.
- `TestRunner` for executing suites/tests.
- `CTalosMemory` for JSON storage and schemaVersion.
- `CTalosLog` for logging and screenshots.

This library is referenced by the WinForms project and is the main integration surface.

---

## 8. Key Interfaces and Responsibilities

### 8.1. ITargetProcessManager

Abstracts process lifecycle for target app:

- Determine if target is running.
- Launch target with specified path and arguments.
- Close target (graceful then kill if needed).
- Retrieve target’s `ProcessId`.

Used by TestRunner to:

- Ensure app is alive before and during tests.
- Detect crashes.
- Relaunch between tests when configured.

### 8.2. IUiAutomationService (continued)

Encapsulates all UI Automation (UIA) interactions and is the *only* place that directly uses `AutomationElement` and UIA patterns. This follows UIA’s recommended approach for automated test tools: obtain elements, get relevant control patterns (e.g., `InvokePattern`, `WindowPattern`), then use those patterns to drive the UI [1].

**Responsibilities:**

1. **Hit‑testing / Inspection**
   - `GetElementAtPoint(x, y)`:
     - Input: screen coordinates.
     - Behavior:
       - Uses UIA hit‑testing to retrieve the element under the cursor.
       - Converts the raw UIA element into a `UiElementInfo`.
     - Used by:
       - Inspect mode (hover to show info + highlight).
       - Record mode (determine element for a click).

2. **Locator‑based Search**
   - `FindElement(locator)`:
     - Input: `ElementLocator`.
     - Behavior:
       - Implements the 4‑tier lookup:
         1. Scope to a top‑level window for `locator.ProcessId`.
         2. Try AutomationId search.
         3. Try (ControlType + Name) inside parent resolved by AncestorPath.
         4. Try AncestorPath walk.
         5. Try `BoundingRectangle` fallback as last resort. [4]
       - Returns a `UiElementInfo` or `null` if no match.
     - Consumers:
       - TestRunner (for each step).
       - Condition evaluation (value checks, window checks).

3. **Actions on Elements**
   - `PerformAction(element, action, text = null)`:
     - Inputs:
       - `UiElementInfo` representing target element.
       - `StepAction` (Invoke, SetValue, SelectItem, SendKeys).
       - Optional `text` for SetValue/SendKeys.
     - Behavior:
       - Resolves the underlying UIA element.
       - Uses control patterns:
         - `InvokePattern` for `Invoke` on buttons, menu items, etc. [1]
         - `ValuePattern` for `SetValue` on text fields, combos. [1]
         - `SelectionItemPattern` for `SelectItem`. [1]
         - Simple keystroke sending or text patterns for `SendKeys`.
       - Returns success/failure.
     - Called by TestRunner for each replayed step.

4. **Value Retrieval**
   - `GetElementValue(element)`:
     - Input: `UiElementInfo`.
     - Behavior:
       - Uses `ValuePattern` when available to read the current value. [1]
       - Falls back to the element’s `Name` for static text / labels.
     - Used by:
       - Evaluation of `ValueCheck` conditions at test end.

5. **Top‑Level Window Enumeration**
   - `GetTopLevelWindowsForProcess(processId)`:
     - Input: process id.
     - Behavior:
       - Enumerates top‑level windows under UIA’s root element.
       - Filters windows whose `ProcessId` equals the given id.
       - Converts each to `UiElementInfo`.
     - Used by:
       - Detection of expected/forbidden windows.
       - Detection of unexpected UI events (via forbidden windows).

By centralizing all UIA interactions here, the rest of TalosUI (TestRunner, recording, conditions, logging) remains decoupled from UIA details and easier to test.

---

### 8.3. TestRunner

The TestRunner is the core automation engine that executes test suites and test cases.

**Responsibilities:**

- **Suite Execution**
  - Input: `TestSuite`.
  - Behavior:
    - Logs the start of a run.
    - Iterates over each `TestCase` in the suite.
    - Before each test:
      - Ensures the target app is running (launch if necessary).
    - After each test:
      - Logs test result.
      - If `AutoRelaunchBetweenTests` is true and more tests remain:
        - Closes the target app.
        - Relaunches it for a clean state.

- **Test Case Execution**
  - For each `TestCase`:
    - For each `Step`:
      1. Check if target process is still running:
         - If not, mark test failed (crash) and stop this test.
      2. Resolve the element via `IUiAutomationService.FindElement(locator)`:
         - If `null`, mark test failed (element not found), log, capture screenshot, stop this test.
      3. Perform the step’s action via `PerformAction`.
         - If action fails, mark test failed, log, capture screenshot, stop this test.
      4. Log step success/failure.
      5. Sleep the `FixedDelayMs` (step‑specific) or `DefaultFixedDelayMs` (suite‑level).
      6. After delay:
         - Check again for crash.
         - Check for forbidden windows:
           - If any forbidden window appears, fail test and stop it.
    - After all steps:
      - Evaluate value checks:
        - For each `ValueCheck`, locate element, read value, compare to `ExpectedValue`.
        - If any mismatch or unresolved element, fail test.
      - Evaluate expected windows:
        - For each expected window pattern, ensure a matching top‑level window exists.
        - If any expected window missing, fail test.
    - If no failures occurred, mark test as Passed.

- **Error Handling and Logging**
  - On any failure:
    - Record reason (e.g., crash, element not found, condition failed).
    - Invoke CTalosLog to:
      - Write step/test results.
      - Capture screenshot.

The TestRunner never directly uses UIA types or WinForms; it only depends on `IUiAutomationService`, `ITargetProcessManager`, and `CTalosLog`.

---

### 8.4. CTalosMemory

CTalosMemory manages **persisted test data** and schema evolution.

**Responsibilities:**

- **Serialization / Deserialization**
  - Save `TestSuite` objects to JSON files (`*.talos.json`).
  - Load from JSON into `TestSuite`.
  - Preserve all fields (suite metadata, tests, steps, locators, conditions, schemaVersion).

- **Schema Versioning**
  - Include a `SchemaVersion` integer in each file.
  - On load:
    - If version == current:
      - Load normally.
    - If version is older:
      - Either reject with a clear message or perform migration (future feature).
    - If version is newer:
      - Reject with “unsupported version” message.

- **File Management**
  - Provide simple APIs to:
    - Load a suite from a path.
    - Save a suite to a path.
  - Hide file system details from UI and TestRunner.

This module ensures that test definitions remain stable over time and that the rest of the system can treat them as in‑memory objects.

---

### 8.5. CTalosLog

CTalosLog is responsible for **run‑time logging** and artifacts (text reports, JSON results, screenshots).

**Responsibilities:**

- **Run Summary (Text)**
  - Generate a human‑readable `.txt` report after running a suite:
    - Suite name, target app, run start/end time.
    - For each test:
      - Name, overall status (Pass/Fail), failure reason if any.
    - Optionally a summary line per step (e.g., “Step 3: Passed”).

- **Structured Results (JSON)**
  - Produce a JSON file with:
    - Run metadata (timestamps, machine name, etc. as needed).
    - Per‑test results:
      - Name, status, failure reason.
      - Optional per‑step results (id, action, status, messages).
  - Usable by external tools and pipelines.

- **Screenshots**
  - Capture screenshots **on failure**:
    - At least one per failed test.
    - Either entire screen(s) or main target window.
  - Associate screenshot file names with the test/step in logs.

- **API Surface for Core Modules**
  - Provide methods like:
    - Start/end run.
    - Log per‑step results.
    - Log per‑test results.
    - Capture failure screenshot.

The implementation details (file paths, naming conventions, formats) are encapsulated so TestRunner and UI only call simple logging methods.

---

### 8.6. Highlight Overlay Component

The highlight overlay is a small but important UX component that visually indicates which element is under inspection or about to be recorded.

**Responsibilities:**

- **Display**
  - Draw a translucent rectangle around the element’s `BoundingRectangle` (from `UiElementInfo`) during Inspect and Record modes.

- **Non‑Intrusive Behavior**
  - Always on top but click‑through:
    - Must not steal focus or intercept mouse/keyboard input.
  - Should not interfere with UIA hit‑testing or the target app’s behavior.

- **Lifecycle**
  - Shown when:
    - `currentHoverElement` is set based on cursor dwell.
  - Hidden when:
    - Leaving Inspect/Record modes or when no valid element is under the cursor.

The overlay’s implementation details (transparent window styles, drawing) are part of the WinForms UI layer, not the core logic.

---

## 9. Timing and Synchronization (v1)

For v1, TalosUI uses **fixed delays** only:

- **Suite‑Level Default**
  - `DefaultFixedDelayMs` defines the default wait time after each step.

- **Step‑Level Override**
  - Each `Step` may define its own `FixedDelayMs`:
    - If present, it overrides the suite default for that step.

- **Execution Flow**
  - After performing a step’s action successfully:
    - TestRunner sleeps for the configured delay before:
      - Checking for crashes.
      - Checking forbidden windows.
      - Proceeding to the next step.

**Future (v2)**: explicit waits:

- Introduce actions like `WaitForElement` and `WaitForWindow` with timeouts and polling, to replace or complement fixed delays.

---

## 10. Testing Strategy (High Level)

TalosUI’s testing strategy follows the idea that good tests validate observable behavior, not implementation details [2].

**Unit Tests:**

- **Locator Resolution**
  - Given a synthetic tree model and a `ElementLocator`, verify:
    - Correct element when AutomationId matches.
    - Correct element when using (ControlType + Name) with a parent.
    - Correct resolution using AncestorPath.
    - Proper fallback behavior when all searches fail.

- **TestRunner Logic**
  - Simulate:
    - Successful test with all steps passing.
    - Element not found → test fails and stops.
    - Forbidden window appears → test fails immediately.
    - Target crash mid‑test → test fails with correct reason.
  - Use mocks for:
    - `IUiAutomationService`.
    - `ITargetProcessManager`.
    - `CTalosLog`.

- **CTalosMemory**
  - Verify:
    - Correct round‑trip serialize/deserialize for various suite structures.
    - Handling of schemaVersion.

- **CTalosLog**
  - With I/O abstracted, confirm:
    - Proper events are logged for steps and tests.

**Integration / Manual Testing:**

- Use small demo apps (sample WinForms/WPF apps) as targets:
  - Verify end‑to‑end:
    - Inspect behavior.
    - Recording clicks as steps.
    - Replay flows.
    - Handling of common error conditions (e.g., unexpected pop‑ups, crashes).

---

## 11. Future Enhancements (Beyond v1)

Planned (but not required) for later versions:

- **Improved Multi‑Window Handling**
  - Extend locators to explicitly identify the top‑level window (e.g., by name/class).
  - Use this to pick the correct root window when multiple windows share the same process.

- **Explicit Waits**
  - `WaitForElement` and `WaitForWindow` steps with timeouts and fail‑on‑timeout behavior.

- **Richer Conditions**
  - Step‑scoped conditions (e.g., “after step 4, this label must have value X”).
  - Composite conditions (AND/OR).

- **Enhanced Recording**
  - Automatic detection of text entry and SetValue steps.
  - Recording key sequences (SendKeys) beyond just clicks.

- **Editing and UX**
  - Graphical test/suite editor with drag‑drop reordering of steps.
  - Rich condition editors with element pickers.

- **Configuration**
  - Global settings for:
    - Default delays.
    - Log locations.
    - Screenshot formats.

