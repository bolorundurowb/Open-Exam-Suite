using System.Reflection;
using Avalonia.Platform;
using OpenExamSuite.Storage.Enums;
using OpenExamSuite.Storage.Interfaces;
using OpenExamSuite.Storage.Models;

namespace OpenExamSuite.Simulator.Services;

public static class AppInfo
{
    public const string RepositoryUrl = "https://github.com/bolorundurowb/Open-Exam-Suite";
    public const string IssuesUrl = RepositoryUrl + "/issues";
    public const string WikiUrl = RepositoryUrl + "/wiki";
    public const string ChangelogVersionKey = "Simulator.LastChangelogVersion";

    /// <summary>The window title and product name, on every screen.</summary>
    public const string ProductName = "Open Exam Simulator";

    public static string Version =>
        typeof(AppInfo).Assembly.GetName().Version?.ToString() ?? "0.0.0";

    /// <summary>Version without a trailing ".0" revision, for display.</summary>
    public static string DisplayVersion
    {
        get
        {
            var version = typeof(AppInfo).Assembly.GetName().Version;
            return version == null ? "0.0.0" : $"{version.Major}.{version.Minor}.{Math.Max(0, version.Build)}";
        }
    }

    public static string ReadAsset(string assetName)
    {
        var uri = new Uri($"avares://OpenExamSuite.Simulator/Assets/{assetName}");
        using var stream = AssetLoader.Open(uri);
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }

    /// <summary>
    /// The changelog is shown once per version, on the first launch after an update.
    /// </summary>
    public static bool ShouldShowChangelog(IAppSettingsService settings) =>
        settings.Get(ChangelogVersionKey, AppSettingsType.Other)?.Value != Version;

    public static void MarkChangelogShown(IAppSettingsService settings) =>
        settings.Set(new AppSetting { Key = ChangelogVersionKey, Value = Version }, AppSettingsType.Other);
}
