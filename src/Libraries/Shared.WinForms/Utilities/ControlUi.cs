namespace OpenExamSuite.Shared.WinForms;

/// <summary>
/// Small WinForms helpers shared across app GUIs.
/// </summary>
public static class ControlUi
{
    /// <summary>
    /// Sets <see cref="Control.Visible"/> for every control in <paramref name="controls"/>.
    /// </summary>
    public static void SetVisibility(IEnumerable<Control> controls, bool visible)
    {
        foreach (var control in controls)
        {
            control.Visible = visible;
        }
    }
}
