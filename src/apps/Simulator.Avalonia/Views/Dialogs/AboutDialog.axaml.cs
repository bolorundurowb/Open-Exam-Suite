using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using OpenExamSuite.Shared.Avalonia.Services;

namespace OpenExamSuite.Simulator.Views.Dialogs;

public partial class AboutDialog : Window
{
    public AboutDialog() => InitializeComponent();
    private void InitializeComponent() => AvaloniaXamlLoader.Load(this);

    private async Task OpenAsync(string url)
    {
        var svc = (Avalonia.Application.Current as App)?.Services.GetService(typeof(IOpenUrlService)) as IOpenUrlService;
        if (svc is not null) await svc.OpenUriAsync(url);
    }

    private async void OnOpenWeb   (object? s, RoutedEventArgs e) => await OpenAsync("https://bolorundurowb.github.io/Open-Exam-Suite");
    private async void OnOpenIssues(object? s, RoutedEventArgs e) => await OpenAsync("https://github.com/bolorundurowb/Open-Exam-Suite/issues");
    private void OnClose(object? s, RoutedEventArgs e) => Close();
}
