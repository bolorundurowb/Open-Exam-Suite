using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using OmniAssert;
using OpenExamSuite.Simulator.Session.Models;
using OpenExamSuite.Simulator.Session.States;
using OpenExamSuite.Storage.Enums;
using OpenExamSuite.Storage.Models;
using Xunit;

namespace OpenExamSuite.Simulator.Session.Tests;

public class SimulatorSessionTests : IClassFixture<SimulatorSessionTestFixture>
{
    private readonly SimulatorSessionTestFixture _fixture;

    public SimulatorSessionTests(SimulatorSessionTestFixture fixture)
    {
        _fixture = fixture;
        _fixture.Library.Reset();
        _fixture.Settings.Reset();
        _fixture.AppPaths.Reset();
        _fixture.FileSystem.Reset();
        _fixture.Library.Setup(x => x.GetAttempts(It.IsAny<string>())).Returns(new List<Storage.Models.ExamAttempt>());
    }

    [Fact]
    public async Task StartAttempt_Practice_HasNoTimer()
    {
        var session = _fixture.CreateSession();
        var examPath = _fixture.SaveExam(SimulatorSessionTestFixture.CreateExam());
        SetupEmptyLibrary();

        await session.LoadExamAsync(examPath);
        await session.UpdatePreExamSettingsAsync(SimulatorSessionTestFixture.PracticeSettings());
        var attempt = await session.StartAttemptAsync();

        attempt.IsTimerRunning.Must().BeFalse();
        attempt.TimeRemaining.Must().Be(TimeSpan.Zero);
    }

    [Fact]
    public async Task StartAttempt_Exam_SetsSessionTimeLimit()
    {
        var session = _fixture.CreateSession();
        var examPath = _fixture.SaveExam(SimulatorSessionTestFixture.CreateExam());
        SetupEmptyLibrary();

        await session.LoadExamAsync(examPath);
        await session.UpdatePreExamSettingsAsync(SimulatorSessionTestFixture.ExamSettings(overrideMinutes: 5));
        var attempt = await session.StartAttemptAsync();

        attempt.IsTimerRunning.Must().BeTrue();
        attempt.TimeRemaining.TotalMinutes.Must().BeApproximately(5, 0.01);
    }

    [Fact]
    public async Task AnswerQuestion_Skipped_DoesNotAdvanceProgress()
    {
        var session = _fixture.CreateSession();
        var examPath = _fixture.SaveExam(SimulatorSessionTestFixture.CreateExam());
        SetupEmptyLibrary();

        await session.LoadExamAsync(examPath);
        await session.UpdatePreExamSettingsAsync(SimulatorSessionTestFixture.PracticeSettings());
        var attempt = await session.StartAttemptAsync();

        attempt.AnsweredCount.Must().Be(0);
        attempt.UnansweredCount.Must().Be(attempt.Questions.Count);

        await session.AnswerQuestionAsync(AnswerSelection.Unanswered());
        attempt = (AttemptState)session.CurrentState;

        attempt.AnsweredCount.Must().Be(0);
        attempt.UnansweredCount.Must().Be(attempt.Questions.Count);
        attempt.CurrentAnswer.IsUnanswered.Must().BeTrue();
    }

    [Fact]
    public async Task AnswerQuestion_Answered_AdvancesProgress()
    {
        var session = _fixture.CreateSession();
        var examPath = _fixture.SaveExam(SimulatorSessionTestFixture.CreateExam());
        SetupEmptyLibrary();

        await session.LoadExamAsync(examPath);
        await session.UpdatePreExamSettingsAsync(SimulatorSessionTestFixture.PracticeSettings());
        var attempt = await session.StartAttemptAsync();

        await session.AnswerQuestionAsync(AnswerSelection.Answered('A'));
        attempt = (AttemptState)session.CurrentState;

        attempt.AnsweredCount.Must().Be(1);
        attempt.UnansweredCount.Must().Be(attempt.Questions.Count - 1);
    }

