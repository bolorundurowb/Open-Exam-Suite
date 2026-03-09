# Open Exam Suite

[![Build, Test & Coverage](https://github.com/bolorundurowb/Open-Exam-Suite/actions/workflows/build-and-test.yml/badge.svg)](https://github.com/bolorundurowb/Open-Exam-Suite/actions/workflows/build-and-test.yml)
[![Download Open Exam Suite](https://img.shields.io/sourceforge/dm/open-exam-suite.svg)](https://sourceforge.net/projects/open-exam-suite/files/latest/download) [![.NET 10](https://img.shields.io/badge/.net-10.0-0066b6.svg)](https://dotnet.microsoft.com/download/dotnet/10.0) [![License](https://img.shields.io/badge/license-GPLv3-orange.svg)](./LICENSE)

Open Exam Suite is an open-source alternative to Avanset's Visual CertExam Suite. It provides a platform for designing, creating, and simulating computer-based exams, offering a complete solution for anyone looking to build or take practice tests.

The project includes an **Exam Creator** for designing exams in the `oef` (Open Exam Format) and an **Exam Simulator** for conducting tests.

## 🚀 Key Features

- **Exam Creator:**
    - Group questions into sections.
    - Support for images in questions and multiple options.
    - Export exams to JSON or XML.
    - Full support for Undo/Redo, Copy/Cut/Paste operations.
- **Exam Simulator:**
    - Simulate timed exams with custom limits.
    - Filter questions by section or select a random set.
    - Real-time answer checking and explanations.
    - Result printing and progress tracking.
- **Compatibility:**
    - Built-in converter to upgrade older v1/v2 exam files to the modern v3 format.

## 🏗️ Project Modernization (2026)

This project has been modernized from .NET Framework 4.0 to **.NET 10**.

- **Target Framework:** Upgraded all projects to `.net10.0-windows`.
- **Project System:** Migrated to modern **SDK-style** project files.
- **Project Structure:** Simplified the project structure.
- **Dependency Management:** Converted to `PackageReference`.
- **CI/CD:** Powered by **GitHub Actions**.
- **Testing:** Updated to `xUnit 2.9.2`.

## 🛠️ Getting Started

### Prerequisites
- [.NET 10.0 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- Windows (WinForms-based applications)

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
   - **Creator:** `dotnet run --project src/apps/Creator/Creator.csproj`
   - **Simulator:** `dotnet run --project src/apps/Simulator/Simulator.csproj`

## 📦 Downloads
You can find pre-built installers on [SourceForge](https://sourceforge.net/projects/open-exam-suite).

## 🤝 Contributing
Feel free to create an [issue](https://github.com/bolorundurowb/Open-Exam-Suite/issues) for feature requests or bug reports. Contributions are welcome via Pull Requests.

If this project has been of benefit to you, please give it a ⭐ on GitHub!

## 📜 Changelog
For a detailed history of changes, see the [CHANGELOG.md](./CHANGELOG.md) file.

## ⚖️ License
This project is licensed under the **GPLv3** license. See the [LICENSE](./LICENSE) file for details.

---
Created and maintained by [bolorundurowb](https://github.com/bolorundurowb). Twitter: [@Mr_Bolorunduro](https://twitter.com/Mr_Bolorunduro).
