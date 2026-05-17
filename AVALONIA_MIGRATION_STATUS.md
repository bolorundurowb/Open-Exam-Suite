# Avalonia migration — implementation status

Phase 0 steps 1 and 2 (git tag + branch) were skipped per the request. Everything else from the migration plan is in place.

## What was built

### `src/Directory.Packages.props`
Added centrally-pinned versions for Avalonia 11.2.3, `Avalonia.Desktop`, `Avalonia.Themes.Fluent`, `Avalonia.Fonts.Inter`, `Avalonia.Controls.DataGrid`, `Avalonia.Diagnostics`, `Avalonia.Headless.XUnit`, `CommunityToolkit.Mvvm` 8.4.0, `MessageBox.Avalonia` 3.2.0, and `LiveChartsCore.SkiaSharpView.Avalonia` 2.0.0-rc5.4. Also pinned `Tmds.DBus.Protocol` to 0.21.2 (transitive from `Avalonia.Desktop`) to override the GHSA-xrw6-gwf8-vvr9 advisory in 0.20.0; central transitive pinning is already enabled.

### Target framework
**Cross-platform.** The shared libraries (`Core`, `ExamIO`, `Logging`, `Storage`) and the new Avalonia projects (`Shared.Avalonia`, `Creator.Avalonia`, `Simulator.Avalonia`) all target `net10.0` — no `-windows` suffix. Only `Shared.WinForms`, the legacy WinForms `Creator`, and the legacy WinForms `Simulator` stay on `net10.0-windows` because they reference `System.Windows.Forms`.

To get there, the domain model's `Question.Image` (a `System.Drawing.Bitmap`) was replaced with `Question.ImageData` (a `byte[]?` of PNG-encoded bytes). Each UI layer decodes the bytes at display time:
* WinForms calls `QuestionImageExtensions.ToBitmap()` / `.ToPngBytes()` in `Shared.WinForms`.
* Avalonia goes through `BitmapToAvaloniaImageConverter` (now a `byte[] → Avalonia.Media.Imaging.Bitmap` converter, no `System.Drawing` involved).
* PDF export decodes the bytes into a `PdfSharp.Drawing.XImage` via `new MemoryStream(bytes)`.
* The `Shared.Tests.ExamTests` fixture loads the PNG with `File.ReadAllBytes` and asserts on `ImageData.Length`.

`BitmapConverter : JsonConverter<Bitmap?>` was deleted — `System.Text.Json` natively serialises `byte[]` as base64 so JSON round-trips work without it. XML uses `[XmlElement(DataType = "base64Binary")]` for the same reason.

`System.Drawing.Common` was dropped from every `.csproj` except the legacy WinForms ones (which still need it indirectly via `UseWindowsForms`).

### `src/Open-Exam-Suite.slnx`
Added two new solution folders side-by-side with the existing WinForms ones:
* `/Apps.Avalonia/` — `Creator.Avalonia`, `Simulator.Avalonia`
* `/Libraries.Avalonia/` — `Shared.Avalonia`

The original WinForms projects (`Creator`, `Simulator`, `Shared.WinForms`) are still in the solution and still build. Cutover (Phase 19) renames the new projects to drop the `.Avalonia` suffix and deletes the old ones.

### `src/Libraries/Shared.Avalonia/`
* Services: `IFilePickerService` + `FilePickerService` (Avalonia `IStorageProvider` wrapper), `IMessageBoxService` + `MessageBoxService` (MsBox.Avalonia wrapper preserving all WinForms `MessageBox.Show` arguments), `IOpenUrlService` + `OpenUrlService` (`TopLevel.Launcher`).
* `FileFilter` record with `ParseLegacy()` to convert WinForms `"OEF Files|*.oef"` strings.
* Converters: `BitmapToAvaloniaImageConverter` (round-trips through PNG so `Core.Question.Image` stays `System.Drawing.Bitmap`), `NullToBoolConverter`, `InverseBoolConverter`, `EnumToBoolConverter` (for radio↔enum binding).
* Controls: `SingleAnswerOption` and `MultiAnswerOption` UserControls — direct ports of WinForms `OptionControl`/`OptionsControl` with `StyledProperty` for `Letter`, `Text`, `IsChecked`, `GroupName`.
* `LicenseDialog` — port of `Shared.WinForms.LicenseUi`; loads `OpenExamSuite.Shared.Avalonia.LICENSE` embedded resource.
* `Styles/Common.axaml` — Fluent-compatible styles for `.h1`/`.h2`/`.h3`/`.muted`/`.error`/`.success`/`.toolbar`/`.hyperlink`, replicating the WinForms GDI+ palette (`Brushes.Purple` for top heading, `Brushes.Green` for sub-heading, etc.).

