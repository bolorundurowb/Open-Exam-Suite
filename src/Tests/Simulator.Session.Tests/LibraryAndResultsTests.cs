using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using OmniAssert;
using OpenExamSuite.Shared.Utilities;
using OpenExamSuite.Simulator.Session.Models;
using OpenExamSuite.Simulator.Session.States;
using OpenExamSuite.Storage.Enums;
using OpenExamSuite.Storage.Models;
using Xunit;

namespace OpenExamSuite.Simulator.Session.Tests;

public class LibraryAndResultsTests : IClassFixture<SimulatorSessionTestFixture>, IDisposable
{
    private readonly SimulatorSessionTestFixture _fixture;
    private readonly string _directory;

    public LibraryAndResultsTests(SimulatorSessionTestFixture fixture)
    {
        _fixture = fixture;
        _fixture.Library.Reset();
        _fixture.Settings.Reset();
        _fixture.AppPaths.Reset();
        _fixture.FileSystem.Reset();
        _fixture.Library.Setup(x => x.GetAttempts(It.IsAny<string>())).Returns(new List<ExamAttempt>());
        _fixture.Library.Setup(x => x.GetExams(It.IsAny<ExamCatalog>())).Returns(new List<ExamEntry>());
        _directory = Path.Combine(Path.GetTempPath(), $"oes-library-tests-{Guid.NewGuid():N}");
        Directory.CreateDirectory(_directory);
        _fixture.AppPaths.SetupGet(x => x.UserDataDirectory).Returns(_directory);
        _fixture.FileSystem
            .Setup(x => x.ExistsAsync(It.IsAny<string>(), default))
            .ReturnsAsync((string p, System.Threading.CancellationToken _) => File.Exists(p));
    }

    public void Dispose()
    {
        try { Directory.Delete(_directory, recursive: true); } catch { /* best effort */ }
    }

    [Fact]
    public async Task Tick_WhileReviewing_StaysOnReviewAndKeepsCountingDown()
    {
        var session = _fixture.CreateSession();
        var examPath = _fixture.SaveExam(SimulatorSessionTestFixture.CreateExam());

        await session.LoadExamAsync(examPath);
        await session.UpdatePreExamSettingsAsync(SimulatorSessionTestFixture.ExamSettings(overrideMinutes: 10));
        await session.StartAttemptAsync();
        await session.EnterReviewAndSubmitAsync();

        _fixture.TimeProvider.Advance(TimeSpan.FromSeconds(30));

        session.CurrentState.Kind.Must().Be(SessionStateKind.ReviewAndSubmit);
        var review = (ReviewAndSubmitState)session.CurrentState;
        (review.Attempt.TimeRemaining < TimeSpan.FromMinutes(10)).Must().BeTrue();
    }

    [Fact]
    public async Task PassMark_IsReportedAsPercent()
    {
        var session = _fixture.CreateSession();
        var examPath = _fixture.SaveExam(SimulatorSessionTestFixture.CreateExam());

        var preExam = await session.LoadExamAsync(examPath);

        preExam.Exam.PassMarkPercent.Must().BeApproximately(70, 0.001);
    }

    [Fact]
    public async Task Initialize_AfterSeeding_DoesNotReAddRemovedSamples()
    {
        var session = _fixture.CreateSession();
        _fixture.Settings
            .Setup(x => x.Get("Simulator.SamplesSeeded", AppSettingsType.Other))
            .Returns(new AppSetting { Key = "Simulator.SamplesSeeded", Value = "1" });
        _fixture.AppPaths.SetupGet(x => x.BundledSamplesRoot).Returns("/installed/Samples");
        _fixture.FileSystem
            .Setup(x => x.GetFilesAsync("/installed/Samples", "*.oef", default))
            .ReturnsAsync(["/installed/Samples/Basic Science.oef"]);

        await session.InitializeAsync();

        _fixture.Library.Verify(
            x => x.AddExam(It.IsAny<ExamCatalog>(), It.IsAny<string>(), It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public async Task AddExam_Oef_ReferencesFileInPlace()
    {
        var session = _fixture.CreateSession();
        var examPath = _fixture.SaveExam(SimulatorSessionTestFixture.CreateExam());

        var result = await session.AddExamAsync(examPath);

        result.Success.Must().BeTrue();
        result.FilePath.Must().Be(examPath);
        _fixture.Library.Verify(
            x => x.AddExam(ExamCatalog.Simulator, examPath, Path.GetFileNameWithoutExtension(examPath)),
            Times.Once);
    }

    [Fact]
    public async Task AddExam_Json_ImportsAsOefInUserData()
    {
        var session = _fixture.CreateSession();
        var jsonPath = Path.Combine(_directory, "Imported Exam.json");
        new Writer().ToJson(SimulatorSessionTestFixture.CreateExam(), jsonPath);

        var result = await session.AddExamAsync(jsonPath);

        result.Success.Must().BeTrue();
        Path.GetExtension(result.FilePath).Must().Be(".oef");
        File.Exists(result.FilePath).Must().BeTrue();
        _fixture.Library.Verify(
            x => x.AddExam(ExamCatalog.Simulator, result.FilePath!, It.IsAny<string>()),
            Times.Once);
    }

    [Fact]
    public async Task AddExam_MissingFile_ReportsFileNotFound()
    {
        var session = _fixture.CreateSession();

        var result = await session.AddExamAsync(Path.Combine(_directory, "nope.oef"));

        result.Status.Must().Be(LibraryActionStatus.FileNotFound);
    }

    [Fact]
    public async Task AddExam_UnsupportedExtension_IsRejected()
    {
        var session = _fixture.CreateSession();
        var path = Path.Combine(_directory, "notes.txt");
        await File.WriteAllTextAsync(path, "hello");

        var result = await session.AddExamAsync(path);

        result.Status.Must().Be(LibraryActionStatus.UnsupportedFormat);
    }

    [Fact]
    public async Task AddExam_CorruptFile_IsRejected()
    {
        var session = _fixture.CreateSession();
        var path = Path.Combine(_directory, "broken.oef");
        await File.WriteAllBytesAsync(path, [1, 2, 3, 4]);

        var result = await session.AddExamAsync(path);

        result.Status.Must().Be(LibraryActionStatus.CorruptFile);
    }

    [Fact]
    public async Task RemoveExam_RemovesEntryOnly()
    {
        var session = _fixture.CreateSession();
        var examPath = _fixture.SaveExam(SimulatorSessionTestFixture.CreateExam());

        await session.RemoveExamAsync(examPath);

        _fixture.Library.Verify(x => x.RemoveExam(ExamCatalog.Simulator, examPath), Times.Once);
        File.Exists(examPath).Must().BeTrue();
    }

    [Fact]
    public async Task ExportResultsPdf_ProducesPdfBytes()
    {
        var session = _fixture.CreateSession();
        var examPath = _fixture.SaveExam(SimulatorSessionTestFixture.CreateExam());

        await session.LoadExamAsync(examPath);
        await session.UpdatePreExamSettingsAsync(SimulatorSessionTestFixture.PracticeSettings());
        await session.StartAttemptAsync();
        await session.AnswerQuestionAsync(AnswerSelection.Answered('A'));
        await session.SubmitAsync();

        var bytes = await session.ExportResultsPdfAsync();

        Encoding.ASCII.GetString(bytes, 0, 4).Must().Be("%PDF");
    }
}
