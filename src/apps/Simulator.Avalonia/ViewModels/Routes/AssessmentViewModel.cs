using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using OpenExamSuite.Shared;
using OpenExamSuite.Shared.Avalonia.Services;
using OpenExamSuite.Shared.Models;
using OpenExamSuite.Simulator.Services;
using OpenExamSuite.Simulator.ViewModels.Items;

namespace OpenExamSuite.Simulator.ViewModels.Routes;

public enum AssessmentPhase
{
    Intro,
    Running,
}

/// <summary>
/// Replaces the WinForms <c>AssessmentUi</c>. The single biggest behavioural
/// redesign in the migration: instead of toggling <c>Visible</c> on 16+
/// controls in <c>EnableControls()</c>, the view binds two stacked
/// <c>IsVisible</c> regions to <see cref="IsIntro"/> and <see cref="IsRunning"/>.
/// </summary>
public sealed partial class AssessmentViewModel : ObservableObject, IRouteViewModel
{
    private readonly Exam _exam;
    private readonly Settings _settings;
    private readonly IServiceProvider _sp;
    private readonly INavigationService _nav;
    private readonly IMessageBoxService _msg;
    private readonly ITimerService _timers;

    private ICountdownTimer? _timer;
    private int _timeLeft;
    private int _currentQuestionIndex;
    private readonly object?[] _userAnswers;

    // Intro phase fields
    public string ExamCode         => _exam.Properties.Code;
    public string ExamTitle        => _exam.Properties.Title;
    public string ExamInstructions => _exam.Properties.Instructions;

    public bool ExamHidesAnswers => _exam.Properties.HideAnswers;

    [ObservableProperty] private AssessmentPhase _phase = AssessmentPhase.Intro;

    [ObservableProperty] private string _elapsedTimeText = "00:00:00";
    [ObservableProperty] private string _questionText = string.Empty;
    [ObservableProperty] private byte[]? _questionImageData;
    [ObservableProperty] private string _sectionTitle = string.Empty;
    [ObservableProperty] private string _questionNumber = string.Empty;
    [ObservableProperty] private string _progressText = string.Empty;
    [ObservableProperty] private string _explanation = string.Empty;
    [ObservableProperty] private bool _showAnswer;

    public string ShowAnswerButtonText => ShowAnswer ? "Hide Answer" : "Show Answer";
    public bool IsIntro   => Phase == AssessmentPhase.Intro;
    public bool IsRunning => Phase == AssessmentPhase.Running;
    public bool IsExplanationVisible => ShowAnswer && !_exam.Properties.HideAnswers;
    public bool IsShowAnswerVisible  => !_exam.Properties.HideAnswers;

    public ObservableCollection<AnswerOptionViewModel> Options { get; } = new();

    public AssessmentViewModel(
        Exam exam,
        Settings settings,
        IServiceProvider sp,
        INavigationService nav,
        IMessageBoxService msg)
    {
        _exam = exam;
        _settings = settings;
        _sp = sp;
        _nav = nav;
        _msg = msg;
        _timers = sp.GetRequiredService<ITimerService>();

        _timeLeft = settings.TimeLimit * 60;
        _userAnswers = new object?[settings.Questions.Count];
    }

    partial void OnPhaseChanged(AssessmentPhase value)
    {
        OnPropertyChanged(nameof(IsIntro));
        OnPropertyChanged(nameof(IsRunning));
        PreviousCommand.NotifyCanExecuteChanged();
        NextCommand.NotifyCanExecuteChanged();
    }

    partial void OnShowAnswerChanged(bool value)
    {
        OnPropertyChanged(nameof(ShowAnswerButtonText));
        OnPropertyChanged(nameof(IsExplanationVisible));
    }

    // ---------------------------------------------------------------------
    // Commands
    // ---------------------------------------------------------------------

