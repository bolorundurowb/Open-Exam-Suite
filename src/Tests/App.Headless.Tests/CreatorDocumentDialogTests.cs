using System.Text;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Headless;
using OmniAssert;
using OpenExamSuite.Creator.Views.Dialogs;
using Xunit;

namespace OpenExamSuite.App.Headless.Tests;

public class CreatorDocumentDialogTests
{
    [Fact]
    public async Task License_And_Changelog_Show_The_Bundled_Documents()
    {
        using var session = HeadlessUnitTestSession.StartNew(typeof(CreatorHeadlessEntry));
        await session.Dispatch(() =>
        {
            var license = new LicenseDialog().FindControl<TextBlock>("DocumentText")!.Text;
            license.Must().Contain("GNU GENERAL PUBLIC LICENSE");
            license.Must().NotContain("License text not found.");

            var changelog = ReadRenderedText(new ChangelogDialog().FindControl<ContentControl>("DocumentHost")!.Content as Control);
            changelog.Must().Contain("Changelog");
            changelog.Must().Contain("[v5.0.0-preview.2]");
            changelog.Must().Contain("Help > Changelog");
            changelog.Must().NotContain("# Changelog");
            changelog.Must().NotContain("**Help > Changelog**");
            changelog.Must().NotContain("Changelog not found.");
        }, CancellationToken.None);
    }

    private static string ReadRenderedText(Control? control)
    {
        var builder = new StringBuilder();
        if (control is not null)
            Collect(control, builder);

        return builder.ToString();

        static void Collect(Control control, StringBuilder builder)
        {
            if (control is SelectableTextBlock text)
            {
                if (text.Inlines is { Count: > 0 })
                {
                    foreach (var inline in text.Inlines)
                    {
                        if (inline is Run run)
                            builder.Append(run.Text);
                    }
                }
                else
                {
                    builder.Append(text.Text);
                }

                builder.Append('\n');
            }
            else if (control is Panel panel)
            {
                foreach (var child in panel.Children)
                    Collect(child, builder);
            }
        }
    }
}
