using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OpenExamSuite.Simulator.Session.Models;
using OpenExamSuite.Simulator.Session.States;
using OpenExamSuite.Shared;

namespace OpenExamSuite.Simulator.Session.Services;

/// <summary>
/// UI-neutral session layer for Simulator taking flow.
/// Orchestrates state transitions and business logic without any UI framework references.
/// </summary>
public interface ISimulatorSession
{
    /// <summary>
    /// Current session state.
    /// </summary>
    ISessionState CurrentState { get; }

    /// <summary>
    /// Event raised when the session state changes.
    /// </summary>
    event Action<ISessionState> StateChanged;

    /// <summary>
    /// Initializes the session at the Library state.
    /// </summary>
    Task InitializeAsync();

    /// <summary>
    /// Loads an exam and enters the pre-exam sheet state.
    /// </summary>
    Task<PreExamState> LoadExamAsync(string examFilePath);

    /// <summary>
    /// Updates pre-exam settings.
    /// </summary>
    Task UpdatePreExamSettingsAsync(PreExamSettings settings);

    /// <summary>
    /// Starts the attempt from the pre-exam sheet.
    /// </summary>
    Task<AttemptState> StartAttemptAsync();

    /// <summary>
    /// Navigates to a specific question index.
    /// </summary>
    Task NavigateToQuestionAsync(int questionIndex);

    /// <summary>
    /// Navigates to the next question.
    /// </summary>
    Task NavigateNextAsync();

    /// <summary>
    /// Navigates to the previous question.
    /// </summary>
    Task NavigatePreviousAsync();

    /// <summary>
    /// Records an answer for the current question.
    /// </summary>
    Task AnswerQuestionAsync(AnswerSelection selection);

    /// <summary>
    /// Toggles the flag on the current question.
    /// </summary>
    Task ToggleFlagAsync();

    /// <summary>
    /// Practice mode: checks the answer, reveals explanation, locks the question.
    /// </summary>
    Task<AnswerSelection> CheckAnswerAsync();

    /// <summary>
    /// Pauses the attempt.
    /// </summary>
    Task PauseAsync();

    /// <summary>
    /// Resumes a paused attempt.
    /// </summary>
    Task ResumeAsync();

    /// <summary>
    /// Records that the operating system is suspending so sleep does not consume exam time.
    /// </summary>
    Task NotifySystemSuspendingAsync();

    /// <summary>
    /// Resumes timing after an operating-system suspend interval.
    /// </summary>
    Task NotifySystemResumedAsync();

    /// <summary>
    /// Enters the review-and-submit state.
    /// </summary>
    Task EnterReviewAndSubmitAsync();

    /// <summary>
    /// Returns to the attempt from review-and-submit.
    /// </summary>
    Task ReturnToAttemptAsync();

    /// <summary>
    /// Submits the exam (with confirmation handled by host).
    /// </summary>
    Task<ResultsState> SubmitAsync();

    /// <summary>
    /// Handles time-up transition.
    /// </summary>
    Task HandleTimeUpAsync();

    /// <summary>
    /// Enters answer review from results.
    /// </summary>
    Task<AnswerReviewState> EnterAnswerReviewAsync();

    /// <summary>
    /// Navigates within answer review.
    /// </summary>
    Task NavigateReviewAsync(int direction);

    /// <summary>
    /// Changes the answer review filter.
    /// </summary>
    Task SetReviewFilterAsync(AnswerReviewFilter filter);

    /// <summary>
    /// Retakes the exam with the same pre-exam selections.
    /// </summary>
    Task<PreExamState> RetakeAsync();

    /// <summary>
    /// Returns to library from results.
    /// </summary>
    Task ReturnToLibraryAsync();

    /// <summary>
    /// Exports results to PDF.
    /// </summary>
    Task<byte[]> ExportResultsPdfAsync();

    /// <summary>
    /// Prints results.
    /// </summary>
    Task PrintResultsAsync();

    /// <summary>
    /// Practice these again - starts a practice session with missed questions.
    /// </summary>
    Task<PreExamState> PracticeMissedAgainAsync();
}