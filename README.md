# Open Exam Suite

[![Build, Test & Coverage](https://github.com/bolorundurowb/Open-Exam-Suite/actions/workflows/build-and-test.yml/badge.svg)](https://github.com/bolorundurowb/Open-Exam-Suite/actions/workflows/build-and-test.yml)
[![SourceForge Downloads](https://img.shields.io/sourceforge/dm/open-exam-suite)](https://sourceforge.net/projects/open-exam-suite/files/)
[![License](https://img.shields.io/badge/license-GPLv3-orange.svg)](./LICENSE)
[![.NET 11](https://img.shields.io/badge/.net-11.0-0066b6.svg)](https://dotnet.microsoft.com/download/dotnet/11.0)

Open Exam Suite is an open-source desktop suite for creating and taking computer-based exams. It includes two apps:

- **Creator** — build exams, organize questions into sections, add images and answer options, and edit with undo and redo. Exams can be exported to JSON or XML.
- **Simulator** — take exams in practice or timed exam mode, review answers and results, and print or export a results report as PDF.

Both apps run on Windows, macOS, and Linux. The codebase targets .NET 11 and uses Avalonia for its cross-platform desktop interface. Shared exam, storage, and app-engine libraries keep core behavior separate from the user interface. GitHub Actions builds and tests the solution across Windows, macOS, and Linux.

## Exam file compatibility

Open Exam Suite uses the `.oef` format. It reads older v1 and v2 exam files and upgrades them to the current format when they are saved. JSON and XML export is also available from Creator.

## Build and run

Install the [.NET 11 SDK](https://dotnet.microsoft.com/download/dotnet/11.0), then run these commands from the repository root:

```bash
git clone https://github.com/bolorundurowb/Open-Exam-Suite.git
cd Open-Exam-Suite/src
dotnet restore
dotnet build --configuration Release --no-restore
```

Run either app from the `src` directory:

```bash
dotnet run --project Apps/Creator/Creator.csproj
dotnet run --project Apps/Simulator/Simulator.csproj
```

Run the test suite from `src` with:

```bash
dotnet test --configuration Release
```

## Downloads

Pre-built releases are available on [GitHub Releases](https://github.com/bolorundurowb/Open-Exam-Suite/releases) and [SourceForge](https://sourceforge.net/projects/open-exam-suite/files/).

| Platform | Release files |
| --- | --- |
| Windows x64 | `OpenExamSuite-{version}-Setup-x64.exe` installer; `OpenExamSuite-{version}-Portable-x64.zip` portable package |
| macOS Apple silicon | `OpenExamSuite-{version}-macOS-arm64.dmg` |
| macOS Intel | `OpenExamSuite-{version}-macOS-x64.dmg` |
| Linux x64 | `OpenExamSuite-{version}-Linux-x64.tar.gz` portable package; `open-exam-suite_{version}_amd64.deb` package |

The Windows installer and portable package include both apps and the sample exams. Each macOS DMG contains both apps. The Linux tarball includes the Simulator, Creator, and samples; the Debian package installs them under `/opt/open-exam-suite`, registers `.oef` files with the Simulator, and adds the `open-exam-suite` and `open-exam-suite-creator` commands.

## Contributing

Bug reports and feature requests are welcome through [GitHub Issues](https://github.com/bolorundurowb/Open-Exam-Suite/issues). Contributions can be submitted as pull requests.

## License

Open Exam Suite is licensed under [GPL-3.0](./LICENSE). See the [changelog](./CHANGELOG.md) for release history.

Maintained by [bolorundurowb](https://github.com/bolorundurowb).
