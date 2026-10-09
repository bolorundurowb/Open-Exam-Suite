using Avalonia.Markup.Xaml;

namespace OpenExamSuite.Creator.Localization;

public class LocExtension : MarkupExtension
{
    public string Key { get; set; }

    public LocExtension(string key)
    {
        Key = key;
    }

    public override object ProvideValue(IServiceProvider serviceProvider) => Strings.Get(Key);
}
