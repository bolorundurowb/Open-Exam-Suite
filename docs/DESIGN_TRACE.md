# Design Trace: Mapping Design Brief to Creator & Simulator

**Status**: Complete  
**Source**: `DESIGN_BRIEF.md` (sections 4–6) + `Open Exam Suite.html` (hi-fi artboards)  
**Target Apps**: Creator (burgundy accent `#801638`→`#3B0A1A`), Simulator (teal accent `#017777`→`#013838`)

---

## 1. Product Name Mapping (Author → Creator, Library → Simulator Home)

| Design Brief Term | Actual Product | Window Title | Menu / Installer / Shortcuts | Accent |
|-------------------|----------------|--------------|------------------------------|--------|
| **Author** | **Creator** | "Open Exam Creator" | Creator | Burgundy `#801638` → `#3B0A1A` |
| **Library** | **Simulator Home** | "Open Exam Simulator" | Simulator | Teal `#017777` → `#013838` |
| **Practice / Exam** | **Simulator Screens** (modes, not apps) | — | — | Teal (same as Simulator) |

> **Note**: The brief's "one app with three modes" assumption is **rejected**. Two executables ship: `Creator.exe` and `Simulator.exe`. File association and the single-instance named pipe belong to Simulator.

---

## 2. Artboard → Session State / Component Mapping

### 2.1 Shared / Cross-Cutting

| Artboard (hi-fi) | Session State / Component | App | Notes |
|------------------|---------------------------|-----|-------|
| Style Sheet | `StyleSheet` (global resource dictionary) | Both | Colours, typography, elevation, density, focus rings |
| Dialog: Unsaved Changes | `UnsavedChangesDialog` | Creator | Modal: Save / Don't Save / **Cancel stops action** |
| Dialog: Abandon Exam | `AbandonExamDialog` | Simulator | "Abandon this exam?" — answers are not saved and no result is recorded. Actions: Keep going / Abandon |
| Dialog: Legacy File | `LegacyFileDialog` | Both | "This file will be upgraded on save" |
| Dialog: Corrupt File | `CorruptFileDialog` | Both | Clear message + next step, never a crash |
| Dialog: Crash Recovery | `CrashRecoveryDialog` | Creator | Offers autosaved recovery copy |
| Toast: Saved / Routine Confirmation | `ToastService.Show("Saved")` | Both | Routine confirmations are toasts, not modals |

### 2.2 Simulator Artboards

| Artboard (hi-fi) | Session State / Component | Key Behaviours |
|------------------|---------------------------|----------------|
| **Library – Light Theme** | `Simulator.Home.Light` | Grid/list toggle, exam cards, search/filter/sort, Add exam (drag-drop + New exam → launches Creator), Recent attempts strip, Empty state illustration |
| **Library – Dark Theme** | `Simulator.Home.Dark` | Same as light, colours inverted per style sheet |
| **Library – Empty** | `Simulator.Home.Empty` | Illustration + "Add or create your first exam" CTA (New exam → Creator) |
| **Pre-Exam Sheet** | `Simulator.PreExamSheet` | Mode selector (Practice/Exam), Candidate name (prefilled), Question set (All / Sections / Random N), Shuffle Q / Shuffle Options, Timer override (Exam only), Live summary, **Start disabled with inline explanation if zero questions** |
| **Exam View – Practice** | `Simulator.ExamView.Practice` | Untimed, Check Answer button, instant feedback, explanation panel, choice locked after check |
| **Exam View – Exam** | `Simulator.ExamView.Exam` | Timed, countdown (normal, low at 5 minutes remaining, critical at 1 minute remaining), no feedback until review, Pause cover hides question |
| **Exam View – Pause** | `Simulator.ExamView.Pause` | Calm "Paused" cover, timer stops, Resume button |
| **Exam View – Time Up** | `Simulator.ExamView.TimeUp` | Notice → Review & Submit → Auto-submit after "Submitting in 6 s" countdown |
| **Review & Submit** | `Simulator.ReviewSubmit` | Summary (answered/unanswered/flagged), clickable Unanswered/Flagged chips, full navigator, Back to exam / Submit. Confirmation when any remain: "You have 6 unanswered questions. Submit anyway?" |
| **Results** | `Simulator.Results` | Large Passed/Not Passed + icon, % + scaled/1000, single horizontal bar with pass mark, section breakdown (weakest flagged), actions: Review answers, Retake (→ pre-exam sheet with same selections, fresh random set if Random), Back to library, Export PDF / Print |
| **Answer Review** | `Simulator.AnswerReview` | Split view: filterable list (All/Wrong/Unanswered/Flagged/Correct) + detail (question, image, options with "Your answer"/"Correct answer" icons+labels, explanation), Next/Prev in filter, "Practice these again" shortcut |

