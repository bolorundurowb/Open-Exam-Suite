using Avalonia.Controls;

namespace OpenExamSuite.Simulator.Services;

/// <summary>
/// Gives host services access to the main window once it exists.
/// </summary>
public sealed class MainWindowAccessor
{
    public Window? Window { get; set; }
}
