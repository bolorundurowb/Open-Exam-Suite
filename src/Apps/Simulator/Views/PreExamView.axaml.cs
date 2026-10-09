using Avalonia.Controls;
using Avalonia.Interactivity;

namespace OpenExamSuite.Simulator.Views;

public partial class PreExamView : UserControl
{
    public PreExamView()
    {
        InitializeComponent();
        AttachedToVisualTree += (_, _) => Focus();
    }
}
