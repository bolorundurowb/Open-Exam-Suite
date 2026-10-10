# Browser Host Contract

This document describes the host contracts required for a browser-based host of Open Exam Suite.

> [!NOTE]
> This phase establishes and documents the host-port contracts; it **does not** implement a browser host.
> Desktop applications (Simulator and Creator) run on Avalonia across Windows, macOS, and Linux.

The browser host supplies its own file picker, download, timer lifecycle, and report adapter by implementing the host ports in `OpenExamSuite.Simulator.Engine`. Those ports stay free of Avalonia types. Creator file results live in `OpenExamSuite.Creator.Engine` and are also free of Avalonia types.

---

## 1. Session and State Management

- **Simulator Engine:** `ISimulatorSession` / `SimulatorSession` and `ISessionState` reside in `OpenExamSuite.Simulator.Engine`.
- **Flow & Rules:** Attempt lifecycle, scoring, section filtering, random question selection, answer validation, and review navigation stay entirely within the session engine.
- **State observation:** Hosts consume reactive state transitions via `SimulatorSession.CurrentState` without needing custom business or session logic.

## 2. Timer and TimeProvider Lifecycle

- The session takes a standard .NET `TimeProvider` and manages its own exam duration timers.
- A browser host must supply a `TimeProvider` whose timer lifecycle integrates with browser tab and page execution (for example handling tab suspension or background throttling).
- The browser host **must not** run an independent or duplicate exam countdown clock; all timing and countdown events originate from the engine session.

## 3. File Picking and Downloads

- `IPrompts.PickFileAsync` and `IPrompts.SaveFileAsync` (in `OpenExamSuite.Simulator.Engine.HostPorts`) return path strings or abstract file identifiers.
- `IFileSystem` opens streams for reading and writing those target paths.
- A browser host maps file upload dialogs and client-side downloads onto these abstractions, allowing virtual paths or in-memory blobs without requiring local disk system access.

## 4. Report Adapter and Printing

- `IPrintService` (in `OpenExamSuite.Simulator.Engine.HostPorts`) receives a generated PDF `Stream` along with job title and configuration for print, preview, or export.
- A browser host decides how the PDF stream is handled:
  - Triggering a browser PDF download,
  - Opening the stream in a new tab via a Blob URL (`URL.createObjectURL`), or
  - Invoking browser print preview.

## 5. Application Paths and Storage

- `IAppPaths` defines roots for bundled sample exams, user data, temporary scratch files, and documents:
  - `BundledSamplesRoot`
  - `UserDataDirectory`
  - `TempDirectory`
  - `DocumentsDirectory`
  - `GetBundledSamplePath(fileName)`
- A browser host provides an implementation of `IAppPaths` backed by browser-compatible storage (such as IndexedDB, OPFS / Origin Private File System, or server-backed endpoints) rather than physical desktop paths.

## 6. Document and File Results

The file-result types stay free of Avalonia, WinForms, and browser types:

- `ExamReadResult` (`OpenExamSuite.Shared.Utilities`, `src/Libraries/ExamIO/ExamReadResult.cs`) carries `Exam`, `Success`, `ExamIoError`, and `IsLegacy`. `IsLegacy` means the `.oef` source was a legacy NRBF payload that has not been upgraded in place.
- `DocumentLoadResult` (`OpenExamSuite.Creator.Engine.Models`) carries `Success`, `Error`, `IsLegacy`, and `FilePath`.
- `DocumentSaveResult` in the same file carries `Success`, `Error`, `Detail`, and `FilePath`.

## 7. Trimming and Native AOT

`PublishTrimmed` and `PublishAot` stay `false` on the Creator and Simulator app projects, and on every `dotnet publish` in the release workflow. `XmlSerializer` and protobuf-net reflection do not survive trimming or native AOT. A later browser plan must not turn them on.
