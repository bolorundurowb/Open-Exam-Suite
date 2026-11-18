using Avalonia.Controls;
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

            var changelog = new ChangelogDialog().FindControl<TextBlock>("DocumentText")!.Text;
            changelog.Must().Contain("# Changelog");
            changelog.Must().NotContain("Changelog not found.");
        }, CancellationToken.None);
    }
}
