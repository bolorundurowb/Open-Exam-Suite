# Open Exam Suite

[![Build, Test & Coverage](https://github.com/bolorundurowb/Open-Exam-Suite/actions/workflows/build-and-test.yml/badge.svg)](https://github.com/bolorundurowb/Open-Exam-Suite/actions/workflows/build-and-test.yml)
[![SourceForge Downloads](https://img.shields.io/sourceforge/dm/open-exam-suite)](https://sourceforge.net/projects/open-exam-suite/files/)
[![License](https://img.shields.io/badge/license-GPLv3-orange.svg)](./LICENSE)
[![.NET 10](https://img.shields.io/badge/.net-10.0-0066b6.svg)](https://dotnet.microsoft.com/download/dotnet/10.0)


Open Exam Suite is an open-source alternative to Avanset's Visual CertExam Suite. It provides a platform for designing, creating, and simulating computer-based exams, offering a complete solution for anyone looking to build or take practice tests.

The project includes an **Exam Creator** for designing exams in the `oef` (Open Exam Format) and an **Exam Simulator** for conducting tests.

## Key Features

- **Exam Creator:**
    - Group questions into sections.
    - Support for images in questions and multiple options.
    - Export exams to JSON or XML.
    - Full support for Undo/Redo, Copy/Cut/Paste operations.
- **Exam Simulator** (cross-platform Avalonia app for Windows, macOS and Linux, light and dark themes):
    - A library of exams with search, filter, sort, drag-and-drop and sample exams on first launch.
    - Practice mode (untimed, Check answer and explanations) and Exam mode (timed, with a pause cover and 5-minute and 1-minute warnings).
    - Filter questions by section or select a random set.
    - Question navigator, flagging, and a review-and-submit screen that confirms unanswered questions.
    - Results with the pass mark, section breakdown, answer review, retake, and PDF export or print.
    - Keyboard-only operation: A-Z select, Left/Right move, F flag, P pause, Enter next, Ctrl+Enter review.
    - Opening a `.oef` file while the Simulator is running shows it in the existing window.
- **Compatibility:**
    - Built-in converter to upgrade older v1/v2 exam files to the modern v3 format.

## Project Modernisation (2026)

This project has been modernised from .NET Framework 4.0 to **.NET 10**.

- **Target Framework:** Upgraded all projects to `.net10.0-windows`.
- **Project System:** Migrated to modern **SDK-style** project files.
- **Project Structure:** Simplified the project structure.
- **Dependency Management:** Converted to `PackageReference`.
- **CI/CD:** Powered by **GitHub Actions**.
- **.oef File Format:** Upgraded from deprecated `BinaryFormatter` to **Protobuf**. Legacy `.oef` files are read without modification (never written to) and upgraded to protobuf only when explicitly saved.

## 🛠️ Getting Started

### Prerequisites
- [.NET 10.0 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- The Creator and Simulator are Avalonia apps and run on Windows, macOS, and Linux. A WinForms Creator build is published for one release cycle as a rollback.

### Building from Source
1. Clone the repository:
   ```bash
   git clone https://github.com/bolorundurowb/Open-Exam-Suite.git
   cd Open-Exam-Suite
   ```
2. Build the solution:
   ```powershell
   dotnet build
   ```
3. Run the applications:
   - **Creator (Avalonia):** `dotnet run --project src/Apps/Creator/Creator.csproj`
   - **Simulator (Avalonia):** `dotnet run --project src/Apps/Simulator/Simulator.csproj`
   - **Creator (WinForms rollback):** `dotnet run --project src/Apps/Creator.WinForms/Creator.WinForms.csproj`

## Downloads

Pre-built packages are attached to each [GitHub Release](https://github.com/bolorundurowb/Open-Exam-Suite/releases) and mirrored on [SourceForge](https://sourceforge.net/projects/open-exam-suite).

| Platform | Download |
|----------|----------|
| Windows | `OpenExamSuite-{version}-Setup-x64.exe` — Avalonia Simulator and Creator |
| macOS (Apple silicon) | `OpenExamSuite-{version}-macOS-arm64.dmg` |
| macOS (Intel) | `OpenExamSuite-{version}-macOS-x64.dmg` |
| Linux | `OpenExamSuite-{version}-Linux-x64.tar.gz`, `open-exam-suite_*_amd64.deb`, `open-exam-suite-creator_*_amd64.deb` |

The Linux packages install the Simulator and samples under `/opt/open-exam-suite`, register the `.oef` file type, and add the `open-exam-suite` and `open-exam-suite-creator` commands. Install the Creator package only after the Simulator package.

**WinForms rollback:** `OpenExamSuite-{version}-WinForms-Setup-x64.exe` and `OpenExamSuite-{version}-WinForms-Portable-x64.zip` ship the previous WinForms Creator beside the Avalonia Simulator. They are published for one release cycle so a regression can be rolled back with another file from the same release, and will be removed once the Avalonia Creator is proven.

## Contributing
Feel free to create an [issue](https://github.com/bolorundurowb/Open-Exam-Suite/issues) for feature requests or bug reports. Contributions are welcome via Pull Requests.

If this project has been of benefit to you, please give it a ⭐ on GitHub!

## Changelog
For a detailed history of changes, see the [CHANGELOG.md](./CHANGELOG.md) file.

## License
This project is licensed under the **GPLv3** license. See the [LICENSE](./LICENSE) file for details.

---
Created and maintained by [bolorundurowb](https://github.com/bolorundurowb). Twitter: [@Mr_Bolorunduro](https://twitter.com/Mr_Bolorunduro).
