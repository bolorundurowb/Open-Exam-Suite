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

            settingsService.Set(new AppSetting
            {
                Key = gmatSample,
                Value = Path.GetFileNameWithoutExtension(gmatSample)
            }, AppSettingsType.Simulator);
            settingsService.Set(new AppSetting
            {
                Key = basicScienceSample,
                Value = Path.GetFileNameWithoutExtension(basicScienceSample)
            }, AppSettingsType.Simulator);

            Settings.Default.FirstRun = false;
            Settings.Default.Save();
        }

        foreach (var settings in settingsService.GetAll(AppSettingsType.Simulator))
        {
            dataGridView.Rows.Add(settings.Value, settings.Key);
        }
    }
}
