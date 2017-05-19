using System.Windows.Forms;

namespace Simulator.Util
{
    public class RowManager
    {
        public static void RemoveRow(DataGridView dataGridView)
        {
            foreach (DataGridViewRow row in dataGridView.SelectedRows)
            {
                dataGridView.Rows.Remove(row);
            }
        }
    }
}