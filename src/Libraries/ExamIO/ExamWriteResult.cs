using OpenExamSuite.Shared.Enums;

namespace OpenExamSuite.Shared.Utilities;

/// <summary>
/// Result of writing an exam, carrying a success flag and a typed failure reason.
/// </summary>
public sealed record ExamWriteResult(bool Success, ExamIoError Error = ExamIoError.None);
