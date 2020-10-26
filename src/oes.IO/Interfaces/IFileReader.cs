using oes.Models;

namespace oes.IO.Interfaces
{
    public interface IFileReader
    {
        Exam FromFile(string filePath);

        Exam FromText(string text);
    }
}