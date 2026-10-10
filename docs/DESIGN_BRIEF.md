# Open Exam Suite: Design Brief

## 1. What we are designing

Open Exam Suite is a free, open-source desktop app for creating and taking computer-based practice exams, a free alternative to Avanset's Visual CertExam Suite. It is being rebuilt from two Windows-only WinForms apps (Creator and Simulator) onto Avalonia UI. The new app runs on Windows, macOS and Linux.

Design a single desktop application with three modes:

- **Library**: the home screen where exams are found, started and managed.
- **Practice / Exam**: taking an exam (replaces the Simulator).
- **Author**: building and editing exams (replaces the Creator).

Deliverables requested: high-fidelity screens for every screen in section 5, in light and dark theme, plus a small component and style sheet (section 4) and the key states listed under each screen.

## 2. Users and goals

- **Test-taker** (main user): a student or professional preparing for a certification exam. Wants to drill questions, simulate real exam conditions, and learn from mistakes. Often anxious and time-pressed.
- **Author**: usually the same person, building practice exams from study material. Wants fast, forgiving entry of many questions, with images and explanations, and confidence that nothing gets lost.
- **Instructor** (occasional): builds an exam, hands the file out, and may want learners to see a take-only experience.

Product goals, in order:

1. Taking an exam must feel trustworthy and calm. Nothing irreversible happens without confirmation.
2. Learning comes from reviewing mistakes, so results must lead straight into review.
3. Authoring many questions must be fast, and work is never silently lost.
4. One coherent app, not two apps that happen to share a file format.

## 3. Design principles

1. **Calm under pressure.** The exam view is quiet, high-contrast and uncluttered. Only the question, the options, the clock and the navigator are prominent.
2. **No irreversible surprises.** Submitting, abandoning, deleting and replacing always give a clear summary and a way back.
3. **Show state, never hide it.** Answered, flagged, unanswered, correct and wrong are always visible, using colour plus an icon plus text. Colour is never the only signal.
4. **Practice and Exam are different modes, chosen by the taker.** Practice is untimed, with instant feedback. Exam is timed, with feedback only at the end.
5. **Content first.** Long question text, long option text, and large images must always lay out well. Text wraps, panes scroll, nothing overlaps or clips.
6. **Desktop-native, keyboard-first.** Everything is reachable by keyboard. Shortcuts are shown in the UI.

## 4. Visual direction and component set

**Identity.** The current icons give two brand accents. Use them as mode accents on a neutral base:

- Author mode accent: burgundy, gradient `#801638` to `#3B0A1A`.
- Practice / Exam mode accent: teal, gradient `#017777` to `#013838`.
- Neutral greys for surfaces and text. Use the accent for primary actions, selection and the mode indicator in the title area.

**Semantic colours** (always paired with an icon and a label): correct, wrong, flagged, unanswered, warning for low time, and pass/fail.

**Themes.** Light and dark, both fully designed. Follow the OS setting by default, with a manual override.

**Type.** A modern system-friendly sans (Inter or the platform UI font). A clear scale: display for the result, title, body, caption, and a monospaced or tabular style for the countdown and scores.

**Density.** Comfortable by default. The exam view and the author outline can use the compact spacing step.

**Components to define:**

- Exam card
- Primary / secondary / quiet buttons
- Option row, single-answer (radio) and multi-answer (checkbox), in default, selected, hover, focus, disabled, correct, wrong and missed states
- Question navigator cell (unanswered, answered, flagged, current)
- Countdown timer with normal, low (5 min) and critical (1 min) states
- Section chip, progress bar, status banner and toast
- Outline tree row (exam, section, question) with a drag handle and a context menu
- Inline validation message
- Empty state, and confirmation dialog

**Window.** Resizable, with a sensible minimum size (about 960 x 640). The layout must hold at 200% scaling.

## 5. Screens and flows

### 5.0 Global shell

- A top bar with the app name, the current mode indicator (Library / Practice / Exam / Author) in the mode accent colour, and a theme toggle.
- A menu (File, Edit, Help) on Windows and Linux, and the native menu bar on macOS. Help includes About, License and Changelog (shown on first launch after an update).
- A non-blocking toast area for confirmations such as "Saved". Routine confirmations never use a modal dialog.

### 5.1 Library (home)

Purpose: find an exam, start it, or edit it.

