# Open Exam Suite — WinForms → Avalonia UI Migration Plan

**Target:** 1:1 feature / behavioural / workflow parity with the current `net10.0-windows` WinForms implementation, re-platformed onto Avalonia UI 11.x using MVVM with `CommunityToolkit.Mvvm`. Windows-first, cross-platform-friendly.

**In scope:** The two applications (`Creator`, `Simulator`) and the WinForms-specific shared library (`Libraries/Shared.WinForms`).
**Out of scope (preserved as-is):** `Libraries/Core`, `Libraries/ExamIO`, `Libraries/Logging`, `Libraries/Storage`, and the `.oef` file format / Protobuf migration logic.

---

## 1. Executive summary

The current solution is a classic late-90s-style WinForms application: every screen lives in a `partial class : Form` with a giant `*.Designer.cs`, business state is held on the form instance, and screen transitions are driven by `Hide()` + `new SomeOtherForm().ShowDialog()` chains or by `Controls.Add` / `Controls.Remove` on a `SplitContainer` panel. The non-UI layer is already in good shape — `Core`, `ExamIO`, `Storage`, and `Logging` are independent class libraries with no `System.Windows.Forms` references except `System.Drawing.Common`, which is the only real porting friction.

The migration is therefore essentially a **GUI layer rewrite** rather than a rewrite of the application. The plan below:

1. Keeps every non-UI library identical and references them unchanged from the new Avalonia projects.
2. Replaces both WinForms entry-point projects (`Creator`, `Simulator`) with two Avalonia projects of the same name, plus a new `Libraries/Shared.Avalonia` project replacing `Libraries/Shared.WinForms`.
3. Reproduces every screen, dialog, menu, toolbar, context menu, keyboard shortcut, file-dialog filter, validation rule, and message box exactly.
4. Replaces three specific WinForms anti-patterns — the right-pane `Controls.Add/Remove` swap in Creator, the visibility-toggled "intro vs running" state of `AssessmentUi`, and the `Hide()` + `ShowDialog()` simulator workflow — with state-driven `ContentControl` + `DataTemplate` composition driven from a `MainWindowViewModel`. The end-user experience is unchanged.
5. Replaces `System.Drawing.Bitmap` round-tripping in the UI with `Avalonia.Media.Imaging.Bitmap`. The `Question.Image` model property stays a `System.Drawing.Bitmap` (so `Core` and `ExamIO` stay untouched), and converters are added at the ViewModel boundary.
6. Replaces `System.Drawing.Printing.PrintDocument` with `QuestPDF` (or PdfSharp, which is already a dependency) for both Creator print/preview and Simulator score-sheet print — preserving the same outputs without WinForms `PrintDialog`/`PrintPreviewDialog`.

The migration is staged so that at the end of every phase the solution builds and at least one app runs.

---

## 2. Current-state inventory (verified against source)

### 2.1 Solution layout

```
src/
├── Apps/
│   ├── Creator/          (WinForms, OutputType=WinExe, net10.0-windows)
│   └── Simulator/        (WinForms, OutputType=WinExe, net10.0-windows)
├── Libraries/
│   ├── Core/             (Domain model: Exam, Section, Question, Option, Properties, Settings)
│   ├── ExamIO/           (Reader, Writer, ExamFileLoader — .oef / .json / .xml / .pdf)
│   ├── Logging/          (Logger)
│   ├── Shared.WinForms/  (OptionControl, OptionsControl, TreeNodes, LicenseUi, ControlUi)
│   └── Storage/          (AppSettingsService backed by LiteDB)
└── Tests/                (xUnit + Shouldly + Moq)
```

### 2.2 Forms inventory

| Form (file) | LOC (cs+Designer) | Role | Notes |
|---|---|---|---|
| `Creator/GUI/HomeUi` | 1308 + 1300+ | Creator main window | Menu, toolbar, split container, exam tree, three right-pane panels swapped at runtime, undo/redo, print, file I/O, history list |
| `Creator/GUI/AboutUi` | small | Modal about box | LinkLabel → `Process.Start` URLs |
| `Creator/GUI/Dialogs/AddSection` | small | Modal | Exposes `Title` property |
| `Creator/GUI/Dialogs/EditSection` | small | Modal | Same shape, pre-filled |
| `Creator/GUI/Dialogs/PrintOptions` | small | Modal | Chooses CurrentQuestion / CurrentSection / AllQuestions based on selected tree node |
| `Simulator/GUI/HomeUi` | small | Simulator main window | DataGridView of exams, Start/Properties/Remove/Add buttons, menu |
| `Simulator/GUI/ExamPropertiesUi` | small | Modal | Read-only exam metadata |
| `Simulator/GUI/ExamSettingsUi` | small | Modal | Section selection, fixed-N selection, custom timer, candidate name |
| `Simulator/GUI/AssessmentUi` | ~430 | Test-taking window | Two-phase (intro vs running), timer, dynamic option controls, show/hide answer with colour highlights |
| `Simulator/GUI/ScoreSheetUi` | ~140 | Score display & print | `Chart` from `System.Windows.Forms.DataVisualization`, DataGridView, PrintDocument |
| `Simulator/GUI/AboutUi` | small | Same as Creator's |
| `Shared.WinForms/Dialogs/LicenseUi` | small | Modal | Loads embedded `LICENSE` from `OpenExamSuite.Shared.LICENSE` resource |
| `Shared.WinForms/Controls/OptionControl` | small | UserControl | Single-answer option (RadioButton + TextBox) |
| `Shared.WinForms/Controls/OptionsControl` | small | UserControl | Multi-answer option (CheckBox + TextBox) |

### 2.3 Keyboard shortcuts (verified in Designer files)

| Action | Shortcut | Owner |
|---|---|---|
| New | Ctrl+N | Creator |
| Open | Ctrl+O | Creator |
| Save | Ctrl+S | Creator |
| Print | Ctrl+P | Creator |
| Undo | Ctrl+Z | Creator |
| Redo | Ctrl+Y | Creator |
| Cut | Ctrl+X | Creator |
| Copy | Ctrl+C | Creator |
| Paste | Ctrl+V | Creator |
| Delete question | Del | Creator (cms_question context menu) |

The Simulator does not currently bind any explicit shortcut keys; only menu mnemonics. The migration preserves that.

### 2.4 File dialog filters (verified)

| Dialog | Filter |
|---|---|
| `ofd_open_exam` (Creator) | `OEF Files\|*.oef` |
| `sfd_save_as_exam` (Creator) | `OEF File\|*.oef` |
| `ofd_select_image` (Creator) | `JPEG Files\|*.jpg\|PNG Files\|*.png` |
| Inline JSON import/export (Creator) | `JSON Files\|*.json` |
| Inline XML export (Creator) | `XML Files\|*.xml` |
| Inline PDF export (Creator) | `PDF Files\|*.pdf` |
| `ofd_exam` (Simulator) | `Open Exam Files (*.oef)\|*.oef`, multi-select |

### 2.5 Visibility-toggle anti-patterns identified

These three locations are the only acceptable deviation from a verbatim re-implementation — everywhere else, behaviour is preserved exactly.

**A. Creator right-pane panel swap.** In `splitContainer2.Panel2`, the code adds/removes one of three panels at runtime:
* `pan_splash` — shown when no exam is loaded; contains banner + history of recent files.
* `pan_exam_properties` — shown when the root `ExamNode` is selected in the tree.
* `pan_display_questions` — shown when a `SectionNode` (disabled) or a `QuestionNode` (enabled) is selected.

Trigger points: `New`, `Open` (success), `AfterSelect` (3 branches), `Close`. The current code does:
```csharp
splitContainer2.Panel2.Controls.Remove(pan_exam_properties);
splitContainer2.Panel2.Controls.Add(pan_display_questions);
```
**Avalonia replacement:** `ContentControl` bound to `MainWindowViewModel.CurrentRightPane` of type `IRightPaneViewModel`, with three concrete VMs (`SplashPaneViewModel`, `ExamPropertiesPaneViewModel`, `QuestionEditorPaneViewModel`) and matching `DataTemplate`s.

**B. AssessmentUi intro vs running phase.** Thirteen controls have `Visible = false` set in the designer and are toggled in `EnableControls()`:
`label1, lbl_elapsed_time, btn_previous, btn_next, btn_pause, btn_end, pct_image, lbl_explanation, txt_question, lbl_question_number, label3, lbl_section_title, label2, btn_show_answer, dspExamProgress, lblExamProgress, btn_begin (hidden after start)` and the inverse hide for `lbl_exam_code, lbl_exam_instructions, lbl_exam_title, btn_begin`.

**Avalonia replacement:** A single `AssessmentViewModel` exposes `Phase` (`enum { Intro, Running }`). The view uses two `DataTemplate`s inside a `ContentControl` (or two `Grid`s with `IsVisible="{Binding IsIntro}"` / `IsVisible="{Binding IsRunning}"`). All state-flag plumbing inside `EnableControls()` disappears.

**C. Simulator inter-form workflow.** Today: `HomeUi → ExamSettingsUi.ShowDialog() → AssessmentUi.ShowDialog() (with HomeUi/ExamSettingsUi hidden) → ScoreSheetUi.ShowDialog()`. Each form hides the previous and closes itself when done.

**Avalonia replacement:** `MainWindowViewModel.CurrentView` of type `IRouteViewModel`, with `HomeViewModel`, `ExamSettingsViewModel`, `AssessmentViewModel`, `ScoreSheetViewModel`. All "screen transitions" become assignments to `CurrentView`, and the main window's `ContentControl` swaps templates. `ExamPropertiesUi` and `AboutUi`/`LicenseUi` remain modal because they are popups, not phases of a flow.

### 2.6 Other patterns that need explicit translation

* **Dynamic control creation.** `pan_options.Controls.Add(new OptionControl { ... })` (Creator), and `pan_display.Controls.Add(new RadioButton/CheckBox { ... })` (Simulator AssessmentUi). Both become `ItemsControl` with an `ItemsSource` bound to an `ObservableCollection`.
* **TreeView nodes.** `ExamNode`, `SectionNode`, `QuestionNode` (subclasses of `TreeNode`) become regular ViewModels (`ExamNodeViewModel : NodeViewModel`, etc.) used by Avalonia's `TreeView` with `HierarchicalDataTemplate`.
* **Event handler suppression hack.** `DisconnectHandlers()` / `ReconnectHandlers()` in Creator suppresses `TextChanged` while populating textboxes from the model. In MVVM with one-way-to-source bindings driven by the selected ViewModel, this disappears — when the selected question changes, the bound VM changes, and there is no model-→view loop to break.
* **`Image` ↔ `Bitmap` round-tripping.** `pct_image.Image = question.Image` (a `System.Drawing.Bitmap`). In Avalonia, `Image.Source` is `IImage`. We add a `BitmapToAvaloniaImageConverter` that converts at the binding boundary so `Core` can stay on `System.Drawing.Bitmap`.
* **`PrintDocument` + `PrintPageEventArgs`.** Replaced by PdfSharp output (which is already in `ExamIO`) plus a "Save as PDF → open in OS handler" or an in-app PDF preview (PdfPig/Avalonia.PDF). Print-to-printer becomes Print-to-PDF; print-preview becomes PDF preview.
* **`System.Windows.Forms.DataVisualization.Chart`.** Replaced by `LiveChartsCore.SkiaSharpView.Avalonia` (or `OxyPlot.Avalonia`), rendering a horizontal bar chart with two series ("Pass Mark", "Your Score").
* **`Process.Start(url)`.** Replaced by a small `IOpenUrlService` that uses `TopLevel.Launcher.LaunchUriAsync` for cross-platform safety.
* **`MessageBox.Show`.** Replaced by `MsBox.Avalonia` (`MessageBoxManager`), preserving icon/buttons/result semantics.

