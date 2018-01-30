using System;
using System.Windows.Forms;

namespace Creator.GUI.Dialogs
{
    public partial class EditSection : Form
    {
        public string Title => txt_title.Text;

        public EditSection(string currentTitle)
        {
            InitializeComponent();
            txt_title.Text = currentTitle;
            txt_title.SelectAll();
        }

        private void btn_add_section_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
