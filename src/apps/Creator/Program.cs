using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using OpenExamSuite.Creator.GUI;
using OpenExamSuite.Shared.Dialogs;
using OpenExamSuite.Storage.Enums;
using OpenExamSuite.Storage.Interfaces;
using OpenExamSuite.Storage.Models;
using OpenExamSuite.Storage.Services;

namespace OpenExamSuite.Creator;

public static class Program
{
    private const string ChangelogVersionKey = "Creator.LastChangelogVersion";

    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    public static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        var services = new ServiceCollection();
        services.AddSingleton<IAppSettingsService>(_ => new AppSettingsService());
        using var provider = services.BuildServiceProvider();
        var appSettings = provider.GetRequiredService<IAppSettingsService>();

        var mainForm = new HomeUi(appSettings);
        mainForm.Shown += (_, _) => ShowChangelogIfUpdated(appSettings);

        Application.Run(mainForm);
    }

    private static void ShowChangelogIfUpdated(IAppSettingsService appSettings)
    {
        var currentVersion = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "0.0.0";
        if (appSettings.Get(ChangelogVersionKey, AppSettingsType.Other)?.Value == currentVersion)
            return;

        using var changelog = new ChangelogUi();
        changelog.ShowDialog();
        appSettings.Set(new AppSetting { Key = ChangelogVersionKey, Value = currentVersion }, AppSettingsType.Other);
    }
}
