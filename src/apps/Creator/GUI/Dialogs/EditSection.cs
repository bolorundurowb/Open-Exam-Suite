using System;
using System.Windows.Forms;

namespace Creator.GUI.Dialogs
{
    public partial class EditSection : Form
    {
        public string Title => txt_title.Text;

        public int DefaultNumberOfQuestions => (int)num_numberOfQuestions.Value;

        public EditSection(string currentTitle, int defaultNumberOfQuestions)
        {
            InitializeComponent();
            txt_title.Text = currentTitle;
            txt_title.SelectAll();
            num_numberOfQuestions.Value = defaultNumberOfQuestions;
        }

        private void btn_add_section_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