    [Fact]
    public async Task ToggleFlag_IncrementsFlaggedCount()
    {
        var session = _fixture.CreateSession();
        var examPath = _fixture.SaveExam(SimulatorSessionTestFixture.CreateExam());
        SetupEmptyLibrary();

        await session.LoadExamAsync(examPath);
        await session.UpdatePreExamSettingsAsync(SimulatorSessionTestFixture.PracticeSettings());
        var attempt = await session.StartAttemptAsync();

        attempt.FlaggedCount.Must().Be(0);

        await session.ToggleFlagAsync();
        attempt = (AttemptState)session.CurrentState;

        attempt.FlaggedCount.Must().Be(1);
        attempt.CurrentAnswer.IsFlagged.Must().BeTrue();

        await session.ToggleFlagAsync();
        attempt = (AttemptState)session.CurrentState;

        attempt.FlaggedCount.Must().Be(0);
        attempt.CurrentAnswer.IsFlagged.Must().BeFalse();
    }

    [Fact]
    public async Task CheckAnswer_Practice_LocksAndRevealsExplanation()
    {
        var session = _fixture.CreateSession();
        var examPath = _fixture.SaveExam(SimulatorSessionTestFixture.CreateExam());
        SetupEmptyLibrary();

        await session.LoadExamAsync(examPath);
        await session.UpdatePreExamSettingsAsync(SimulatorSessionTestFixture.PracticeSettings());
        await session.StartAttemptAsync();

        await session.AnswerQuestionAsync(AnswerSelection.Answered('A'));
        var selection = await session.CheckAnswerAsync();

        selection.ExplanationRevealed.Must().BeTrue();
        selection.IsLocked.Must().BeTrue();
        ((AttemptState)session.CurrentState).CurrentAnswer.ExplanationRevealed.Must().BeTrue();

        await Xunit.Assert.ThrowsAsync<InvalidOperationException>(
            () => session.AnswerQuestionAsync(AnswerSelection.Answered('B')));

        await session.ToggleFlagAsync();
        ((AttemptState)session.CurrentState).CurrentAnswer.IsFlagged.Must().BeTrue();
        ((AttemptState)session.CurrentState).CurrentAnswer.SingleChoice.Must().Be('A');
    }

    [Fact]
    public async Task CheckAnswer_Exam_Throws()
    {
        var session = _fixture.CreateSession();
        var examPath = _fixture.SaveExam(SimulatorSessionTestFixture.CreateExam());
        SetupEmptyLibrary();

        await session.LoadExamAsync(examPath);
        await session.UpdatePreExamSettingsAsync(SimulatorSessionTestFixture.ExamSettings());
        await session.StartAttemptAsync();
        await session.AnswerQuestionAsync(AnswerSelection.Answered('A'));

        await Xunit.Assert.ThrowsAsync<InvalidOperationException>(() => session.CheckAnswerAsync());
    }

    [Fact]
    public async Task ReviewAndSubmit_ReportsUnansweredAndFlaggedCounts()
    {
        var session = _fixture.CreateSession();
        var examPath = _fixture.SaveExam(SimulatorSessionTestFixture.CreateExam());
        SetupEmptyLibrary();

        await session.LoadExamAsync(examPath);
        await session.UpdatePreExamSettingsAsync(SimulatorSessionTestFixture.ExamSettings());
        await session.StartAttemptAsync();

        // Answer first, flag second, leave remaining unanswered.
        await session.AnswerQuestionAsync(AnswerSelection.Answered('A'));
        await session.NavigateNextAsync();
        await session.ToggleFlagAsync();
        await session.EnterReviewAndSubmitAsync();

        var review = (ReviewAndSubmitState)session.CurrentState;
        review.UnansweredIndices.Count.Must().Be(5); // all except first
        review.FlaggedIndices.Count.Must().Be(1); // second
    }

    [Fact]
    public async Task ReviewAndSubmit_CanReturnToAttempt()
    {
        var session = _fixture.CreateSession();
        var examPath = _fixture.SaveExam(SimulatorSessionTestFixture.CreateExam());
        SetupEmptyLibrary();

        await session.LoadExamAsync(examPath);
        await session.UpdatePreExamSettingsAsync(SimulatorSessionTestFixture.ExamSettings());
        var attempt = await session.StartAttemptAsync();
        await session.EnterReviewAndSubmitAsync();
        await session.ReturnToAttemptAsync();

        session.CurrentState.Must().Be(attempt);
    }

