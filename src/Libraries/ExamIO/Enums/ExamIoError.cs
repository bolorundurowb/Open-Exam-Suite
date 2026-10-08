namespace OpenExamSuite.Shared.Enums;

/// <summary>
/// Reason a read or write operation failed. Kept distinct from the per-extension
/// <see cref="ExamFileLoadError"/> so that every Reader/Writer call can surface the
/// underlying cause without resorting to <c>null</c>/<c>false</c> or the static logger.
/// </summary>
public enum ExamIoError
{
    None,
    FileNotFound,
    UnsupportedOrCorrupt,
    WriteFailed
}
