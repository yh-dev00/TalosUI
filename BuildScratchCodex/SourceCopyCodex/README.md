# TalosUI

TalosUI is a Visual Studio 2015 WinForms UI automation tool targeting .NET Framework 4.5.2. The behavior and v1 scope are defined in `PRD.md`.

## Phase 0 Project Shape

- `TalosUI/` contains the WinForms application.
- `Src/` contains linked core/source files used by the WinForms project.
- `Logs/` is reserved for runtime logs.
- `Reports/` is reserved for generated run reports.
- `TestAssets/` is reserved for sample target apps and suite files.

## Build Targets

The solution provides x64 and x86 configurations. Per the PRD, x64 is the preferred v1 TalosUI build target. x86 is kept as a best-effort configuration for compatibility checks.

## Running TalosUI

TalosUI should be started as Administrator when automating elevated target applications. The v1 manifest intentionally uses `asInvoker`, so Windows will not show an automatic elevation prompt; start it from an elevated shell or use "Run as administrator" when needed.