    [Fact]
    public async Task Submit_SavesAttemptWithCorrectFilePath()
    {
        var session = _fixture.CreateSession();
        var examPath = _fixture.SaveExam(SimulatorSessionTestFixture.CreateExam());
        SetupEmptyLibrary();
        ExamAttempt? savedAttempt = null;
        _fixture.Library.Setup(x => x.SaveAttempt(It.IsAny<ExamAttempt>()))
            .Callback<ExamAttempt>(a => savedAttempt = a);

        await session.LoadExamAsync(examPath);
        await session.UpdatePreExamSettingsAsync(SimulatorSessionTestFixture.ExamSettings());
        await session.StartAttemptAsync();
        await session.AnswerQuestionAsync(AnswerSelection.Answered('A'));
        await session.SubmitAsync();

        savedAttempt.Must().NotBeNull();
        savedAttempt!.ExamFilePath.Must().Be(examPath);
        savedAttempt.Total.Must().Be(6);
        savedAttempt.Correct.Must().Be(1);
        savedAttempt.Score.Must().Be(166); // integer truncation of 1/6 * 1000
        savedAttempt.TimeUsedSeconds.Must().BeApproximately(0, 1);
    }

    [Fact]
    public async Task Timer_FiveMinuteWarning_IsSet()
    {
        var session = _fixture.CreateSession();
        var examPath = _fixture.SaveExam(SimulatorSessionTestFixture.CreateExam());
        SetupEmptyLibrary();

        await session.LoadExamAsync(examPath);
        await session.UpdatePreExamSettingsAsync(SimulatorSessionTestFixture.ExamSettings(overrideMinutes: 10));
        await session.StartAttemptAsync();

        _fixture.TimeProvider.Advance(TimeSpan.FromMinutes(5).Add(TimeSpan.FromSeconds(1)));
        await Task.Delay(50); // let the timer loop run

        var attempt = (AttemptState)session.CurrentState;
        attempt.IsLowTimeWarning.Must().BeTrue();
        attempt.IsCriticalTimeWarning.Must().BeFalse();
    }

    [Fact]
    public async Task Timer_OneMinuteWarning_IsSet()
    {
        var session = _fixture.CreateSession();
        var examPath = _fixture.SaveExam(SimulatorSessionTestFixture.CreateExam());
        SetupEmptyLibrary();

        await session.LoadExamAsync(examPath);
        await session.UpdatePreExamSettingsAsync(SimulatorSessionTestFixture.ExamSettings(overrideMinutes: 2));
        await session.StartAttemptAsync();

        _fixture.TimeProvider.Advance(TimeSpan.FromMinutes(1).Add(TimeSpan.FromSeconds(1)));
        await Task.Delay(50);

        var attempt = (AttemptState)session.CurrentState;
        attempt.IsCriticalTimeWarning.Must().BeTrue();
    }

    [Fact]
    public async Task Timer_TimeUp_AutoSubmits()
    {
        var session = _fixture.CreateSession();
        var examPath = _fixture.SaveExam(SimulatorSessionTestFixture.CreateExam());
        SetupEmptyLibrary();
        _fixture.Library.Setup(x => x.SaveAttempt(It.IsAny<ExamAttempt>()));

        await session.LoadExamAsync(examPath);
        await session.UpdatePreExamSettingsAsync(SimulatorSessionTestFixture.ExamSettings(overrideMinutes: 1));
        await session.StartAttemptAsync();

        _fixture.TimeProvider.Advance(TimeSpan.FromMinutes(1));
        ((TimeUpState)session.CurrentState).AutoSubmitCountdownSeconds.Must().Be(6);

        _fixture.TimeProvider.Advance(TimeSpan.FromSeconds(6));

        session.CurrentState.Kind.Must().Be(SessionStateKind.Results);
        _fixture.Library.Verify(x => x.SaveAttempt(It.IsAny<ExamAttempt>()), Times.Once);
    }

