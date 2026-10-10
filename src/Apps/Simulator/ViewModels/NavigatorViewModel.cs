using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OpenExamSuite.Shared;
using OpenExamSuite.Simulator.Localization;
using OpenExamSuite.Simulator.Engine.Models;
using OpenExamSuite.Simulator.Engine.States;

namespace OpenExamSuite.Simulator.ViewModels;

/// <summary>
/// One numbered cell in the question navigator: unanswered, answered, flagged or current.
/// </summary>
public sealed partial class NavCellViewModel : ObservableObject
{
    public NavCellViewModel(int index, Func<int, Task> jump)
    {
        Index = index;
        JumpCommand = new AsyncRelayCommand(() => jump(index));
    }

    public IAsyncRelayCommand JumpCommand { get; }

    public int Index { get; }

    public int Number => Index + 1;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(AutomationName))]
    private bool _isAnswered;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(AutomationName))]
    private bool _isFlagged;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(AutomationName))]
    private bool _isCurrent;

    public string AutomationName
    {
        get
        {
            var parts = new List<string>
            {
                Strings.Format("Nav_Question", Number),
                Strings.Get(IsAnswered ? "Nav_Answered" : "Nav_Unanswered")
            };
            if (IsFlagged)
                parts.Add(Strings.Get("Nav_Flagged"));
            if (IsCurrent)
                parts.Add(Strings.Get("Nav_Current"));
            return string.Join(", ", parts);
        }
    }
}

public sealed class NavSectionViewModel
{
    public NavSectionViewModel(string title, IReadOnlyList<NavCellViewModel> cells)
    {
        Title = title;
        Cells = cells;
    }

    public string Title { get; }

    public IReadOnlyList<NavCellViewModel> Cells { get; }
}

/// <summary>
/// The question navigator, grouped by section. Cells are updated in place so keyboard focus survives.
/// </summary>
public sealed class NavigatorViewModel
{
    private readonly Func<int, Task> _jump;
    private IReadOnlyList<Question>? _questions;
    private readonly List<NavCellViewModel> _cells = [];

    public NavigatorViewModel(Func<int, Task> jump)
    {
        _jump = jump;
    }

    public ObservableCollection<NavSectionViewModel> Sections { get; } = [];

    public void Update(AttemptState attempt)
    {
        if (!ReferenceEquals(_questions, attempt.Questions))
            Rebuild(attempt);

        foreach (var cell in _cells)
        {
            var selection = attempt.Answers.TryGetValue(cell.Index, out var answer) ? answer : AnswerSelection.Unanswered();
            cell.IsAnswered = selection.IsAnswered;
            cell.IsFlagged = selection.IsFlagged;
            cell.IsCurrent = cell.Index == attempt.CurrentQuestionIndex;
        }
    }

    private void Rebuild(AttemptState attempt)
    {
        _questions = attempt.Questions;
        _cells.Clear();
        Sections.Clear();

        for (var i = 0; i < attempt.Questions.Count; i++)
            _cells.Add(new NavCellViewModel(i, _jump));

        var assigned = new HashSet<int>();
        foreach (var section in attempt.Sections)
        {
            var members = new HashSet<Question>(section.Questions);
            var cells = new List<NavCellViewModel>();
            for (var i = 0; i < attempt.Questions.Count; i++)
            {
                if (members.Contains(attempt.Questions[i]) && assigned.Add(i))
                    cells.Add(_cells[i]);
            }

            if (cells.Count > 0)
                Sections.Add(new NavSectionViewModel(section.Title, cells));
        }

        // Anything not attached to a section still gets a cell.
        var orphans = _cells.Where(c => !assigned.Contains(c.Index)).ToList();
        if (orphans.Count > 0)
            Sections.Add(new NavSectionViewModel(string.Empty, orphans));
    }
}
