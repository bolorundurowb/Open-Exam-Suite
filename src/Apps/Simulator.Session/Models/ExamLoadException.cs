using OpenExamSuite.Shared.Enums;

namespace OpenExamSuite.Simulator.Session.Models;

/// <summary>
/// Thrown when an exam file cannot be opened. The host maps <see cref="Error"/> to a localised message.
/// </summary>
public sealed class ExamLoadException : InvalidOperationException
{
    public ExamLoadException(ExamFileLoadError error, string message)
        : base(message)
    {
        Error = error;
    }

    public ExamFileLoadError Error { get; }
}
