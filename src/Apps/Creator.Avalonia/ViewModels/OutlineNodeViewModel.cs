using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OpenExamSuite.Creator.Session.Models;
using OpenExamSuite.Creator.Session.Services;
using OpenExamSuite.Shared;

namespace OpenExamSuite.Creator.ViewModels;

public sealed partial class OutlineNodeViewModel : ViewModelBase
{
    private readonly CreatorDocument _document;

    public OutlineNodeViewModel(DocumentNode node, OutlineNodeViewModel? parent, CreatorDocument document)
    {
        _document = document;
        Id = node.Id;
        Type = node.Type;
        ParentId = parent?.Id;
        Node = node;
        Children = [];
    }

    public string Id { get; }

    public NodeType Type { get; }

    public string? ParentId { get; }

    public DocumentNode Node { get; }

    public ObservableCollection<OutlineNodeViewModel> Children { get; }

    [ObservableProperty] private bool _isExpanded = true;

    private bool CanChangeStructure() => Type != NodeType.Exam;

    [RelayCommand(CanExecute = nameof(CanChangeStructure))]
    private void Duplicate() => _document.DuplicateNode(Id);

    [RelayCommand(CanExecute = nameof(CanChangeStructure))]
    private void Delete() => _document.DeleteNode(Id);

    public string Title => Type switch
    {
        NodeType.Exam => _document.Exam.Properties.Title,
        NodeType.Section => Node.Section?.Title ?? string.Empty,
        NodeType.Question => $"Q{Node.Question?.No}. {FirstLine(Node.Question?.Text)}" ?? string.Empty,
        _ => string.Empty
    };

    public bool HasImage => Type == NodeType.Question && Node.Question?.ImageData != null;

    public bool IsMultipleChoice => Type == NodeType.Question && Node.Question?.IsMultipleChoice == true;

    public bool HasProblem => _document.Problems.Any(p => p.NodeId == Id);

    private static string FirstLine(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return "(empty)";

        var line = text.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries).FirstOrDefault() ?? text;
        return line.Length > 60 ? line[..57] + "..." : line;
    }
}
