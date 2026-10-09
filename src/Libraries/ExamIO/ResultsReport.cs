using OpenExamSuite.Shared.Models;

namespace OpenExamSuite.Shared;

/// <summary>
/// Everything a results PDF needs, kept free of any UI or session types so the report can be
/// produced by any host.
/// </summary>
public sealed record ResultsReport(
    string ExamTitle,
    string ExamCode,
    string CandidateName,
    DateTime CompletedAt,
    TimeSpan TimeUsed,
    TimeSpan? TimeAllowed,
    int TotalQuestions,
    int CorrectAnswers,
    double PercentScore,
    int ScaledScore,
    int PassMarkScaled,
    bool Passed,
    IReadOnlyList<SectionResult> Sections,
    IReadOnlyList<ResultsReportQuestion> Questions);

/// <summary>
/// One row of the per-question outcome list in a results report.
/// </summary>
public sealed record ResultsReportQuestion(int Number, ResultsReportOutcome Outcome, string Text);

public enum ResultsReportOutcome
{
    Correct,
    Wrong,
    Unanswered
}

/// <summary>
/// Caption text for a results report. Defaults are English; hosts supply localised captions.
/// </summary>
public sealed record ResultsReportLabels
{
    public string Title { get; init; } = "Results report";
    public string Exam { get; init; } = "Exam";
    public string Code { get; init; } = "Code";
    public string Candidate { get; init; } = "Candidate";
    public string Date { get; init; } = "Date";
    public string TimeUsed { get; init; } = "Time used";
    public string Result { get; init; } = "Result";
    public string Passed { get; init; } = "Passed";
    public string NotPassed { get; init; } = "Not passed";
    public string Score { get; init; } = "Score";
    public string PassMark { get; init; } = "Pass mark";
    public string Correct { get; init; } = "Correct";
    public string Wrong { get; init; } = "Wrong";
    public string Unanswered { get; init; } = "Unanswered";
    public string Sections { get; init; } = "Sections";
    public string Questions { get; init; } = "Questions";
}
