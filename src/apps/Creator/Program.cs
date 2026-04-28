using Microsoft.Extensions.DependencyInjection;
using OpenExamSuite.Creator.GUI;
using OpenExamSuite.Storage.Interfaces;
using OpenExamSuite.Storage.Services;

namespace OpenExamSuite.Creator;

public static class Program
{
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

        Application.Run(new HomeUi(provider.GetRequiredService<IAppSettingsService>()));
    }
}