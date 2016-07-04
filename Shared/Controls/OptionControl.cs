using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Shared.Controls
{
    public partial class OptionControl : UserControl
    {
        #region Public Properties
        [Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public char Letter
        {
            get
            {
                return Convert.ToChar(rdb_letter.Text);
            }
            set
            {
                rdb_letter.Text = value.ToString();
            }
        }

        [Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public override string Text
        {
            get
            {
                return txt_text.Text;
            }
            set
            {
                txt_text.Text = value;
            }
        }

        [Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public bool Checked
        {
            get
            {
                return rdb_letter.Checked;
            }
            set
            {
                rdb_letter.Checked = value;
            }
        }
        #endregion

        public OptionControl()
        {
            InitializeComponent();
        }

        private void rdb_letter_Click(object sender, EventArgs e)
        {
            rdb_letter.Checked = !rdb_letter.Checked;
        }
    }
}
