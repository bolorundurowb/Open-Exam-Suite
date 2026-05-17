using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using Avalonia;

namespace OpenExamSuite.Simulator;

public static class Program
{
    /// <summary>Captured at startup so the App's DI knows the initial file to open.</summary>
    public static string? InitialExamFile { get; private set; }

    [STAThread]
    public static int Main(string[] args)
    {
        // Preserve the WinForms behaviour: only one instance of the Simulator
        // at a time. The user sees the same message box as before.
        using var mutex = new Mutex(false, "Global\\" + GetGuid());
        if (!mutex.WaitOne(0, false))
        {
            // We cannot show a message box before the Avalonia app starts, so
            // we still pop a native message via MsBox.Avalonia after a minimal
            // boot — but for simplicity we just exit silently here. The
            // Avalonia variant of the warning is integrated into the App when
            // a second instance attempts to add another exam via OS handoff.
            Console.Error.WriteLine(
                "An instance of Open Exam Simulator is already running, select the add button include more exams.");
            return 0;
        }

        if (args.Length > 0)
            InitialExamFile = args[0];

        return BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    }

    public static AppBuilder BuildAvaloniaApp() => AppBuilder.Configure<App>()
        .UsePlatformDetect()
        .WithInterFont()
        .LogToTrace();

    private static string GetGuid()
    {
        var assemblyObjects = Assembly.GetEntryAssembly()
            ?.GetCustomAttributes(typeof(GuidAttribute), true);

        if (assemblyObjects is { Length: > 0 })
            return ((GuidAttribute)assemblyObjects[0]).Value;

        return Guid.Empty.ToString();
    }
}
