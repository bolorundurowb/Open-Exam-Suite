using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Avalonia.Media.Imaging;
using OpenExamSuite.Simulator.Localization;
using OpenExamSuite.Simulator.Models;
using OpenExamSuite.Simulator.Services;
using OpenExamSuite.Simulator.Session.States;

namespace OpenExamSuite.Simulator.ViewModels;

public enum ReviewOutcome
{
    Correct,
    Wrong,
    Unanswered
}

/// <summary>One row in the filterable question list.</summary>
public sealed class ReviewListItemViewModel
{
    public ReviewListItemViewModel(GradingDetail detail)
    {
        Number = detail.QuestionIndex + 1;
        Outcome = detail.IsCorrect
            ? ReviewOutcome.Correct
            : detail.IsAnswered ? ReviewOutcome.Wrong : ReviewOutcome.Unanswered;
        IsFlagged = detail.IsFlagged;
        var text = detail.Question.Text.Replace('\n', ' ').Trim();
        Preview = text.Length > 80 ? text[..80] + "…" : text;
    }

    public int Number { get; }
    public ReviewOutcome Outcome { get; }
    public bool IsFlagged { get; }
    public string Preview { get; }

    public bool IsCorrect => Outcome == ReviewOutcome.Correct;
    public bool IsWrong => Outcome == ReviewOutcome.Wrong;
    public bool IsUnanswered => Outcome == ReviewOutcome.Unanswered;

    public string OutcomeLabel => Strings.Get(Outcome switch
    {
        ReviewOutcome.Correct => "Review_Correct",
        ReviewOutcome.Wrong => "Review_Wrong",
        _ => "Review_Unanswered"
    });

    public string Title => Strings.Format("Review_ListItem", Number);

    public string AutomationName => $"{Title}, {OutcomeLabel}{(IsFlagged ? ", " + Strings.Get("Nav_Flagged") : string.Empty)}";
}

public sealed partial class ReviewFilterViewModel : ObservableObject
{
    public ReviewFilterViewModel(AnswerReviewFilter filter, string label)
    {
        Filter = filter;
        _label = label;
    }

    public AnswerReviewFilter Filter { get; }

    [ObservableProperty]
    private string _label;

    [ObservableProperty]
    private bool _isSelected;
}

/// <summary>
/// Answer review: a filterable list and a question detail. Correct and wrong use an icon and a label.
/// When the author hid the answers, the correct answer and explanation are withheld.
/// </summary>
public sealed partial class AnswerReviewViewModel : ViewModelBase, IDisposable
{
    private readonly ShellServices _shell;
    private bool _applying;
    private AnswerReviewState? _state;

    public AnswerReviewViewModel(ShellServices shell)
    {
        _shell = shell;
        foreach (var filter in new[]
                 {
                     AnswerReviewFilter.All, AnswerReviewFilter.Wrong, AnswerReviewFilter.Unanswered,
                     AnswerReviewFilter.Flagged, AnswerReviewFilter.Correct
                 })
        {
            Filters.Add(new ReviewFilterViewModel(filter, string.Empty));
        }
    }

    public ObservableCollection<ReviewFilterViewModel> Filters { get; } = [];

    public ObservableCollection<ReviewListItemViewModel> Items { get; } = [];

    public ObservableCollection<OptionItemViewModel> Options { get; } = [];

