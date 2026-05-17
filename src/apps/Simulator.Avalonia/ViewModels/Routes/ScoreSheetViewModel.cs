using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using Microsoft.Extensions.DependencyInjection;
using OpenExamSuite.Shared;
using OpenExamSuite.Shared.Models;
using OpenExamSuite.Simulator.Services;
using SkiaSharp;

namespace OpenExamSuite.Simulator.ViewModels.Routes;

public sealed partial class ScoreSheetViewModel : ObservableObject, IRouteViewModel
{
    private readonly Settings _settings;
    private readonly Exam _exam;
    private readonly IServiceProvider _sp;
    private readonly INavigationService _nav;

    public string CandidateName  => _settings.CandidateName;
    public string Date           => DateTime.Now.ToShortDateString();
    public string ElapsedTime    => _settings.ElapsedTime.TotalMinutes.ToString("F");
    public string ExamCode       => _exam.Properties.Code;
    public string TimeAllowed    => _settings.TimeLimit.ToString();

    [ObservableProperty] private string _status = string.Empty;
    [ObservableProperty] private IBrush _statusBrush = Brushes.Black;

    public ISeries[] ChartSeries { get; private set; } = Array.Empty<ISeries>();
    public List<SectionResult> Breakdown => _settings.ResultSpread;

    public ScoreSheetViewModel(Settings settings, Exam exam, IServiceProvider sp, INavigationService nav)
    {
        _settings = settings;
        _exam = exam;
        _sp = sp;
        _nav = nav;
        Compute();
    }

    private void Compute()
    {
        var normalizedScore = _settings.Questions.Count > 0
            ? _settings.NumberOfCorrectAnswers * 1000 / _settings.Questions.Count
            : 0;

        var passed = normalizedScore >= _exam.Properties.Passmark;
        Status = passed ? "Passed" : "Failed";
        StatusBrush = passed ? Brushes.Green : Brushes.Red;

        ChartSeries = new ISeries[]
        {
            new RowSeries<double>
            {
                Name = "Pass Mark",
                Values = new double[] { _exam.Properties.Passmark },
                Fill = new SolidColorPaint(SKColors.SteelBlue),
            },
            new RowSeries<double>
            {
                Name = "Your Score",
                Values = new double[] { normalizedScore },
                Fill = new SolidColorPaint(passed ? SKColors.Green : SKColors.IndianRed),
            },
        };
    }

    [RelayCommand]
    private void Retake()
    {
        // Mirrors the WinForms btn_retake → Close() (which returned to HomeUi).
        _nav.GoBack(); // exam settings
        _nav.GoBack(); // home (best effort)
    }

    [RelayCommand]
    private void Exit()
    {
        if (Avalonia.Application.Current?.ApplicationLifetime is
                Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop)
            desktop.Shutdown();
    }

    [RelayCommand]
    private async Task PrintAsync()
    {
        var svc = _sp.GetRequiredService<IScoreSheetPrintService>();
        await svc.PrintAsync(_settings, _exam);
    }
}
