using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenExam_Suite
{
    public partial class Exam_UI : Form
    {
        //Global Variables
        int timeLeft;

        public Exam_UI()
        {
            InitializeComponent();
        }

        public Exam_UI(string filename)
        {
            InitializeComponent();
            timeLeft = Properties.Settings.Default.TimerValue;
        }

        private void btn_pause_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Exam paused, Click 'OK' to continue.", "Paused", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btn_end_Click(object sender, EventArgs e)
        {

        }

        private void btn_next_Click(object sender, EventArgs e)
        {

        }

        private void btn_previous_Click(object sender, EventArgs e)
        {

        }

        private void btn_begin_Click(object sender, EventArgs e)
        {
            pan_display.Controls.Clear();
            btn_begin.Enabled = false;
            btn_begin.Visible = false;
            btn_end.Enabled = true;
            btn_end.Visible = true;
            btn_pause.Enabled = true;
            btn_pause.Visible = true;
            btn_previous.Enabled = true;
            btn_previous.Visible = true;
            btn_next.Enabled = true;
            btn_next.Visible = true;
            lbl_elapsed_time.Visible = true;
            lbl_elapsed_time.Enabled = true;
            label1.Enabled = true;
            label1.Visible = true;
        }

        private void timer_Tick(object sender, EventArgs e)
        {

        }

        private void Exam_UI_Load(object sender, EventArgs e)
        {

        }
    }
}
