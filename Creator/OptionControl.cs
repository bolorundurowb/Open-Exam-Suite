using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Creator
{
    public partial class OptionControl : UserControl
    {
        bool isChecked;

        public OptionControl()
        {
            InitializeComponent();
        }

        //Properties
        public char OptionLetter
        {
            get 
            {
                return Convert.ToChar(rdb_option.Text);
            }
            set
            { 
                rdb_option.Text = value.ToString();
            }
        }

        public string OptionText
        {
            get
            {
                return txt_option.Text;
            }
            set
            {
                txt_option.Text = value.ToString();
            }
        }

        public bool IsChecked
        {
            get
            {
                return isChecked;
            }
            set
            {
                isChecked = value;
            }
        }

        private void rdb_option_CheckedChanged(object sender, EventArgs e)
        {
            isChecked = rdb_option.Checked;
        }

        private void rdb_option_Click(object sender, EventArgs e)
        {
            if (rdb_option.Checked)
            {
                rdb_option.Checked = false;
            }
            else
            {
                rdb_option.Checked = true;
            }
        }
    }
}