    [Fact]
    public async Task Pause_StopsTimerAndAccumulatesPausedTime()
    {
        var session = _fixture.CreateSession();
        var examPath = _fixture.SaveExam(SimulatorSessionTestFixture.CreateExam());
        SetupEmptyLibrary();

        await session.LoadExamAsync(examPath);
        await session.UpdatePreExamSettingsAsync(SimulatorSessionTestFixture.ExamSettings(overrideMinutes: 10));
        await session.StartAttemptAsync();

        _fixture.TimeProvider.Advance(TimeSpan.FromMinutes(2));
        await session.PauseAsync();

        _fixture.TimeProvider.Advance(TimeSpan.FromMinutes(3));
        await session.ResumeAsync();

        var attempt = (AttemptState)session.CurrentState;
        attempt.TimeRemaining.TotalMinutes.Must().BeApproximately(8, 0.1); // 10 - 2, pause does not count
    }

    [Fact]
    public async Task SystemSleep_DoesNotConsumeExamTime()
    {
        var session = _fixture.CreateSession();
        var examPath = _fixture.SaveExam(SimulatorSessionTestFixture.CreateExam());
        SetupEmptyLibrary();

        await session.LoadExamAsync(examPath);
        await session.UpdatePreExamSettingsAsync(SimulatorSessionTestFixture.ExamSettings(overrideMinutes: 10));
        await session.StartAttemptAsync();

        _fixture.TimeProvider.Advance(TimeSpan.FromMinutes(2));
        await session.NotifySystemSuspendingAsync();
        _fixture.TimeProvider.Advance(TimeSpan.FromHours(1));
        await session.NotifySystemResumedAsync();
        _fixture.TimeProvider.Advance(TimeSpan.FromSeconds(1));

        var attempt = (AttemptState)session.CurrentState;
        attempt.TimeRemaining.TotalMinutes.Must().BeApproximately(8, 0.1);
    }

    [Fact]
    public async Task HideAnswers_BlocksPracticeMode()
    {
        var session = _fixture.CreateSession();
        var examPath = _fixture.SaveExam(SimulatorSessionTestFixture.CreateExam(hideAnswers: true));
        SetupEmptyLibrary();

        await session.LoadExamAsync(examPath);
        var preExam = (PreExamState)session.CurrentState;

        preExam.Settings.Mode.Must().Be(ExamMode.Exam);
        preExam.StartDisabled.Must().BeFalse();
    }

    [Fact]
    public async Task HideAnswers_PreExamPracticeSelectionIsDisabled()
    {
        var session = _fixture.CreateSession();
        var examPath = _fixture.SaveExam(SimulatorSessionTestFixture.CreateExam(hideAnswers: true));
        SetupEmptyLibrary();

        await session.LoadExamAsync(examPath);
        await session.UpdatePreExamSettingsAsync(new PreExamSettings { Mode = ExamMode.Practice });

        var preExam = (PreExamState)session.CurrentState;
        preExam.StartDisabled.Must().BeTrue();
        preExam.StartDisabledReason.Must().Contain("hidden answers");
        await Xunit.Assert.ThrowsAsync<InvalidOperationException>(() => session.StartAttemptAsync());
    }

    [Fact]
    public async Task AnswerReview_HideAnswers_WithholdsCorrectAnswerAndExplanation()
    {
        var session = _fixture.CreateSession();
        var examPath = _fixture.SaveExam(SimulatorSessionTestFixture.CreateExam(hideAnswers: true));
        SetupEmptyLibrary();
        _fixture.Library.Setup(x => x.SaveAttempt(It.IsAny<ExamAttempt>()));

        await session.LoadExamAsync(examPath);
        await session.UpdatePreExamSettingsAsync(SimulatorSessionTestFixture.ExamSettings());
        await session.StartAttemptAsync();
        await session.AnswerQuestionAsync(AnswerSelection.Answered('A'));
        await session.SubmitAsync();
        await session.EnterAnswerReviewAsync();

        var review = (AnswerReviewState)session.CurrentState;
        var detail = review.GradingDetails[0];
        detail.IsHidden.Must().BeTrue();
        detail.CorrectAnswer.Must().BeNull();
        detail.Explanation.Must().BeNull();
    }