    [RelayCommand(CanExecute = nameof(CanBegin))]
    private void Begin()
    {
        Phase = AssessmentPhase.Running;
        _timer = _timers.Create(TimeSpan.FromSeconds(1));
        _timer.Tick += OnTick;
        _timer.Start();
        NavigateExam(NavOption.Begin);
    }
    private bool CanBegin() => IsIntro && _settings.Questions.Count > 0;

    [RelayCommand(CanExecute = nameof(CanGoPrevious))]
    private void Previous() => NavigateExam(NavOption.Previous);
    private bool CanGoPrevious() => IsRunning && _currentQuestionIndex > 0;

    [RelayCommand(CanExecute = nameof(CanGoNext))]
    private void Next() => NavigateExam(NavOption.Next);
    private bool CanGoNext() => IsRunning && _currentQuestionIndex < _settings.Questions.Count - 1;

    [RelayCommand(CanExecute = nameof(IsRunning))]
    private async Task PauseAsync()
    {
        _timer?.Stop();
        await _msg.ShowAsync(
            "Your exam has been paused. Click 'OK' to continue.",
            "Paused", MessageBoxButtons.Ok, MessageBoxIconKind.Information);
        _timer?.Start();
    }

    [RelayCommand(CanExecute = nameof(IsRunning))]
    private void End() => NavigateExam(NavOption.End);

    [RelayCommand(CanExecute = nameof(IsShowAnswerVisibleProxy))]
    private void ToggleShowAnswer()
    {
        if (_exam.Properties.HideAnswers) return;

        ShowAnswer = !ShowAnswer;
        if (ShowAnswer)
            HighlightCorrectAndIncorrectAnswers();
        else
            ClearAnswerHighlights();
    }
    private bool IsShowAnswerVisibleProxy() => !_exam.Properties.HideAnswers;

    // ---------------------------------------------------------------------
    // Navigation engine — port of the WinForms NavigateExam method
    // ---------------------------------------------------------------------

    private enum NavOption { Begin, Next, Previous, End }

    private void NavigateExam(NavOption option)
    {
        // Hide explanation by default when navigating.
        ShowAnswer = false;

        if (option == NavOption.Begin)
        {
            if (_settings.Questions.Count > 0)
            {
                _currentQuestionIndex = 0;
                PrintQuestionToScreen();
            }
        }
        else if (option == NavOption.Next)
        {
            _userAnswers[_currentQuestionIndex] = SelectedAnswer();
            _currentQuestionIndex++;
            PrintQuestionToScreen();
        }
        else if (option == NavOption.Previous)
        {
            _userAnswers[_currentQuestionIndex] = SelectedAnswer();
            _currentQuestionIndex--;
            PrintQuestionToScreen();
        }
        else if (option == NavOption.End)
        {
            _userAnswers[_currentQuestionIndex] = SelectedAnswer();
            // Replace any unanswered with sentinel '\0' (same as WinForms).
            for (var i = 0; i < _userAnswers.Length; i++)
                _userAnswers[i] ??= '\0';

            _settings.ElapsedTime = TimeSpan.FromSeconds(_exam.Properties.TimeLimit * 60 - _timeLeft);

            // Total correct.
            var numOfCorrect = 0;
            for (var i = 0; i < _settings.Questions.Count; i++)
            {
                if (_userAnswers[i]!.GetType().IsArray)
                {
                    if (((char[])_userAnswers[i]!).SequenceEqual(_settings.Questions[i].Answers))
                        numOfCorrect++;
                }
                else if ((char)_userAnswers[i]! == _settings.Questions[i].Answer)
                {
                    numOfCorrect++;
                }
            }
            _settings.NumberOfCorrectAnswers = numOfCorrect;

            // Per-section result spread.
            foreach (var section in _settings.Sections)
            {
                var nq = 0;
                var nc = 0;
                for (var i = 0; i < _settings.Questions.Count; i++)
                {
                    if (!section.Questions.Contains(_settings.Questions[i])) continue;
                    nq++;
                    if (_userAnswers[i]!.GetType().IsArray)
                    {
                        if (((char[])_userAnswers[i]!).SequenceEqual(_settings.Questions[i].Answers))
                            nc++;
                    }
                    else if ((char)_userAnswers[i]! == _settings.Questions[i].Answer)
                    {
                        nc++;
                    }
                }
                _settings.ResultSpread.Add(new SectionResult(section.Title, nq, nc));
            }

            _timer?.Stop();
            var ss = new ScoreSheetViewModel(_settings, _exam, _sp, _nav);
            _nav.GoTo(ss);
        }

        PreviousCommand.NotifyCanExecuteChanged();
        NextCommand.NotifyCanExecuteChanged();
    }

