using OpenExamSuite.Creator.Services;
using OpenExamSuite.Creator.Session.Models;

namespace OpenExamSuite.Creator.ViewModels;

public sealed class ValidationProblemViewModel
{
    private readonly WorkspaceViewModel _workspace;

    public ValidationProblemViewModel(ValidationProblem problem, WorkspaceViewModel workspace)
    {
        _workspace = workspace;
        Kind = problem.Kind;
        NodeId = problem.NodeId;
        Message = problem.Message;
    }

    public ProblemKind Kind { get; }

    public string NodeId { get; }

    public string Message { get; }

    public void GoTo() => _workspace.SelectNode(NodeId);
}