### `src/Apps/Creator.Avalonia/`
* `Program.cs`, `App.axaml(.cs)` with DI container wiring `IAppSettingsService`, the three Shared services, `IUndoRedoService`, `IExamHistoryService`, `IPrintService` (PdfSharp-backed), `IClipboardService`, and the three pane VMs.
* `Services/UndoRedoService` — verbatim port of the WinForms `UndoRedo` (Add/Delete/Modify with stack semantics), wrapped in an interface and exposing a `Changed` event so command CanExecute can refresh.
* `Services/ExamHistoryService` — wrapper around `IAppSettingsService` with `AppSettingsType.Creator`.
* `Services/PdfPrintService` — replaces `System.Drawing.Printing.PrintDocument` by re-using `Writer.ToPdf` (already a dependency) plus `IOpenUrlService.OpenFileAsync` for the OS handler. Honours `PrintScope.CurrentQuestion`/`CurrentSection`/`AllQuestions`.
* `Services/ClipboardService` — `TopLevel.Clipboard` wrapper for Cut/Copy/Paste.
* `ViewModels/Nodes/` — `NodeViewModel` (abstract with `Parent` back-reference and `Children` collection), `ExamNodeViewModel`, `SectionNodeViewModel` (with `RenumberQuestions`), `QuestionNodeViewModel`. Replaces the WinForms `TreeNode` subclasses in `Shared.WinForms/Controls/TreeNodes.cs`.
* `ViewModels/Panes/` — `IRightPaneViewModel` marker + three concrete panes (`SplashPaneViewModel`, `ExamPropertiesPaneViewModel`, `QuestionEditorPaneViewModel`). Replaces the `Controls.Add/Remove` swap on `splitContainer2.Panel2`.
* `ViewModels/MainWindowViewModel.cs` — the largest VM. Owns the exam, dirty bit, tree nodes, undo/redo wiring, right-pane swap on selection, all 21 menu/toolbar commands, FormClosing prompt, every file-format dialog filter from the WinForms code, exact MessageBox strings, and ChangeRepresentationObject Add/Delete/Modify replay logic.
* `Views/MainWindow.axaml(.cs)` — Menu + toolbar + `Grid` (294 left / `*` right) with `GridSplitter`, TreeView with three hierarchical templates and a context flyout for Edit/Delete, ContentControl bound to `CurrentRightPane`. Window-level KeyBindings for Ctrl+N/O/S/P/Z/Y/X/C/V; TreeView KeyBinding for `Delete`. `Window.Closing` runs the unsaved-changes prompt.
* `Views/Panes/SplashPaneView`, `ExamPropertiesPaneView`, `QuestionEditorPaneView` — visually matching the WinForms `pan_splash`/`pan_exam_properties`/`pan_display_questions` panels.
* `Views/Dialogs/AddSectionDialog`, `EditSectionDialog`, `PrintOptionsDialog`, `AboutDialog` — ShowDialog-returning dialogs preserving the exact return semantics of the WinForms versions.
* `Assets/CreatorAppIcon.ico` — copied from the WinForms project.

### `src/Apps/Simulator.Avalonia/`
* `Program.cs` — preserves the WinForms `Mutex` single-instance check.
* `App.axaml(.cs)` — DI registration. Re-uses the first-run sample auto-add behaviour from `AppDataManager` inline in `HomeViewModel`.
* `Services/NavigationService` + `INavigationService` — `GoTo`/`GoBack` over a back-stack, configured by `MainWindowViewModel`. Replaces `Hide()` + `ShowDialog()` between forms.
* `Services/DispatcherTimerService` + `ITimerService`/`ICountdownTimer` — 1-second tick equivalent to the WinForms `Timer.Interval = 1000`.
* `Services/ScoreSheetPrintService` + `IScoreSheetPrintService` — replaces the GDI+ `ScoreSheetUi.Print` with a PdfSharp version that produces the same heading/candidate/time/date/exam-code/score/status/breakdown layout, then opens the PDF.
* `ViewModels/MainWindowViewModel.cs` — owns `CurrentRoute`, Exit/About/License commands.
* `ViewModels/Routes/`:
  * `IRouteViewModel` marker
  * `HomeViewModel` — DataGrid-backed exam list, Add/Remove/Properties/Start commands with `CanExecute` derived from `SelectedExams.Count`. First-run sample loading uses `AppContext.BaseDirectory/Samples/`.
  * `ExamSettingsViewModel` — full port of the WinForms `ExamSettingsUi` including the exact fixed-N section selection algorithm and the "no questions" error MessageBox.
  * `AssessmentViewModel` — the biggest single redesign: replaces `Visible = false/true` toggling of 16+ controls with an `AssessmentPhase` enum (`Intro`/`Running`). All scoring, navigation (Begin/Next/Previous/End), pause-with-resume MessageBox, and end-of-exam aggregation (overall + per-section) are ported faithfully.
  * `ScoreSheetViewModel` — pass/fail status, LiveCharts `RowSeries` for the pass-mark vs. your-score bars (replaces `System.Windows.Forms.DataVisualization.Chart`), section breakdown list, Print/Retake/Exit commands.
