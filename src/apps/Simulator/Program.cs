using Microsoft.Extensions.DependencyInjection;
using OpenExamSuite.Simulator.GUI;
using OpenExamSuite.Storage.Interfaces;
using OpenExamSuite.Storage.Services;

namespace OpenExamSuite.Simulator;

static class Program
{
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main(string[] args)
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        using var mutex = new Mutex(false, "Global\\" + GetGuid());
        if (!mutex.WaitOne(0, false))
        {
            MessageBox.Show("An instance of Open Exam Simulator is already running, select the add button include more exams.", "OES Simulator", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            return;
        }

        var services = new ServiceCollection();
        services.AddSingleton<IAppSettingsService>(_ => new AppSettingsService());
        using var provider = services.BuildServiceProvider();
        var appSettings = provider.GetRequiredService<IAppSettingsService>();

        Application.Run(args.Length == 0
            ? new HomeUi(appSettings)
            : new HomeUi(appSettings, args[0]));
    }

    static string GetGuid()
    {
        var assemblyGuid = Guid.Empty;
        var assemblyObjects = System.Reflection.Assembly.GetEntryAssembly().GetCustomAttributes(typeof(System.Runtime.InteropServices.GuidAttribute), true);
        if (assemblyObjects.Length > 0)
        {
            assemblyGuid = new Guid(((System.Runtime.InteropServices.GuidAttribute)assemblyObjects[0]).Value);
        }
        return assemblyGuid.ToString();
    }
}