    [ObservableProperty] private int _selectedIndex = -1;
    [ObservableProperty] private bool _hasCurrent;
    [ObservableProperty] private bool _isEmpty;
    [ObservableProperty] private string _questionTitle = string.Empty;
    [ObservableProperty] private string _questionText = string.Empty;
    [ObservableProperty] private string _outcomeLabel = string.Empty;
    [ObservableProperty] private bool _outcomeCorrect;
    [ObservableProperty] private bool _outcomeWrong;
    [ObservableProperty] private bool _outcomeUnanswered;
    [ObservableProperty] private bool _isFlagged;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasImage))]
    private Bitmap? _image;

    public bool HasImage => Image != null;

    [ObservableProperty] private bool _isHidden;
    [ObservableProperty] private bool _showExplanation;
    [ObservableProperty] private string _explanation = string.Empty;
    [ObservableProperty] private bool _canPrevious;
    [ObservableProperty] private bool _canNext;
    [ObservableProperty] private bool _canPracticeAgain;
    [ObservableProperty] private string _positionText = string.Empty;
    [ObservableProperty] private string _announcement = string.Empty;

    public void Apply(AnswerReviewState state)
    {
        _applying = true;
        try
        {
            var previous = _state;
            _state = state;

            foreach (var filter in Filters)
            {
                var count = state.GradingDetails.Count(d => d.MatchesFilter(filter.Filter));
                filter.Label = Strings.Format("Review_FilterLabel", Strings.Get("Review_Filter_" + filter.Filter), count);
                filter.IsSelected = filter.Filter == state.Filter;
            }

            var indices = state.FilteredIndices;
            var listChanged = previous == null
                              || previous.Filter != state.Filter
                              || !ReferenceEquals(previous.GradingDetails, state.GradingDetails);
            if (listChanged)
            {
                Items.Clear();
                foreach (var index in indices)
                    Items.Add(new ReviewListItemViewModel(state.GradingDetails[index]));
            }

            IsEmpty = indices.Count == 0;
            HasCurrent = state.HasCurrent;
            SelectedIndex = state.HasCurrent ? state.CurrentReviewIndex : -1;
            CanPrevious = state.CurrentReviewIndex > 0;
            CanNext = state.CurrentReviewIndex < indices.Count - 1;
            CanPracticeAgain = !state.Exam.Properties.HideAnswers && state.GradingDetails.Any(d => !d.IsCorrect);
            PositionText = state.HasCurrent
                ? Strings.Format("Review_Position", state.CurrentReviewIndex + 1, indices.Count)
                : string.Empty;

            if (state.HasCurrent)
                ShowDetail(state.GradingDetails[state.CurrentQuestionIndex]);
            else
                ClearDetail();
        }
        finally
        {
            _applying = false;
        }
    }

    private void ShowDetail(GradingDetail detail)
    {
        var item = new ReviewListItemViewModel(detail);
        QuestionTitle = item.Title;
        QuestionText = detail.Question.Text;
        OutcomeLabel = item.OutcomeLabel;
        OutcomeCorrect = item.IsCorrect;
        OutcomeWrong = item.IsWrong;
        OutcomeUnanswered = item.IsUnanswered;
        IsFlagged = detail.IsFlagged;
        IsHidden = detail.IsHidden;
        Announcement = $"{item.Title}. {item.OutcomeLabel}";

        SetImage(detail.Question.ImageData);

        var selected = AnswerMath.SelectedLetters(detail.Selection);
        IReadOnlyList<char> correct = detail.IsHidden ? Array.Empty<char>() : AnswerMath.CorrectLetters(detail.Question);

        Options.Clear();
        foreach (var option in detail.Question.Options)
        {
            var isSelected = selected.Contains(option.Alphabet);
            var isCorrect = correct.Contains(option.Alphabet);
            var view = new OptionItemViewModel(option.Alphabet, option.Text, detail.Question.IsMultipleChoice)
            {
                IsSelected = isSelected,
                Mark = detail.IsHidden
                    ? (isSelected ? OptionMark.YourAnswer : OptionMark.None)
                    : isSelected
                        ? (isCorrect ? OptionMark.YourAnswerCorrect : OptionMark.YourAnswerWrong)
                        : (isCorrect ? OptionMark.CorrectAnswer : OptionMark.None)
            };
            Options.Add(view);
        }

        ShowExplanation = !detail.IsHidden && !string.IsNullOrWhiteSpace(detail.Explanation);
        Explanation = ShowExplanation ? detail.Explanation! : string.Empty;
    }

    private void ClearDetail()
    {
        QuestionTitle = string.Empty;
        QuestionText = string.Empty;
        Options.Clear();
        SetImage(null);
        ShowExplanation = false;
        Explanation = string.Empty;
        OutcomeLabel = string.Empty;
        OutcomeCorrect = OutcomeWrong = OutcomeUnanswered = false;
    }

    private void SetImage(byte[]? data)
    {
        var old = Image;
        Image = null;
        old?.Dispose();
        if (data is not { Length: > 0 })
            return;

        try
        {
            using var stream = new MemoryStream(data);
            Image = new Bitmap(stream);
        }
        catch (Exception)
        {
            Image = null;
        }
    }

    partial void OnSelectedIndexChanged(int value)
    {
        if (_applying || _state == null || value < 0 || value == _state.CurrentReviewIndex)
            return;

        var delta = value - _state.CurrentReviewIndex;
        _ = _shell.RunAsync(() => _shell.Session.NavigateReviewAsync(delta));
    }

    [RelayCommand]
    private Task SetFilterAsync(ReviewFilterViewModel? filter) =>
        filter == null ? Task.CompletedTask : _shell.RunAsync(() => _shell.Session.SetReviewFilterAsync(filter.Filter));

    [RelayCommand]
    public Task PreviousAsync() => _shell.RunAsync(() => _shell.Session.NavigateReviewAsync(-1));

    [RelayCommand]
    public Task NextAsync() => _shell.RunAsync(() => _shell.Session.NavigateReviewAsync(1));

    [RelayCommand]
    private Task PracticeAgainAsync() => _shell.RunAsync(() => _shell.Session.PracticeMissedAgainAsync());

    [RelayCommand]
    private Task BackToResultsAsync() => _shell.RunAsync(() => _shell.Session.ReturnToResultsAsync());

    [RelayCommand]
    private Task BackToLibraryAsync() => _shell.RunAsync(() => _shell.Session.ReturnToLibraryAsync());

    [RelayCommand]
    private Task RetakeAsync() => _shell.RunAsync(() => _shell.Session.RetakeAsync());

    [RelayCommand]
    private async Task EnlargeImageAsync()
    {
        if (Image != null)
            await _shell.Dialogs.ShowImageAsync(Image);
    }

    public void Dispose()
    {
        var old = Image;
        Image = null;
        old?.Dispose();
    }
}
