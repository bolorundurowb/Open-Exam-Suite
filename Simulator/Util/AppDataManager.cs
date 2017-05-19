using System.Collections.Specialized;
using System.IO;
using System.Windows.Forms;
using Simulator.Properties;

namespace Simulator.Util
{
    public class AppDataManager
    {
        public static void SaveAppData(DataGridView dataGridView)
        {
            Settings.Default.ExamPaths = new StringCollection();
            foreach (DataGridViewRow row in dataGridView.Rows)
                Settings.Default.ExamPaths.Add(row.Cells[1].Value.ToString());
            Settings.Default.Save();
        }

        public static void LoadAppData(DataGridView dataGridView)
        {
            if(Settings.Default.FirstRun)
            {
                var suiteRootFolder = Application.StartupPath;
                var samplesFolder = Path.Combine(suiteRootFolder, "Samples");
                var gmatSample = Path.Combine(samplesFolder, "GMAT Sample.oef");
                var basicScienceSample = Path.Combine(samplesFolder, "Basic Science.oef");
                if(Settings.Default.ExamPaths == null)
                {
                    Settings.Default.ExamPaths = new StringCollection();
                }
                Settings.Default.ExamPaths.Add(gmatSample);
                Settings.Default.ExamPaths.Add(basicScienceSample);
                Settings.Default.FirstRun = false;
                Settings.Default.Save();
            }
            if (Settings.Default.ExamPaths == null) return;
            foreach(var path in Settings.Default.ExamPaths)
            {
                dataGridView.Rows.Add(Path.GetFileNameWithoutExtension(path), path);
            }
        }
    }
}