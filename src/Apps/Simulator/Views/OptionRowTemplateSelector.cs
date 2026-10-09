using Avalonia.Controls;
using Avalonia.Controls.Templates;
using OpenExamSuite.Simulator.ViewModels;

namespace OpenExamSuite.Simulator.Views;

/// <summary>
/// Builds either a radio row or a checkbox row. Both must not exist together: a hidden radio button
/// still belongs to the group and clears every other option, so a multiple-answer question can only
/// keep one selection.
/// </summary>
public sealed class OptionRowTemplateSelector : IDataTemplate
{
    public IDataTemplate? Single { get; set; }

    public IDataTemplate? Multiple { get; set; }

    public bool Match(object? data) => data is OptionItemViewModel;

    public Control? Build(object? data)
    {
        if (data is not OptionItemViewModel option)
            return null;

        var template = option.IsMultiple ? Multiple : Single;
        return template?.Build(data);
    }
}
