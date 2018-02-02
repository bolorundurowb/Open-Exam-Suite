using System.Collections.Specialized;
using System.IO;
using System.Windows.Forms;
using Simulator.Properties;
using Storage.Enums;
using Storage.Models;
using Storage.Services;

namespace Simulator.Util
{
    public static class AppDataManager
    {
        public static void LoadAppData(DataGridView dataGridView)
        {
            var settingsService = AppSettingsService.Instance;

            if (Settings.Default.FirstRun)
            {
                var suiteRootFolder = Application.StartupPath;
                var samplesFolder = Path.Combine(suiteRootFolder, "Samples");
                var gmatSample = Path.Combine(samplesFolder, "GMAT Sample.oef");
                var basicScienceSample = Path.Combine(samplesFolder, "Basic Science.oef");

                settingsService.Add(new AppSetting
                {
                    Name = Path.GetFileNameWithoutExtension(gmatSample),
                    FilePath = gmatSample
                }, AppSettingsType.Simulator);
                settingsService.Add(new AppSetting
                {
                    Name = Path.GetFileNameWithoutExtension(basicScienceSample),
                    FilePath = basicScienceSample
                }, AppSettingsType.Simulator);

                // save first run data
                Settings.Default.FirstRun = false;
                Settings.Default.Save();
            }

            foreach (var settings in settingsService.GetAll(AppSettingsType.Simulator))
            {
                dataGridView.Rows.Add(settings.Name, settings.FilePath);
            }
        }
    }
}