using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OpenExamSuite.Creator.Localization;
using OpenExamSuite.Creator.Services;
using OpenExamSuite.Creator.Session.Models;
using OpenExamSuite.Shared;

namespace OpenExamSuite.Creator.ViewModels;

public sealed partial class WorkspaceViewModel : ViewModelBase
{
    private readonly ShellServices _shell;

    public WorkspaceViewModel(ShellServices shell)
    {
        _shell = shell;
        SearchQuery = string.Empty;
        _shell.Document.Changed += OnDocumentChanged;
        _shell.Document.SelectionChanged += OnSelectionChanged;
        _shell.Document.ProblemsChanged += OnProblemsChanged;

        RefreshOutline();
        RefreshEditor();
        RefreshPreview();
        RefreshProblems();
    }

    public ObservableCollection<OutlineNodeViewModel> OutlineNodes { get; } = [];

    public ObservableCollection<ValidationProblemViewModel> Problems { get; } = [];

    [ObservableProperty] private OutlineNodeViewModel? _selectedNode;

    [ObservableProperty] private object? _editorContent;

    [ObservableProperty] private QuestionPreviewViewModel? _preview;

    [ObservableProperty] private string _searchQuery;

    partial void OnSearchQueryChanged(string value)
    {
        RefreshOutline();
    }

    partial void OnSelectedNodeChanged(OutlineNodeViewModel? value)
    {
        if (value != null)
            _shell.Document.SelectedNodeId = value.Id;
    }

    [RelayCommand]
    private void AddSection()
    {
        _shell.Document.AddSection();
        RefreshOutline();
    }

    [RelayCommand]
    private void AddQuestion()
    {
        var sectionId = SelectedNode?.Type switch
        {
            NodeType.Section => SelectedNode.Id,
            NodeType.Question => SelectedNode.ParentId,
            _ => OutlineNodes.FirstOrDefault(n => n.Type == NodeType.Section)?.Id
        };

        if (string.IsNullOrEmpty(sectionId))
        {
            _shell.Document.AddSection();
            sectionId = _shell.Document.Nodes.Values.Last(n => n.Type == NodeType.Section).Id;
        }

        _shell.Document.AddQuestion(sectionId);
        RefreshOutline();
    }

    [RelayCommand]
    private void Duplicate()
    {
        if (SelectedNode == null)
            return;

        _shell.Document.DuplicateNode(SelectedNode.Id);
        RefreshOutline();
    }

    [RelayCommand]
    private void Delete()
    {
        if (SelectedNode == null)
            return;

        _shell.Document.DeleteNode(SelectedNode.Id);
        RefreshOutline();
    }

    public void SelectNode(string nodeId)
    {
        var node = OutlineNodes.FirstOrDefault(n => n.Id == nodeId)
            ?? OutlineNodes.SelectMany(n => n.Children).FirstOrDefault(c => c.Id == nodeId);

        if (node != null)
            SelectedNode = node;
    }

    private void OnDocumentChanged()
    {
        RefreshOutline();
        RefreshEditor();
        RefreshPreview();
        RefreshProblems();
    }

    private void OnSelectionChanged(string? nodeId)
    {
        SelectNode(nodeId ?? string.Empty);
        RefreshEditor();
        RefreshPreview();
    }

    private void OnProblemsChanged(IReadOnlyList<ValidationProblem> problems)
    {
        RefreshProblems();
    }

    private void RefreshOutline()
    {
        var selectedId = SelectedNode?.Id;
        OutlineNodes.Clear();

        var nodes = string.IsNullOrWhiteSpace(SearchQuery)
            ? _shell.Document.Nodes.Values
            : _shell.Document.Search(SearchQuery);

        var examNode = nodes.FirstOrDefault(n => n.Type == NodeType.Exam);
        if (examNode != null)
        {
            var examVm = new OutlineNodeViewModel(examNode, null, _shell.Document);
            OutlineNodes.Add(examVm);
            selectedId ??= examVm.Id;
        }

        foreach (var section in nodes.Where(n => n.Type == NodeType.Section).OrderBy(n => _shell.Document.Exam.Sections.IndexOf(n.Section!)))
        {
            var sectionVm = new OutlineNodeViewModel(section, null, _shell.Document);
            OutlineNodes.Add(sectionVm);
            selectedId ??= sectionVm.Id;

            foreach (var question in nodes.Where(n => n.Type == NodeType.Question && n.ParentId == section.Id)
                .OrderBy(n => n.Question!.No))
            {
                sectionVm.Children.Add(new OutlineNodeViewModel(question, sectionVm, _shell.Document));
            }
        }

        if (!string.IsNullOrEmpty(selectedId))
            SelectNode(selectedId);
    }

    private void RefreshEditor()
    {
        var nodeId = _shell.Document.SelectedNodeId;
        if (string.IsNullOrEmpty(nodeId) || !_shell.Document.Nodes.TryGetValue(nodeId, out var node))
        {
            EditorContent = null;
            return;
        }

        EditorContent = node.Type switch
        {
            NodeType.Exam => new ExamPropertiesViewModel(_shell),
            NodeType.Section => new SectionEditorViewModel(_shell, node.Id),
            NodeType.Question => new QuestionEditorViewModel(_shell, node.Id),
            _ => null
        };
    }

    private void RefreshPreview()
    {
        var questionId = _shell.Document.SelectedNodeId;
        if (string.IsNullOrEmpty(questionId)
            || !_shell.Document.Nodes.TryGetValue(questionId, out var node)
            || node.Type != NodeType.Question)
        {
            Preview = null;
            return;
        }

        Preview = new QuestionPreviewViewModel(node.Question!);
    }

    private void RefreshProblems()
    {
        Problems.Clear();
        foreach (var problem in _shell.Document.Problems)
        {
            Problems.Add(new ValidationProblemViewModel(problem, this));
        }
    }
}
