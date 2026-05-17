using System;
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

public enum SelectionMode
{
    SelectedSections,
    FixedNumberOfQuestions,
}

/// <summary>
/// Replaces the WinForms <c>ExamSettingsUi</c> modal. The view binds the
/// "Enabled" state of dependent controls directly to the radio-button-derived
/// VM flags (no event handlers needed).
/// </summary>
public sealed partial class ExamSettingsViewModel : ObservableObject, IRouteViewModel
{
    private readonly Exam _exam;
    private readonly IServiceProvider _sp;
    private readonly INavigationService _nav;
    private readonly IMessageBoxService _msg;

    public ObservableCollection<SectionSelectionViewModel> Sections { get; }

    [ObservableProperty] private string _candidateName = string.Empty;
    [ObservableProperty] private bool _enableCustomTimer;
    [ObservableProperty] private decimal _customTimerMinutes;
    [ObservableProperty] private SelectionMode _mode = SelectionMode.SelectedSections;
    [ObservableProperty] private decimal _fixedNumberOfQuestions = 1;

    public bool UseFixed     => Mode == SelectionMode.FixedNumberOfQuestions;
    public bool UseSelected  => Mode == SelectionMode.SelectedSections;
    public int  MaxQuestions => _exam.NumberOfQuestions;

    partial void OnModeChanged(SelectionMode value)
    {
        OnPropertyChanged(nameof(UseFixed));
        OnPropertyChanged(nameof(UseSelected));
    }

    public ExamSettingsViewModel(Exam exam, IServiceProvider sp, INavigationService nav, IMessageBoxService msg)
    {
        _exam = exam;
        _sp = sp;
        _nav = nav;
        _msg = msg;

        Sections = new ObservableCollection<SectionSelectionViewModel>(
            exam.Sections.Select(s => new SectionSelectionViewModel(s, isChecked: true)));
        CustomTimerMinutes = exam.Properties.TimeLimit;
        FixedNumberOfQuestions = exam.NumberOfQuestions > 0 ? 1 : 0;
    }

    [RelayCommand] private void SelectAll()   { foreach (var s in Sections) s.IsChecked = true;  }
    [RelayCommand] private void DeselectAll() { foreach (var s in Sections) s.IsChecked = false; }

    [RelayCommand]
    private async Task ProceedAsync()
    {
        var settings = new Settings
        {
            CandidateName = CandidateName,
            TimeLimit = EnableCustomTimer ? (int)CustomTimerMinutes : _exam.Properties.TimeLimit,
        };

        if (UseSelected)
        {
            settings.Sections = Sections.Where(s => s.IsChecked).Select(s => s.Section).ToList();
            foreach (var section in settings.Sections)
                settings.Questions.AddRange(section.Questions);
        }
        else if (UseFixed)
        {
            // Faithful port of the WinForms fixed-N algorithm.
            var target = (int)FixedNumberOfQuestions;
            var sum = 0;
            foreach (var section in _exam.Sections)
            {
                if (sum + section.Questions.Count < target)
                {
                    settings.Sections.Add(section);
                    settings.Questions.AddRange(section.Questions);
                    sum += section.Questions.Count;
                }
                else if (sum + section.Questions.Count == target)
                {
                    settings.Sections.Add(section);
                    settings.Questions.AddRange(section.Questions);
                    break;
                }
                else
                {
                    var difference = target - sum;
                    settings.Sections.Add(section);
                    settings.Questions.AddRange(section.Questions.GetRange(0, difference));
                    break;
                }
            }
        }

        if (settings.Questions.Count == 0)
        {
            await _msg.ShowAsync(
                "There are no questions to be displayed based on your selection. Please make a different selection.",
                "Error", MessageBoxButtons.Ok, MessageBoxIconKind.Error);
            return;
        }

        var assessment = new AssessmentViewModel(_exam, settings, _sp, _nav, _msg);
        _nav.GoTo(assessment);
    }

    [RelayCommand]
    private void Cancel() => _nav.GoBack();
}
