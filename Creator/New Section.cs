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
        List<string> enteredText = new List<string>();
        string[] sections;

        /// <summary>
        /// The default constructor
        /// </summary>
        public New_Section()
        {
            InitializeComponent();
        }

        private void btn_add_Click(object sender, EventArgs e)
        {
            enteredText.Add(txt_section_title.Text);
            txt_section_title.Clear();
            txt_section_title.Focus();
        }

        private void txt_section_title_TextChanged(object sender, EventArgs e)
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

        private void New_Section_FormClosing(object sender, FormClosingEventArgs e)
        {
            sections = enteredText.ToArray();
        }

        public string[] Sections
        {
            get
            {
                return sections;
            }
        }
    }
}
