using System.Reflection;

namespace OpenExamSuite.Shared.Dialogs;

public partial class ChangelogUi : Form
{
    public ChangelogUi()
    {
        InitializeComponent();
        LoadChangelogText();
    }

    private void LoadChangelogText()
    {
        try
        {
            using var stream = Assembly.GetExecutingAssembly()
                .GetManifestResourceStream("OpenExamSuite.Shared.CHANGELOG");

            if (stream != null)
            {
                using var reader = new StreamReader(stream);
                mdChangelog.MarkdownText = reader.ReadToEnd();
            }
            else
            {
                mdChangelog.MarkdownText = "Changelog could not be found.";
            }
        }
        catch
        {
            mdChangelog.MarkdownText = "An error occurred while loading the changelog.";
        }
    }
}
