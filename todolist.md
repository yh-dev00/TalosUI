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

- [ ] Define `ITargetProcessManager`.
- [ ] Implement target process launch with optional arguments.
- [ ] Implement running/crash detection.
- [ ] Implement process id retrieval.
- [ ] Implement graceful close.
- [ ] Implement forced kill fallback after graceful close timeout.
- [ ] Implement relaunch support for `AutoRelaunchBetweenTests`.
- [ ] Surface clear errors when target path is missing or invalid.

## Phase 3 - UI Automation Service

- [ ] Define `IUiAutomationService`.
- [ ] Implement `GetElementAtPoint(x, y)` using UI Automation hit testing.
- [ ] Convert `AutomationElement` into `UiElementInfo`.
- [ ] Capture key UIA properties:
  - [ ] `AutomationId`
  - [ ] `Name`
  - [ ] `ControlType`
  - [ ] `ClassName`
  - [ ] `ProcessId`
  - [ ] `NativeWindowHandle`
  - [ ] `BoundingRectangle`
  - [ ] `AncestorPath`
- [ ] Implement `GetTopLevelWindowsForProcess(processId)`.
- [ ] Implement `FindElement(locator)` with scoped root lookup.
- [ ] Implement Tier 1 lookup by `AutomationId`.
- [ ] Implement Tier 2 lookup by `ControlType + Name` under resolved parent.
- [ ] Implement Tier 3 lookup by `AncestorPath`.
- [ ] Implement Tier 4 fallback by bounding rectangle center hit test.
- [ ] Implement tolerance comparison for bounding rectangle fallback.
- [ ] Implement `PerformAction(element, action, text)`.
- [ ] Implement `Invoke` via `InvokePattern`.
- [ ] Implement `SetValue` via `ValuePattern`.
- [ ] Implement `SelectItem` via `SelectionItemPattern`.
- [ ] Implement minimal `SendKeys` support.
- [ ] Implement `GetElementValue(element)` via `ValuePattern` with `Name` fallback.
- [ ] Keep all direct UIA types inside the UI automation service boundary where practical.

## Phase 4 - Inspect Mode

- [ ] Add UI state enum: `Idle`, `Inspect`, `Record`, `Running`.
- [ ] Add start/stop inspect controls in WinForms UI.
- [ ] Track mouse position during inspect mode.
- [ ] Add stable hover dwell timer of at least 1 second.
- [ ] Resolve hovered element through `IUiAutomationService.GetElementAtPoint`.
- [ ] Display inspected element details in the UI.
- [ ] Show AutomationId, Name, ControlType, ClassName, ProcessId, BoundingRectangle, and AncestorPath.
- [ ] Hide stale element details when no valid element is found.
- [ ] Ensure inspect mode stops cleanly when switching to idle/running states.

## Phase 5 - Highlight Overlay

- [ ] Verify existing `HighlighterWindow.xaml` and code-behind build correctly from WinForms project.
- [ ] Make overlay always on top.
- [ ] Make overlay click-through.
- [ ] Draw translucent rectangle over inspected element bounds.
- [ ] Update overlay position when hovered element changes.
- [ ] Hide overlay when inspect/record mode stops.
- [ ] Ensure overlay does not interfere with UIA hit testing.

## Phase 6 - Record Mode

- [ ] Add start/stop record controls in WinForms UI.
- [ ] Ensure record mode also performs hover inspection.
- [ ] Add global left mouse click capture while recording.
- [ ] Ignore clicks on TalosUI itself.
- [ ] Resolve clicked element via UI Automation.
- [ ] Convert clicked `UiElementInfo` to `ElementLocator`.
- [ ] Create `Step` from click with default action `Invoke`.
- [ ] Append recorded step to currently selected test case.
- [ ] Assign sequential unique step ids within each test case.
- [ ] Display recorded steps in the UI.
- [ ] Allow editing step order.
- [ ] Allow editing per-step fixed delay.
- [ ] Allow manually setting action and parameters for `SetValue`, `SelectItem`, and `SendKeys`.

## Phase 7 - Suite and Test Case Editing

- [ ] Add suite metadata editor.
- [ ] Add target exe browser.
- [ ] Add launch argument input.
- [ ] Add default fixed delay input.
- [ ] Add `AutoRelaunchBetweenTests` checkbox.
- [ ] Add test case list.
- [ ] Add create, rename, duplicate, delete test case actions.
- [ ] Add test case description editor.
- [ ] Add step list/editor for selected test case.
- [ ] Add condition editor for value checks.
- [ ] Add condition editor for expected windows.
- [ ] Add condition editor for forbidden windows.
- [ ] Validate required fields before save or run.

## Phase 8 - Persistence (`CTalosMemory`)

- [ ] Define `CTalosMemory` API.
- [ ] Implement save suite to `*.talos.json`.
- [ ] Implement load suite from `*.talos.json`.
- [ ] Preserve all suite, test, step, locator, and condition fields.
- [ ] Add current schema version constant.
- [ ] Reject unsupported newer schema versions with a clear message.
- [ ] Reject or migrate older schema versions.
- [ ] Add file dialogs for open/save suite.
- [ ] Track dirty/unsaved state in the UI.
- [ ] Add sample suite file for manual testing.

## Phase 9 - Test Runner

- [ ] Define result models for suite run, test run, and step run.
- [ ] Implement `TestRunner` dependencies:
  - [ ] `IUiAutomationService`
  - [ ] `ITargetProcessManager`
  - [ ] `CTalosLog`
- [ ] Implement run single test case.
- [ ] Implement run all enabled/selected test cases in order.
- [ ] Ensure target process is running before each test.
- [ ] Launch target if needed.
- [ ] Check for crash before each step.
- [ ] Locate step element via `FindElement`.
- [ ] Fail test if element cannot be found.
- [ ] Perform step action.
- [ ] Fail test if action fails.
- [ ] Wait using step delay override or suite default delay.
- [ ] Check for crash after each step.
- [ ] Check forbidden windows after each step.
- [ ] Evaluate value checks at end of test.
- [ ] Evaluate expected windows at end of test.
- [ ] Mark test passed only when all steps and conditions succeed.
- [ ] Stop current test immediately on failure.
- [ ] Relaunch between tests when configured.
- [ ] Report progress back to WinForms UI.

## Phase 10 - Conditions

- [ ] Implement value comparison logic.
- [ ] Support simple exact string matching for v1 value checks.
- [ ] Implement expected window detection.
- [ ] Implement forbidden window detection.
- [ ] Support simple substring window patterns.
- [ ] Add optional regex support for window patterns if practical.
- [ ] Track whether expected windows appeared during the test or only exist at the end.
- [ ] Produce clear failure reasons for condition failures.

## Phase 11 - Logging and Artifacts (`CTalosLog`)

- [ ] Define `CTalosLog` API.
- [ ] Create run output directory naming convention.
- [ ] Log run start/end timestamps.
- [ ] Log per-test status and failure reason.
- [ ] Log per-step status and messages.
- [ ] Generate human-readable `.txt` report.
- [ ] Generate structured JSON report.
- [ ] Capture screenshot on failure.
- [ ] Associate screenshot path with failed test/step.
- [ ] Show report/log location in the UI after run.

## Phase 12 - WinForms Run UI

- [ ] Add launch target button.
- [ ] Add run selected test button.
- [ ] Add run all tests button.
- [ ] Add stop/cancel run button if feasible for v1.
- [ ] Disable editing controls while running.
- [ ] Show current run status.
- [ ] Show per-test pass/fail results.
- [ ] Show per-step progress/results.
- [ ] Display clear error messages for invalid target, load/save errors, and run failures.

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
