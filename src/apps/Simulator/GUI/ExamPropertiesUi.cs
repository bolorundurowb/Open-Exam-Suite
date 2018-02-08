using System.IO;
using System.Windows.Forms;
using Shared;

namespace Simulator.GUI
{
    public partial class ExamPropertiesUi : Form
    {
        public ExamPropertiesUi(Exam exam, string filePath)
        {
            InitializeComponent();
            //
            var info = new FileInfo(filePath);
            lbl_created.Text = info.CreationTime.ToShortDateString();
            lbl_file_size.Text = info.Length > 1022976 ? (info.Length / 1048576.0).ToString("F") + " MB" : (info.Length / 1024.0).ToString("F") + " KB";

            lbl_file_version.Text = exam.Properties.Version.ToString();
            lbl_full_path.Text = filePath;
            lbl_number_of_questions.Text = exam.NumberOfQuestions.ToString();
            lbl_passing_score.Text = exam.Properties.Passmark.ToString();
            lbl_section_number.Text = exam.Sections.Count.ToString();
            lbl_time_limit.Text = exam.Properties.TimeLimit.ToString();
            lbl_title.Text = exam.Properties.Title;
        }
    }
}