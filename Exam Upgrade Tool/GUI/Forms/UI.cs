using Shared;
using System;
using System.IO;
using System.Windows.Forms;
using Shared.Models;

namespace Exam_Upgrade_Tool
{
    public partial class UI : Form
    {
        public UI()
        {
            InitializeComponent();
        }

        private void btn_browse_open_Click(object sender, System.EventArgs e)
        {
            opf_old_exam.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (opf_old_exam.ShowDialog() == DialogResult.OK)
            {
                txt_old_exam.Text = opf_old_exam.FileName;
                string filename = Path.GetFileNameWithoutExtension(txt_old_exam.Text) + "-new.oef";
                string folder = Path.GetDirectoryName(txt_old_exam.Text);
                string newFileName = Path.Combine(folder, filename);
                txt_new_exam.Text = newFileName;
            }
        }

        private void btn_browse_save_Click(object sender, EventArgs e)
        {
            if(string.IsNullOrWhiteSpace(txt_new_exam.Text))
            {
                sfd_new_exam.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                if(sfd_new_exam.ShowDialog() == DialogResult.OK)
                {
                    txt_new_exam.Text = sfd_new_exam.FileName;
                }
            }
            else
            {
                sfd_new_exam.InitialDirectory = Path.GetDirectoryName(txt_new_exam.Text);
                sfd_new_exam.FileName = Path.GetFileNameWithoutExtension(txt_new_exam.Text);
                if (sfd_new_exam.ShowDialog() == DialogResult.OK)
                {
                    txt_new_exam.Text = sfd_new_exam.FileName;
                }
            }
        }

        private void btn_convert_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txt_new_exam.Text))
            {
                MessageBox.Show("Make sure a new exam file has been selected.", "incomplete info", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (string.IsNullOrWhiteSpace(txt_old_exam.Text))
            {
                MessageBox.Show("Please select an exam to be converted.", "incomplete info", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                try
                {
                    string folderPath = Helper.ExtractExamToFolder(txt_old_exam.Text);
                    if (folderPath == null)
                    {
                        MessageBox.Show("The exam you are trying to convert doesn't need conversion.");
                        throw new Exception();
                    }
                    string xmlFilePath = Helper.GetXmlFilePath(folderPath);
                    Exam exam = Helper.CreateExamFromXml(xmlFilePath);
                    Shared.Util.Helper.WriteExamToFile(exam, txt_new_exam.Text);
                    lbl_status.Text = "The exam was successfully converted.";
                }
                catch (Exception ex)
                {
                    lbl_status.Text = "An error occured. " + ex.Message;
                }
            }
        }
    }
}
