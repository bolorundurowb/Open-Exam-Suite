using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace OpenExamSuite.Creator.Views.Panes;

public partial class SplashPaneView : UserControl
{
    public SplashPaneView() => InitializeComponent();
    private void InitializeComponent() => AvaloniaXamlLoader.Load(this);
}
