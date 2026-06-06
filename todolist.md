# TalosUI Todo List

This checklist is based on `PRD.md` and the current project state. Use it as a living implementation tracker for the v1 WinForms UI automation tool.

## Phase 0 - Project Foundation

- [x] Confirm Visual Studio 2015 / .NET Framework target compatibility.
- [x] Set TalosUI build target to 64-bit as required by the PRD.
- [x] Decide whether `Src` remains linked into the WinForms project or becomes a separate class library project.
- [x] Add a small README or docs note that TalosUI should be run as Administrator.
- [x] Add/update app manifest for administrator execution expectations if desired for v1.
- [x] Remove hard-coded sample target path from `MainForm.cs`.
- [x] Establish folder structure for core code, UI code, logs, reports, and test assets.

## Phase 1 - Core Data Model

- [x] Create `TestSuite` model.
- [x] Add suite fields:
  - [x] `SchemaVersion`
  - [x] `SuiteName`
  - [x] `TargetAppPath`
  - [x] `LaunchParams`
  - [x] `DefaultFixedDelayMs`
  - [x] `AutoRelaunchBetweenTests`
  - [x] `Tests`
- [x] Create `TestCase` model.
- [x] Add test case fields:
  - [x] `Name`
  - [x] `Description`
  - [x] `Steps`
  - [x] `Conditions`
- [x] Create `Step` model.
- [x] Add step fields:
  - [x] `Id`
  - [x] `Locator`
  - [x] `Action`
  - [x] `Parameters`
  - [x] `FixedDelayMs`
- [x] Create `StepAction` enum:
  - [x] `Invoke`
  - [x] `SetValue`
  - [x] `SelectItem`
  - [x] `SendKeys`
- [x] Create `StepParameters` model.
- [x] Create `UiElementInfo` runtime DTO.
- [x] Create `ElementLocator` persistence DTO.
- [x] Create `AncestorDescriptor` model.
- [x] Create rectangle DTO for persisted bounding rectangles.
- [x] Create `TestConditions` model.
- [x] Create `ValueCheck` model.
- [x] Create expected and forbidden window condition models.
- [x] Add conversion from `UiElementInfo` to `ElementLocator`.

## Phase 2 - Target Process Management

- [x] Define `ITargetProcessManager`.
- [x] Implement target process launch with optional arguments.
- [x] Implement running/crash detection.
- [x] Implement process id retrieval.
- [x] Implement graceful close.
- [x] Implement forced kill fallback after graceful close timeout.
- [x] Implement relaunch support for `AutoRelaunchBetweenTests`.
- [x] Surface clear errors when target path is missing or invalid.

## Phase 3 - UI Automation Service

- [x] Define `IUiAutomationService`.
- [x] Implement `GetElementAtPoint(x, y)` using UI Automation hit testing.
- [x] Convert `AutomationElement` into `UiElementInfo`.
- [x] Capture key UIA properties:
  - [x] `AutomationId`
  - [x] `Name`
  - [x] `ControlType`
  - [x] `ClassName`
  - [x] `ProcessId`
  - [x] `NativeWindowHandle`
  - [x] `BoundingRectangle`
  - [x] `AncestorPath`
- [x] Implement `GetTopLevelWindowsForProcess(processId)`.
- [x] Implement `FindElement(locator)` with scoped root lookup.
- [x] Implement Tier 1 lookup by `AutomationId`.
- [x] Implement Tier 2 lookup by `ControlType + Name` under resolved parent.
- [x] Implement Tier 3 lookup by `AncestorPath`.
- [x] Implement Tier 4 fallback by bounding rectangle center hit test.
- [x] Implement tolerance comparison for bounding rectangle fallback.
- [x] Implement `PerformAction(element, action, text)`.
- [x] Implement `Invoke` via `InvokePattern`.
- [x] Implement `SetValue` via `ValuePattern`.
- [x] Implement `SelectItem` via `SelectionItemPattern`.
- [x] Implement minimal `SendKeys` support.
- [x] Implement `GetElementValue(element)` via `ValuePattern` with `Name` fallback.
- [x] Keep all direct UIA types inside the UI automation service boundary where practical.

## Phase 4 - Inspect Mode

