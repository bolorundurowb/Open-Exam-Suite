using CommunityToolkit.Mvvm.ComponentModel;
using OpenExamSuite.Creator.Services;

namespace OpenExamSuite.Creator.ViewModels;

public sealed partial class SectionEditorViewModel : ViewModelBase
{
    private readonly ShellServices _shell;
    private readonly string _sectionId;
    private bool _isSyncing;

    public string SectionId => _sectionId;

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
        if (_isSyncing)
            return;
        _shell.Document.UpdateSectionName(_sectionId, value);
    }

    internal void SyncFromDocument()
    {
        if (!_shell.Document.Nodes.TryGetValue(_sectionId, out var node) || node.Section == null)
            return;

        if (Name != node.Section.Title)
        {
            _isSyncing = true;
            try
            {
                Name = node.Section.Title;
            }
            finally
            {
                _isSyncing = false;
            }
        }
    }
}
