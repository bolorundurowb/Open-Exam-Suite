using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Creator
{
    public partial class New_Section : Form
    {
        public string SectionName { get; set; }

        public New_Section()
        {
            InitializeComponent();
        }

        private void btn_add_Click(object sender, EventArgs e)
        {
            SectionName = txt_section_title.Text;
            this.Close();
        }

        private void txt_section_title_TextChanged(object sender, EventArgs e)
        {
            ShowInvalidTextError();
        }

        private void New_Section_Load(object sender, EventArgs e)
        {
            ShowInvalidTextError();
        }

        private void ShowInvalidTextError()
        {
            if (string.IsNullOrWhiteSpace(txt_section_title.Text))
            {
                err_new_section.SetError(txt_section_title, "Please enter valid text!");
                btn_add.Enabled = false;
            }
            else
            {
                err_new_section.Clear();
                btn_add.Enabled = true;
            }
        }
    }
}