- [x] Add UI state enum: `Idle`, `Inspect`, `Record`, `Running`.
- [x] Add start/stop inspect controls in WinForms UI.
- [x] Track mouse position during inspect mode.
- [x] Add stable hover dwell timer of at least 1 second.
- [x] Resolve hovered element through `IUiAutomationService.GetElementAtPoint`.
- [x] Display inspected element details in the UI.
- [x] Show AutomationId, Name, ControlType, ClassName, ProcessId, BoundingRectangle, and AncestorPath.
- [x] Hide stale element details when no valid element is found.
- [x] Ensure inspect mode stops cleanly when switching to idle/running states.

## Phase 5 - Highlight Overlay

- [x] Verify existing `HighlighterWindow.xaml` and code-behind build correctly from WinForms project.
- [x] Make overlay always on top.
- [x] Make overlay click-through.
- [x] Draw translucent rectangle over inspected element bounds.
- [x] Update overlay position when hovered element changes.
- [x] Hide overlay when inspect/record mode stops.
- [x] Ensure overlay does not interfere with UIA hit testing.

## Phase 6 - Record Mode

- [x] Add start/stop record controls in WinForms UI.
- [x] Ensure record mode also performs hover inspection.
- [x] Add global left mouse click capture while recording.
- [x] Ignore clicks on TalosUI itself.
- [x] Resolve clicked element via UI Automation.
- [x] Convert clicked `UiElementInfo` to `ElementLocator`.
- [x] Create `Step` from click with default action `Invoke`.
- [x] Append recorded step to currently selected test case.
- [x] Assign sequential unique step ids within each test case.
- [x] Display recorded steps in the UI.
- [x] Allow editing step order.
- [x] Allow editing per-step fixed delay.
- [x] Allow manually setting action and parameters for `SetValue`, `SelectItem`, and `SendKeys`.

## Phase 7 - Suite and Test Case Editing

- [x] Add suite metadata editor.
- [x] Add target exe browser.
- [x] Add launch argument input.
- [x] Add default fixed delay input.
- [x] Add `AutoRelaunchBetweenTests` checkbox.
- [x] Add test case list.
- [x] Add create, rename, duplicate, delete test case actions.
- [x] Add test case description editor.
- [x] Add step list/editor for selected test case.
- [x] Add condition editor for value checks.
- [x] Add condition editor for expected windows.
- [x] Add condition editor for forbidden windows.
- [x] Validate required fields before save or run.

## Phase 8 - Persistence (`CTalosMemory`)

- [x] Define `CTalosMemory` API.
- [x] Implement save suite to `*.talos.json`.
- [x] Implement load suite from `*.talos.json`.
- [x] Preserve all suite, test, step, locator, and condition fields.
- [x] Add current schema version constant.
- [x] Reject unsupported newer schema versions with a clear message.
- [x] Reject or migrate older schema versions.
- [x] Add file dialogs for open/save suite.
- [x] Track dirty/unsaved state in the UI.
- [x] Add sample suite file for manual testing.

## Phase 9 - Test Runner

- [x] Define result models for suite run, test run, and step run.
- [x] Implement `TestRunner` dependencies:
  - [x] `IUiAutomationService`
  - [x] `ITargetProcessManager`
  - [x] `CTalosLog`
- [x] Implement run single test case.
- [x] Implement run all enabled/selected test cases in order.
- [x] Ensure target process is running before each test.
- [x] Launch target if needed.
- [x] Check for crash before each step.
- [x] Locate step element via `FindElement`.
- [x] Fail test if element cannot be found.
- [x] Perform step action.
- [x] Fail test if action fails.
- [x] Wait using step delay override or suite default delay.
- [x] Check for crash after each step.
- [x] Check forbidden windows after each step.
- [x] Evaluate value checks at end of test.
- [x] Evaluate expected windows at end of test.
- [x] Mark test passed only when all steps and conditions succeed.
- [x] Stop current test immediately on failure.
- [x] Relaunch between tests when configured.
- [x] Report progress back to WinForms UI.

## Phase 10 - Conditions