### 2.3 Creator Artboards

| Artboard (hi-fi) | Session State / Component | Key Behaviours |
|------------------|---------------------------|----------------|
| **Workspace – Light Theme** | `Creator.Workspace.Light` | Three-pane: Outline (tree, drag-reorder, context menu Duplicate/Move to/Delete), Editor (Exam props / Section / Question), Live Preview (read-only, Practice & Exam style), Validation panel (problems listed, **Save never blocked**), Burgundy accent |
| **Workspace – Dark Theme** | `Creator.Workspace.Dark` | Same as light, colours inverted per style sheet |
| **Section Context Menu** | `Creator.SectionContextMenu` | Duplicate, Move to, Delete |
| **New Exam Properties** | `Creator.ExamProperties` (part of Editor when Exam node selected) | Title, Code, Instructions, Pass mark (% + scaled), Time limit, **Hide Answers** toggle |
| **Question Editor** | `Creator.QuestionEditor` | Text, optional image (add/replace/remove, original bytes kept), Single/Multiple toggle, Options list (letter, text, correct radio/checkbox), Add option, Drag reorder, Delete per option, Explanation field |

---

## 3. HideAnswers Behaviour (Single Source of Truth)

The `Exam.Properties.HideAnswers` flag (author-set in Creator) governs four surfaces:

| Surface | Behaviour when `HideAnswers == true` |
|---------|--------------------------------------|
| **Pre-Exam Sheet** (Simulator) | Practice mode **disabled**; inline explanation: "The author has hidden answers for this exam. Practice mode is unavailable." |
| **Practice** (Simulator) | Not offered. The pre-exam sheet disables Practice, so Check answer is never shown |
| **Answer Review** (Simulator) | Correct answer **withheld** (show "Correct answer: [hidden]"), explanation **withheld** (show "Explanation: [hidden]") |
| **PDF Export** (Creator) | Answers and explanations **omitted** from exported PDF |

> Implementation note: the flag travels with the `.oef` file. Simulator reads it at load; Creator honours it on export.

---

## 4. Key Decisions Preserved in This Trace

| Decision | Recorded Here |
|----------|---------------|
| Two executables; file association + single-instance pipe → Simulator | §1 |
| Results: one horizontal bar, pass mark marked, % + scaled/1000, Passed/Not Passed text+icon, colour never sole signal | §2.2 Results |
| Retake → pre-exam sheet with same selections; Random draws fresh set | §2.2 Results |
| Practice Check Answer locks question, reveals explanation; Exam reveals nothing until review | §2.2 Exam View |
| Countdown states: normal, then low from 5 minutes remaining, then critical from 1 minute remaining; Time-up → review → auto-submit after short countdown | §2.2 Exam View, Time Up |
| Pre-exam sheet: shuffle question order, shuffle option order | §2.2 Pre-Exam Sheet |
| Validation does **not** block Save; problems: no correct answer, <2 options, empty question text, duplicate section name | §2.3 Workspace, Validation panel |
| Creator autosaves recovery copy; offers it after crash | §2.1 Crash Recovery Dialog |
| HideAnswers covers Practice + PDF export; disables Practice on pre-exam sheet; withholds answers/explanations on review | §3 |
| IBM Plex Sans (UI), IBM Plex Mono (countdown, scores) | Style Sheet |
| Light/dark follow OS, manual override | Style Sheet |
| Minimum window ~960×640; layouts hold at 200% scaling, text +30% longer than English | Style Sheet |
| WCAG AA contrast both themes | Style Sheet |
| Routine confirmations → toasts; modals only for decisions | §2.1 Toasts |

