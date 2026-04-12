using System.Reflection;

namespace OpenExamSuite.Shared.Dialogs;

public partial class LicenseUi : Form
{
    public LicenseUi()
    {
        InitializeComponent();
        LoadLicenseText();
        txtLicense.Select(0, 0);
    }

    private void LoadLicenseText()
    {
        try
        {
            using var stream = Assembly.GetExecutingAssembly()
                .GetManifestResourceStream("OpenExamSuite.Shared.LICENSE");

            if (stream != null)
            {
                using var reader = new StreamReader(stream);
                txtLicense.Text = reader.ReadToEnd();
            }
            else
            {
                txtLicense.Text = "License file could not be found.";
            }
        }
        catch
        {
            txtLicense.Text = "An error occurred while loading the license file.";
        }
    }
}