---

## 3. High-level migration strategy

### 3.1 Guiding principles

1. **Domain stays put.** No changes to `Core`, `ExamIO`, `Storage`, `Logging`. They already cross-target cleanly; only their `TargetFramework` may be relaxed from `net10.0-windows` to `net10.0` where `System.Drawing.Common` is the only Windows-only dependency (it still works on net10.0 on Linux/macOS with the `Microsoft.DotNet.SystemDrawing` shim, but if cross-platform is desired later, we abstract `Bitmap` behind an `IImageData` record carrying raw bytes).
2. **MVVM with no view-model knowledge of `Window`/`Control`.** Use `CommunityToolkit.Mvvm` (chosen over ReactiveUI for lower ceremony, source-generated `[ObservableProperty]` / `[RelayCommand]`, and easier onboarding). Avalonia + CommunityToolkit.Mvvm is the official template since 11.x.
3. **No code-behind business logic.** Code-behind is only allowed for: focus management, IME-specific quirks, custom `OnPropertyChanged`, animation triggers, and the strict minimum of dialog plumbing. Every event becomes a `RelayCommand`.
4. **Services injected via `Microsoft.Extensions.DependencyInjection`** — already used in `Program.cs` for `IAppSettingsService`. We extend the container with `IFilePickerService`, `IMessageBoxService`, `INavigationService`, `IPrintService`, `IOpenUrlService`, `IUndoRedoService`.
5. **Avalonia 11.x stable.** No preview packages. Fluent theme as the default theme, with a `Styles.axaml` file replicating the WinForms colour palette where it was explicit (e.g. Purple title, Green sub-heading, etc.).
6. **One `.axaml` per view.** No big monolithic XAML; child panels become user controls.
7. **Bindings are TwoWay or OneWayToSource where the WinForms version was bidirectional.** Where the WinForms version only wrote to the control once and then read it back on save (e.g. `txt_title.Text` for exam title), we still use TwoWay since the VM is the source of truth.

### 3.2 New target solution layout

```
src/
├── Apps/
│   ├── Creator/                      (Avalonia, OutputType=WinExe, net10.0)
│   │   ├── App.axaml(.cs)
│   │   ├── Program.cs
│   │   ├── Views/
│   │   │   ├── MainWindow.axaml(.cs)
│   │   │   ├── Panes/
│   │   │   │   ├── SplashPaneView.axaml(.cs)
│   │   │   │   ├── ExamPropertiesPaneView.axaml(.cs)
│   │   │   │   └── QuestionEditorPaneView.axaml(.cs)
│   │   │   └── Dialogs/
│   │   │       ├── AddSectionDialog.axaml(.cs)
│   │   │       ├── EditSectionDialog.axaml(.cs)
│   │   │       ├── PrintOptionsDialog.axaml(.cs)
│   │   │       └── AboutDialog.axaml(.cs)
│   │   ├── ViewModels/
│   │   │   ├── MainWindowViewModel.cs
│   │   │   ├── Panes/
│   │   │   │   ├── IRightPaneViewModel.cs
│   │   │   │   ├── SplashPaneViewModel.cs
│   │   │   │   ├── ExamPropertiesPaneViewModel.cs
│   │   │   │   └── QuestionEditorPaneViewModel.cs
│   │   │   ├── Nodes/
│   │   │   │   ├── NodeViewModel.cs              (abstract)
│   │   │   │   ├── ExamNodeViewModel.cs
│   │   │   │   ├── SectionNodeViewModel.cs
│   │   │   │   └── QuestionNodeViewModel.cs
│   │   │   ├── OptionRowViewModel.cs
│   │   │   ├── ExamHistoryEntryViewModel.cs
│   │   │   └── Dialogs/ ...
│   │   ├── Services/
│   │   │   ├── IUndoRedoService.cs (+ impl)
│   │   │   ├── IPrintService.cs (+ Pdf-backed impl)
│   │   │   └── IExamHistoryService.cs (+ impl wrapping IAppSettingsService)
│   │   ├── Converters/
│   │   │   ├── BitmapToAvaloniaImageConverter.cs
│   │   │   ├── NullToBoolConverter.cs
│   │   │   └── EnumToBoolConverter.cs
│   │   └── Assets/                   (icons, images extracted from .resx)
│   ├── Simulator/                    (Avalonia, OutputType=WinExe, net10.0)
│   │   ├── App.axaml(.cs)
│   │   ├── Program.cs
│   │   ├── Views/
│   │   │   ├── MainWindow.axaml(.cs)
│   │   │   ├── Routes/
│   │   │   │   ├── HomeView.axaml(.cs)
│   │   │   │   ├── ExamSettingsView.axaml(.cs)
│   │   │   │   ├── AssessmentView.axaml(.cs)
│   │   │   │   └── ScoreSheetView.axaml(.cs)
│   │   │   └── Dialogs/
│   │   │       ├── ExamPropertiesDialog.axaml(.cs)
│   │   │       └── AboutDialog.axaml(.cs)
│   │   ├── ViewModels/
│   │   │   ├── MainWindowViewModel.cs
│   │   │   ├── Routes/
│   │   │   │   ├── IRouteViewModel.cs
│   │   │   │   ├── HomeViewModel.cs
│   │   │   │   ├── ExamSettingsViewModel.cs
│   │   │   │   ├── AssessmentViewModel.cs
│   │   │   │   └── ScoreSheetViewModel.cs
│   │   │   ├── Items/
│   │   │   │   ├── ExamRowViewModel.cs
│   │   │   │   └── SectionSelectionViewModel.cs
│   │   │   └── Dialogs/ ...
│   │   ├── Services/
│   │   │   ├── INavigationService.cs (+ impl)
│   │   │   ├── ITimerService.cs (+ DispatcherTimer impl)
│   │   │   └── IScoreSheetPrintService.cs (+ impl)
│   │   └── Converters/, Assets/
├── Libraries/
│   ├── Core/             ← unchanged
│   ├── ExamIO/           ← unchanged
│   ├── Logging/          ← unchanged
│   ├── Storage/          ← unchanged
│   └── Shared.Avalonia/  ← NEW (replaces Shared.WinForms)
│       ├── Shared.Avalonia.csproj
│       ├── Controls/
│       │   ├── SingleAnswerOption.axaml(.cs)
│       │   └── MultiAnswerOption.axaml(.cs)
│       ├── Dialogs/
│       │   └── LicenseDialog.axaml(.cs)
│       ├── Services/
│       │   ├── IFilePickerService.cs (+ impl using IStorageProvider)
│       │   ├── IMessageBoxService.cs (+ impl using MsBox.Avalonia)
│       │   └── IOpenUrlService.cs (+ impl using TopLevel.Launcher)
│       ├── Converters/
│       │   └── BitmapToAvaloniaImageConverter.cs
│       └── Mvvm/
│           ├── ObservableObjectBase.cs (if needed)
│           └── ValidationViewModelBase.cs
└── Tests/
    ├── Shared.Tests/                 ← unchanged
    ├── Storage.Tests/                ← unchanged
    ├── Creator.ViewModels.Tests/     ← NEW (xUnit, headless Avalonia where required)
    └── Simulator.ViewModels.Tests/   ← NEW
```

`Shared.WinForms` is **deleted at the very end of the migration**, not at the start, so it can be referenced by either old or new app during incremental cutover.

### 3.3 Dependency-graph view of the migration

```
Tests.* ──► ViewModels (new)
            │
            ├──► Services (new, both apps)
            │     │
            │     ├──► Storage  (unchanged)
            │     ├──► ExamIO   (unchanged)
            │     └──► Logging  (unchanged)
            │
            └──► Core (unchanged)

Views (new) ──► ViewModels (new)
            ──► Shared.Avalonia controls / converters
```

Nothing in the new Views or ViewModels references `System.Windows.Forms.*`.

---

## 4. Phased implementation roadmap

Each phase ends with a working build and at least one runnable artefact. Estimates are illustrative.

### Phase 0 — Preparation (½ day)

1. Tag the current `main` as `winforms-final-vN`.
2. Create branch `feature/avalonia-migration`.
3. Add a new solution folder `/Apps.Avalonia/` so the old and new apps can coexist during transition. Both old and new `Creator.csproj` can build side-by-side because the new one is renamed `Creator.Avalonia.csproj` temporarily; we rename at the cutover.
4. Add `Avalonia` package versions to `Directory.Packages.props`:
   ```xml
   <PackageVersion Include="Avalonia"                              Version="11.2.*" />
   <PackageVersion Include="Avalonia.Desktop"                      Version="11.2.*" />
   <PackageVersion Include="Avalonia.Themes.Fluent"                Version="11.2.*" />
   <PackageVersion Include="Avalonia.Fonts.Inter"                  Version="11.2.*" />
   <PackageVersion Include="Avalonia.Controls.DataGrid"            Version="11.2.*" />
   <PackageVersion Include="Avalonia.Diagnostics"                  Version="11.2.*" />
   <PackageVersion Include="Avalonia.ReactiveUI"                   Version="11.2.*" /> <!-- only if we need WhenAnyValue -->
   <PackageVersion Include="CommunityToolkit.Mvvm"                 Version="8.4.*" />
   <PackageVersion Include="MessageBox.Avalonia"                   Version="3.2.*" />
   <PackageVersion Include="LiveChartsCore.SkiaSharpView.Avalonia" Version="2.0.0-rc5.*" />
   <PackageVersion Include="Avalonia.Headless.XUnit"               Version="11.2.*" />
   ```
5. Verify `dotnet build` still succeeds.

### Phase 1 — Build Shared.Avalonia and host an empty window (1 day)

1. Create `Libraries/Shared.Avalonia/Shared.Avalonia.csproj` targeting `net10.0` (no `-windows`).
2. Port `LicenseUi` first — easiest form, no business logic, only loads a manifest resource. Verifies the embedded `LICENSE` resource binding still works.
3. Create `IFilePickerService`, `IMessageBoxService`, `IOpenUrlService` skeleton implementations.
4. Scaffold `Creator/Creator.Avalonia.csproj` with a single `MainWindow` displaying "Hello Avalonia" and DI container wired up.
5. Smoke-run: `dotnet run --project src/Apps/Creator/Creator.Avalonia.csproj`.

### Phase 2 — Creator MVVM skeleton (2–3 days)

1. Define `MainWindowViewModel` with placeholders for menus, toolbar, tree, and right pane.
2. Implement `IRightPaneViewModel` and the three concrete pane VMs (`SplashPaneViewModel`, `ExamPropertiesPaneViewModel`, `QuestionEditorPaneViewModel`). All empty for now.
3. Build the MainWindow XAML: `Menu`, `ToolStrip` (use Avalonia `Menu` + `StackPanel` of `Button`s with images), `SplitView` or `Grid` with `GridSplitter`, `TreeView`, and a `ContentControl` bound to `CurrentRightPane`.
4. Bind keyboard gestures via `Window.KeyBindings` (Ctrl+N etc.) — see §7.2.
5. Wire menu/toolbar commands to `RelayCommand`s that are stubs (`NotImplementedException` in the body) but already enabled/disabled correctly via `CanExecute`.

