using System;
using System.Windows.Forms;

namespace Creator.GUI.Dialogs
{
    public partial class PrintOptions : Form
    {
        internal PrintOption SelectedPrintOption { get; set; }

        public PrintOptions(TreeNode selectedNode)
        {
            InitializeComponent();
            if (selectedNode.Name.Contains("sec"))
                rdb_current_question.Enabled = false;
            else if (selectedNode.Name.Contains("exam"))
            {
                rdb_current_question.Enabled = false;
                rdb_current_section.Enabled = false;
            }
            else
            {
                rdb_current_question.Enabled = true;
                rdb_current_section.Enabled = true;
                rdb_all_questions.Enabled = true;
            }
        }

        private void btn_ok_Click(object sender, EventArgs e)
        {
            if (rdb_all_questions.Checked)
                SelectedPrintOption = PrintOption.AllQuestions;
            else if (rdb_current_question.Checked)
                SelectedPrintOption = PrintOption.CurrentQuestion;
            else
                SelectedPrintOption = PrintOption.CurrentSection;
            this.Close();
        }
    }

    internal enum PrintOption
    {
        CurrentQuestion,
        CurrentSection,
        AllQuestions
    };
}