using System.Collections.Generic;
using System.IO;
using System.Linq;
using OpenExamSuite.Storage.Enums;
using OpenExamSuite.Storage.Interfaces;
using OpenExamSuite.Storage.Models;

namespace OpenExamSuite.Creator.Services;

/// <summary>
/// Thin wrapper around <see cref="IAppSettingsService"/> for the Creator's
/// "Recent Files" history. Mirrors the WinForms <c>AddToHistory</c> /
/// <c>LoadExamHistory</c> behaviour.
/// </summary>
public sealed class ExamHistoryService : IExamHistoryService
{
    private readonly IAppSettingsService _settings;

    public ExamHistoryService(IAppSettingsService settings)
    {
        _settings = settings;
    }

    public IReadOnlyList<string> GetAll()
        => _settings.GetAll(AppSettingsType.Creator)
            .Select(x => x.FilePath)
            .Where(p => !string.IsNullOrWhiteSpace(p))
            .ToList();

    public void Add(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath)) return;
        _settings.Add(new AppSetting
        {
            FilePath = filePath,
            Name = Path.GetFileNameWithoutExtension(filePath),
        }, AppSettingsType.Creator);
    }

    public void Remove(string filePath)
        => _settings.Remove(filePath, AppSettingsType.Creator);

    public void Clear()
        => _settings.Clear(AppSettingsType.Creator);
}
