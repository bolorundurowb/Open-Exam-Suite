using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace OpenExamSuite.Simulator.Views;

public partial class MainWindow : Window
{
    public static MainWindow? Current { get; private set; }

    public MainWindow()
    {
        InitializeComponent();
        Current = this;
    }

    private void InitializeComponent() => AvaloniaXamlLoader.Load(this);
}
