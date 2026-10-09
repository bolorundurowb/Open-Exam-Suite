using OpenExamSuite.Shared.Enums;

namespace OpenExamSuite.Simulator.Session.Models;

/// <summary>
/// Thrown when an exam file cannot be opened. The host maps <see cref="Error"/> to a localised message.
/// </summary>
public sealed class ExamLoadException : InvalidOperationException
{
    public ExamLoadException(ExamFileLoadError error, string message, bool unsupportedFormat = false)
        : base(message)
    {
        Error = error;
        IsUnsupportedFormat = unsupportedFormat;
    }

    public ExamFileLoadError Error { get; }

    /// <summary>True when the file is not an <c>.oef</c> exam. JSON and XML belong to Creator.</summary>
    public bool IsUnsupportedFormat { get; }
}
