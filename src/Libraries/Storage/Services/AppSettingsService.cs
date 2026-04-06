using LiteDB;
using OpenExamSuite.Storage.Enums;
using OpenExamSuite.Storage.Interfaces;
using OpenExamSuite.Storage.Models;

namespace OpenExamSuite.Storage.Services;

public class AppSettingsService : IAppSettingsService
{
    private readonly string _database;

    public AppSettingsService(string? databasePath = null)
    {
        _database = databasePath ?? Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "OpenExamSuite",
            "OpenExamSuite.db");

        var directory = Path.GetDirectoryName(_database);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            Directory.CreateDirectory(directory);
    }

    public void Add(AppSetting settings, AppSettingsType type)
    {
        using var db = new LiteDatabase(_database);
        var collection = db.GetCollection<AppSetting>(GetTableNameFromType(type));
        var record = collection.FindOne(x => x.FilePath == settings.FilePath);

        if (record != null)
            return;

        collection.Insert(settings);
    }

    public void Remove(string filePath, AppSettingsType type)
    {
        using var db = new LiteDatabase(_database);
        var collection = db.GetCollection<AppSetting>(GetTableNameFromType(type));
        collection.DeleteMany(x => x.FilePath == filePath);
    }

    public void Clear(AppSettingsType type)
    {
        using var db = new LiteDatabase(_database);
        db.DropCollection(GetTableNameFromType(type));
    }

    public List<AppSetting> GetAll(AppSettingsType type)
    {
        using var db = new LiteDatabase(_database);
        var collection = db.GetCollection<AppSetting>(GetTableNameFromType(type));
        return collection.FindAll().ToList();
    }

    private static string GetTableNameFromType(AppSettingsType type)
    {
        return type switch
        {
            AppSettingsType.Creator => "CreatorSettings",
            AppSettingsType.Simulator => "SimulatorSettings",
            _ => "OtherSettings"
        };
    }
}
