using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Shared.Controls
{
    public partial class OptionsControl : UserControl
    {

        #region Public Properties
        [Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public char Letter
        {
            get => Convert.ToChar(chkLetter.Text);
            set => chkLetter.Text = value.ToString();
        }

        [Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public override string Text
        {
            get => txtText.Text;
            set => txtText.Text = value;
        }

        [Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public bool Checked
        {
            get => chkLetter.Checked;
            set => chkLetter.Checked = value;
        }
        #endregion
        public OptionsControl()
        {
            InitializeComponent();
        }
    }
}