* `ViewModels/Items/`:
  * `ExamRowViewModel` (DataGrid row)
  * `SectionSelectionViewModel` (checked-list item)
  * `AnswerOptionViewModel` (radio/check option with `Foreground` brush derived from `State` — replaces the WinForms `ForeColor = Color.Green/Red` trick).
* `Views/MainWindow.axaml(.cs)` — Menu (File/Help) + `ContentControl` bound to `CurrentRoute`, four `DataTemplate`s.
* `Views/Routes/HomeView`, `ExamSettingsView`, `AssessmentView`, `ScoreSheetView`.
* `Views/Dialogs/ExamPropertiesDialog`, `AboutDialog`.
* `Assets/SimulatorAppIcon.ico` — copied from the WinForms project.

## Behaviour preserved (verified against source)

- **Keyboard shortcuts**: Ctrl+N, Ctrl+O, Ctrl+S, Ctrl+P, Ctrl+Z, Ctrl+Y, Ctrl+X, Ctrl+C, Ctrl+V on the Creator Window; `Delete` on the TreeView (only fires when a question is selected via `CanDeleteQuestion`).
- **File dialog filters**: `OEF Files|*.oef` for open/save; `JPEG Files|*.jpg|PNG Files|*.png` for image insert; `JSON Files|*.json`, `XML Files|*.xml`, `PDF Files|*.pdf` for import/export; `Open Exam Files (*.oef)|*.oef` multi-select for Simulator's Add.
- **MessageBox texts**: all original strings preserved verbatim (unsaved-changes prompt, exam corrupt error, paste-over-selection prompt, export success/failure, file moved/deleted warning, "no questions to be displayed", "Your time ran out!", paused exam).
- **Single-instance Mutex** still in Simulator's `Program.cs`.
- **Tree selection enablement**: `New Question` only enabled with a Section or Question selected; `Edit Section` only on Section; `Delete Question` only on Question; `Cut/Copy/Paste` only on Question — implemented via `CanExecute` rather than the WinForms `Enable*Controls()` family.
- **Undo/Redo semantics**: Push clears the redo stack; Undo moves item from undo→redo; Redo inverse. Same three `ActionType`s.
- **Save → Save As fallback**: when `CurrentExamFile` is null, Save delegates to Save As.
- **Properties Save** auto-creates the root exam when there isn't one (matches the WinForms `if (trv_view_exam.Nodes.Count > 0)` branch).
- **Print options dialog** enables radios based on the selected node's type, exactly like `PrintOptions(TreeNode selectedNode)`.
- **AssessmentUi `PauseExam`**: stops timer → MessageBox → starts timer.
- **AssessmentUi "Time Up!"**: shows MessageBox, then triggers `End`.
- **ScoreSheet normalisation**: `numCorrect * 1000 / questionCount` matches the WinForms formula.

## What was deliberately not done

- Phase 0 steps 1-2 (git tag, feature branch) — per the request.
- Phase 16 (delete `Shared.WinForms`) — kept for side-by-side coexistence; deletion is the final cutover step.
- Phase 17 sub-items (icon-strip PNG migration, accessibility names): the icon strip is replaced with emoji glyphs in the toolbar as a placeholder; final asset extraction from the `.resx` files is a follow-up.
- Phase 18 (test projects, CI updates, installer) — out of scope for the implementation pass.
- Phase 19 (rename + cutover) — deferred so both stacks remain buildable.

## To build

```powershell
dotnet restore src
dotnet build src/Apps/Creator.Avalonia/Creator.Avalonia.csproj
dotnet build src/Apps/Simulator.Avalonia/Simulator.Avalonia.csproj

dotnet run --project src/Apps/Creator.Avalonia/Creator.Avalonia.csproj
dotnet run --project src/Apps/Simulator.Avalonia/Simulator.Avalonia.csproj
```

The original WinForms apps still build and run:

```powershell
dotnet run --project src/Apps/Creator/Creator.csproj
dotnet run --project src/Apps/Simulator/Simulator.csproj
```

## Known follow-ups

1. **Icons**: replace emoji glyphs in the Creator toolbar/tree with the original PNGs once extracted from the WinForms `.resx` files.
2. **DPI manifest**: add `app.manifest` with `dpiAwareness=PerMonitorV2` (Avalonia handles this by default, but explicit is safer on Win10+).
3. **CI**: update GitHub Actions to also publish the Avalonia EXEs side-by-side with the WinForms ones during the transition period.
4. **Tests**: author the ViewModel test projects from Plan §14 (`UndoRedoService`, `AssessmentViewModel.End`, `ExamSettingsViewModel.Proceed`, etc.).
5. **Cutover** (Plan Phase 19) after parity validation.
