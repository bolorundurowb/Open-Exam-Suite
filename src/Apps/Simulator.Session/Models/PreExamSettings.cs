using System.Collections.Immutable;

namespace OpenExamSuite.Simulator.Session.Models;

/// <summary>
/// Settings chosen on the pre-exam sheet before starting an attempt.
/// </summary>
public sealed record PreExamSettings
{
    /// <summary>
    /// Exam mode: Practice (untimed, instant feedback) or Exam (timed, feedback at end).
    /// </summary>
    public ExamMode Mode { get; init; } = ExamMode.Practice;

    /// <summary>
    /// Candidate name. Pre-filled from last use.
    /// </summary>
    public string CandidateName { get; init; } = string.Empty;

    /// <summary>
    /// Question set selection mode.
    /// </summary>
    public QuestionSetMode QuestionSet { get; init; } = QuestionSetMode.AllQuestions;

    /// <summary>
    /// When QuestionSet == SelectedSections, the titles of sections to include.
    /// </summary>
    public ImmutableArray<string> SelectedSectionTitles { get; init; } = ImmutableArray<string>.Empty;

    /// <summary>
    /// When QuestionSet == RandomN, the number of questions to draw.
    /// </summary>
    public int RandomQuestionCount { get; init; } = 0;

    /// <summary>
    /// Whether to shuffle question order.
    /// </summary>
    public bool ShuffleQuestions { get; init; } = false;

    /// <summary>
    /// Whether to shuffle option order within each question.
    /// </summary>
    public bool ShuffleOptions { get; init; } = false;

    /// <summary>
    /// Timer override in minutes (Exam mode only). 0 means use exam default.
    /// </summary>
    public int TimerOverrideMinutes { get; init; } = 0;

    /// <summary>
    /// Random seed for reproducible random draws. 0 means generate new seed.
    /// </summary>
    public int RandomSeed { get; init; } = 0;

    /// <summary>
    /// When QuestionSet == MissedQuestions, the original indices of the questions to include.
    /// </summary>
    public ImmutableArray<int> MissedQuestionIndices { get; init; } = ImmutableArray<int>.Empty;

    /// <summary>
    /// Returns true if the selection would yield zero questions.
    /// A random count larger than the pool is clamped, not rejected.
    /// </summary>
    public bool WouldYieldZeroQuestions(ExamSummary exam) => QuestionSet switch
    {
        QuestionSetMode.AllQuestions => exam.TotalQuestions == 0,
        QuestionSetMode.SelectedSections => SelectedSectionTitles.IsEmpty || SelectedSectionTitles.All(t => exam.GetSectionQuestionCount(t) == 0),
        QuestionSetMode.RandomN => RandomQuestionCount <= 0 || exam.TotalQuestions == 0,
        QuestionSetMode.MissedQuestions => MissedQuestionIndices.IsEmpty,
        _ => true
    };

    /// <summary>
    /// Creates a summary line for display (e.g., "60 questions, 90 minutes, pass mark 70%").
    /// </summary>
    public string GetSummaryLine(ExamSummary exam)
    {
        var questionCount = QuestionSet switch
        {
            QuestionSetMode.AllQuestions => exam.TotalQuestions,
            QuestionSetMode.SelectedSections => SelectedSectionTitles.Sum(t => exam.GetSectionQuestionCount(t)),
            QuestionSetMode.RandomN => Math.Min(RandomQuestionCount, exam.TotalQuestions),
            QuestionSetMode.MissedQuestions => MissedQuestionIndices.Length,
            _ => 0
        };

        var timeLimit = Mode == ExamMode.Exam ? (TimerOverrideMinutes > 0 ? TimerOverrideMinutes : exam.TimeLimitMinutes) : 0;
        var passMark = exam.PassMarkPercent;

        if (Mode == ExamMode.Exam)
            return $"{questionCount} questions, {timeLimit} minutes, pass mark {passMark}%";
        else
            return $"{questionCount} questions, untimed (Practice)";
    }
}

public enum ExamMode
{
    Practice,
    Exam
}

public enum QuestionSetMode
{
    AllQuestions,
    SelectedSections,
    RandomN,
    MissedQuestions
}

/// <summary>
/// Read-only exam summary for pre-exam sheet display.
/// </summary>
public sealed record ExamSummary(
    string Title,
    string Code,
    string Instructions,
    int TotalQuestions,
    int TotalSections,
    int TimeLimitMinutes,
    double PassMarkPercent,
    bool HideAnswers,
    IReadOnlyDictionary<string, int> SectionQuestionCounts)
{
    public int GetSectionQuestionCount(string sectionTitle) =>
        SectionQuestionCounts.TryGetValue(sectionTitle, out var count) ? count : 0;
}