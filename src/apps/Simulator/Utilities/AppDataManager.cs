using OpenExamSuite.Storage.Enums;
using OpenExamSuite.Storage.Interfaces;
using OpenExamSuite.Storage.Models;
using Simulator.Properties;

namespace OpenExamSuite.Simulator.Utilities;

public static class AppDataManager
{
    public static void LoadAppData(DataGridView dataGridView, IAppSettingsService settingsService)
    {
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

            Settings.Default.FirstRun = false;
            Settings.Default.Save();
        }

        foreach (var settings in settingsService.GetAll(AppSettingsType.Simulator))
        {
            dataGridView.Rows.Add(settings.Name, settings.FilePath);
        }
    }
}
