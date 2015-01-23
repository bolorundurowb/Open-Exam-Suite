using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace OpenExam_Suite
{
    public partial class Exam_Properties : Form
    {
        string fullFilePath;
        public Exam_Properties()
        {
            InitializeComponent();
        }

        public Exam_Properties(string fullFilePath)
        {
            this.fullFilePath = fullFilePath;
            InitializeComponent();
            lbl_full_path.Text = fullFilePath;
            FileInfo file = new FileInfo(fullFilePath);
            lbl_file_size.Text = Math.Round(Convert.ToDouble(file.Length / 1024.00), 2).ToString() + " KB";
            lbl_created.Text = file.CreationTime.ToShortDateString();
        }
    }
}
