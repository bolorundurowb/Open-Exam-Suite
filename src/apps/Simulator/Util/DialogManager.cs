using System;
using System.CodeDom;
using System.IO;
using System.Windows.Forms;
using Logging;
using Shared;
using Shared.CompatibilityProvider;
using Shared.Util;
using Simulator.Enums;
using Simulator.GUI;

namespace Simulator.Util
{
    public static class DialogManager
    {
        public static void DisplayDialog(DialogType dialogType, DataGridView dataGridView)
        {
            CompatibilityProvider compatibilityProvider = new CompatibilityProvider();
            try
            {
                var filePath = dataGridView.SelectedRows[0].Cells[1].Value.ToString();
                var exam = Reader.FromOefFile(filePath);

                compatibilityProvider.Patch(exam, filePath);


                if (dialogType == DialogType.ExamSettings)
                {
                    InitilaizeExamSettings(exam, filePath);
                }
                if (dialogType == DialogType.ExamProperties)
                {
                    InitilaizeExamProperties(exam, filePath);
                }
            }
            catch (FileNotFoundException ex)
            {
                Logger.LogException(ex);

                MessageBox.Show("Sorry, the selected exam does not exist. It may have been moved or deleted.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                RowManager.RemoveRow(dataGridView);
            }
            catch (NullReferenceException ex)
            {
                Logger.LogException(ex);

                MessageBox.Show("Sorry, the exam selected is either old or corrupt. If it is an old exam, please upgrade it with the upgrade tool at:\nhttps://sourceforge.net/projects/exam-upgrade-tool/", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                RowManager.RemoveRow(dataGridView);
            }
        }

        private static void InitilaizeExamProperties(Exam exam, string filePath)
        {
            var properties = new ExamPropertiesUi(exam, filePath);
            properties.ShowDialog();
        }

        private static void InitilaizeExamSettings(Exam exam, string filePath)
        {
            var settings = new ExamSettingsUi(exam, filePath);
            settings.ShowDialog();
        }
    }
}