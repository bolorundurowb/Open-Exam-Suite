using CommunityToolkit.Mvvm.ComponentModel;
using OpenExamSuite.Shared;

namespace OpenExamSuite.Simulator.ViewModels.Items;

public sealed partial class SectionSelectionViewModel : ObservableObject
{
    public Section Section { get; }
    public string Title => Section.Title;

    [ObservableProperty] private bool _isChecked = true;

    public SectionSelectionViewModel(Section section, bool isChecked = true)
    {
        Section = section;
        _isChecked = isChecked;
    }
}
