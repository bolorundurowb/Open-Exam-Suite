using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace OpenExamSuite.Creator.Views.Panes;

public partial class QuestionEditorPaneView : UserControl
{
    public QuestionEditorPaneView() => InitializeComponent();
    private void InitializeComponent() => AvaloniaXamlLoader.Load(this);
}