- A grid or list of **exam cards** (view toggle). Each card shows title, exam code, question count, section count, time limit, pass mark, and last attempt (score and date) with a small pass/fail marker.
- Each card has two primary actions: **Practice** and **Exam**, plus an **Edit** action and an overflow menu (Properties, Duplicate, Show in folder, Remove from library). Double-clicking a card opens the pre-exam sheet.
- Search and filter at the top. Sort by recent, name, or last score.
- An **Add exam** button and a drag-and-drop area. Dropping an `.oef` file adds it. JSON and XML files can be imported. A **New exam** button starts Author mode.
- First run: two sample exams (Basic Science, GMAT Sample) are shown, with a friendly welcome strip.
- Recent attempts: a collapsible strip, or a panel on the right, listing attempts with score, date and a link to its results.

States: empty library (illustration plus a clear "Add or create your first exam"), file missing (a card marked unavailable with "Locate file" and "Remove"), file corrupt or legacy (a clear message with a next step, never a crash), loading.

### 5.2 Pre-exam sheet (replaces the settings dialog and the intro screen)

One screen or side panel with everything needed to start. Shown after pressing Practice or Exam on a card.

- Exam title, code, and the author's **instructions**.
- Mode selector: **Practice** (untimed, instant feedback) or **Exam** (timed, feedback at the end). If the author has locked answers, Practice is disabled with an explanation.
- Candidate name. It is pre-filled from the last use, and never a placeholder that must be deleted.
- Question set, as a segmented choice:
  - All questions.
  - Selected sections: a checklist with Select all / Clear, each section showing its question count.
  - A random set of N questions: a stepper limited to the maximum available.
- Options: shuffle question order, shuffle options.
- Timer (Exam mode only): the exam's default is shown, with an override field in minutes.
- A live summary line, for example "60 questions, 90 minutes, pass mark 70%".
- **Start** (primary) and Cancel. If the selection would give zero questions, Start is disabled with an inline explanation instead of an error dialog after the click.

### 5.3 Exam view (the main experience)

Layout, left to right on a wide window:

- **Main pane**: section name, question number ("Question 12 of 60"), the question text, an optional image shown only when the question has one (scales to fit, click to enlarge), and the answer options. Single-answer questions use radio rows, and multiple-answer questions use checkbox rows with a clear "Select all that apply" or "Select 2" hint. Option rows are full-width, wrap long text, and have a large hit area.
- **Right pane**: the **question navigator**, a grid of numbered cells grouped by section, showing answered, unanswered, flagged and current. Clicking a cell jumps to that question. This pane collapses on narrow windows.
- **Top bar**: the countdown (Exam mode) with normal, low and critical states and a subtle non-intrusive warning at 5 minutes and 1 minute, and a progress indicator ("34 of 60 answered", which counts only questions with an actual answer).
- **Bottom bar**: Previous, Next, **Flag for review**, and Review & submit. Pause sits in the top bar.

Practice mode additions: a **Check answer** button reveals correct and wrong options with icon and text plus the explanation panel below the options. After checking, the choice is locked or clearly marked. Exam mode has no feedback.

**Pause.** The question and options are replaced by a calm "Paused" cover with a Resume button, so the question can't be read while paused. The timer stops.

**Time up.** A short, clear notice, then the review screen, then automatic submission after a few seconds.

Keyboard: A to Z select an option, Left and Right for previous and next, F to flag, P to pause, Enter for Next, Ctrl+Enter for Review & submit.

### 5.4 Review and submit (a new step)

Shown from "Review & submit", or automatically when time runs out.

- A summary: answered, unanswered, flagged.
- Two lists, **Unanswered** and **Flagged**, each a set of clickable question chips that jump back to the question and return here.
- The full navigator grid for reference.
- Actions: **Back to exam** (secondary) and **Submit exam** (primary, with a confirmation that states the unanswered count: "You have 6 unanswered questions. Submit anyway?").

### 5.5 Results

- A large, prominent **Passed** or **Not passed** with an icon, followed by the score as a percentage and as the scaled score out of 1000, with the pass mark marked on a single horizontal bar.
- Key facts: candidate, exam code, date, time used out of time allowed, questions correct out of total, plus the previous attempt as a comparison when one exists.
- **Section breakdown**: one row per section with a small bar, correct out of total, and the percentage. The weakest section is flagged.
- Primary actions: **Review answers** (leads to 5.6), **Retake** (returns to the pre-exam sheet with the same selections, or restarts a fresh set), **Back to library**. Secondary: **Export PDF report** and Print.
- The attempt is saved automatically to the library's history.

### 5.6 Answer review (a new screen)

