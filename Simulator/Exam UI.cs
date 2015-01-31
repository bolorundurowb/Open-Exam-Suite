using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.XPath;
using System.IO;

namespace OpenExam_Suite
{
    public partial class Exam_UI : Form
    {
        //Global Variables
        int timeLeft;
        string filename;
        string outputPath;
        List<Question> questions;

        public Exam_UI()
        {
            InitializeComponent();
        }

        /// <summary>
        /// The alternate and correct constructor
        /// </summary>
        /// <param name="fileName">the name of the exam file without extensions</param>
        public Exam_UI(string fileName)
        {
            InitializeComponent();
            timeLeft = Properties.Settings.Default.TimerValue * 60;
            this.filename = fileName;
            this.outputPath = Properties.Settings.Default.ExamPath;
        }

        private void btn_pause_Click(object sender, EventArgs e)
        {
            timer.Stop();
            DialogResult result = MessageBox.Show("Exam paused, Click 'OK' to continue.", "Paused", MessageBoxButtons.OK, MessageBoxIcon.Information);
            if (result == DialogResult.OK)
            {
                timer.Start(); 
            }
        }

        private void btn_end_Click(object sender, EventArgs e)
        {

        }

        private void btn_next_Click(object sender, EventArgs e)
        {
            pan_display.Controls.Clear();
        }

        private void btn_previous_Click(object sender, EventArgs e)
        {
            pan_display.Controls.Clear();
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
            timer.Start();
            //
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if (timeLeft > 0)
            {
                // Display the new time left 
                // by updating the Time Left label.
                timeLeft = timeLeft - 1;
                double x = timeLeft/3600;
                double hours = Math.Floor(x);
                double y = (timeLeft % 3600) / 60;
                double minutes = Math.Floor(y);
                double seconds = timeLeft % 60;
                lbl_elapsed_time.Text = hours.ToString() + ":" + minutes.ToString() + ":" + seconds.ToString();
            }
            else
            {
                // If the user ran out of time, stop the timer, show 
                // a MessageBox, and fill in the answers.
                timer.Stop();
                lbl_elapsed_time.Text = "Time's up!";
            }
        }

        private void Exam_UI_Load(object sender, EventArgs e)
        {
            try
            {
                XPathDocument doc = new XPathDocument(outputPath + Path.DirectorySeparatorChar + filename + ".xml");
                XPathNavigator nav = doc.CreateNavigator();
                // Compile a standard XPath expression
                XPathExpression expr;
                expr = nav.Compile("//ExamTitle");
                XPathNodeIterator iterator = nav.Select(expr);
                // Iterate on the node set
                while (iterator.MoveNext())
                {
                    lbl_exam_title.Text = iterator.Current.Value;
                }
                expr = nav.Compile("//ExamCode");
                iterator = nav.Select(expr);
                // Iterate on the node set
                while (iterator.MoveNext())
                {
                    lbl_exam_code.Text = iterator.Current.Value;
                }
                expr = nav.Compile("//ExamInstructions");
                iterator = nav.Select(expr);
                // Iterate on the node set
                while (iterator.MoveNext())
                {
                    lbl_exam_instructions.Text = iterator.Current.Value;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            questions = new List<Question>();
            questions = Question.GetQuestions(outputPath);
        }

        /// <summary>
        /// A method to remove the empty strings in the selected strings value
        /// </summary>
        /// <param name="sections">the sring gotten from settings</param>
        /// <returns>a string array containing the title of the selected sections</returns>
        public static string[] GetSections (string sections)
        {
            List<string> temp2 = new List<string>();
            try
            {
                string[] temp1 = sections.Split(',');
                for (int i = 0; i < temp1.Length; i++)
                {
                    if (string.IsNullOrWhiteSpace(temp1[i]))
                    {
                        //blah
                    }
                    else
                    {
                        temp2.Add(temp1[i]);
                    }
                }
            }
            catch (NullReferenceException)
            {
                MessageBox.Show("Please select at least one section", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            string[] returnValue = temp2.ToArray();
            return returnValue;
        }
    }
}
