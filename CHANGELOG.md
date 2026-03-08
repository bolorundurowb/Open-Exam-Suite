# Changelog

All notable changes to this project will be documented in this file.

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
