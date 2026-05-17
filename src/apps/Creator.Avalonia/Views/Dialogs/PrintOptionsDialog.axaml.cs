using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using OpenExamSuite.Creator.Services;
using OpenExamSuite.Creator.ViewModels.Nodes;

namespace OpenExamSuite.Creator.Views.Dialogs;

public partial class PrintOptionsDialog : Window
{
    private RadioButton? _rdoCurrentQ;
    private RadioButton? _rdoCurrentS;
    private RadioButton? _rdoAll;

    public PrintOptionsDialog() : this(null) { }

    public PrintOptionsDialog(NodeViewModel? selected)
    {
        InitializeComponent();
        _rdoCurrentQ = this.FindControl<RadioButton>("RdoCurrentQuestion");
        _rdoCurrentS = this.FindControl<RadioButton>("RdoCurrentSection");
        _rdoAll      = this.FindControl<RadioButton>("RdoAllQuestions");

        // Mirror the WinForms PrintOptions enablement: at the exam root only
        // "All questions" is allowed; at a section, only "Current section" /
        // "All questions"; at a question, all three.
        switch (selected)
        {
            case QuestionNodeViewModel:
                if (_rdoCurrentQ is not null) { _rdoCurrentQ.IsEnabled = true;  _rdoCurrentQ.IsChecked = true; }
                if (_rdoCurrentS is not null) _rdoCurrentS.IsEnabled = true;
                if (_rdoAll      is not null) _rdoAll.IsEnabled = true;
                break;
            case SectionNodeViewModel:
                if (_rdoCurrentQ is not null) _rdoCurrentQ.IsEnabled = false;
                if (_rdoCurrentS is not null) { _rdoCurrentS.IsEnabled = true; _rdoCurrentS.IsChecked = true; }
                if (_rdoAll      is not null) _rdoAll.IsEnabled = true;
                break;
            default:
                if (_rdoCurrentQ is not null) _rdoCurrentQ.IsEnabled = false;
                if (_rdoCurrentS is not null) _rdoCurrentS.IsEnabled = false;
                if (_rdoAll      is not null) { _rdoAll.IsEnabled = true; _rdoAll.IsChecked = true; }
                break;
        }
    }

    private void InitializeComponent() => AvaloniaXamlLoader.Load(this);

    private void OnOk(object? sender, RoutedEventArgs e)
    {
        if (_rdoCurrentQ?.IsChecked == true)
            Close(PrintScope.CurrentQuestion);
        else if (_rdoCurrentS?.IsChecked == true)
            Close(PrintScope.CurrentSection);
        else if (_rdoAll?.IsChecked == true)
            Close(PrintScope.AllQuestions);
        else
            Close(null);
    }

    private void OnCancel(object? sender, RoutedEventArgs e) => Close(null);
}
