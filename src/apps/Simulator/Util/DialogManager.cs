using System;
using System.IO;
using System.Windows.Forms;
using Shared;
using Shared.Util;
using Simulator.Enums;
using Simulator.GUI;

namespace Simulator.Util
{
    public static class DialogManager
    {
        public static void DisplayDialog(DialogType dialogType, DataGridView dataGridView)
        {
            try
            {
                var filePath = dataGridView.SelectedRows[0].Cells[1].Value.ToString();
                var exam = Reader.FromOefFile(filePath);
                if (dialogType == DialogType.ExamSettings)
                {
                    InitilaizeExamSettings(exam);
                }
                if (dialogType == DialogType.ExamProperties)
                {
                    InitilaizeExamProperties(exam, filePath);
                }
            }
            catch (FileNotFoundException)
            {
                MessageBox.Show("Sorry, the selected exam does not exist. It may have been moved or deleted.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                RowManager.RemoveRow(dataGridView);
            }
            catch(NullReferenceException)
            {
                MessageBox.Show("Sorry, the exam selected is either old or corrupt. If it is an old exam, please upgrade it with the upgrade tool at:\nhttps://sourceforge.net/projects/exam-upgrade-tool/", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                RowManager.RemoveRow(dataGridView);
            }
        }

        private static void InitilaizeExamProperties(Exam exam, string filePath)
        {
            var properties = new ExamPropertiesUi(exam, filePath);
            properties.ShowDialog();
        }

        private static void InitilaizeExamSettings(Exam exam)
        {
            var settings = new ExamSettingsUi(exam);
            settings.ShowDialog();
        }
    }
}