### Phase 3 — Creator: SplashPane (½ day)

1. Port the splash/history panel. Render banner `Image` (asset), a list of `ExamHistoryEntryViewModel` items (file path), and a "Clear History" link.
2. Inject `IExamHistoryService`.
3. Verify clicking a history entry opens an exam (Phase 4 dependency: stub until Phase 4 completes the Open command).

### Phase 4 — Creator: file commands (New, Open, Save, SaveAs, Close, Import/Export, Exit) (1–2 days)

1. Wire `New` → reset `MainWindowViewModel.Exam` to `new Exam()`, set `CurrentRightPane = ExamPropertiesPaneViewModel`.
2. Wire `Open` → `IFilePickerService.PickOpenFileAsync(filters: "OEF Files (*.oef)|*.oef")`, then call existing `ExamFileLoader.TryLoad` and rebuild the tree.
3. Wire Import/Export (JSON / XML / PDF) using the same filters as today.
4. Implement `IsDirty` tracking via `ObservableObject.PropertyChanged` aggregated on the root VM.
5. `FormClosing` → `Window.Closing` handler that calls `MainWindowViewModel.ConfirmCloseAsync()` and cancels if the user chooses Cancel.

### Phase 5 — Creator: tree view + node ViewModels (1 day)

1. Replace `TreeNode` subclasses with `NodeViewModel` hierarchy.
2. Bind `TreeView.ItemsSource` to `MainWindowViewModel.ExamNodes`; use `HierarchicalDataTemplate` per node type (icon + text).
3. Implement `SelectedItem` two-way binding (using `TreeView.SelectionChanged` adapter — see §7.4).
4. Implement context menus per node type via `ContextFlyout` resources.
5. Implement `Del` key on `QuestionNode` → `DeleteQuestionCommand`.

### Phase 6 — Creator: ExamPropertiesPane (½ day)

1. Bind all property editors (`Title`, `Code`, `Pass Mark`, `Time Limit`, `Instructions`, `Hide Answers`, `Version`) to `ExamPropertiesPaneViewModel`.
2. `Save Properties` button → `RelayCommand` that commits to the underlying `Exam.Properties` and sets `IsDirty = true`. (In the WinForms version this also enables some toolbar items — that becomes the natural `CanExecute` consequence in MVVM, no explicit `Enable*Controls()` calls needed.)

### Phase 7 — Creator: QuestionEditorPane + options (2 days)

1. Port question text/explanation/image fields, with a `Bitmap` ↔ `IImage` converter.
2. Port `IsMultipleChoice` toggle: when toggled, replace the existing `OptionRowViewModel` items with the alternative type. Show a confirmation MessageBox if there are existing options of the other type (matches the current "you cannot mix option types" guard).
3. Use `ItemsControl` for options, with `DataTemplate`s for `SingleAnswerOptionViewModel` vs `MultiAnswerOptionViewModel`.
4. Add / Remove option commands.
5. Insert image (`IFilePickerService` with JPEG/PNG filter), Clear image.

### Phase 8 — Creator: Undo / Redo / Cut / Copy / Paste (1 day)

1. Move `Creator/Utilities/UndoRedo.cs` into a service `IUndoRedoService`. The data shape (`ChangeRepresentationObject` with `Action` + `Question` + `SectionTitle`) stays exactly the same.
2. Implement Undo/Redo commands that replay the same three action kinds (Add / Delete / Modify) against the VM tree.
3. Cut / Copy / Paste delegate to the focused `TextBox` via Avalonia's `Cut()`/`Copy()`/`Paste()` methods through a custom `IClipboardService` (so we still preserve "Do you want to paste over current selection?" prompt).

### Phase 9 — Creator: AddSection / EditSection / About / License / PrintOptions modals (1 day)

1. Each becomes an `await dialog.ShowDialog<TResult>(owner)` pattern returning a strongly-typed result instead of mutating a public property.
2. `PrintOptions` re-uses the same three-radio-button layout and the same enablement rules (driven from the selected `NodeViewModel` type).

### Phase 10 — Creator: Print / PrintPreview (1–2 days)

1. The current Creator already has `Writer.ToPdf` (PdfSharp). Print becomes: render to a temp PDF, then `IOpenUrlService.OpenFile(tempPath)` on Windows, or `IPrintService.PrintPdf(tempPath)` if we want to hit the spooler directly.
2. PrintPreview: render PDF and display in an `Image` (rasterise first page with a small Pdfium / `PDFtoImage` helper). The user already sees a similar preview in the WinForms `PrintPreviewDialog`.

### Phase 11 — Simulator MVVM skeleton + Home route (1 day)

1. Build `Simulator/MainWindow.axaml` with `ContentControl` bound to `CurrentRoute`.
2. Implement `HomeViewModel`: `ObservableCollection<ExamRowViewModel> Exams`, `RelayCommand`s for `Add`, `Remove`, `Properties`, `Start`.
3. Replace `DataGridView` with `Avalonia.Controls.DataGrid` (two columns: Name, Path; full-row selection; row headers hidden).
4. `SelectionChanged` → `OnPropertyChanged(nameof(CanStart))` etc. — no explicit `if/else if/else` block needed (CanExecute on the commands does the enablement).
5. Mutex single-instance check stays in `Program.cs`, only the `MessageBox` call switches to `IMessageBoxService`.

### Phase 12 — Simulator: ExamProperties modal (½ day)

1. `ExamPropertiesViewModel` takes an `Exam` + file path; exposes formatted read-only strings (file size in KB/MB, creation date, version, etc.).

### Phase 13 — Simulator: ExamSettings route (1 day)

1. `ExamSettingsViewModel`: `CandidateName`, `EnableCustomTimer`, `CustomTimerMinutes`, `Mode` (`enum SelectionMode { AllSections, FixedNumberOfQuestions, SelectedSections }`), `SectionSelections : ObservableCollection<SectionSelectionViewModel>`, `Proceed` and `Cancel` commands.
2. Bind `IsEnabled` on `num_time_limit`, `num_questions`, `clb_section_options` to the appropriate radio-button properties (replaces the three CheckedChanged handlers verbatim, but in XAML).
3. Replace `CheckedListBox` with a `ListBox` whose item template is a `CheckBox` bound to `SectionSelectionViewModel.IsChecked`.
4. Implement Select All / Deselect All.
5. Validation: replicate the existing "no questions to be displayed based on your selection" error MessageBox.

### Phase 14 — Simulator: Assessment route (2–3 days)

This is the riskiest single screen. Plan:

1. `AssessmentViewModel` exposes:
   * `Phase` (`Intro` / `Running`).
   * `ExamTitle`, `ExamCode`, `ExamInstructions` (for Intro).
   * `TimeLeft` (`TimeSpan`), formatted as `HH:mm:ss`.
   * `CurrentQuestionIndex`, `QuestionNumber`, `SectionTitle`, `QuestionText`, `QuestionImage`, `Explanation`.
   * `Options : ObservableCollection<AnswerOptionViewModel>`.
   * `ShowAnswer` flag, `ShowAnswerButtonText` derived ("Show Answer" / "Hide Answer").
   * `Progress` ("X / Y answered").
   * Commands: `Begin`, `Next`, `Previous`, `Pause`, `End`, `ToggleShowAnswer`.
2. Use `DispatcherTimer` (1-second tick) injected via `ITimerService` so it can be mocked in tests.
3. Pause: stop timer, show MessageBox; resume on dismiss (matches existing UX).
4. Highlight correct/incorrect: bind the option foreground to a `OptionStateBrush` derived value (Black / Green / Red) — the WinForms code sets `ForeColor` directly; we move that into the VM.
5. End-of-exam aggregation logic (per-section score breakdown) moves *verbatim* into the VM (it is non-UI logic that just happens to live in the form today).
6. On End → assign `MainWindowViewModel.CurrentRoute = new ScoreSheetViewModel(...)`.

### Phase 15 — Simulator: ScoreSheet route + print (1–2 days)

1. Replace `System.Windows.Forms.DataVisualization.Chart` with `LiveChartsCore.SkiaSharpView.Avalonia` rendering a horizontal `BarSeries` with two values: Pass Mark, Your Score, on a normalised 0–1000 scale (matches the WinForms calculation `_settings.NumberOfCorrectAnswers * 1000 / _settings.Questions.Count`).
2. Use `Avalonia.Controls.DataGrid` for the section breakdown.
3. PrintResult → render the same content into a PDF (PdfSharp) and open it. The current GDI+ print code becomes a `PdfPrintService.PrintScoreSheet(_settings, _exam, chartBitmap)` method that produces an identical layout (heading, candidate name, time allowed, date, exam code, chart image, status, table).
4. Retake → `Window.Close()` (route VM raises a `CloseRequested` event picked up by the host).

### Phase 16 — Replace remaining Shared.WinForms usage (½ day)

1. Move `LicenseUi` permanently to `Shared.Avalonia` (already done in Phase 1, just refactor references).
2. Delete `Shared.WinForms` project and remove from `.slnx`.

### Phase 17 — Cross-cutting polish (1–2 days)

1. Fluent theme tweaks; light/dark switch (optional but trivial).
2. DPI verification on a 4K display (Avalonia is DPI-independent by default — see §13).
3. Icon-strip migration (extract from `.resx` to PNG / SVG assets).
4. Keyboard-navigation pass (`TabIndex` → `TabIndex` attached property in Avalonia; mostly works automatically).
5. Accessibility pass: `AutomationProperties.Name` on all unlabeled controls.

### Phase 18 — Tests, CI, packaging (2 days)

1. Author ViewModel tests for `UndoRedoService`, `AssessmentViewModel.End`, `ExamSettingsViewModel.Proceed`, `MainWindowViewModel.TreeSelection`.
2. Add Avalonia.Headless tests for a few critical bindings (TreeView SelectedItem, ContentControl swap).
3. Update GitHub Actions workflow — drop `windows-latest` constraint where possible; keep it for the published WinExe (single-file `PublishSingleFile=true`).
4. Update installer (Inno Setup or whatever `build/`/`installer/` uses) to ship the Avalonia binaries.

### Phase 19 — Cutover (½ day)

1. Rename Avalonia projects to drop `.Avalonia` suffix.
2. Delete old `Creator/Creator.csproj` and `Simulator/Simulator.csproj`.
3. Update README and CHANGELOG.

**Total estimate: ~20 working days for one engineer, parallelisable to ~12 days with two.**

---

## 5. UI inventory and detailed mapping

### 5.1 Creator HomeUi mapping

