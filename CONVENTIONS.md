# Coding Conventions 

Project-wide naming rules. 

## Types

| Element | Prefix / style | Example |
|---|---|---|
| Class | `C` + PascalCase | `CNetDllProvider` |
| Struct | `ST` + PascalCase | `STInvokeArg` |
| Interface | `I` + PascalCase | `IDllProvider`, `IMethodExecutor` |
| Enum type | `E` + PascalCase | `EArgDirection` |
| Form | `Frm` + PascalCase | `FrmMain`, `FrmScopeView` |

## Variables / fields (Hungarian prefix by type)

| Type | Prefix | Example |
|---|---|---|
| int | `i` | `iCount` |
| double | `d` | `dElapsed` |
| float | `f` | `fRatio` |
| long | `l` | `lTicks` |
| byte | `by` | `byFlags` |
| unsigned int | `ui` | `uiIndex` |
| bool | `b` | `bIsReady` |
| string | `s` | `sPath` |
| class instance | `c` | `cLoader` |
| struct instance | `st` | `stArg` |

Private fields use the type prefix only — **no** `m_` prefix.

## Methods

PascalCase verbs (.NET norm): `LoadDll`, `InvokeMember`, `CreateSubject`.

## UI controls

| Control | Prefix | Control | Prefix |
|---|---|---|---|
| Button | `btn` | TreeView | `tree` |
| TextBox | `txt` | ComboBox | `cmb` |
| List | `list` | CheckBox | `chk` |
| Grid | `grid` | Label | `lbl` |
| ListColumn | `listCol` | Chart | `chart` |
| GridColumn | `gridCol` | Panel | `pnl` |
|  |  | MenuItem | `mnu` |

## UI construction rule

No UI **controls/containers** are created at runtime — all are declared in
`*.Designer.cs` at compile time. **Data items** (tree nodes, grid rows, chart
series, list items) ARE populated at runtime.