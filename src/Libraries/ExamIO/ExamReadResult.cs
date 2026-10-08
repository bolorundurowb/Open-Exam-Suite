using OpenExamSuite.Shared.Enums;

namespace OpenExamSuite.Shared.Utilities;

/// <summary>
/// Result of reading an exam. Carries the decoded exam, a success flag, a typed failure
/// reason, and (for .oef files) whether the source was a legacy NRBF payload that has not
/// been upgraded in place.
/// </summary>
public sealed record ExamReadResult(
    Exam? Exam,
    bool Success,
    ExamIoError Error,
    bool IsLegacy = false);
