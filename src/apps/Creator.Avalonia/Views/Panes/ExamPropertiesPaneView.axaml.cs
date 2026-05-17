using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace OpenExamSuite.Creator.Views.Panes;

public partial class ExamPropertiesPaneView : UserControl
{
    public ExamPropertiesPaneView() => InitializeComponent();
    private void InitializeComponent() => AvaloniaXamlLoader.Load(this);
}
