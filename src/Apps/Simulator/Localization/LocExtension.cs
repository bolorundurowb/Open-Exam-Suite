using Avalonia.Markup.Xaml;

namespace OpenExamSuite.Simulator.Localization;

/// <summary>
/// XAML markup extension: <c>Text="{loc:Loc Library_Title}"</c>.
/// </summary>
public sealed class LocExtension : MarkupExtension
{
    public LocExtension()
    {
    }

    public LocExtension(string key)
    {
        Key = key;
    }

    public string Key { get; set; } = string.Empty;

    public override object ProvideValue(IServiceProvider serviceProvider) => Strings.Get(Key);
}
