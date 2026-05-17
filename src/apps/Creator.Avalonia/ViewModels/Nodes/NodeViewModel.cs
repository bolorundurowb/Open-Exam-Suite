using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace OpenExamSuite.Creator.ViewModels.Nodes;

/// <summary>
/// Base ViewModel for items shown in the Creator's exam TreeView. Mirrors the
/// WinForms <c>TreeNode</c>-derived hierarchy (ExamNode, SectionNode,
/// QuestionNode) in <c>Shared.WinForms/Controls/TreeNodes.cs</c>.
/// </summary>
public abstract partial class NodeViewModel : ObservableObject
{
    [ObservableProperty] private string _displayName = string.Empty;
    [ObservableProperty] private bool _isExpanded;
    [ObservableProperty] private bool _isSelected;

    public ObservableCollection<NodeViewModel> Children { get; }

    /// <summary>Back-reference to the containing node. Null at the root.</summary>
    public NodeViewModel? Parent { get; internal set; }

    protected NodeViewModel()
    {
        Children = new ObservableCollection<NodeViewModel>();
        Children.CollectionChanged += (_, e) =>
        {
            if (e.NewItems != null)
                foreach (NodeViewModel child in e.NewItems)
                    child.Parent = this;
            // We intentionally don't null Parent on remove — the removed
            // node may need its old context for undo replay.
        };
    }
}
