using OpenExamSuite.Simulator.Engine.Models;
using OpenExamSuite.Shared;
using OpenExamSuite.Shared.Models;

namespace OpenExamSuite.Simulator.Engine.States;

/// <summary>
/// Base interface for all session states.
/// </summary>
public interface ISessionState
{
    SessionStateKind Kind { get; }
}

public enum SessionStateKind
{
    Library,
    PreExam,
    Attempt,
    Paused,
    TimeUp,
    ReviewAndSubmit,
    Results,
    AnswerReview
}

/// <summary>
/// Library state - the home screen with exam cards.
/// </summary>
public sealed record LibraryState(
    IReadOnlyList<ExamCard> Exams,
    IReadOnlyList<AttemptSummary> RecentAttempts,
    string? SearchQuery,
    string? SortBy,
    bool IsLoading) : ISessionState
{
    public SessionStateKind Kind => SessionStateKind.Library;
}

/// <summary>
/// Pre-exam sheet state - configuration before starting an attempt.
/// </summary>
public sealed record PreExamState(
    ExamSummary Exam,
    PreExamSettings Settings,
    bool StartDisabled,
    string? StartDisabledReason) : ISessionState
{
    public SessionStateKind Kind => SessionStateKind.PreExam;
}

/// <summary>
/// Active attempt state - taking the exam.
/// </summary>
public sealed record AttemptState(
    Exam Exam,
    PreExamSettings Settings,
    IReadOnlyList<Question> Questions,
    IReadOnlyList<Section> Sections,
    int CurrentQuestionIndex,
    IReadOnlyDictionary<int, AnswerSelection> Answers,
    TimeSpan TimeRemaining,
    bool IsTimerRunning,
    bool IsLowTimeWarning,      // <= 5 minutes
    bool IsCriticalTimeWarning, // <= 1 minute
    int AnsweredCount,
    int FlaggedCount,
    int UnansweredCount) : ISessionState
{
    public SessionStateKind Kind => SessionStateKind.Attempt;

    public Question CurrentQuestion => Questions[CurrentQuestionIndex];
    public AnswerSelection CurrentAnswer => Answers.TryGetValue(CurrentQuestionIndex, out var a) ? a : AnswerSelection.Unanswered();
    public double ProgressPercent => Questions.Count > 0 ? (double)AnsweredCount / Questions.Count * 100 : 0;
}

/// <summary>
/// Paused state - exam is paused, question hidden.
/// </summary>
public sealed record PausedState(
    AttemptState Attempt) : ISessionState
{
    public SessionStateKind Kind => SessionStateKind.Paused;
}

/// <summary>
/// Time-up state - exam time expired, transitioning to review.
/// </summary>
public sealed record TimeUpState(
    AttemptState Attempt,
    int AutoSubmitCountdownSeconds) : ISessionState
{
    public SessionStateKind Kind => SessionStateKind.TimeUp;
}

/// <summary>
/// Review and submit state - candidate reviews unanswered/flagged questions before submitting.
/// </summary>
public sealed record ReviewAndSubmitState(
    AttemptState Attempt,
    IReadOnlyList<int> UnansweredIndices,
    IReadOnlyList<int> FlaggedIndices,
    bool SubmitConfirmed) : ISessionState
{
    public SessionStateKind Kind => SessionStateKind.ReviewAndSubmit;
}

/// <summary>
/// Results state - exam completed, showing score and breakdown.
/// Carries the presented questions and answers so answer review can be entered without
/// reconstructing the attempt.
/// </summary>
public sealed record ResultsState(
    Exam Exam,
    PreExamSettings Settings,
    string CandidateName,
    DateTime CompletedAt,
    TimeSpan ElapsedTime,
    int TotalQuestions,
    int CorrectAnswers,
    double PercentScore,
    int ScaledScore,
    double PassMarkPercent,
    bool Passed,
    IReadOnlyList<SectionResult> SectionBreakdown,
    int? PreviousScorePercent,
    int? PreviousScaledScore,
    DateTime? PreviousDate,
    bool IsRetake,
    IReadOnlyList<Question>? PresentedQuestions = null,
    IReadOnlyList<Section>? PresentedSections = null,
    IReadOnlyDictionary<int, AnswerSelection>? Answers = null,
    IReadOnlyList<GradingDetail>? GradingDetails = null) : ISessionState
{
    public SessionStateKind Kind => SessionStateKind.Results;
}

/// <summary>
/// Answer review state - reviewing individual questions after results.
/// </summary>
public sealed record AnswerReviewState(
    Exam Exam,
    PreExamSettings Settings,
    IReadOnlyList<Question> Questions,
    IReadOnlyList<Section> Sections,
    IReadOnlyDictionary<int, AnswerSelection> Answers,
    IReadOnlyList<GradingDetail> GradingDetails,
    AnswerReviewFilter Filter,
    int CurrentReviewIndex) : ISessionState
{
    public SessionStateKind Kind => SessionStateKind.AnswerReview;

    public IReadOnlyList<int> FilteredIndices => GradingDetails
        .Select((d, i) => (Detail: d, Index: i))
        .Where(t => t.Detail.MatchesFilter(Filter))
        .Select(t => t.Index)
        .ToList();

    public int FilteredCount => FilteredIndices.Count;
    public bool HasCurrent => CurrentReviewIndex >= 0 && CurrentReviewIndex < FilteredCount;
    public int CurrentQuestionIndex => HasCurrent ? FilteredIndices[CurrentReviewIndex] : -1;
}

/// <summary>
/// Grading detail for a single question in answer review.
/// </summary>
public sealed record GradingDetail(
    int QuestionIndex,
    Question Question,
    AnswerSelection Selection,
    object? GradingAnswer,
    bool IsCorrect,
    char? CorrectAnswer,
    char[]? CorrectAnswers,
    string? Explanation,
    bool IsHidden)
{
    public bool IsAnswered => Selection.IsAnswered;
    public bool IsFlagged => Selection.IsFlagged;
    public bool IsUnanswered => Selection.IsUnanswered;

    public bool MatchesFilter(AnswerReviewFilter filter) => filter switch
    {
        AnswerReviewFilter.All => true,
        AnswerReviewFilter.Correct => IsCorrect,
        AnswerReviewFilter.Wrong => IsAnswered && !IsCorrect,
        AnswerReviewFilter.Unanswered => IsUnanswered,
        AnswerReviewFilter.Flagged => IsFlagged,
        _ => true
    };
}

public enum AnswerReviewFilter
{
    All,
    Correct,
    Wrong,
    Unanswered,
    Flagged
}

/// <summary>
/// Exam card for library display.
/// </summary>
public sealed record ExamCard(
    string FilePath,
    string Title,
    string Code,
    int QuestionCount,
    int SectionCount,
    int TimeLimitMinutes,
    double PassMarkPercent,
    DateTime? LastAttemptDate,
    int? LastScorePercent,
    bool? LastPassed,
    bool IsMissing,
    bool IsCorrupt);

/// <summary>
/// Recent attempt summary for library display.
/// </summary>
public sealed record AttemptSummary(
    string ExamFilePath,
    string ExamTitle,
    string ExamCode,
    DateTime Date,
    TimeSpan TimeUsed,
    int ScorePercent,
    int ScaledScore,
    bool Passed);