    private void PrintQuestionToScreen()
    {
        var q = _settings.Questions[_currentQuestionIndex];
        QuestionNumber = q.No.ToString();
        SectionTitle = _settings.Sections.First(s => s.Questions.Contains(q)).Title;
        Explanation = q.Explanation;
        QuestionText = q.Text;
        QuestionImageData = q.ImageData;

        Options.Clear();
        for (var i = 0; i < q.Options.Count; i++)
        {
            var o = q.Options[i];
            var checkedAnswer = _userAnswers[_currentQuestionIndex];
            bool isChecked;
            if (q.IsMultipleChoice)
                isChecked = checkedAnswer is char[] arr && arr.Contains(o.Alphabet);
            else
                isChecked = checkedAnswer is char c && c == o.Alphabet;

            Options.Add(new AnswerOptionViewModel
            {
                Letter = o.Alphabet,
                Text = o.Text,
                IsChecked = isChecked,
                IsMultipleChoice = q.IsMultipleChoice,
                State = AnswerOptionState.Neutral,
            });
        }

        ProgressText =
            $"{_userAnswers.Count(x => x is not null)} / {_settings.Questions.Count} answered";
    }

    private object SelectedAnswer()
    {
        var q = _settings.Questions[_currentQuestionIndex];

        if (q.IsMultipleChoice)
            return Options.Where(o => o.IsChecked).Select(o => o.Letter).ToArray();

        var sel = Options.FirstOrDefault(o => o.IsChecked);
        return sel is null ? '\0' : sel.Letter;
    }

    private void ClearAnswerHighlights()
    {
        foreach (var o in Options)
            o.State = AnswerOptionState.Neutral;
    }

    /// <summary>
    /// Replicates <c>AssessmentUi.HighlightCorrectAndIncorrectAnswers</c>:
    /// green for the correct answers, red for any wrongly-chosen ones.
    /// </summary>
    private void HighlightCorrectAndIncorrectAnswers()
    {
        var q = _settings.Questions[_currentQuestionIndex];

        if (q.IsMultipleChoice)
        {
            foreach (var o in Options)
            {
                var isAnswer = q.Answers.Contains(o.Letter);
                if (isAnswer)
                    o.State = AnswerOptionState.Correct;
                else if (o.IsChecked && !isAnswer)
                    o.State = AnswerOptionState.Incorrect;
                else
                    o.State = AnswerOptionState.Neutral;
            }
        }
        else
        {
            foreach (var o in Options)
            {
                if (o.Letter == q.Answer)
                    o.State = AnswerOptionState.Correct;
                else if (o.IsChecked && o.Letter != q.Answer)
                    o.State = AnswerOptionState.Incorrect;
                else
                    o.State = AnswerOptionState.Neutral;
            }
        }
    }

    // ---------------------------------------------------------------------
    // Timer
    // ---------------------------------------------------------------------

    private async void OnTick(object? sender, EventArgs e)
    {
        _timeLeft--;
        if (_timeLeft <= 0)
        {
            _timer?.Stop();
            ElapsedTimeText = "Time Up!";
            await _msg.ShowAsync(
                "Your time ran out!", "Time out",
                MessageBoxButtons.Ok, MessageBoxIconKind.Information);
            NavigateExam(NavOption.End);
        }
        else
        {
            var ts = TimeSpan.FromSeconds(_timeLeft);
            ElapsedTimeText =
                $"{ts.Hours:00}:{ts.Minutes:00}:{ts.Seconds:00}";
        }
    }
}
