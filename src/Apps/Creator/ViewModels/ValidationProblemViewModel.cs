using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using OpenExamSuite.Creator.Engine.Models;

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
        GoToCommand = new RelayCommand(() => _workspace.RevealNode(NodeId));
    }

    public ProblemKind Kind { get; }

    public string NodeId { get; }

    public string Message { get; }

    public ICommand GoToCommand { get; }
}