| WinForms element | Avalonia equivalent | Notes |
|---|---|---|
| `MenuStrip menuStrip1` | `Menu` | One `MenuItem` per top-level entry; nested `MenuItem`s for items. |
| `ToolStrip toolStrip1` | `StackPanel Orientation="Horizontal"` of `Button`s with `Image` content, or `Avalonia.Controls.ToolStrip` (community), or `WrapPanel`. | `ToolStripSeparator` → `Separator`. |
| `SplitContainer splitContainer2` | `Grid` with two columns and a `GridSplitter`. | `SplitterDistance=294` ⇒ `ColumnDefinitions="294,5,*"`. |
| `GroupBox groupBox2 (Exam Explorer)` | `HeaderedContentControl` or `Border` + `TextBlock` header. | Avalonia has no `GroupBox` by default; use the `HeaderedContentControl` Fluent template. |
| `TreeView trv_view_exam` | `TreeView` with `HierarchicalDataTemplate`. | `ImageList` → bind a `Source` per VM type. |
| `Panel pan_splash` etc. | `UserControl` + `DataTemplate`s in `MainWindow.Resources`. | Replaces `Controls.Add/Remove` (see §6.1). |
| `LinkLabel` history items | `Button Classes="link"` with custom Fluent link style. | Click → `OpenExamCommand` with the file path as parameter. |
| `Label` (static text) | `TextBlock`. | |
| `TextBox` (single-line) | `TextBox`. | `Multiline=true` → `AcceptsReturn=true TextWrapping=Wrap`. |
| `NumericUpDown num_passmark / num_time_limit` | `Avalonia.Controls.NumericUpDown`. | Same `Minimum`/`Maximum`/`Increment`. |
| `CheckBox chk_hide_answers, chkMulipleChoice` | `CheckBox`. | |
| `PictureBox pictureBox1, pct_image` | `Image`. | Bind `Source` to `IImage`. |
| `OpenFileDialog`, `SaveFileDialog` | `TopLevel.StorageProvider.OpenFilePickerAsync` / `SaveFilePickerAsync`. | Encapsulated in `IFilePickerService`. Filters are `FilePickerFileType("OEF Files") { Patterns = new[] { "*.oef" } }`. |
| `PrintDialog`, `PrintPreviewDialog`, `PrintDocument` | PdfSharp render → temp file → preview/print. | See §10.1. |
| `ContextMenuStrip cms_section, cms_question` | `MenuFlyout` assigned to `TreeViewItem.ContextFlyout`. | |
| `ToolStripMenuItem.ShortcutKeys` | `KeyBinding` on the window. | See §7.2. |

### 5.2 Creator dialogs mapping

| WinForms form | Avalonia | Result mechanism |
|---|---|---|
| `AddSection` (returns `Title`) | `AddSectionDialog : Window` | `await dialog.ShowDialog<string?>(owner)` returning `Title` or `null`. |
| `EditSection` | `EditSectionDialog : Window` | Same. |
| `PrintOptions` (returns `PrintOption` enum) | `PrintOptionsDialog : Window` | Returns `PrintOption?`. |
| `AboutUi` | `AboutDialog : Window` | No return value. |

### 5.3 Simulator HomeUi mapping

| WinForms element | Avalonia equivalent |
|---|---|
| `MenuStrip menuStrip1` | `Menu` |
| `DataGridView dgv_exams` | `Avalonia.Controls.DataGrid` (NuGet `Avalonia.Controls.DataGrid`). `MultiSelect=true`, `RowHeadersVisible=false`, `SelectionMode=Extended`. |
| `name` column auto-sized to 1/3 of grid | `DataGridTextColumn.Width=new DataGridLength(1, DataGridLengthUnitType.Star)` weighted 1:2. Replaces the `SizeChanged → ChangeHeaderSize` handler entirely (no code required). |
| `btn_start / btn_properties / btn_remove / btn_add` | `Button`s bound to `Start`/`Properties`/`Remove`/`Add` commands with `CanExecute` derived from `SelectedItems`. |
| `OpenFileDialog ofd_exam` | `IFilePickerService.PickOpenFilesAsync` with `Patterns = new[] { "*.oef" }`, `AllowMultiple=true`. |

### 5.4 Simulator ExamSettings mapping

| WinForms element | Avalonia |
|---|---|
| `CheckedListBox clb_section_options` | `ListBox` with `ItemTemplate` `<CheckBox IsChecked="{Binding IsChecked}" Content="{Binding Title}"/>`. |
| `NumericUpDown num_questions / num_time_limit` | `NumericUpDown`. |
| `RadioButton rdb_fixed_number_questions` / `rdb_selected_sections` | `RadioButton GroupName="ExamMode"`. |
| `CheckBox chk_enable_timer` | `CheckBox`. |
| Enabled-state cascading via `CheckedChanged` | XAML `IsEnabled="{Binding EnableCustomTimer}"` etc. — no event handler code. |

### 5.5 Simulator Assessment mapping

| WinForms element | Avalonia |
|---|---|
| Visible-toggled intro labels (`lbl_exam_*`) | `<Border IsVisible="{Binding IsIntro}">` block. |
| Visible-toggled running labels | `<Grid IsVisible="{Binding IsRunning}">` block. |
| `Timer` (`Interval=1000`) | `DispatcherTimer` in `ITimerService`. |
| Dynamic option `CheckBox`/`RadioButton` | `ItemsControl ItemsSource="{Binding Options}"` with `DataTemplate`s for `MultiAnswerOptionViewModel` and `SingleAnswerOptionViewModel` (the latter using a `RadioButton` with `GroupName="{Binding $parent[ItemsControl].DataContext.OptionGroup}"`). |
| `ForeColor=Color.Green/Red` after Show Answer | `Foreground="{Binding StateBrush}"` where `StateBrush` returns Green/Red/Default. |
| `Hide(); ScoreSheetUi.ShowDialog()` | `NavigationService.GoTo(new ScoreSheetViewModel(...))`. |

### 5.6 Simulator ScoreSheet mapping

| WinForms element | Avalonia |
|---|---|
| `System.Windows.Forms.DataVisualization.Chart` | `LiveChartsCore.SkiaSharpView.Avalonia.CartesianChart` with horizontal `BarSeries`. |
| `DataGridView dgv_show_breakdown` | `DataGrid`. |
| `PrintDocument pnt_doc` + `PrintPreviewDialog` | `IScoreSheetPrintService.GeneratePdfAsync` + open/preview. |

---

## 6. Architectural improvements (the three sanctioned redesigns)

### 6.1 Creator right pane: ContentControl + DataTemplates

**Current code (HomeUi.cs L621–717 abridged):**
```csharp
private void AfterSelect(object sender, TreeViewEventArgs e)
{
    if (trv_view_exam.SelectedNode.GetType() == typeof(ExamNode)) {
        newQuestionToolStripButton.Enabled = false;
        if (splitContainer2.Panel2.Controls.Contains(pan_display_questions)) {
            splitContainer2.Panel2.Controls.Remove(pan_display_questions);
            splitContainer2.Panel2.Controls.Add(pan_exam_properties);
        } else if (splitContainer2.Panel2.Controls.Contains(pan_splash)) {
            splitContainer2.Panel2.Controls.Remove(pan_splash);
            splitContainer2.Panel2.Controls.Add(pan_exam_properties);
        }
    } else if (trv_view_exam.SelectedNode.GetType() == typeof(SectionNode)) { ... }
    else { ... } // QuestionNode
}
```

**Avalonia replacement.**

```csharp
public interface IRightPaneViewModel { }

public sealed partial class MainWindowViewModel : ObservableObject
{
    [ObservableProperty] private NodeViewModel? selectedNode;
    [ObservableProperty] private IRightPaneViewModel currentRightPane;

    public MainWindowViewModel(...)
    {
        currentRightPane = new SplashPaneViewModel(historyService);
    }

    partial void OnSelectedNodeChanged(NodeViewModel? value)
    {
        CurrentRightPane = value switch
        {
            null                           => new SplashPaneViewModel(historyService),
            ExamNodeViewModel exam         => new ExamPropertiesPaneViewModel(exam),
            SectionNodeViewModel section   => new QuestionEditorPaneViewModel(section, editable: false),
            QuestionNodeViewModel question => new QuestionEditorPaneViewModel(question, editable: true),
            _ => CurrentRightPane,
        };
    }
}
```

```xml
<!-- MainWindow.axaml -->
<Window.Resources>
    <DataTemplate DataType="vm:SplashPaneViewModel">          <views:SplashPaneView/>          </DataTemplate>
    <DataTemplate DataType="vm:ExamPropertiesPaneViewModel">  <views:ExamPropertiesPaneView/>  </DataTemplate>
    <DataTemplate DataType="vm:QuestionEditorPaneViewModel">  <views:QuestionEditorPaneView/>  </DataTemplate>
</Window.Resources>

<Grid ColumnDefinitions="294,5,*">
    <views:ExamTreeView Grid.Column="0"/>
    <GridSplitter Grid.Column="1"/>
    <ContentControl Grid.Column="2" Content="{Binding CurrentRightPane}"/>
</Grid>
```

The whole branching `AfterSelect` block disappears. `newQuestionToolStripButton.Enabled = false/true` becomes `NewQuestionCommand.CanExecute` derived from `SelectedNode is SectionNodeViewModel or QuestionNodeViewModel`.

### 6.2 AssessmentUi: state-driven phase

```csharp
public enum AssessmentPhase { Intro, Running }

public sealed partial class AssessmentViewModel : ObservableObject, IRouteViewModel
{
    [ObservableProperty] private AssessmentPhase phase = AssessmentPhase.Intro;

    public bool IsIntro   => Phase == AssessmentPhase.Intro;
    public bool IsRunning => Phase == AssessmentPhase.Running;

    partial void OnPhaseChanged(AssessmentPhase value)
    {
        OnPropertyChanged(nameof(IsIntro));
        OnPropertyChanged(nameof(IsRunning));
    }

    [RelayCommand]
    private void Begin()
    {
        Phase = AssessmentPhase.Running;
        _timer.Start();
        NavigateExam(NavOption.Begin);
    }
    // ...
}
```

```xml
<Grid>
    <StackPanel IsVisible="{Binding IsIntro}">
        <TextBlock Text="{Binding ExamTitle}"/>
        <TextBlock Text="{Binding ExamCode}"/>
        <TextBlock Text="{Binding ExamInstructions}" TextWrapping="Wrap"/>
        <Button Content="Begin" Command="{Binding BeginCommand}" HotKey="Enter"/>
    </StackPanel>

    <Grid IsVisible="{Binding IsRunning}" RowDefinitions="Auto,*,Auto">
        <!-- header: progress, timer -->
        <DockPanel Grid.Row="0">
            <TextBlock DockPanel.Dock="Right" Text="{Binding ElapsedTimeText}"/>
            <TextBlock Text="{Binding ProgressText}"/>
        </DockPanel>

        <!-- body: question, image, options, explanation -->
        <ScrollViewer Grid.Row="1">
            <StackPanel>
                <TextBlock Text="{Binding SectionTitle}" Classes="h3"/>
                <TextBlock Text="{Binding QuestionText}" TextWrapping="Wrap"/>
                <Image    Source="{Binding QuestionImage, Converter={x:Static c:Bmp.Conv}}"
                          IsVisible="{Binding QuestionImage, Converter={x:Static c:NullToBool.Inverse}}"/>
                <ItemsControl ItemsSource="{Binding Options}"/>
                <TextBox IsReadOnly="True" AcceptsReturn="True"
                         Text="{Binding Explanation}"
                         IsVisible="{Binding ShowAnswer}"/>
            </StackPanel>
        </ScrollViewer>

        <!-- footer: nav buttons -->
        <UniformGrid Grid.Row="2" Columns="5">
            <Button Content="Previous" Command="{Binding PreviousCommand}"/>
            <Button Content="Next"     Command="{Binding NextCommand}"/>
            <Button Content="Pause"    Command="{Binding PauseCommand}"/>
            <Button Content="{Binding ShowAnswerButtonText}"
                    Command="{Binding ToggleShowAnswerCommand}"
                    IsVisible="{Binding !ExamHidesAnswers}"/>
            <Button Content="End"      Command="{Binding EndCommand}"/>
        </UniformGrid>
    </Grid>
</Grid>
```

