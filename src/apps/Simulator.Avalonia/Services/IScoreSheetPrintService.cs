using System.Threading.Tasks;
using OpenExamSuite.Shared;
using OpenExamSuite.Shared.Models;

namespace OpenExamSuite.Simulator.Services;

public interface IScoreSheetPrintService
{
    Task PrintAsync(Settings settings, Exam exam);
}
