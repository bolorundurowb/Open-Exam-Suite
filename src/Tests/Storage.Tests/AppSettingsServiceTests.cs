using LiteDB;
using OpenExamSuite.Storage.Enums;
using OpenExamSuite.Storage.Models;
using OpenExamSuite.Storage.Services;
using OmniAssert;
using Xunit;

namespace OpenExamSuite.Storage.Tests;

public class AppSettingsServiceTests : IDisposable
{
    private readonly string _databasePath;
    private readonly AppSettingsService _sut;

    public AppSettingsServiceTests()
    {
        _databasePath = Path.Combine(Path.GetTempPath(), $"oes-test-{Guid.NewGuid():N}.db");
        _sut = new AppSettingsService(_databasePath);
    }

    public void Dispose()
    {
        if (File.Exists(_databasePath))
            File.Delete(_databasePath);
    }

    [Fact]
    public void Set_DistinctKeys_BothStored()
    {
        _sut.Set(new AppSetting { Key = "/a.oef", Value = "A" }, AppSettingsType.Simulator);
        _sut.Set(new AppSetting { Key = "/b.oef", Value = "B" }, AppSettingsType.Simulator);

        var all = _sut.GetAll(AppSettingsType.Simulator);
        all.Count.Must().Be(2);
    }

    [Fact]
    public void Set_DuplicateKey_UpdatesInsteadOfInserting()
    {
        _sut.Set(new AppSetting { Key = "/same.oef", Value = "A" }, AppSettingsType.Simulator);
        _sut.Set(new AppSetting { Key = "/same.oef", Value = "B" }, AppSettingsType.Simulator);

        var all = _sut.GetAll(AppSettingsType.Simulator);
        all.Count.Must().Be(1);
        all[0].Value.Must().Be("B");
    }

    [Fact]
    public void Remove_DeletesMatchingKey()
    {
        _sut.Set(new AppSetting { Key = "/x.oef", Value = "X" }, AppSettingsType.Simulator);
        _sut.Remove("/x.oef", AppSettingsType.Simulator);

        _sut.GetAll(AppSettingsType.Simulator).Must().BeEmpty();
    }

    [Fact]
    public void Clear_RemovesAllForType()
    {
        _sut.Set(new AppSetting { Key = "/a.oef", Value = "A" }, AppSettingsType.Simulator);
        _sut.Set(new AppSetting { Key = "/b.oef", Value = "B" }, AppSettingsType.Creator);

        _sut.Clear(AppSettingsType.Simulator);

        _sut.GetAll(AppSettingsType.Simulator).Must().BeEmpty();
        _sut.GetAll(AppSettingsType.Creator).Count.Must().Be(1);
    }

    [Fact]
    public void Get_MissingKey_ReturnsNull()
    {
        _sut.Get("missing", AppSettingsType.Other).Must().BeNull();
    }

    [Fact]
    public void Set_NewPreference_StoresValue()
    {
        _sut.Set(new AppSetting { Key = "key", Value = "value" }, AppSettingsType.Other);

        var stored = _sut.Get("key", AppSettingsType.Other);
        stored.Must().NotBeNull();
        stored!.Value.Must().Be("value");
    }

    [Fact]
    public void Set_ExistingPreference_UpdatesValue()
    {
        _sut.Set(new AppSetting { Key = "key", Value = "first" }, AppSettingsType.Other);
        _sut.Set(new AppSetting { Key = "key", Value = "second" }, AppSettingsType.Other);

        var stored = _sut.Get("key", AppSettingsType.Other);
        stored.Must().NotBeNull();
        stored!.Value.Must().Be("second");
    }

    [Fact]
    public void GetAll_LegacyRecords_AreStillReadable()
    {
        using (var db = new LiteDatabase(_databasePath))
        {
            var collection = db.GetCollection("SimulatorSettings");
            collection.Insert(new BsonDocument
            {
                ["_id"] = 1,
                ["FilePath"] = "/legacy.oef",
                ["Name"] = "Legacy"
            });
        }

        var all = _sut.GetAll(AppSettingsType.Simulator);
        all.Count.Must().Be(1);
        all[0].Key.Must().Be("/legacy.oef");
        all[0].Value.Must().Be("Legacy");
    }
}