No `EnableControls()`, no per-control `Visible = true/false`, no global state-flag bookkeeping. Same screen behaviour for the user.

### 6.3 Simulator workflow: navigation service

```csharp
public interface INavigationService
{
    void GoTo(IRouteViewModel route);
    void GoBack();
}

public sealed partial class MainWindowViewModel : ObservableObject
{
    [ObservableProperty] private IRouteViewModel currentRoute;
    public MainWindowViewModel(INavigationService nav, IServiceProvider sp)
    {
        ((NavigationService)nav).Configure(this);
        CurrentRoute = sp.GetRequiredService<HomeViewModel>();
    }
}
```

Then `HomeViewModel.Start` does `_nav.GoTo(_sp.GetRequiredService<ExamSettingsViewModel>().Initialize(exam))`. `ExamSettingsViewModel.Proceed` calls `_nav.GoTo(new AssessmentViewModel(...))`. `AssessmentViewModel.End` calls `_nav.GoTo(new ScoreSheetViewModel(...))`. `ScoreSheetViewModel.Retake` calls `_nav.GoBack()`.

Visually identical to today. Replacement of `_form.Hide(); next.ShowDialog(); Close();` with `_nav.GoTo(...)`.

---

## 7. Translating WinForms idioms to MVVM

### 7.1 Event handler → command

| WinForms | Avalonia / CommunityToolkit.Mvvm |
|---|---|
| `private void Save(object sender, EventArgs e) { ... }` and `this.saveToolStripMenuItem.Click += Save;` | `[RelayCommand(CanExecute = nameof(CanSave))] private void Save() { ... }` and `Command="{Binding SaveCommand}"` |
| `if (chk_enable_timer.Checked) num_time_limit.Enabled = true;` | XAML: `<NumericUpDown IsEnabled="{Binding EnableCustomTimer}"/>` and `[ObservableProperty] private bool enableCustomTimer;` |
| `txt_explanation.TextChanged += QuestionChanged; QuestionChanged builds a ChangeRepresentationObject and pushes onto Undo stack` | `partial void OnTextChanged(string value)` in `OptionRowViewModel` or whichever owns it, calling `_undoRedo.Push(...)`. Suppression via `IsLoadingFromModel` flag in VM. |
| `DisconnectHandlers()` / `ReconnectHandlers()` hack | Not needed. Setting a property on the VM from inside the VM doesn't re-enter the binding. If you must, wrap in an `IDisposable` `using (_undo.Suspend()) { ... }`. |

### 7.2 Keyboard shortcuts → `KeyBinding`

```xml
<Window.KeyBindings>
    <KeyBinding Gesture="Ctrl+N" Command="{Binding NewCommand}"/>
    <KeyBinding Gesture="Ctrl+O" Command="{Binding OpenCommand}"/>
    <KeyBinding Gesture="Ctrl+S" Command="{Binding SaveCommand}"/>
    <KeyBinding Gesture="Ctrl+P" Command="{Binding PrintCommand}"/>
    <KeyBinding Gesture="Ctrl+Z" Command="{Binding UndoCommand}"/>
    <KeyBinding Gesture="Ctrl+Y" Command="{Binding RedoCommand}"/>
    <KeyBinding Gesture="Ctrl+X" Command="{Binding CutCommand}"/>
    <KeyBinding Gesture="Ctrl+C" Command="{Binding CopyCommand}"/>
    <KeyBinding Gesture="Ctrl+V" Command="{Binding PasteCommand}"/>
</Window.KeyBindings>
```

`Del` on the tree:

```xml
<TreeView ...>
    <TreeView.KeyBindings>
        <KeyBinding Gesture="Delete"
                    Command="{Binding DeleteSelectedCommand}"/>
    </TreeView.KeyBindings>
</TreeView>
```

`MenuItem` shortcut hints (so users see "Ctrl+S" in the menu):
```xml
<MenuItem Header="_Save" Command="{Binding SaveCommand}" InputGesture="Ctrl+S"/>
```

### 7.3 Modal dialogs returning a value

```csharp
public sealed partial class AddSectionDialog : Window
{
    public AddSectionDialog() => InitializeComponent();

    private void OnOk(object? s, RoutedEventArgs e)
        => Close(((AddSectionViewModel)DataContext!).Title);

    private void OnCancel(object? s, RoutedEventArgs e)
        => Close(null);
}

// caller:
var dlg = new AddSectionDialog { DataContext = new AddSectionViewModel() };
var title = await dlg.ShowDialog<string?>(owner);
if (!string.IsNullOrWhiteSpace(title)) {
    Exam.AddSection(title);
}
```

### 7.4 TreeView `SelectedItem` two-way binding

Avalonia 11.x `TreeView.SelectedItem` is read-only in bindings; we wire it via a small attached behaviour or via code-behind in the `MainWindow`:

```csharp
private void OnTreeSelectionChanged(object? s, SelectionChangedEventArgs e)
    => ((MainWindowViewModel)DataContext!).SelectedNode =
        ((TreeView)s!).SelectedItem as NodeViewModel;
```

Limited code-behind for selection plumbing only is acceptable per the brief.

### 7.5 File pickers via `IStorageProvider`

```csharp
public sealed class FilePickerService(IServiceProvider sp) : IFilePickerService
{
    private static TopLevel GetTopLevel(IServiceProvider sp) =>
        TopLevel.GetTopLevel(sp.GetRequiredService<MainWindow>())
            ?? throw new InvalidOperationException("No top level");

    public async Task<string?> PickOpenFileAsync(IReadOnlyList<FileFilter> filters)
    {
        var files = await GetTopLevel(sp).StorageProvider.OpenFilePickerAsync(new()
        {
            AllowMultiple = false,
            FileTypeFilter = filters.Select(f =>
                new FilePickerFileType(f.Display) { Patterns = f.Patterns }).ToArray(),
        });
        return files.Count == 0 ? null : files[0].TryGetLocalPath();
    }
}
```

The `Filter = "OEF Files|*.oef"` strings in the WinForms code map directly: split by `|` into name/pattern pairs.

### 7.6 MessageBox

```csharp
public async Task<MessageBoxResult> AskAsync(string message, string title,
    MessageBoxButton buttons = MessageBoxButton.OK,
    MessageBoxIcon icon = MessageBoxIcon.Information)
{
    var box = MessageBoxManager.GetMessageBoxStandard(title, message,
        Map(buttons), Map(icon));
    var result = await box.ShowAsync();
    return Map(result);
}
```

Every WinForms call site (`MessageBox.Show("Exam has been successfully saved.", ...)`) becomes `await _msg.AskAsync(...)` with the same arguments.

### 7.7 `Process.Start(url)` (About box)

```csharp
public async Task OpenUriAsync(Uri uri, TopLevel topLevel)
    => await topLevel.Launcher.LaunchUriAsync(uri);
```

Wire `LinkLabel` clicks → `<Button Classes="hyperlink" Content="https://..." Command="{Binding OpenUrlCommand}"/>`.

### 7.8 Anchor / Dock / Padding

Most WinForms `Anchor=Top|Bottom|Left|Right` becomes a `Grid` cell — no anchor required because cells stretch by default. `Dock=Fill` becomes "place in `Grid` with `*` row/column". `DockStyle.Top/Bottom` becomes `DockPanel.Dock="Top"` inside a `DockPanel`.

---

## 8. Suggested folder/project structure

(See §3.2.)

### 8.1 Naming convention

* Views: `XxxView.axaml` / `XxxView.axaml.cs` (a `UserControl`) or `XxxWindow.axaml` for windows.
* ViewModels: `XxxViewModel.cs` matching the view 1:1, plus `IxxViewModel.cs` for interfaces that participate in DataTemplate selection.
* Services: `IXxxService.cs` + `XxxService.cs`.
* No `Designer.cs` files.

### 8.2 DI registration

```csharp
public static IServiceCollection AddCreatorServices(this IServiceCollection s) => s
    .AddSingleton<IAppSettingsService, AppSettingsService>()
    .AddSingleton<IExamHistoryService, ExamHistoryService>()
    .AddSingleton<IUndoRedoService, UndoRedoService>()
    .AddSingleton<IFilePickerService, FilePickerService>()
    .AddSingleton<IMessageBoxService, MessageBoxService>()
    .AddSingleton<IOpenUrlService, OpenUrlService>()
    .AddSingleton<IPrintService, PdfPrintService>()
    .AddTransient<MainWindowViewModel>()
    .AddTransient<SplashPaneViewModel>()
    .AddTransient<ExamPropertiesPaneViewModel>()
    .AddTransient<QuestionEditorPaneViewModel>();

public static IServiceCollection AddSimulatorServices(this IServiceCollection s) => s
    .AddSingleton<IAppSettingsService, AppSettingsService>()
    .AddSingleton<INavigationService, NavigationService>()
    .AddSingleton<ITimerService, DispatcherTimerService>()
    .AddSingleton<IScoreSheetPrintService, ScoreSheetPrintService>()
    .AddTransient<MainWindowViewModel>()
    .AddTransient<HomeViewModel>()
    .AddTransient<ExamSettingsViewModel>()
    .AddTransient<AssessmentViewModel>()
    .AddTransient<ScoreSheetViewModel>();
```

---

## 9. Reusable component strategy

| Component | Lives in | Used by |
|---|---|---|
| `SingleAnswerOption` UserControl | `Shared.Avalonia` | Creator (question editor), Simulator (assessment) |
| `MultiAnswerOption` UserControl | `Shared.Avalonia` | Creator (question editor), Simulator (assessment) |
| `LicenseDialog` | `Shared.Avalonia` | Creator, Simulator |
| `IFilePickerService`, `IMessageBoxService`, `IOpenUrlService` | `Shared.Avalonia` | Both apps |
| `BitmapToAvaloniaImageConverter`, `NullToBoolConverter`, `EnumToBoolConverter` | `Shared.Avalonia` | Both apps |

The two existing UserControls in `Shared.WinForms/Controls` map cleanly to two Avalonia `UserControl`s. They are templated so that callers can bind `Letter`, `Text`, `IsChecked`.

---

## 10. Print / preview replacement

### 10.1 Strategy

The current code uses `System.Drawing.Printing` for `PrintPageEventArgs`-based GDI+ drawing. Replacing this with Avalonia.Headless rendering is plausible but heavyweight. Since `ExamIO/Utilities/Writer.cs` *already* contains `Writer.ToPdf` using PdfSharp, the cleanest path is:

1. **Creator Print** → write to a temp `.pdf` via `Writer.ToPdf` and use `Process.Start { UseShellExecute = true, FileName = tempPdf, Verb = "print" }` on Windows. On non-Windows, just open the PDF.
2. **Creator PrintPreview** → write the same temp PDF and open it via the OS default handler. Optionally, render the first N pages via `PDFtoImage` (a tiny SkiaSharp-backed library) into an Avalonia `Carousel` for in-app preview. The user sees an equivalent preview.
3. **Creator PrintOptions** (current question / current section / all) → translates into a `Writer.ToPdf` overload that takes a `PrintScope` enum and an optional `Section`/`Question` reference.
4. **Simulator ScoreSheet Print** → port the GDI+ draw code (heading, candidate, time, date, exam code, chart image, status, table) into a PdfSharp routine in `IScoreSheetPrintService`. Chart image is captured by `LiveCharts` `chart.GetImage()` (returns a `SKImage` we encode to PNG and embed in the PDF).

### 10.2 Behavioural parity caveat

