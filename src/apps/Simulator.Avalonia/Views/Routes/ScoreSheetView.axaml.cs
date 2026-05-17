using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace OpenExamSuite.Simulator.Views.Routes;

public partial class ScoreSheetView : UserControl
{
    public ScoreSheetView() => InitializeComponent();
    private void InitializeComponent() => AvaloniaXamlLoader.Load(this);
}
