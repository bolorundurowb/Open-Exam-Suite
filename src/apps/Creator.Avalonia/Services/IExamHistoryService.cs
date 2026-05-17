using System.Collections.Generic;

namespace OpenExamSuite.Creator.Services;

public interface IExamHistoryService
{
    IReadOnlyList<string> GetAll();
    void Add(string filePath);
    void Remove(string filePath);
    void Clear();
}
