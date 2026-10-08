using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using OpenExamSuite.Shared.Dialogs;
using OpenExamSuite.Simulator.GUI;
using OpenExamSuite.Storage.Enums;
using OpenExamSuite.Storage.Interfaces;
using OpenExamSuite.Storage.Models;
using OpenExamSuite.Storage.Services;

namespace OpenExamSuite.Simulator;

public static class Program
{
    private const string ChangelogVersionKey = "Simulator.LastChangelogVersion";

    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    public static void Main(string[] args)
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        using var mutex = new Mutex(false, "Global\\" + GetGuid());
        if (!mutex.WaitOne(0, false))
        {
            MessageBox.Show(
                "An instance of Open Exam Simulator is already running, select the add button include more exams.",
                "OES Simulator", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            return;
        }

        var services = new ServiceCollection();
        services.AddSingleton<IAppSettingsService>(_ => new AppSettingsService());
        using var provider = services.BuildServiceProvider();
        var appSettings = provider.GetRequiredService<IAppSettingsService>();

        var mainForm = args.Length == 0
            ? new HomeUi(appSettings)
            : new HomeUi(appSettings, args[0]);
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

    private static string GetGuid()
    {
        var assemblyGuid = Guid.Empty;
        var assemblyObjects = System.Reflection.Assembly.GetEntryAssembly()
            ?.GetCustomAttributes(typeof(System.Runtime.InteropServices.GuidAttribute), true);

        if (assemblyObjects?.Length > 0)
            assemblyGuid = new Guid(((System.Runtime.InteropServices.GuidAttribute)assemblyObjects[0]).Value);

        return assemblyGuid.ToString();
    }
}
