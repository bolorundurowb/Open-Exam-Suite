using System;
using System.Windows.Forms;

namespace Creator.GUI.Dialogs
{
    public partial class AddSection : Form
    {
        public string Title => txt_title.Text;

        public AddSection()
        {
            InitializeComponent();
        }

        private void btn_add_section_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
