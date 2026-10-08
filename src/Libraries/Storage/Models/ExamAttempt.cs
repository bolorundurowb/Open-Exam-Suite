namespace OpenExamSuite.Storage.Models;

/// <summary>
/// A single completed exam attempt, shared between the Creator and Simulator so both can show
/// a user's history.
/// </summary>
public class ExamAttempt
{
    public int Id { get; set; }

    public string ExamFilePath { get; set; } = string.Empty;

    public string CandidateName { get; set; } = string.Empty;

    /// <summary>Normalized score in the 0-1000 scale used by the app.</summary>
    public int Score { get; set; }

    public bool Passed { get; set; }

    public int Correct { get; set; }

    public int Total { get; set; }

    public double TimeUsedSeconds { get; set; }

    public DateTime TakenAt { get; set; }
}
