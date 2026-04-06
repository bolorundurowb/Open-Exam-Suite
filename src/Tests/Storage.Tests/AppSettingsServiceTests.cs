using OpenExamSuite.Storage.Enums;
using OpenExamSuite.Storage.Models;
using OpenExamSuite.Storage.Services;
using Shouldly;
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
    public void Add_DistinctPaths_BothStored()
    {
        _sut.Add(new AppSetting { Name = "A", FilePath = "/a.oef" }, AppSettingsType.Simulator);
        _sut.Add(new AppSetting { Name = "B", FilePath = "/b.oef" }, AppSettingsType.Simulator);

        var all = _sut.GetAll(AppSettingsType.Simulator);
        all.Count.ShouldBe(2);
    }

    [Fact]
    public void Add_DuplicatePath_IgnoresSecondInsert()
    {
        _sut.Add(new AppSetting { Name = "A", FilePath = "/same.oef" }, AppSettingsType.Simulator);
        _sut.Add(new AppSetting { Name = "B", FilePath = "/same.oef" }, AppSettingsType.Simulator);

        _sut.GetAll(AppSettingsType.Simulator).Count.ShouldBe(1);
    }

    [Fact]
    public void Remove_DeletesMatchingPath()
    {
        _sut.Add(new AppSetting { Name = "A", FilePath = "/x.oef" }, AppSettingsType.Simulator);
        _sut.Remove("/x.oef", AppSettingsType.Simulator);

        _sut.GetAll(AppSettingsType.Simulator).ShouldBeEmpty();
    }

    [Fact]
    public void Clear_RemovesAllForType()
    {
        _sut.Add(new AppSetting { Name = "A", FilePath = "/a.oef" }, AppSettingsType.Simulator);
        _sut.Add(new AppSetting { Name = "B", FilePath = "/b.oef" }, AppSettingsType.Creator);

        _sut.Clear(AppSettingsType.Simulator);

        _sut.GetAll(AppSettingsType.Simulator).ShouldBeEmpty();
        _sut.GetAll(AppSettingsType.Creator).Count.ShouldBe(1);
    }
}
