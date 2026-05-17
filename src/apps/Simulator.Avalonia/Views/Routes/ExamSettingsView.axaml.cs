using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace OpenExamSuite.Simulator.Views.Routes;

public partial class ExamSettingsView : UserControl
{
    public ExamSettingsView() => InitializeComponent();
    private void InitializeComponent() => AvaloniaXamlLoader.Load(this);
}
