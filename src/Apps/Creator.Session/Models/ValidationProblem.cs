namespace OpenExamSuite.Creator.Session.Models;

public enum ProblemKind
{
    NoCorrectAnswer,
    FewerThanTwoOptions,
    EmptyQuestionText,
    DuplicateSectionName
}

public sealed class ValidationProblem
{
    public ProblemKind Kind { get; }

    public string NodeId { get; }

    public string Message { get; }

    public ValidationProblem(ProblemKind kind, string nodeId, string message)
    {
        Kind = kind;
        NodeId = nodeId;
        Message = message;
    }
}
