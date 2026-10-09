using LiteDB;
using OpenExamSuite.Storage;
using OpenExamSuite.Storage.Enums;
using OpenExamSuite.Storage.Models;
using OpenExamSuite.Storage.Services;
using OmniAssert;
using Xunit;

namespace OpenExamSuite.Storage.Tests;

public class ExamLibraryServiceTests : IDisposable
{
    private readonly string _databasePath;
    private readonly ExamLibraryService _sut;

    public ExamLibraryServiceTests()
    {
        _databasePath = Path.Combine(Path.GetTempPath(), $"oes-library-{Guid.NewGuid():N}.db");
        _sut = new ExamLibraryService(_databasePath);
    }

    public void Dispose()
    {
        if (File.Exists(_databasePath))
            File.Delete(_databasePath);
    }

    [Fact]
    public void AddExam_DistinctPaths_BothStored()
    {
        _sut.AddExam(ExamCatalog.Simulator, "/a.oef", "A");
        _sut.AddExam(ExamCatalog.Simulator, "/b.oef", "B");

        var exams = _sut.GetExams(ExamCatalog.Simulator);
        exams.Count.Must().Be(2);
    }

    [Fact]
    public void AddExam_DuplicatePath_UpdatesInsteadOfInserting()
    {
        _sut.AddExam(ExamCatalog.Creator, "/same.oef", "A");
        _sut.AddExam(ExamCatalog.Creator, "/same.oef", "B");

        var exams = _sut.GetExams(ExamCatalog.Creator);
        exams.Count.Must().Be(1);
        exams[0].Name.Must().Be("B");
    }

    [Fact]
    public void AddExam_OtherCatalog_StaysOnItsOwnList()
    {
        _sut.AddExam(ExamCatalog.Creator, "/creator.oef", "Creator");
        _sut.AddExam(ExamCatalog.Simulator, "/simulator.oef", "Simulator");

        _sut.GetExams(ExamCatalog.Creator).Single().FilePath.Must().Be("/creator.oef");
        _sut.GetExams(ExamCatalog.Simulator).Single().FilePath.Must().Be("/simulator.oef");
    }

    [Fact]
    public void RemoveExam_DeletesMatchingPathFromThatCatalogOnly()
    {
        _sut.AddExam(ExamCatalog.Creator, "/x.oef", "X");
        _sut.AddExam(ExamCatalog.Simulator, "/x.oef", "X");

        _sut.RemoveExam(ExamCatalog.Simulator, "/x.oef");

        _sut.GetExams(ExamCatalog.Simulator).Must().BeEmpty();
        _sut.GetExams(ExamCatalog.Creator).Count.Must().Be(1);
    }

    [Fact]
    public void ClearExams_RemovesWorkedOnExamsAndKeepsSamples()
    {
        _sut.AddExam(ExamCatalog.Creator, "/a.oef", "A");
        _sut.AddExam(ExamCatalog.Simulator, "/b.oef", "B");

        var samples = CreateSampleDirectory();
        _sut.SeedBundledSamples(ExamCatalog.Creator, samples);
        _sut.ClearExams(ExamCatalog.Creator);

        var creator = _sut.GetExams(ExamCatalog.Creator);
        creator.All(x => x.IsSample).Must().BeTrue();
        creator.Count.Must().Be(BundledSamples.FileNames.Length);
        _sut.GetExams(ExamCatalog.Simulator).Count.Must().Be(1);
    }

    [Fact]
    public void SeedBundledSamples_AddsTheSameFilesToEachCatalogOnce()
    {
        var samples = CreateSampleDirectory();

        _sut.SeedBundledSamples(ExamCatalog.Creator, samples);
        _sut.SeedBundledSamples(ExamCatalog.Simulator, samples);
        _sut.SeedBundledSamples(ExamCatalog.Creator, samples);

        var creator = _sut.GetExams(ExamCatalog.Creator);
        var simulator = _sut.GetExams(ExamCatalog.Simulator);

        creator.Count.Must().Be(BundledSamples.FileNames.Length);
        simulator.Count.Must().Be(BundledSamples.FileNames.Length);
        foreach (var name in BundledSamples.FileNames)
        {
            creator.Select(x => Path.GetFileName(x.FilePath)).Must().Contain(name);
            simulator.Select(x => Path.GetFileName(x.FilePath)).Must().Contain(name);
        }
        creator.All(x => x.IsSample).Must().BeTrue();
    }

