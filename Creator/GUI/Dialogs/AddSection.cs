using System;
using System.Windows.Forms;

namespace Creator
{
    public partial class AddSection : Form
    {
        public string Title
        {
            get
            {
                return txt_title.Text;
            }
        }

        public AddSection()
        {
            InitializeComponent();
        }

        private void btn_add_section_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