    [Fact]
    public async Task AnswerReview_FiltersRecomputeWhenChanged()
    {
        var session = _fixture.CreateSession();
        var examPath = _fixture.SaveExam(SimulatorSessionTestFixture.CreateExam());
        SetupEmptyLibrary();
        _fixture.Library.Setup(x => x.SaveAttempt(It.IsAny<ExamAttempt>()));

        await session.LoadExamAsync(examPath);
        await session.UpdatePreExamSettingsAsync(SimulatorSessionTestFixture.PracticeSettings());
        await session.StartAttemptAsync();
        await session.AnswerQuestionAsync(AnswerSelection.Answered('A'));
        await session.NavigateNextAsync();
        await session.AnswerQuestionAsync(AnswerSelection.Answered('B', isFlagged: true));
        await session.SubmitAsync();
        await session.EnterAnswerReviewAsync();

        await session.SetReviewFilterAsync(AnswerReviewFilter.Correct);
        ((AnswerReviewState)session.CurrentState).FilteredCount.Must().Be(1);
        await session.SetReviewFilterAsync(AnswerReviewFilter.Wrong);
        ((AnswerReviewState)session.CurrentState).FilteredCount.Must().Be(1);
        await session.SetReviewFilterAsync(AnswerReviewFilter.Unanswered);
        ((AnswerReviewState)session.CurrentState).FilteredCount.Must().Be(4);
        await session.SetReviewFilterAsync(AnswerReviewFilter.Flagged);
        ((AnswerReviewState)session.CurrentState).FilteredCount.Must().Be(1);
    }

    [Fact]
    public async Task Retake_RestoresPreExamSelections()
    {
        var session = _fixture.CreateSession();
        var examPath = _fixture.SaveExam(SimulatorSessionTestFixture.CreateExam());
        SetupEmptyLibrary();
        _fixture.Library.Setup(x => x.SaveAttempt(It.IsAny<ExamAttempt>()));

        var originalSettings = new PreExamSettings
        {
            Mode = ExamMode.Exam,
            CandidateName = "Alice",
            QuestionSet = QuestionSetMode.RandomN,
            RandomQuestionCount = 4,
            RandomSeed = 42,
            ShuffleQuestions = true,
            ShuffleOptions = true,
            TimerOverrideMinutes = 7
        };

        await session.LoadExamAsync(examPath);
        await session.UpdatePreExamSettingsAsync(originalSettings);
        await session.StartAttemptAsync();
        await session.AnswerQuestionAsync(AnswerSelection.Answered('A'));
        await session.SubmitAsync();

        var preExam = await session.RetakeAsync();

        preExam.Settings.Mode.Must().Be(originalSettings.Mode);
        preExam.Settings.CandidateName.Must().Be(originalSettings.CandidateName);
        preExam.Settings.QuestionSet.Must().Be(originalSettings.QuestionSet);
        preExam.Settings.RandomQuestionCount.Must().Be(originalSettings.RandomQuestionCount);
        preExam.Settings.ShuffleQuestions.Must().Be(originalSettings.ShuffleQuestions);
        preExam.Settings.ShuffleOptions.Must().Be(originalSettings.ShuffleOptions);
        preExam.Settings.TimerOverrideMinutes.Must().Be(originalSettings.TimerOverrideMinutes);
        preExam.Settings.RandomSeed.Must().Be(0); // fresh random draw
    }

    [Fact]
    public async Task PracticeMissedAgain_StartsPracticeWithMissedQuestions()
    {
        var session = _fixture.CreateSession();
        var examPath = _fixture.SaveExam(SimulatorSessionTestFixture.CreateExam());
        SetupEmptyLibrary();
        _fixture.Library.Setup(x => x.SaveAttempt(It.IsAny<ExamAttempt>()));

        await session.LoadExamAsync(examPath);
        await session.UpdatePreExamSettingsAsync(SimulatorSessionTestFixture.ExamSettings());
        await session.StartAttemptAsync();

        // Answer first correctly, skip the rest.
        await session.AnswerQuestionAsync(AnswerSelection.Answered('A'));
        for (int i = 1; i < 6; i++)
        {
            await session.NavigateNextAsync();
        }

        await session.SubmitAsync();
        await session.EnterAnswerReviewAsync();
        var preExam = await session.PracticeMissedAgainAsync();

        preExam.Settings.Mode.Must().Be(ExamMode.Practice);
        preExam.Settings.QuestionSet.Must().Be(QuestionSetMode.MissedQuestions);
        preExam.Settings.MissedQuestionIndices.Length.Must().Be(5);

        var practiceAttempt = await session.StartAttemptAsync();
        practiceAttempt.Questions.Count.Must().Be(5);
    }

