using System.Reflection;

namespace OpenExamSuite.Shared.Dialogs;

public partial class ChangelogUi : Form
{
    public ChangelogUi()
    {
        InitializeComponent();
        LoadChangelogText();
        txtChangelog.Select(0, 0);
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
                txtChangelog.Text = reader.ReadToEnd();
            }
            else
            {
                txtChangelog.Text = "Changelog could not be found.";
            }
        }
        catch
        {
            txtChangelog.Text = "An error occurred while loading the changelog.";
        }
    }
}
