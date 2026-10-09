# Changelog

All notable changes to this project will be documented in this file.

## [Unreleased]
### Simulator Section
- Rebuilt the Simulator on Avalonia so it runs on Windows, macOS and Linux, with light and dark themes that follow the operating system and can be overridden from the top bar. The window title is always "Simulator".
- New library home with grid and list cards, search, filter, sort, drag-and-drop of `.oef` files, JSON and XML import, sample exams on first launch, a recent attempts strip, and clear states for missing, corrupt and older-format files.
- Added a pre-exam sheet, a question navigator with flagging, a pause cover that hides the question, 5-minute and 1-minute warnings that are also announced to screen readers, and a time-up countdown that submits automatically.
- Added a review-and-submit screen that lists unanswered and flagged questions and asks "Submit anyway?" before submitting with questions unanswered.
- Results now show Passed or Not passed with an icon, one score bar with the pass mark, the previous attempt and a section breakdown with the weakest section flagged, and can be exported to PDF or printed.
- Added answer review with All, Wrong, Unanswered, Flagged and Correct filters and a "Practice these again" shortcut. Exams that hide answers withhold the correct answer and explanation.
- Keyboard shortcuts: A-Z select an answer, Left and Right move, F flags, P pauses, Enter goes to the next question, Ctrl+Enter opens review.
- Opening an `.oef` file while the Simulator is already running now opens it in the running window instead of being dropped.
- Help now has About, License and Changelog. The changelog is shown once after each version change.
- Fixed the sample exams path, a skipped question being counted as answered, Retake only closing the sheet, and the exam being readable while paused.
- Pass marks are now shown as percentages instead of the stored 0 to 1000 scale.

## [4.0.5]
### General improvements
- Bundled the changelog into the product and display it on first startup after a version change. It is also available from the **Help > Changelog** menu in both the Creator and Simulator.
- Unified application settings storage into a single key/value model.

### Creator Section
- Fixed an issue where creating a new exam after opening an existing one could overwrite the previously opened exam when saving.
- Question images now maintain their aspect ratio and scale with the window instead of being stretched.

### Simulator Section
- Fixed a crash that occurred when ending an exam immediately after it began.
- Ending an exam now stops the exam timer, preventing duplicate grading.
- Question images now maintain their aspect ratio and scale with the window instead of being stretched.

## [4.0.4]
### General improvements
- Fixed CI build.
- Replaced `Shouldly` test assertions with `OmniAssert`.
- Updated the PolyInstall manifest to make `Creator` and `Samples` optional selectable components and added `.oef` file association.
- Updated PolyInstall to v2.0.0 and simplified the release workflow using a reusable action.

## [4.0.3]
### General improvements
- Added a YAML-based installer definition using PolyInstall, replacing Inno Setup in the release workflow.
- Updated package dependencies.

## [4.0.2]
### General improvements
- Added support for portable distributions.
- Fixed exam creation in locales that use commas as decimal separators.

### Simulator Section
- Added support for hiding the "Show Answer" option during exams.

## [4.0.1]
### General improvements
- Replaced `Newtonsoft.Json` with `System.Text.Json` and refactored the project into modular components (`Core`, `ExamIO`, `Shared.WinForms`).
- Added dependency injection for application services.
- Updated application icons.
- Added a License dialog to the Help menu.

### Creator Section
- Added support for importing questions from `JSON`.

## [4.0.0]
### General improvements
- Support for questions with multiple answers has been added.
- Modernised from .NET Framework 4.0 to **.NET 10**.
- Target Framework: Upgraded all projects to `net10.0-windows`.
- Project System: Migrated from legacy `.csproj` to modern **SDK-style** project files.
- Dependency Management: Converted `packages.config` to `PackageReference`.
- CI/CD: Migrated from AppVeyor to **GitHub Actions**.
- Serialization: Replaced obsolete `BinaryFormatter` with `Newtonsoft.Json` for `.oef` files to ensure compatibility with modern .NET runtimes.
- Testing: Updated unit tests to use `xUnit 2.9.2` and ensured they pass on .NET 10.

### Creator Section
- Support for exporting exams as `JSON` has been added.
- Support for exporting exams as `XML` has been added.

## [3.0.0]
### General improvements
- Coded using modern OOP principles.
- Developed using Test-Driven Development (TDD).
- Exam file (`.oef`) format changed from ZIP-based to a serializable binary (now JSON in 3.1).
- Added a converter to upgrade old exam files (v1/v2) to the v3 format.

### Creator Section
- UI overhaul for improved intuition and usability.
- Faster response times.
- Support for **Undo** and **Redo**.
- Support for **Copy**, **Cut**, and **Paste**.
- Integrated documentation in the help section.

### Simulator Section
- Full rewrite for improved stability.

## [2.0.0]
### Creator Section
- Added support for adding explanations to questions.
- Improved OOP structure compared to v1.0.
- Performance improvements and bug fixes.
- New, more stable and intuitive UI.

### Simulator Section
- Added support for checking the correct answer while taking exams.
- Added support for viewing answer explanations.
- Added support for printing exam results.

## [1.0.0]
### Creator Section
- Support for grouping questions into sections.
- Support for images in questions.
- Support for an unlimited number of options per question.
- Support for importing and editing existing exam files.
- Support for setting time limits in exams.

### Simulator Section
- Support for taking exams as designed.
- Support for selecting specific sections to take.
- Support for selecting the number of questions to be taken.
- Support for changing time limits during simulation.
