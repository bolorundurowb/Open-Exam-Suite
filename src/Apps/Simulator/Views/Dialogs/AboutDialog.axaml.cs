using Avalonia.Controls;
using Avalonia.Input;
using OpenExamSuite.Simulator.Localization;
using OpenExamSuite.Simulator.Services;
using OpenExamSuite.Simulator.Session.HostPorts;

namespace OpenExamSuite.Simulator.Views.Dialogs;

public partial class AboutDialog : Window
{
    public AboutDialog()
        : this(null)
    {
    }

    public AboutDialog(IUriLauncher? launcher)
    {
        InitializeComponent();
        Title = Strings.Get("About_Title");
        NameText.Text = AppInfo.ProductName;
        VersionText.Text = Strings.Format("About_Version", AppInfo.DisplayVersion);
        DescriptionText.Text = Strings.Get("About_Description");

        WebLink.Content = AppInfo.RepositoryUrl;
        IssuesLink.Content = AppInfo.IssuesUrl;
        WikiLink.Content = AppInfo.WikiUrl;
        WebLink.Click += (_, _) => _ = launcher?.LaunchUriAsync(AppInfo.RepositoryUrl);
        IssuesLink.Click += (_, _) => _ = launcher?.LaunchUriAsync(AppInfo.IssuesUrl);
        WikiLink.Click += (_, _) => _ = launcher?.LaunchUriAsync(AppInfo.WikiUrl);

        CloseButton.Click += (_, _) => Close();
        KeyDown += (_, e) =>
        {
            if (e.Key == Key.Escape)
                Close();
        };
        Opened += (_, _) => CloseButton.Focus();
    }
}
