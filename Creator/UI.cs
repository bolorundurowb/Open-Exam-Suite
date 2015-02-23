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
    public partial class UI : Form
    {
        public UI()
        {
            InitializeComponent();
        }

        private void btn_new_exam_Click(object sender, EventArgs e)
        {
            Exam_and_Sections exsec = new Exam_and_Sections();
            exsec.ShowDialog();
        }
    }
}
