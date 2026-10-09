using OpenExamSuite.Shared;

namespace OpenExamSuite.Creator.Session.Models;

public enum NodeType
{
    Exam,
    Section,
    Question
}

/// <summary>
/// A session-level identity for an exam, section or question. The underlying <see cref="Exam"/>
/// schema does not store these IDs; they are assigned when a document is loaded or created and
/// survive rename, reorder, delete and undo inside the current session.
/// </summary>
public sealed class DocumentNode
{
    public string Id { get; }

    public NodeType Type { get; }

    /// <summary>The parent section node ID. Null for exam and section nodes.</summary>
    public string? ParentId { get; }

    /// <summary>Reference to the live section. Null for the exam node.</summary>
    public Section? Section { get; internal set; }

    /// <summary>Reference to the live question. Null for exam and section nodes.</summary>
    public Question? Question { get; internal set; }

    public DocumentNode(string id, NodeType type, string? parentId = null)
    {
        Id = id;
        Type = type;
        ParentId = parentId;
    }
}
