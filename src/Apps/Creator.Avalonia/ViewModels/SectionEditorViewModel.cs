using CommunityToolkit.Mvvm.ComponentModel;
using OpenExamSuite.Creator.Services;

namespace OpenExamSuite.Creator.ViewModels;

public sealed partial class SectionEditorViewModel : ViewModelBase
{
    private readonly ShellServices _shell;
    private readonly string _sectionId;

    public SectionEditorViewModel(ShellServices shell, string sectionId)
    {
        _shell = shell;
        _sectionId = sectionId;
        _name = _shell.Document.Nodes.TryGetValue(sectionId, out var node) && node.Section != null
            ? node.Section.Title
            : string.Empty;
    }

    [ObservableProperty] private string _name;

    partial void OnNameChanged(string value)
    {
        _shell.Document.UpdateSectionName(_sectionId, value);
    }
}
