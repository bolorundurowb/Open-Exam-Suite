using OpenExamSuite.Storage.Enums;
using OpenExamSuite.Storage.Services;

namespace OpenExamSuite.Simulator.Utilities;

public class RowManager
{
    public static void RemoveRow(DataGridView dataGridView)
    {
        foreach (DataGridViewRow row in dataGridView.SelectedRows)
        {
            dataGridView.Rows.Remove(row);
                
            // remove from storage
            var appSettingService = AppSettingsService.Instance;
            appSettingService.Remove(row.Cells[1].Value.ToString(), AppSettingsType.Simulator);
        }
    }
}