    [Fact]
    public async Task PracticeMissedAgain_UsesPresentedRandomQuestions()
    {
        var session = _fixture.CreateSession();
        var examPath = _fixture.SaveExam(SimulatorSessionTestFixture.CreateExam(questionsPerSection: 10));
        SetupEmptyLibrary();
        _fixture.Library.Setup(x => x.SaveAttempt(It.IsAny<ExamAttempt>()));

        await session.LoadExamAsync(examPath);
        await session.UpdatePreExamSettingsAsync(new PreExamSettings
        {
            Mode = ExamMode.Practice,
            QuestionSet = QuestionSetMode.RandomN,
            RandomQuestionCount = 4,
            RandomSeed = 27
        });
        var attempt = await session.StartAttemptAsync();
        await session.AnswerQuestionAsync(AnswerSelection.Answered(attempt.CurrentQuestion.Answer));
        var expectedMissed = attempt.Questions.Skip(1).Select(q => q.Text).OrderBy(text => text).ToList();

        await session.SubmitAsync();
        await session.EnterAnswerReviewAsync();
        await session.PracticeMissedAgainAsync();
        var practiceAttempt = await session.StartAttemptAsync();

        practiceAttempt.Questions.Select(q => q.Text).OrderBy(text => text)
            .SequenceEqual(expectedMissed)
            .Must().BeTrue();
    }

    [Fact]
    public async Task RandomDraw_SameSeed_IsStable()
    {
        var sessionA = _fixture.CreateSession();
        var sessionB = _fixture.CreateSession();
        var examPath = _fixture.SaveExam(SimulatorSessionTestFixture.CreateExam());
        SetupEmptyLibrary();

        await sessionA.LoadExamAsync(examPath);
        await sessionA.UpdatePreExamSettingsAsync(new PreExamSettings
        {
            Mode = ExamMode.Practice,
            QuestionSet = QuestionSetMode.RandomN,
            RandomQuestionCount = 4,
            RandomSeed = 12345
        });
        var attemptA = await sessionA.StartAttemptAsync();

        await sessionB.LoadExamAsync(examPath);
        await sessionB.UpdatePreExamSettingsAsync(new PreExamSettings
        {
            Mode = ExamMode.Practice,
            QuestionSet = QuestionSetMode.RandomN,
            RandomQuestionCount = 4,
            RandomSeed = 12345
        });
        var attemptB = await sessionB.StartAttemptAsync();

        var textsA = attemptA.Questions.Select(q => q.Text).ToList();
        var textsB = attemptB.Questions.Select(q => q.Text).ToList();
        textsA.SequenceEqual(textsB).Must().BeTrue();
    }

    [Fact]
    public async Task RandomDraw_DifferentConfiguredSeeds_Differ()
    {
        var sessionA = _fixture.CreateSession();
        var sessionB = _fixture.CreateSession();
        var examPath = _fixture.SaveExam(SimulatorSessionTestFixture.CreateExam(questionsPerSection: 10));
        SetupEmptyLibrary();

        await sessionA.LoadExamAsync(examPath);
        await sessionA.UpdatePreExamSettingsAsync(new PreExamSettings
        {
            Mode = ExamMode.Practice,
            QuestionSet = QuestionSetMode.RandomN,
            RandomQuestionCount = 6,
            RandomSeed = 1
        });
        var attemptA = await sessionA.StartAttemptAsync();

        await sessionB.LoadExamAsync(examPath);
        await sessionB.UpdatePreExamSettingsAsync(new PreExamSettings
        {
            Mode = ExamMode.Practice,
            QuestionSet = QuestionSetMode.RandomN,
            RandomQuestionCount = 6,
            RandomSeed = 2
        });
        var attemptB = await sessionB.StartAttemptAsync();

        attemptA.Questions.Select(q => q.Text)
            .SequenceEqual(attemptB.Questions.Select(q => q.Text))
            .Must().BeFalse();
    }