- [x] Implement value comparison logic.
- [x] Support simple exact string matching for v1 value checks.
- [x] Implement expected window detection.
- [x] Implement forbidden window detection.
- [x] Support simple substring window patterns.
- [x] Add optional regex support for window patterns if practical.
- [x] Track whether expected windows appeared during the test or only exist at the end.
- [x] Produce clear failure reasons for condition failures.

## Phase 11 - Logging and Artifacts (`CTalosLog`)

- [x] Define `CTalosLog` API.
- [x] Create run output directory naming convention.
- [x] Log run start/end timestamps.
- [x] Log per-test status and failure reason.
- [x] Log per-step status and messages.
- [x] Generate human-readable `.txt` report.
- [x] Generate structured JSON report.
- [x] Capture screenshot on failure.
- [x] Associate screenshot path with failed test/step.
- [x] Show report/log location in the UI after run.

## Phase 12 - WinForms Run UI

- [x] Add launch target button.
- [x] Add run selected test button.
- [x] Add run all tests button.
- [x] Add stop/cancel run button if feasible for v1.
- [x] Disable editing controls while running.
- [x] Show current run status.
- [x] Show per-test pass/fail results.
- [x] Show per-step progress/results.
- [x] Display clear error messages for invalid target, load/save errors, and run failures.

## Phase 13 - Testing

- [ ] Add unit test project compatible with the chosen Visual Studio/.NET setup.
- [ ] Unit test locator resolution with synthetic UI tree abstraction or mock service.
- [ ] Unit test TestRunner success path.
- [ ] Unit test element-not-found failure.
- [ ] Unit test forbidden-window failure.
- [ ] Unit test crash mid-test failure.
- [ ] Unit test `CTalosMemory` JSON round trip.
- [ ] Unit test schema version handling.
- [ ] Unit test logging result generation where possible.
- [ ] Create small sample WinForms target app for manual testing.
- [ ] Create small sample WPF target app for manual testing.
- [ ] Manually verify inspect hover behavior.
- [ ] Manually verify record click behavior.
- [ ] Manually verify replay against sample target.
- [ ] Manually verify screenshots on failure.
- [ ] Manually verify administrator/elevated target behavior.

## Phase 14 - Documentation

- [ ] Document supported targets and v1 limitations.
- [ ] Document administrator requirement and risks.
- [ ] Document how to create a suite.
- [ ] Document how to record a test case.
- [ ] Document how to edit steps and conditions.
- [ ] Document how to replay tests.
- [ ] Document report outputs and screenshot locations.
- [ ] Document known limitations:
  - [ ] UWP not supported.
  - [ ] Web/browser automation not supported.
  - [ ] Cross-session/remote desktop automation not supported.
  - [ ] Multi-window disambiguation is limited in v1.
  - [ ] Fixed delays are used instead of explicit waits.

## V1 Acceptance Checklist

- [ ] User can select a target `.exe`.
- [ ] User can launch the target app from TalosUI.
- [ ] User can inspect UI elements by hovering.
- [ ] TalosUI displays useful UIA details for hovered elements.
- [ ] TalosUI highlights the inspected element.
- [ ] User can record clicks as steps.
- [ ] User can create multiple test cases in one suite.
- [ ] User can save and load `*.talos.json` suites.
- [ ] User can replay one test case.
- [ ] User can replay all test cases.
- [ ] TalosUI can locate elements using tiered locator strategy.
- [ ] TalosUI can execute `Invoke` steps reliably against sample targets.
- [ ] TalosUI can evaluate basic value checks.
- [ ] TalosUI can detect forbidden windows.
- [ ] TalosUI can detect target process crashes.
- [ ] TalosUI generates text and JSON reports.
- [ ] TalosUI captures screenshots on failures.

## Deferred / V2 Backlog

- [ ] Better multi-window disambiguation.
- [ ] Explicit `WaitForElement` step.
- [ ] Explicit `WaitForWindow` step.
- [ ] Step-scoped conditions.
- [ ] Composite conditions with AND/OR logic.
- [ ] Automatic text-entry recording.
- [ ] Richer keyboard recording.
- [ ] Drag-and-drop step reordering.
- [ ] Rich condition editors with element pickers.
- [ ] Global settings for default delays.
- [ ] Configurable log locations.
- [ ] Configurable screenshot formats.
- [ ] Better 32-bit target support if v1 best-effort behavior is insufficient.
