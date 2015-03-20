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
    public partial class EditSectionTitle : Form
    {
        //Blah
        string newName;

        public EditSectionTitle()
        {
            InitializeComponent();
        }

        private void btn_edit_ok_Click(object sender, EventArgs e)
        {
            newName = txt_new_title.Text;
            this.Close();
        }

        public string NewName
        {
            get
            {
                return this.newName;
            }
        }
    }
}
