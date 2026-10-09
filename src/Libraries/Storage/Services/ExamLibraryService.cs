using LiteDB;
using OpenExamSuite.Storage.Enums;
using OpenExamSuite.Storage.Interfaces;
using OpenExamSuite.Storage.Models;

namespace OpenExamSuite.Storage.Services;

/// <summary>
/// One LiteDB file opened with <c>Connection=Shared</c> by Creator and Simulator.
/// Exam lists stay separate: Creator stores exams it has worked on, Simulator stores its
/// library, and the shipped samples are seeded into both. Attempt history is shared.
/// </summary>
public class ExamLibraryService : IExamLibraryService
{
    private const string CreatorExamsCollection = "creatorExams";
    private const string SimulatorExamsCollection = "simulatorExams";
    private const string AttemptsCollection = "attempts";
    private const string MetaCollection = "libraryMeta";

    private readonly string _databasePath;
    private readonly object _migrateLock = new();
    private bool _migrated;

    public ExamLibraryService(string? databasePath = null)
    {
        _databasePath = databasePath ?? Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "OpenExamSuite",
            "OpenExamSuite.db");

        var directory = Path.GetDirectoryName(_databasePath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            Directory.CreateDirectory(directory);
    }

    private string SharedConnectionString => $"Filename={_databasePath};Connection=Shared";

    public void AddExam(ExamCatalog catalog, string filePath, string name)
    {
        using var db = OpenShared();
        EnsureMigrated(db);
        Upsert(db, catalog, filePath, name, isSample: false);
    }

    public void RemoveExam(ExamCatalog catalog, string filePath)
    {
        using var db = OpenShared();
        EnsureMigrated(db);

        Collection(db, catalog).DeleteMany(x => x.FilePath == filePath);
    }

    public void ClearExams(ExamCatalog catalog)
    {
        using var db = OpenShared();
        EnsureMigrated(db);

        Collection(db, catalog).DeleteMany(x => x.IsSample == false);
    }

    public List<ExamEntry> GetExams(ExamCatalog catalog)
    {
        using var db = OpenShared();
        EnsureMigrated(db);

        return Collection(db, catalog).FindAll().ToList();
    }

    public void SeedBundledSamples(ExamCatalog catalog, string applicationDirectory)
    {
        var samplePaths = BundledSamples.Resolve(applicationDirectory);
        if (samplePaths.Count == 0)
            return;

        using var db = OpenShared();
        EnsureMigrated(db);

        var meta = db.GetCollection<AppSetting>(MetaCollection);
        var key = $"samplesSeeded.{catalog}";
        if (meta.FindOne(x => x.Key == key) != null)
            return;

        foreach (var path in samplePaths)
            Upsert(db, catalog, path, Path.GetFileNameWithoutExtension(path), isSample: true);

        meta.Insert(new AppSetting { Key = key, Value = "1" });
    }

    public void SaveAttempt(ExamAttempt attempt)
    {
        using var db = OpenShared();
        EnsureMigrated(db);

        db.GetCollection<ExamAttempt>(AttemptsCollection).Insert(attempt);
    }

    public List<ExamAttempt> GetAttempts(string examFilePath)
    {
        using var db = OpenShared();
        EnsureMigrated(db);

        return db.GetCollection<ExamAttempt>(AttemptsCollection)
            .Find(x => x.ExamFilePath == examFilePath)
            .ToList();
    }

    public void ClearAttempts()
    {
        using var db = OpenShared();
        EnsureMigrated(db);

        db.GetCollection<ExamAttempt>(AttemptsCollection).DeleteAll();
    }

    private LiteDatabase OpenShared() => new(SharedConnectionString);

    private void EnsureMigrated(LiteDatabase db)
    {
        if (_migrated)
            return;

        lock (_migrateLock)
        {
            if (_migrated)
                return;

            // Shared mode has a single data reader, so avoid Count()/Exists() and always
            // materialize with ToList() before iterating.
            RemoveNonExamEntries(db, ExamCatalog.Creator);
            RemoveNonExamEntries(db, ExamCatalog.Simulator);
            MigrateLegacyTable(db, "CreatorSettings", ExamCatalog.Creator);
            MigrateLegacyTable(db, "SimulatorSettings", ExamCatalog.Simulator);

            _migrated = true;
        }
    }

    private static void MigrateLegacyTable(LiteDatabase db, string tableName, ExamCatalog catalog)
    {
        var exams = Collection(db, catalog);
        if (exams.FindAll().ToList().Count > 0 || !db.CollectionExists(tableName))
            return;

        var legacy = db.GetCollection<AppSetting>(tableName);
        foreach (var setting in legacy.FindAll().ToList())
        {
            // The same tables also hold preferences, such as CreatorTheme. Only file paths
            // are exam history.
            if (!IsExamFilePath(setting.Key))
                continue;

            exams.Insert(new ExamEntry
            {
                Catalog = catalog,
                FilePath = setting.Key,
                Name = setting.Value
            });
        }
    }

    private static void Upsert(LiteDatabase db, ExamCatalog catalog, string filePath, string name, bool isSample)
    {
        var collection = Collection(db, catalog);
        var existing = collection.FindOne(x => x.FilePath == filePath);
        if (existing == null)
        {
            collection.Insert(new ExamEntry
            {
                Catalog = catalog,
                FilePath = filePath,
                Name = name,
                IsSample = isSample
            });
            return;
        }

        existing.Name = name;
        if (isSample)
            existing.IsSample = true;
        collection.Update(existing);
    }

    private static ILiteCollection<ExamEntry> Collection(LiteDatabase db, ExamCatalog catalog)
    {
        var name = catalog switch
        {
            ExamCatalog.Creator => CreatorExamsCollection,
            ExamCatalog.Simulator => SimulatorExamsCollection,
            _ => throw new ArgumentOutOfRangeException(nameof(catalog))
        };

        return db.GetCollection<ExamEntry>(name);
    }

    /// <summary>
    /// Drops rows that were copied from a preference key, such as CreatorTheme, before
    /// migration learned to ignore them.
    /// </summary>
    private static void RemoveNonExamEntries(LiteDatabase db, ExamCatalog catalog)
    {
        var exams = Collection(db, catalog);
        foreach (var entry in exams.FindAll().ToList())
        {
            if (!IsExamFilePath(entry.FilePath))
                exams.Delete(entry.Id);
        }
    }

    private static bool IsExamFilePath(string? path)
    {
        if (string.IsNullOrWhiteSpace(path))
            return false;

        var extension = Path.GetExtension(path);
        return extension.Equals(".oef", StringComparison.OrdinalIgnoreCase)
            || extension.Equals(".json", StringComparison.OrdinalIgnoreCase)
            || extension.Equals(".xml", StringComparison.OrdinalIgnoreCase);
    }
}