---

## 5. Key Flows → Session State Transitions

| Flow (Brief §6) | Session State Sequence |
|-----------------|------------------------|
| **1. First Launch** | `Simulator.Home.Light` (with samples) → `Simulator.PreExamSheet` (Practice) → `Simulator.ExamView.Practice` (×N, Check Answer) → `Simulator.Results` → `Simulator.AnswerReview` |
| **2. Timed Exam** | `Simulator.Home` → `Simulator.PreExamSheet` (Exam, Random 20) → `Simulator.ExamView.Exam` (flag 2, skip 1, low-time warning) → `Simulator.ReviewSubmit` (unanswered confirmation) → `Simulator.Results` → `Simulator.AnswerReview` (Wrong filter) → `Simulator.PreExamSheet` (Retake, same selections) |
| **3. Pause & Time Up** | `Simulator.ExamView.Exam` → `Simulator.ExamView.Pause` (Resume) → `Simulator.ExamView.TimeUp` → `Simulator.ReviewSubmit` → auto-submit |
| **4. Create Exam** | `Creator.Workspace` (New) → `Creator.ExamProperties` → `Creator.SectionContextMenu` (Add Section) → `Creator.QuestionEditor` (multi-answer + image) → Validation panel catches missing correct answer → Live Preview → Save → `Simulator.ExamView.Practice` (Try this exam) → back to `Creator.Workspace` |
| **5. Edit & Recover** | `Creator.Workspace` (Open existing) → drag-reorder → Undo → Close with unsaved (Cancel stays) → Simulated crash → `CrashRecoveryDialog` → recover → `Creator.Workspace` |

---

## 6. Component Inventory (from Brief §4.53–63)

| Component | Status | Owner App |
|-----------|--------|-----------|
| Exam Card | Defined in Style Sheet | Simulator |
| Primary / Secondary / Quiet Buttons | Defined in Style Sheet | Both |
| Option Row (radio/checkbox) – 8 states | Defined in Style Sheet | Both |
| Question Navigator Cell – 4 states | Defined in Style Sheet | Simulator |
| Countdown Timer – 3 states | Defined in Style Sheet | Simulator |
| Section Chip, Progress Bar, Status Banner, Toast | Defined in Style Sheet | Both |
| Outline Tree Row (drag handle, context menu) | Defined in Style Sheet | Creator |
| Inline Validation Message | Defined in Style Sheet | Creator |
| Empty State, Confirmation Dialog | Defined in Style Sheet | Both |

---

## 7. Exit Checklist (from Issue)

- [x] Hi-fi HTML stored in repository as `docs/Open Exam Suite.html`
- [x] Every artboard mapped to a named session state or component; Author artboards recorded as Creator
- [x] HideAnswers behaviour written for pre-exam sheet, Practice, review, and PDF export
- [x] No second flow document contradicts the brief's screens or the product-name mapping above

---

## 8. File References

| File | Purpose |
|------|---------|
| `docs/DESIGN_BRIEF.md` | Source of truth for screens, flows, decisions |
| `docs/Open Exam Suite.html` | Hi-fi artboards (copied from Downloads) |
| `docs/DESIGN_TRACE.md` | This document – the trace |
| `docs/BROWSER_HOST.md` | Browser host contracts and host ports |
| `src/Apps/Simulator/` | Simulator implementation (Avalonia) |
| `src/Apps/Creator/` | Creator implementation (Avalonia) |
| `src/Libraries/Core/Models/Settings.cs` | `HideAnswers` property on `Exam.Properties` |
| `src/Libraries/Core/Services/ExamEditor.cs` | Validation logic (non-blocking) |
| `src/Libraries/ExamIO/Utilities/Writer.cs` | PDF export (honours `HideAnswers`) |

---

*End of trace. This document is the authoritative mapping for Phase 4 and all subsequent Avalonia migration phases.*
