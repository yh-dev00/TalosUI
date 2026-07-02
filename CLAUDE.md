# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

TalosUI is a WinForms UI automation tool (similar in spirit to Microsoft Inspect.exe plus a recorder/replayer). It uses Microsoft UI Automation (UIA) to inspect elements of a target Windows desktop app, record user clicks as steps, save them as test suites (`*.talos.json`), replay them, and decide pass/fail from value checks, expected/forbidden windows, and crash detection.

- `PRD.md` is the authoritative spec (scope, locator strategy, module responsibilities).
- `todolist.md` is the living phase tracker — phases 0–12 are complete; Phase 13 (testing) and 14 (docs) remain. Check items off as work completes. Commits follow the pattern `Add feature for Phase N`.

## Build

Visual Studio 2015-era project targeting .NET Framework 4.5.2. Build with MSBuild 14.0 (verified path on this machine):

```powershell
& "C:\Program Files (x86)\MSBuild\14.0\Bin\MSBuild.exe" TalosUI.sln /p:Configuration=Debug /p:Platform=x64 /v:minimal
```

- Solution platforms are `x64` and `x86` only (no AnyCPU in the .sln). **x64 is the primary v1 target**; x86 is best-effort.
- Output: `TalosUI\bin\x64\Debug\TalosUI.exe`.
- If output files are locked (app still running, VS open), build to a scratch directory with `/p:OutDir=<path>` rather than killing processes. Stray `BuildScratch*` / `build*` folders in the repo are disposable outputs from such builds — never commit them.

There is no test project yet (Phase 13). No lint tooling is configured.

Run the app from the built exe. It must be started as Administrator to automate elevated targets (the manifest is intentionally `asInvoker`, so no auto-elevation prompt). A sample suite for manual testing is at `TestAssets\sample-suite.talos.json`.

## Language and Dependency Constraints

- **C# 6 maximum** (MSBuild 14 compiler). No C# 7+ features: no `out var`, pattern matching, tuples, local functions, or expression-bodied methods.
- The existing style is deliberately conservative: explicit types (no `var`), full-bodied properties/constructors initializing every field, `string.Empty`, string-literal parameter names in `ArgumentNullException`. Match it.
- **No NuGet packages.** JSON serialization uses `JavaScriptSerializer` from `System.Web.Extensions` — do not introduce Json.NET.
- The WinForms project references WPF assemblies (PresentationCore/Framework, WindowsBase) solely for the highlight overlay window.

## Architecture

Two layers in one project: the WinForms app in `TalosUI/`, and core logic in `Src/` (namespace `TalosCore`) which is **linked** into `TalosUI.csproj` as `<Compile Include="..\Src\..." Link="...">` items — a new core file must be added to the csproj the same way or it won't compile.

### Core (`Src/`, namespace `TalosCore`)

| File | Contents |
|---|---|
| `TalosModels.cs` | Persisted data model: `TestSuite` → `TestCase` → `Step` (`ElementLocator` + `StepAction` + `StepParameters`), plus `TestConditions` (`ValueCheck`, expected/forbidden window conditions), `UiElementInfo` (runtime DTO), `AncestorDescriptor`, `PersistedRectangle`. |
| `TalosInspect.cs` | `IUiAutomationService` / `UiAutomationService` — the **only** code allowed to touch `System.Windows.Automation` types. Hit-testing (`GetElementAtPoint`), tiered `FindElement`, `PerformAction` via UIA patterns (Invoke/Value/SelectionItem), `GetElementValue`, top-level window enumeration. |
| `TalosProcess.cs` | `ITargetProcessManager` / `TargetProcessManager` — launch/close/kill/relaunch of the target exe, crash detection. |
| `TalosRunner.cs` | `TestRunner` plus run-result models (`TestSuiteRunResult`/`TestCaseRunResult`/`StepRunResult`) and progress events. Depends only on `IUiAutomationService`, `ITargetProcessManager`, and `CTalosLog` — never on UIA or WinForms types directly. |
| `TalosMemory.cs` | `CTalosMemory` — load/save `*.talos.json` with `SchemaVersion` validation (rejects newer versions; current version constant lives on `TestSuite`). |
| `TalosLog.cs` | `CTalosLog` — run logs, `.txt` and JSON reports, failure screenshots (written under `Logs/` / `Reports/`). |
| `HighlighterWindow.xaml(.cs)` | WPF translucent, click-through, always-on-top overlay that highlights the inspected element without stealing focus or interfering with UIA hit-testing. |
| `TalosCore.cs` | Near-empty placeholder partial class `CTalosCore`. |

### UI (`TalosUI/`)

`MainForm.cs` (~1900 lines) holds all UI logic: a `TalosUiState` state machine (Idle / Inspect / Record / Running), the hover-dwell timer that resolves elements after ≥1s of stable cursor, global click capture during Record mode (clicks on TalosUI itself are ignored), suite/test-case/step/condition editing grids with dirty tracking, and async suite runs with progress marshalled back onto the UI thread.

### Key invariants

- **UIA isolation**: every direct use of `AutomationElement`/UIA patterns stays inside `UiAutomationService`. Runner, persistence, logging, and recording work only with `UiElementInfo`/`ElementLocator` DTOs.
- **Tiered locator strategy** (`FindElement`, spec in PRD §5): scope to the target process's top-level window, then (1) AutomationId, (2) ControlType+Name under the parent resolved from `AncestorPath`, (3) full `AncestorPath` walk, (4) `BoundingRectangle` center hit-test with tolerance. Returns null if all tiers fail → step and test fail.
- **Timing is fixed delays only in v1**: per-step `FixedDelayMs` override, else suite `DefaultFixedDelayMs`. Explicit waits are v2 (see PRD §11 / todolist backlog).
- Persistence must round-trip every model field; bump `TestSuite.CurrentSchemaVersion` when changing the persisted shape.