    [Fact]
    public void SaveAttempt_IsReadableForExam()
    {
        _sut.SaveAttempt(new ExamAttempt
        {
            ExamFilePath = "/a.oef",
            CandidateName = "Tester",
            Score = 700,
            Passed = true,
            Correct = 7,
            Total = 10,
            TimeUsedSeconds = 300,
            TakenAt = DateTime.UtcNow
        });

        var attempts = _sut.GetAttempts("/a.oef");
        attempts.Count.Must().Be(1);
        attempts[0].Score.Must().Be(700);
        attempts[0].Passed.Must().BeTrue();
    }

    [Fact]
    public void TwoConnections_SeeSameExamsAndAttempts()
    {
        var second = new ExamLibraryService(_databasePath);

        _sut.AddExam(ExamCatalog.Simulator, "/shared.oef", "Shared");
        _sut.SaveAttempt(new ExamAttempt
        {
            ExamFilePath = "/shared.oef",
            CandidateName = "Tester",
            Score = 500,
            TakenAt = DateTime.UtcNow
        });

        second.GetExams(ExamCatalog.Simulator).Single().FilePath.Must().Be("/shared.oef");
        second.GetExams(ExamCatalog.Creator).Must().BeEmpty();
        second.GetAttempts("/shared.oef").Single().Score.Must().Be(500);

        second.AddExam(ExamCatalog.Simulator, "/from-second.oef", "From Second");
        _sut.GetExams(ExamCatalog.Simulator).Select(x => x.FilePath).Must().Contain("/from-second.oef");
    }

    [Fact]
    public void GetExams_MigratesLegacyPerAppTablesIntoSeparateLists()
    {
        using (var db = new LiteDatabase(_databasePath))
        {
            db.GetCollection<AppSetting>("CreatorSettings")
                .Insert(new AppSetting { Key = "/creator.oef", Value = "Creator" });
            db.GetCollection<AppSetting>("CreatorSettings")
                .Insert(new AppSetting { Key = "CreatorTheme", Value = "Light" });
            db.GetCollection<AppSetting>("SimulatorSettings")
                .Insert(new AppSetting { Key = "/sim.oef", Value = "Sim" });
        }

        _sut.GetExams(ExamCatalog.Creator).Single().FilePath.Must().Be("/creator.oef");
        _sut.GetExams(ExamCatalog.Simulator).Single().FilePath.Must().Be("/sim.oef");
    }

    [Fact]
    public void GetExams_DropsPreferenceRowsAlreadyCopiedIntoTheExamList()
    {
        using (var db = new LiteDatabase(_databasePath))
        {
            var exams = db.GetCollection<ExamEntry>("creatorExams");
            exams.Insert(new ExamEntry { Catalog = ExamCatalog.Creator, FilePath = "/real.oef", Name = "Real" });
            exams.Insert(new ExamEntry { Catalog = ExamCatalog.Creator, FilePath = "CreatorTheme", Name = "Light" });
        }

        var stored = _sut.GetExams(ExamCatalog.Creator);
        stored.Single().FilePath.Must().Be("/real.oef");
    }

    private static string CreateSampleDirectory()
    {
        var applicationDirectory = Path.Combine(Path.GetTempPath(), $"oes-app-{Guid.NewGuid():N}");
        var samples = Path.Combine(applicationDirectory, "Samples");
        Directory.CreateDirectory(samples);
        foreach (var name in BundledSamples.FileNames)
            File.WriteAllText(Path.Combine(samples, name), "sample");

        return applicationDirectory;
    }
}