- A split view: a filterable list of questions (All, Wrong, Unanswered, Flagged, Correct) on the left, and a question detail on the right.
- The detail shows the question, the image, every option with the taker's choice and the correct answer marked using icons plus labels ("Your answer", "Correct answer"), and the author's explanation.
- Next and previous within the current filter. A "Review missed questions" shortcut starts with the Wrong filter.
- Optional: "Practice these again", which starts a Practice session from the missed questions.

### 5.7 Author workspace

A three-pane master-detail layout, in the burgundy accent.

- **Left: outline.** A tree of the exam, its sections, and its questions. Each question row shows its number and the first line of its text (not just "Question 12"), a small icon for image or multi-answer, and a validation dot if it is incomplete. Supports search, drag-to-reorder, drag between sections, and a context menu (Duplicate, Move to, Delete). Buttons for Add section and Add question.
- **Centre: editor.** Selecting the exam shows **Exam properties**: title, code, instructions, pass mark (shown as a percentage with the scaled value alongside), time limit, a toggle "Hide answers in Practice and PDF export". Selecting a section shows its name and questions. Selecting a question shows the **question editor**: question text, optional image (add, replace, remove, with the original image bytes kept), a toggle between single answer and multiple answers, an options list where each row has a letter, text, and a "correct" control (radio or checkbox), an Add option control, drag to reorder options, and a delete control per option, and an explanation field.
- **Right: live preview.** A read-only rendering of the question exactly as the taker will see it, in both Practice and Exam style, updating as the author types.
- **Validation panel** (bottom or an inline strip): lists problems such as "no correct answer", "fewer than 2 options", "empty question text", and "duplicate section name", each linking to the item. Saving is always allowed, but problems are visible.
- **One Save** (Ctrl+S) with autosave and crash recovery, and a quiet "Saved" or "Unsaved changes" indicator in the title bar. Properties are part of the document, with no separate Save button. Undo and redo (Ctrl+Z, Ctrl+Y) are always available.
- Bulk tools: import questions from JSON, and export to JSON, XML and PDF. Printing goes through PDF.
- "Try this exam" runs the current exam in Practice mode and returns to the editor.

Unsaved changes: closing, New and Open show Save / Don't save / Cancel. **Cancel must stop the action.**

### 5.8 Dialogs and system screens

Properties summary (file path, size, version, counts), About, License, Changelog (rendered markdown, shown once per version), a file missing or corrupt message, and a "legacy file will be upgraded on save" notice. Use real dialogs only when a decision is needed.

## 6. Key flows to illustrate

1. **First launch**: Library with samples, then choose Practice on a sample, pre-exam sheet, a few questions with Check answer, then results, then answer review.
2. **Timed exam**: Library, Exam, pre-exam sheet with a random 20 questions, exam view (flag two, skip one, low-time warning), review and submit with the unanswered confirmation, results, answer review of wrong questions, retake.
3. **Pause and time up**: Pause cover, resume, then the time-up transition to review and auto-submit.
4. **Create an exam**: Library, New exam, properties, add a section, add a multiple-answer question with an image, validation catching a missing correct answer, live preview, Save, Try this exam.
5. **Edit and recover**: open an existing exam, reorder questions by dragging, undo, close with unsaved changes (Cancel stays put), reopen after a simulated crash with the recovery prompt.

## 7. Accessibility and quality bar

- Contrast meets WCAG AA in both themes. Focus is always visibly indicated.
- Full keyboard operation with a logical tab order, and all controls have accessible names. Screen reader announcements for timer warnings, question changes and check-answer results.
- Correct, wrong, flagged and warning states use an icon plus text, never colour alone.
- Layouts hold at 200% scaling and on a minimum window of about 960 x 640. Long text, 8+ options, large images and many sections must not break the layout. Show examples with such content.
- All user-visible strings are externalised, so allow for 30% longer text.

## 8. Constraints for the designer

- Desktop only (Windows, macOS, Linux) and built with Avalonia, so favour standard controls, flat or subtle elevation, and simple gradients. No effects that depend on a browser.
- Keep layouts structured so a future browser version can reuse the same flows.
- Exam data available to show: title, code, version, pass mark (scale of 1000), time limit, instructions; sections; questions with number, text, optional image, single or multiple choice, lettered options, correct answer(s), explanation; attempt history with score, date and time used.
- Assumption to confirm: this is one app with three modes. If the product ships a take-only build for learners, it is the same app with the Author mode hidden. No separate design is needed.
