using OpenExamSuite.Shared.Enums;

namespace OpenExamSuite.Shared.Utilities;

/// <summary>
/// Result of writing an exam. Failures carry the typed <see cref="ExamIoError"/> and, when
/// the runtime reported one, the IO exception message.
/// </summary>
public sealed record ExamWriteResult(bool Success, ExamIoError Error = ExamIoError.None, string? Detail = null);