    [Fact]
    public async Task ShuffleQuestions_UsesSeedAndStoresPresentedOrder()
    {
        var sessionA = _fixture.CreateSession();
        var sessionB = _fixture.CreateSession();
        var examPath = _fixture.SaveExam(SimulatorSessionTestFixture.CreateExam());
        SetupEmptyLibrary();
        var settings = new PreExamSettings
        {
            Mode = ExamMode.Practice,
            ShuffleQuestions = true,
            RandomSeed = 17
        };

        await sessionA.LoadExamAsync(examPath);
        await sessionA.UpdatePreExamSettingsAsync(settings);
        var attemptA = await sessionA.StartAttemptAsync();

        await sessionB.LoadExamAsync(examPath);
        await sessionB.UpdatePreExamSettingsAsync(settings);
        var attemptB = await sessionB.StartAttemptAsync();

        var orderA = attemptA.Questions.Select(q => q.Text).ToList();
        orderA.SequenceEqual(attemptB.Questions.Select(q => q.Text)).Must().BeTrue();
        orderA.SequenceEqual(Enumerable.Range(1, 6).Select(i => $"Question {i}")).Must().BeFalse();
    }

    [Fact]
    public async Task ShuffleOptions_PreservesCorrectAnswer()
    {
        var session = _fixture.CreateSession();
        var examPath = _fixture.SaveExam(SimulatorSessionTestFixture.CreateExam());
        SetupEmptyLibrary();

        await session.LoadExamAsync(examPath);
        await session.UpdatePreExamSettingsAsync(new PreExamSettings
        {
            Mode = ExamMode.Practice,
            ShuffleOptions = true,
            RandomSeed = 99
        });
        var attempt = await session.StartAttemptAsync();

        var question = attempt.Questions[0];
        var correctOption = question.Options.First(o => o.Alphabet == question.Answer);
        correctOption.Text.Must().Be("Alpha");
    }

    [Fact]
    public async Task MultipleChoice_Submission_GradesCorrectly()
    {
        var session = _fixture.CreateSession();
        var examPath = _fixture.SaveExam(SimulatorSessionTestFixture.CreateExamWithMultipleChoice());
        SetupEmptyLibrary();
        _fixture.Library.Setup(x => x.SaveAttempt(It.IsAny<ExamAttempt>()));

        await session.LoadExamAsync(examPath);
        await session.UpdatePreExamSettingsAsync(SimulatorSessionTestFixture.PracticeSettings());
        await session.StartAttemptAsync();

        await session.AnswerQuestionAsync(AnswerSelection.Answered(ImmutableArray.Create('A', 'B')));
        await session.SubmitAsync();

        var results = (ResultsState)session.CurrentState;
        results.CorrectAnswers.Must().Be(1);
        results.Passed.Must().BeTrue();
    }

    [Fact]
    public async Task Initialize_SeedsSamplesFromAppPaths()
    {
        var session = _fixture.CreateSession();
        const string samplesRoot = "/installed/Samples";
        var samplePaths = new[] { "/installed/Samples/Basic Science.oef", "/installed/Samples/GMAT Sample.oef" };
        _fixture.AppPaths.SetupGet(x => x.BundledSamplesRoot).Returns(samplesRoot);
        _fixture.FileSystem.Setup(x => x.GetFilesAsync(samplesRoot, "*.oef", default)).ReturnsAsync(samplePaths);
        SetupEmptyLibrary();

        await session.InitializeAsync();

        foreach (var path in samplePaths)
        {
            _fixture.Library.Verify(
                x => x.AddExam(ExamCatalog.Simulator, path, System.IO.Path.GetFileNameWithoutExtension(path)),
                Times.Once);
        }
    }

    private void SetupEmptyLibrary()
    {
        _fixture.Library.Setup(x => x.GetExams(It.IsAny<ExamCatalog>())).Returns(new List<Storage.Models.ExamEntry>());
    }
}
