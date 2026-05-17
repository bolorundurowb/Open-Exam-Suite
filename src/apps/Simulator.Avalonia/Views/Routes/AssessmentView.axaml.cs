using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace OpenExamSuite.Simulator.Views.Routes;

public partial class AssessmentView : UserControl
{
    public AssessmentView() => InitializeComponent();
    private void InitializeComponent() => AvaloniaXamlLoader.Load(this);
}