* The current Print "Current Section" branch is a stub (only renders the heading) — see HomeUi.cs L1040-L1052. We preserve that stub behaviour to be faithful; the migration should not silently add a feature. If you want full implementation, do it as a separate PR.

---

## 11. Risk assessment

| Risk | Impact | Likelihood | Mitigation |
|---|---|---|---|
| `System.Drawing.Bitmap` keeps `Core` Windows-locked | M | High | Acceptable for now (Windows-first). For cross-platform, replace with `byte[] ImageBytes` + `IImage` in views (Core already round-trips bytes via ProtoBuf). |
| Avalonia `TreeView` binding gaps (`SelectedItem`) | L | High | Use code-behind plumbing (allowed exception). |
| `System.Windows.Forms.DataVisualization.Chart` has no direct Avalonia equivalent | M | High | Use `LiveChartsCore.SkiaSharpView.Avalonia`. Validated for `Bar` series, two-series rendering. |
| `PrintDocument` re-implementation drift | M | M | Add a "render-to-PDF golden file" test that compares output for a fixed exam. |
| `WindowsFormsHost`-style interop tempting devs | L | M | Forbid; document in CONTRIBUTING.md. |
| Cross-platform `IStorageProvider` UI subtle differences | L | M | Accept and document; behaviour is similar on Windows, slight UX differences on Linux/macOS. |
| Performance regression on large exams (1k+ questions) | M | M | Use `ItemsRepeater`/virtualised `TreeView` if needed; the WinForms `TreeView` is also non-virtualised so parity is not the issue, perf headroom is. |
| `FormClosing.Cancel` semantics — Avalonia equivalent (`WindowClosingEventArgs.Cancel`) is async-friendly | L | L | Use `Window.Closing` with `e.Cancel = true; await ConfirmAsync()`. |
| `Mutex` single-instance check in Simulator | L | L | Keep as-is in `Program.cs`. |
| Old `.oef` legacy migration on read | L | L | Handled in `Reader.FromOefFile`; unaffected. |
| Designer-time data context (XAML preview) for newcomers | L | L | Provide `d:DataContext` design-time VMs. |

---

## 12. Styling / theming

### 12.1 Theme

Fluent theme as the base (`<FluentTheme/>` in `App.axaml`). Light mode by default to match the existing WinForms look. Dark mode is one toggle away should we want it.

```xml
<Application xmlns="https://github.com/avaloniaui" ...>
    <Application.Styles>
        <FluentTheme/>
        <StyleInclude Source="avares://OpenExamSuite.Shared.Avalonia/Styles/Common.axaml"/>
    </Application.Styles>
</Application>
```

### 12.2 Common.axaml

* Typography matching `Microsoft Sans Serif` 8.25pt (Avalonia default Inter / Segoe UI is fine; bind a setting for those who really want the legacy font).
* Brushes for `PassedBrush=Green` / `FailedBrush=Red` / `PrintHeaderBrush=Purple` / `PrintSubHeaderBrush=Green`, replicating the colours used by the GDI+ print code (`Brushes.Purple`, `Brushes.Green`, `Brushes.Black`).
* `Hyperlink` style for the recent-files link list and About-box links.
* A `GroupBox` style implemented as `HeaderedContentControl` with a thin border + bold header.

### 12.3 Icons

The `imglst_node_images` ImageList (TreeView icons for exam / section / question) is replicated as three PNG assets in `Apps/Creator/Assets/Icons/`. Bind `Image.Source="{StaticResource ExamIcon}"` etc.

---

## 13. DPI scaling

WinForms with `Application.SetCompatibleTextRenderingDefault(false)` and `AutoScaleMode.Font` (used by every form here) is OK but tends to misbehave at >150% DPI with images and PictureBoxes.

Avalonia is fully DPI-aware. `Window.RenderScaling` is automatic. No special handling required. Validate on 100%, 125%, 150%, 200%, and 4K monitor configurations during Phase 17.

