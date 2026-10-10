# Browser Host Contract

This document describes the host contracts required for a browser-based host of Open Exam Suite.

> [!NOTE]
> This phase establishes and documents the host-port contracts; it **does not** implement a browser host.
> Desktop applications (Simulator and Creator) run on Avalonia across Windows, macOS, and Linux.

The browser host supplies its own file picking, file downloads, timer lifecycle, storage, and report presentation by implementing the host-port interfaces defined in `OpenExamSuite.Simulator.Engine` and `OpenExamSuite.Creator.Engine`. All engine host ports remain completely free of UI framework types (including Avalonia).

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

The core file load and save contracts do not reference any UI framework:
- **Simulator / File I/O:** `ExamReadResult` in `OpenExamSuite.ExamIO` exposes `Exam`, `Success`, `ExamIoError`, and `IsLegacy`.
- **Creator Engine:** `DocumentLoadResult` and `DocumentSaveResult` in `OpenExamSuite.Creator.Engine.Models` expose document nodes, load state, and error information.

Neither model references Avalonia, WinForms, or browser-specific types.

## 7. Trimming and Native AOT Constraints

Trimming (`PublishTrimmed`) and Native AOT (`PublishAot`) **must remain disabled** (`false`).
- Legacy `.oef` loading, `XmlSerializer`, and `protobuf-net` serialization rely on dynamic code generation and reflection that do not survive .NET trimming or Ahead-of-Time compilation.
- Future browser compilation (e.g. WebAssembly / Blazor WASM) must keep trimming and AOT disabled or configure explicit preserve directives so reflection structures remain intact.