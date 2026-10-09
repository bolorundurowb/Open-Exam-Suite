using OpenExamSuite.Creator.Session.Services;
using OpenExamSuite.Shared.Utilities;
using OpenExamSuite.Storage.Enums;
using OpenExamSuite.Storage.Interfaces;
using OpenExamSuite.Storage.Models;
using Xunit;

namespace OpenExamSuite.Creator.Session.Tests;

public class RecentExamsServiceTests
{
    [Fact]
    public void GetRecents_ReadsCountsFromFile()
    {
        var examPath = WriteExam(questionCount: 2, sectionCount: 1);
        try
        {
            var library = new FakeLibrary();
            library.AddExam(ExamCatalog.Creator, examPath, "Sample");

            var recents = new RecentExamsService(library, new Reader()).GetRecents();

            var single = Assert.Single(recents);
            Assert.Equal(examPath, single.FilePath);
            Assert.Equal("Sample", single.Title);
            Assert.Equal(2, single.QuestionCount);
            Assert.Equal(1, single.SectionCount);
            Assert.True(single.IsReadable);
            Assert.True(single.ModifiedAt > DateTime.MinValue);
        }
        finally
        {
            File.Delete(examPath);
        }
    }

    [Fact]
    public void GetRecents_MissingFileIsNotReadable()
    {
        var library = new FakeLibrary();
        var missing = Path.Combine(Path.GetTempPath(), $"oes-missing-{Guid.NewGuid():N}.oef");
        library.AddExam(ExamCatalog.Creator, missing, "Gone");

        var recents = new RecentExamsService(library, new Reader()).GetRecents();

        var single = Assert.Single(recents);
        Assert.False(single.IsReadable);
        Assert.Equal(0, single.QuestionCount);
        Assert.Equal(0, single.SectionCount);
        Assert.Equal("Gone", single.Title);
    }

    [Fact]
    public void GetRecents_SortsByModifiedDescending()
    {
        var newer = WriteExam(questionCount: 1, sectionCount: 1);
        var older = WriteExam(questionCount: 1, sectionCount: 1);
        try
        {
            File.SetLastWriteTime(newer, DateTime.Now);
            File.SetLastWriteTime(older, DateTime.Now.AddDays(-1));

            var library = new FakeLibrary();
            library.AddExam(ExamCatalog.Creator, older, "Older");
            library.AddExam(ExamCatalog.Creator, newer, "Newer");

            var recents = new RecentExamsService(library, new Reader()).GetRecents();

            Assert.Equal(2, recents.Count);
            Assert.Equal(newer, recents[0].FilePath);
            Assert.Equal(older, recents[1].FilePath);
        }
        finally
        {
            File.Delete(newer);
            File.Delete(older);
        }
    }

    private static string WriteExam(int questionCount, int sectionCount)
    {
        var path = Path.Combine(Path.GetTempPath(), $"oes-exam-{Guid.NewGuid():N}.oef");
        var doc = new CreatorDocument(new Reader(), new Writer());
        doc.NewDocument();
        for (var s = 0; s < sectionCount; s++)
        {
            var section = doc.AddSection($"S{s + 1}");
            var perSection = (int)Math.Ceiling((double)questionCount / sectionCount);
            for (var q = 0; q < perSection; q++)
                doc.AddQuestion(section.Id);
        }

        Assert.True(doc.Save(path).Success);
        return path;
    }

    private sealed class FakeLibrary : IExamLibraryService
    {
        private readonly List<ExamEntry> _entries = [];

        public void AddExam(ExamCatalog catalog, string filePath, string name) =>
            _entries.Add(new ExamEntry { Catalog = catalog, FilePath = filePath, Name = name });

        public void RemoveExam(ExamCatalog catalog, string filePath) { }

        public void ClearExams(ExamCatalog catalog) { }

        public List<ExamEntry> GetExams(ExamCatalog catalog) =>
            _entries.Where(e => e.Catalog == catalog).ToList();

        public void SeedBundledSamples(ExamCatalog catalog, string applicationDirectory) { }

        public void SaveAttempt(ExamAttempt attempt) { }

        public List<ExamAttempt> GetAttempts(string examFilePath) => [];

        public void ClearAttempts() { }
    }
}