Manifest the new EXEs as DPI-aware via the project file:
```xml
<PropertyGroup>
    <ApplicationManifest>app.manifest</ApplicationManifest>
</PropertyGroup>
```
with `dpiAwareness=PerMonitorV2` in the manifest. (Avalonia's default desktop entry already does the right thing on Windows 10+, but explicit is safer.)

---

## 14. Testing strategy

### 14.1 Unit tests (no UI)

Add `Tests/Creator.ViewModels.Tests` and `Tests/Simulator.ViewModels.Tests` projects (xUnit + Shouldly + Moq, matching the existing test stack).

Cover:
* `UndoRedoService`: push, undo, redo, redo-cleared-after-push.
* `ExamPropertiesPaneViewModel.SaveCommand`: dirty bit set, model updated.
* `QuestionEditorPaneViewModel`: option add/remove, mixed-types guard, multiple-choice toggle behaviour.
* `ExamSettingsViewModel.ProceedCommand`: section selection, fixed-number arithmetic (the existing 35-line block), zero-questions error path.
* `AssessmentViewModel.EndCommand`: full scoring logic — convert the WinForms `End` branch into a pure VM method and unit-test it for single-answer, multi-answer, mixed, and `'\0'` (unanswered) paths.
* `MainWindowViewModel.OnSelectedNodeChanged`: produces the right `IRightPaneViewModel`.

### 14.2 Avalonia.Headless smoke tests

* TreeView selection triggers correct `CurrentRightPane` swap.
* `Ctrl+S` key gesture fires `SaveCommand`.
* `AssessmentView` shows intro panel by default and switches to running panel after `Begin`.

### 14.3 Golden-file PDF tests

* Render a fixed `Exam` to PDF with `Writer.ToPdf` and assert byte-level (or PDF-text-extract) match against a checked-in golden file.

### 14.4 Manual test plan

Checklist enumerating every command in §5 — execute each on the migrated app and verify the WinForms-equivalent behaviour:

* Open `samples/Basic Science.oef`, edit a question, save, reopen — same content.
* Open `samples/GMAT Sample.oef`, run the simulator end-to-end, capture the score sheet PDF, compare visually to the WinForms equivalent.
* Ctrl+Z / Ctrl+Y across all three action types (Add / Delete / Modify).
* Print preview, then Print — same on-screen layout.
* "Hide Answers" toggle in properties → in Simulator, the Show Answer button should disappear during assessment.

---

## 15. Incremental migration approach

To preserve a working `main` at all times:

1. **Side-by-side projects.** Keep `src/Apps/Creator` and `src/Apps/Simulator` (WinForms) alive throughout. Add `src/Apps/Creator.Avalonia` and `src/Apps/Simulator.Avalonia` next to them.
2. **Shared libraries are reused, not duplicated.** Both old and new apps consume `Core` / `ExamIO` / `Storage` / `Logging` unchanged.
3. **Per-phase merges.** Each phase in §4 ends with an open PR that builds and passes tests. Avalonia projects gain feature parity incrementally; the WinForms versions remain shippable.
4. **Cutover** happens only once both Avalonia apps reach feature parity (after Phase 17). The cutover renames Avalonia projects, deletes WinForms projects, removes `Shared.WinForms`.
5. **Roll-back is trivial** — `git revert` the cutover commit; the WinForms code is still in history.

---

## 16. Performance considerations

* **`TreeView` virtualisation.** Avalonia's `TreeView` does not virtualise by default. For exams with >1000 questions, switch to `<TreeView><TreeView.ItemsPanel><ItemsPanelTemplate><VirtualizingStackPanel/></ItemsPanelTemplate></TreeView.ItemsPanel></TreeView>`. The current WinForms `TreeView` is non-virtualised too, so this is a free win, not a regression target.
* **`ItemsControl` of options.** Use `ItemsControl` with default `StackPanel`; only switch to `VirtualizingStackPanel` if option counts exceed ~50 (they don't in practice).
* **`Image` loading.** Bitmaps from the model are loaded once and cached on the VM. Image decode happens on the UI thread; for large images, dispatch to a background thread and post the decoded `Bitmap` back.
* **Dispatcher timer interval = 1s.** Identical to the WinForms `Timer.Interval = 1000`. No drift concerns at that resolution.
* **PDF render for preview** is the only potentially-slow operation. Run it on a background `Task.Run`; bind `IsBusy` to a progress overlay.

---

## 17. Known Avalonia limitations / workarounds

| Limitation | Workaround |
|---|---|
| `TreeView.SelectedItem` is read-only binding source. | Wire via code-behind `SelectionChanged` to VM (≤5 lines, allowed exception). |
| No built-in `GroupBox`. | Use `HeaderedContentControl` + style. |
| No native `PrintDialog`. | Render PDF and open via OS, or use `Process.Start` `print` verb on Windows. |
| `DataGrid` is in a separate package (`Avalonia.Controls.DataGrid`). | Add the package; theme integration with Fluent is built in. |
| `NumericUpDown` doesn't auto-blank on focus. | Identical to WinForms, no action. |
| `ContextMenuStrip` ⇒ `MenuFlyout`: there's no per-item ShortcutKeys property. | Display shortcut in the `MenuItem.InputGesture` and bind the key globally via `KeyBinding`. |
| `FormClosing` cancel-with-prompt: Avalonia `Window.Closing` runs synchronously by default. | Use `WindowClosingEventArgs.Cancel` synchronously when needed, or `OnClosingAsync` via Avalonia 11.1+ async closing. |
| `System.Drawing.Bitmap` ⇄ `Avalonia.Media.Imaging.Bitmap`. | Custom converter via `MemoryStream` round-trip; cached per VM. |
| Embedded resource names differ (`avares://` URI). | Update `Reader.GetManifestResourceStream` call sites in `LicenseDialog` to use `AssetLoader.Open`. |
| No `BackgroundWorker`. | Use `Task.Run` + `Dispatcher.UIThread.Post` (none needed in current code, but worth noting). |
| No `DragDrop` parity in `TreeView`. | Not used in the current app, no action. If added later, Avalonia has `DragDrop.AllowDrop` + `DragDrop.DropEvent`. |

---

## 18. Anti-patterns to avoid during migration

1. **"Just call `View.Find<Control>()` from the VM."** No. The VM never touches the visual tree.
2. **Mirroring `Designer.cs` line-for-line.** XAML is declarative; resist the urge to set `Margin = new Thickness(2, 3, 2, 3)` for every control. Use styles.
3. **Re-introducing WinForms message pump.** No `Application.DoEvents()`. Avalonia is async-friendly; `await` it.
4. **Static UI helpers like `DialogManager`.** The current `Simulator/Utilities/DialogManager.cs` is a procedural cul-de-sac that takes a `DataGridView` as input and mutates it. Replace with `INavigationService` + view model state.
5. **`Visible = !Visible` flag soup.** Use a single state enum + bindings (see §6.2).
6. **`Controls.Add(new XControl())` in code.** Use `ItemsControl` with `DataTemplate`.
7. **`MessageBox.Show` from a VM.** Always go through `IMessageBoxService`.
8. **Hard-coded coordinates.** Replace `Location = new Point(2, 2 + i*36)` with `StackPanel` + spacing.
9. **`Application.Exit()` from a VM.** Raise an `ExitRequested` event and let the host shut down; or call `IClassicDesktopStyleApplicationLifetime.Shutdown()` via a service.
10. **Suppressing recursive events with a flag (the current `Disconnect/ReconnectHandlers` trick).** Set the VM property; binding will not re-enter. If you need to skip side effects, use an `IsLoading` flag *in the VM* — not in the view.

---

## 19. Recommended Avalonia packages (final list)

| Package | Version line | Use |
|---|---|---|
| `Avalonia` | 11.2.x | core |
| `Avalonia.Desktop` | 11.2.x | desktop runtime |
| `Avalonia.Themes.Fluent` | 11.2.x | theme |
| `Avalonia.Fonts.Inter` | 11.2.x | bundled font (optional) |
| `Avalonia.Controls.DataGrid` | 11.2.x | DataGrid replacement |
| `Avalonia.Diagnostics` | 11.2.x | F12 devtools (Debug only) |
| `CommunityToolkit.Mvvm` | 8.4.x | source-gen observable / commands |
| `Microsoft.Extensions.DependencyInjection` | already on `10.0.7` | DI |
| `MessageBox.Avalonia` | 3.2.x | MessageBox replacement |
| `LiveChartsCore.SkiaSharpView.Avalonia` | 2.0.0-rc5.x | Chart replacement |
| `PDFtoImage` (optional) | 4.x | PDF first-page rasterisation for preview |
| `Avalonia.Headless.XUnit` | 11.2.x | headless tests |

(`PdfSharp` and `protobuf-net` are already in `Directory.Packages.props`; reused as-is.)

---

## 20. Concrete code examples

### 20.1 `MainWindowViewModel` for Creator (excerpt)

```csharp
public sealed partial class MainWindowViewModel : ObservableObject
{
    private readonly IFilePickerService     _files;
    private readonly IMessageBoxService     _msg;
    private readonly IExamHistoryService    _history;
    private readonly IUndoRedoService       _undo;
    private readonly IPrintService          _print;
    private readonly Func<SplashPaneViewModel>           _splashFactory;
    private readonly Func<ExamPropertiesPaneViewModel>   _propsFactory;
    private readonly Func<QuestionEditorPaneViewModel>   _editorFactory;

    [ObservableProperty] private Exam? _exam;
    [ObservableProperty] private string? _currentExamFile;
    [ObservableProperty] private bool _isDirty;
    [ObservableProperty] private NodeViewModel? _selectedNode;
    [ObservableProperty] private IRightPaneViewModel _currentRightPane = null!;
    public ObservableCollection<NodeViewModel> Nodes { get; } = new();

    public MainWindowViewModel(/* DI */)
    {
        // ...
        CurrentRightPane = _splashFactory();
    }

    partial void OnSelectedNodeChanged(NodeViewModel? value)
    {
        if (value is QuestionNodeViewModel q && CurrentRightPane is QuestionEditorPaneViewModel current)
            current.CommitToModel();   // mirrors CommitQuestion()

        CurrentRightPane = value switch
        {
            null                           => _splashFactory(),
            ExamNodeViewModel e            => _propsFactory().Initialize(e),
            SectionNodeViewModel s         => _editorFactory().InitializeFromSection(s),
            QuestionNodeViewModel question => _editorFactory().InitializeFromQuestion(question),
            _ => CurrentRightPane,
        };

        NewQuestionCommand.NotifyCanExecuteChanged();
    }

    [RelayCommand]
    private async Task NewAsync()
    {
        if (!await ConfirmDiscardChangesAsync()) return;
        Exam = new Exam();
        Nodes.Clear();
        _undo.Reset();
        CurrentRightPane = _propsFactory().Initialize(new ExamNodeViewModel(Exam.Properties));
    }

    [RelayCommand]
    private async Task OpenAsync()
    {
        var path = await _files.PickOpenFileAsync([new FileFilter("OEF Files", "*.oef")]);
        if (path is null) return;
        await OpenAsync(path);
    }

    public async Task OpenAsync(string path)
    {
        var load = ExamFileLoader.TryLoad(path);
        if (!string.IsNullOrEmpty(load.ErrorMessage))
        {
            await _msg.ShowAsync(load.ErrorMessage, "Error", MessageBoxButton.OK, MessageBoxIcon.Error);
            return;
        }
        if (!load.Success || load.Exam is null)
        {
            await _msg.ShowAsync(
                "Sorry, the exam selected is either old or corrupt. ...",
                "Error", MessageBoxButton.OK, MessageBoxIcon.Error);
            return;
        }
        Exam = load.Exam;
        CurrentExamFile = Path.GetExtension(path)?.ToLowerInvariant() is ".json" or ".xml" ? null : path;
        RebuildTreeFromExam();
        _history.Add(load.PathForHistory!);
    }

    [RelayCommand(CanExecute = nameof(CanSave))]
    private async Task SaveAsync() { /* mirrors WinForms Save */ }
    private bool CanSave => Exam is not null;

    [RelayCommand]
    private async Task SaveAsAsync() { /* SaveFileDialog → Save */ }

    [RelayCommand(CanExecute = nameof(CanUndo))]   private void Undo() => _undo.Undo()?.Apply(this);
    [RelayCommand(CanExecute = nameof(CanRedo))]   private void Redo() => _undo.Redo()?.Apply(this);
    private bool CanUndo => _undo.CanUndo;
    private bool CanRedo => _undo.CanRedo;
}
```

### 20.2 `MainWindow.axaml` for Creator (skeleton)

```xml
<Window xmlns="https://github.com/avaloniaui"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:vm="using:OpenExamSuite.Creator.ViewModels"
        xmlns:p="using:OpenExamSuite.Creator.ViewModels.Panes"
        xmlns:v="using:OpenExamSuite.Creator.Views.Panes"
        x:Class="OpenExamSuite.Creator.Views.MainWindow"
        x:DataType="vm:MainWindowViewModel"
        Title="Creator"
        Width="1031" Height="760" MinWidth="870" MinHeight="564"
        WindowStartupLocation="CenterScreen">

    <Window.Resources>
        <DataTemplate DataType="p:SplashPaneViewModel">         <v:SplashPaneView/>         </DataTemplate>
        <DataTemplate DataType="p:ExamPropertiesPaneViewModel"> <v:ExamPropertiesPaneView/> </DataTemplate>
        <DataTemplate DataType="p:QuestionEditorPaneViewModel"> <v:QuestionEditorPaneView/> </DataTemplate>
    </Window.Resources>

    <Window.KeyBindings>
        <KeyBinding Gesture="Ctrl+N" Command="{Binding NewCommand}"/>
        <KeyBinding Gesture="Ctrl+O" Command="{Binding OpenCommand}"/>
        <KeyBinding Gesture="Ctrl+S" Command="{Binding SaveCommand}"/>
        <KeyBinding Gesture="Ctrl+P" Command="{Binding PrintCommand}"/>
        <KeyBinding Gesture="Ctrl+Z" Command="{Binding UndoCommand}"/>
        <KeyBinding Gesture="Ctrl+Y" Command="{Binding RedoCommand}"/>
        <KeyBinding Gesture="Ctrl+X" Command="{Binding CutCommand}"/>
        <KeyBinding Gesture="Ctrl+C" Command="{Binding CopyCommand}"/>
        <KeyBinding Gesture="Ctrl+V" Command="{Binding PasteCommand}"/>
    </Window.KeyBindings>

    <DockPanel>
        <Menu DockPanel.Dock="Top">
            <MenuItem Header="_File">
                <MenuItem Header="_New"      Command="{Binding NewCommand}"      InputGesture="Ctrl+N"/>
                <MenuItem Header="_Open..."  Command="{Binding OpenCommand}"     InputGesture="Ctrl+O"/>
                <Separator/>
                <MenuItem Header="_Save"     Command="{Binding SaveCommand}"     InputGesture="Ctrl+S"/>
                <MenuItem Header="Save _As..." Command="{Binding SaveAsCommand}"/>
                <Separator/>
                <MenuItem Header="_Import">
                    <MenuItem Header="From _JSON..." Command="{Binding ImportFromJsonCommand}"/>
                </MenuItem>
                <MenuItem Header="_Export">
                    <MenuItem Header="As _JSON..." Command="{Binding ExportAsJsonCommand}"/>
                    <MenuItem Header="As _XML..."  Command="{Binding ExportAsXmlCommand}"/>
                    <MenuItem Header="As _PDF..."  Command="{Binding ExportAsPdfCommand}"/>
                </MenuItem>
                <Separator/>
                <MenuItem Header="_Print..."         Command="{Binding PrintCommand}"        InputGesture="Ctrl+P"/>
                <MenuItem Header="Print Pre_view..." Command="{Binding PrintPreviewCommand}"/>
                <Separator/>
                <MenuItem Header="_Close" Command="{Binding CloseCommand}"/>
                <Separator/>
                <MenuItem Header="E_xit"  Command="{Binding ExitCommand}"/>
            </MenuItem>
            <MenuItem Header="_Edit">
                <MenuItem Header="_Undo"  Command="{Binding UndoCommand}"  InputGesture="Ctrl+Z"/>
                <MenuItem Header="_Redo"  Command="{Binding RedoCommand}"  InputGesture="Ctrl+Y"/>
                <Separator/>
                <MenuItem Header="Cu_t"   Command="{Binding CutCommand}"   InputGesture="Ctrl+X"/>
                <MenuItem Header="_Copy"  Command="{Binding CopyCommand}"  InputGesture="Ctrl+C"/>
                <MenuItem Header="_Paste" Command="{Binding PasteCommand}" InputGesture="Ctrl+V"/>
            </MenuItem>
            <MenuItem Header="_Help">
                <MenuItem Header="_About"   Command="{Binding AboutCommand}"/>
                <MenuItem Header="_License" Command="{Binding LicenseCommand}"/>
            </MenuItem>
        </Menu>

        <StackPanel DockPanel.Dock="Top" Orientation="Horizontal" Classes="toolbar">
            <Button Command="{Binding NewCommand}"          ToolTip.Tip="New (Ctrl+N)">    <PathIcon Data="{StaticResource NewIcon}"/></Button>
            <Button Command="{Binding OpenCommand}"         ToolTip.Tip="Open (Ctrl+O)">   <PathIcon Data="{StaticResource OpenIcon}"/></Button>
            <Button Command="{Binding SaveCommand}"         ToolTip.Tip="Save (Ctrl+S)">   <PathIcon Data="{StaticResource SaveIcon}"/></Button>
            <Button Command="{Binding PrintCommand}"        ToolTip.Tip="Print (Ctrl+P)">  <PathIcon Data="{StaticResource PrintIcon}"/></Button>
            <Separator/>
            <Button Command="{Binding NewSectionCommand}"   ToolTip.Tip="New Section"><PathIcon Data="{StaticResource SectionIcon}"/></Button>
            <Button Command="{Binding NewQuestionCommand}"  ToolTip.Tip="New Question"><PathIcon Data="{StaticResource QuestionIcon}"/></Button>
            <Separator/>
            <Button Command="{Binding CutCommand}"   ToolTip.Tip="Cut">  <PathIcon Data="{StaticResource CutIcon}"/></Button>
            <Button Command="{Binding CopyCommand}"  ToolTip.Tip="Copy"> <PathIcon Data="{StaticResource CopyIcon}"/></Button>
            <Button Command="{Binding PasteCommand}" ToolTip.Tip="Paste"><PathIcon Data="{StaticResource PasteIcon}"/></Button>
            <Separator/>
            <Button Command="{Binding HelpCommand}" ToolTip.Tip="Help"><PathIcon Data="{StaticResource HelpIcon}"/></Button>
        </StackPanel>

        <Grid ColumnDefinitions="294,5,*">
            <HeaderedContentControl Grid.Column="0" Header="Exam Explorer">
                <TreeView ItemsSource="{Binding Nodes}"
                          SelectionChanged="OnTreeSelectionChanged">
                    <TreeView.DataTemplates>
                        <TreeDataTemplate DataType="vm:ExamNodeViewModel"     ItemsSource="{Binding Children}">
                            <StackPanel Orientation="Horizontal" Spacing="4">
                                <Image Source="{StaticResource ExamIconImage}" Width="16" Height="16"/>
                                <TextBlock Text="{Binding Title}"/>
                            </StackPanel>
                        </TreeDataTemplate>
                        <TreeDataTemplate DataType="vm:SectionNodeViewModel"  ItemsSource="{Binding Children}">
                            <StackPanel Orientation="Horizontal" Spacing="4">
                                <Image Source="{StaticResource SectionIconImage}" Width="16" Height="16"/>
                                <TextBlock Text="{Binding Title}"/>
                            </StackPanel>
                        </TreeDataTemplate>
                        <TreeDataTemplate DataType="vm:QuestionNodeViewModel">
                            <StackPanel Orientation="Horizontal" Spacing="4">
                                <Image Source="{StaticResource QuestionIconImage}" Width="16" Height="16"/>
                                <TextBlock Text="{Binding DisplayName}"/>
                            </StackPanel>
                        </TreeDataTemplate>
                    </TreeView.DataTemplates>
                </TreeView>
            </HeaderedContentControl>

            <GridSplitter Grid.Column="1"/>

            <ContentControl Grid.Column="2" Content="{Binding CurrentRightPane}"/>
        </Grid>
    </DockPanel>
</Window>
```

### 20.3 `ExamSettingsViewModel` and view (full)

```csharp
public sealed partial class ExamSettingsViewModel : ObservableObject, IRouteViewModel
{
    private readonly INavigationService _nav;
    private readonly IMessageBoxService _msg;

    public Exam Exam { get; }
    public ObservableCollection<SectionSelectionViewModel> Sections { get; }

    [ObservableProperty] private string _candidateName = string.Empty;
    [ObservableProperty] private bool _enableCustomTimer;
    [ObservableProperty] private decimal _customTimerMinutes;
    [ObservableProperty] private SelectionMode _mode = SelectionMode.SelectedSections;
    [ObservableProperty] private decimal _fixedNumberOfQuestions = 1;

    public int MaxQuestions => Exam.NumberOfQuestions;

    public ExamSettingsViewModel(Exam exam, INavigationService nav, IMessageBoxService msg)
    {
        Exam = exam;
        _nav = nav;
        _msg = msg;
        Sections = new(exam.Sections.Select(s => new SectionSelectionViewModel(s, isChecked: true)));
        CustomTimerMinutes = exam.Properties.TimeLimit;
    }

    [RelayCommand] private void SelectAll()   { foreach (var s in Sections) s.IsChecked = true;  }
    [RelayCommand] private void DeselectAll() { foreach (var s in Sections) s.IsChecked = false; }

    [RelayCommand]
    private async Task ProceedAsync()
    {
        var settings = new Settings { CandidateName = CandidateName };
        settings.TimeLimit = EnableCustomTimer ? (int)CustomTimerMinutes : Exam.Properties.TimeLimit;

        if (Mode == SelectionMode.SelectedSections)
        {
            settings.Sections = Sections.Where(s => s.IsChecked).Select(s => s.Section).ToList();
            foreach (var s in settings.Sections) settings.Questions.AddRange(s.Questions);
        }
        else if (Mode == SelectionMode.FixedNumberOfQuestions)
        {
            int target = (int)FixedNumberOfQuestions, sum = 0;
            foreach (var section in Exam.Sections)
            {
                if (sum + section.Questions.Count < target)
                { settings.Sections.Add(section); settings.Questions.AddRange(section.Questions); sum += section.Questions.Count; }
                else if (sum + section.Questions.Count == target)
                { settings.Sections.Add(section); settings.Questions.AddRange(section.Questions); break; }
                else
                { settings.Sections.Add(section); settings.Questions.AddRange(section.Questions.Take(target - sum)); break; }
            }
        }

        if (settings.Questions.Count == 0)
        {
            await _msg.ShowAsync(
                "There are no questions to be displayed based on your selection. Please make a different selection.",
                "Error", MessageBoxButton.OK, MessageBoxIcon.Error);
            return;
        }

        _nav.GoTo(new AssessmentViewModel(Exam, settings, /* services */));
    }

    [RelayCommand] private void Cancel() => _nav.GoBack();
}

public enum SelectionMode { AllSections, FixedNumberOfQuestions, SelectedSections }
```

```xml
<UserControl x:DataType="vm:ExamSettingsViewModel" ...>
    <Grid ColumnDefinitions="*,*" RowDefinitions="Auto,*,Auto">
        <HeaderedContentControl Header="Question Selection" Grid.Column="0" Grid.Row="0" Grid.RowSpan="2">
            <StackPanel Spacing="6">
                <RadioButton GroupName="Mode" Content="All sections"
                             IsChecked="{Binding Mode, Converter={x:Static c:EnumToBool.Conv}, ConverterParameter={x:Static vm:SelectionMode.AllSections}}"/>
                <RadioButton GroupName="Mode" Content="Fixed number of questions"
                             IsChecked="{Binding Mode, Converter={x:Static c:EnumToBool.Conv}, ConverterParameter={x:Static vm:SelectionMode.FixedNumberOfQuestions}}"/>
                <NumericUpDown Value="{Binding FixedNumberOfQuestions}"
                               Minimum="1" Maximum="{Binding MaxQuestions}"
                               IsEnabled="{Binding Mode, Converter={x:Static c:EnumToBool.Conv}, ConverterParameter={x:Static vm:SelectionMode.FixedNumberOfQuestions}}"/>
                <RadioButton GroupName="Mode" Content="Selected sections"
                             IsChecked="{Binding Mode, Converter={x:Static c:EnumToBool.Conv}, ConverterParameter={x:Static vm:SelectionMode.SelectedSections}}"/>
                <ListBox ItemsSource="{Binding Sections}"
                         IsEnabled="{Binding Mode, Converter={x:Static c:EnumToBool.Conv}, ConverterParameter={x:Static vm:SelectionMode.SelectedSections}}">
                    <ListBox.ItemTemplate>
                        <DataTemplate>
                            <CheckBox Content="{Binding Title}" IsChecked="{Binding IsChecked}"/>
                        </DataTemplate>
                    </ListBox.ItemTemplate>
                </ListBox>
                <StackPanel Orientation="Horizontal" Spacing="6">
                    <Button Content="Select All"   Command="{Binding SelectAllCommand}"/>
                    <Button Content="Deselect All" Command="{Binding DeselectAllCommand}"/>
                </StackPanel>
            </StackPanel>
        </HeaderedContentControl>

        <HeaderedContentControl Header="Other Settings" Grid.Column="1" Grid.Row="0" Grid.RowSpan="2">
            <StackPanel Spacing="6">
                <TextBlock Text="Candidate Name"/>
                <TextBox Text="{Binding CandidateName}"/>
                <CheckBox Content="Enable custom timer" IsChecked="{Binding EnableCustomTimer}"/>
                <NumericUpDown Value="{Binding CustomTimerMinutes}"
                               Minimum="1" Maximum="600"
                               IsEnabled="{Binding EnableCustomTimer}"/>
            </StackPanel>
        </HeaderedContentControl>

        <StackPanel Grid.Column="0" Grid.ColumnSpan="2" Grid.Row="2"
                    Orientation="Horizontal" HorizontalAlignment="Right" Spacing="6">
            <Button Content="OK"     Command="{Binding ProceedCommand}" Classes="accent"/>
            <Button Content="Cancel" Command="{Binding CancelCommand}"/>
        </StackPanel>
    </Grid>
</UserControl>
```

### 20.4 `BitmapToAvaloniaImageConverter`

```csharp
public sealed class BitmapToAvaloniaImageConverter : IValueConverter
{
    public static readonly BitmapToAvaloniaImageConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not System.Drawing.Bitmap bmp) return null;
        using var ms = new MemoryStream();
        bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
        ms.Position = 0;
        return new Avalonia.Media.Imaging.Bitmap(ms);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
```

### 20.5 `Program.cs` for Avalonia Creator

```csharp
public static class Program
{
    [STAThread]
    public static int Main(string[] args)
        => BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);

    public static AppBuilder BuildAvaloniaApp() => AppBuilder.Configure<App>()
        .UsePlatformDetect()
        .WithInterFont()
        .LogToTrace();
}

public partial class App : Application
{
    public IServiceProvider Services { get; private set; } = null!;

    public override void Initialize() => AvaloniaXamlLoader.Load(this);

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var services = new ServiceCollection();
            services.AddCreatorServices();
            services.AddSingleton<MainWindow>();
            Services = services.BuildServiceProvider();

            var window = Services.GetRequiredService<MainWindow>();
            window.DataContext = Services.GetRequiredService<MainWindowViewModel>();
            desktop.MainWindow = window;
        }
        base.OnFrameworkInitializationCompleted();
    }
}
```

---

## 21. Developer workflow

1. **Day-to-day:** `dotnet watch --project src/Apps/Creator.Avalonia` for hot reload of XAML and code.
2. **Diagnostics:** F12 in Debug builds opens Avalonia DevTools (tree, styles, events) — equivalent to WinForms `Spy++`.
3. **Designer-time DataContext:** every view has a `d:DataContext="{x:Static vm:DesignData.MainWindow}"` providing dummy data so previews render. Stored in `DesignData.cs` per app.
4. **Hot-reload caveats:** structural XAML changes require a restart; property changes hot-swap.
5. **Branching:** feature-branch per phase; PR review focuses on (a) feature parity with WinForms and (b) absence of forbidden patterns (§18).
6. **Commit hygiene:** the WinForms removal must be a single, clearly-labelled cutover commit.
7. **CI matrix:** `ubuntu-latest` for build + unit tests + headless UI tests; `windows-latest` for build + the final WinExe artefact + smoke test that the EXE launches.

---

## 22. Cutover checklist

* [ ] All Phase 1–17 PRs merged.
* [ ] `dotnet test` green on both new test projects.
* [ ] Both apps launch on Windows 11 (manual).
* [ ] Both apps launch on Ubuntu 22.04 (manual, smoke).
* [ ] All keyboard shortcuts work (§2.3).
* [ ] All file dialog filters preserved (§2.4).
* [ ] All MessageBox calls preserved (text + buttons + icons identical).
* [ ] Round-trip: open every file in `samples/` and re-save without diff.
* [ ] Score sheet PDF golden test passes.
* [ ] Installer produces a working artefact.
* [ ] CHANGELOG entry, README update.
* [ ] WinForms projects deleted, `Shared.WinForms` deleted, solution file pruned.
* [ ] Git tag `v5.0.0` cut.

---

## 23. Summary

The migration is a finite, scoped, well-bounded rewrite of two `Form` classes and ~ten dialogs into Avalonia views + view models. Eighty percent of the work is mechanical translation (control-for-control, event-for-command). The remaining twenty percent is the three sanctioned redesigns — Creator's right pane, Simulator's assessment-phase toggling, and Simulator's inter-form workflow — all of which become cleaner, more testable, and identical-looking to the user. The non-UI libraries do not move. End users get the same product on Windows plus a path to Linux/macOS later, and the codebase gains MVVM, dependency injection of UI services, and headless-testable view models.
