using Shared;
using System.Windows.Forms;

namespace Creator
{
    public partial class UI : Form
    {
        #region Class Variables
        Exam exam;
        #endregion

        public UI()
        {
            InitializeComponent();
        }

        private void New(object sender, System.EventArgs e)
        {
            this.exam = new Exam();
            splitContainer2.Panel2.Controls.Remove(pan_splash);
            splitContainer2.Panel2.Controls.Add(pan_exam_properties);
        }

        private void Open(object sender, System.EventArgs e)
        {

        }

        private void Save(object sender, System.EventArgs e)
        {

        }

        private void SaveAs(object sender, System.EventArgs e)
        {

        }

        private void Print(object sender, System.EventArgs e)
        {

        }

        private void PrintPreview(object sender, System.EventArgs e)
        {

        }

        private void Exit(object sender, System.EventArgs e)
        {
            Application.Exit();
        }

        private void Undo(object sender, System.EventArgs e)
        {

        }

        private void Redo(object sender, System.EventArgs e)
        {

        }

        private void NewSection(object sender, System.EventArgs e)
        {
            AddSection addSection = new AddSection();
            addSection.ShowDialog();
            //
            Section section = new Section();
            section.Title = addSection.Title;
            this.exam.Sections.Add(section);
            //
            SectionNode sectionNode = new SectionNode(section);
            trv_view_exam.Nodes[0].Nodes.Add(sectionNode);
            //
            trv_view_exam.ExpandAll();
        }

        private void NewQuestion(object sender, System.EventArgs e)
        {

        }

        private void Cut(object sender, System.EventArgs e)
        {

        }

        private void Copy(object sender, System.EventArgs e)
        {

        }

        private void Paste(object sender, System.EventArgs e)
        {

        }

        private void Help(object sender, System.EventArgs e)
        {

        }

        private void About(object sender, System.EventArgs e)
        {
            About about = new About();
            about.ShowDialog();
        }

        private void AfterSelect(object sender, TreeViewEventArgs e)
        {

        }

        private void BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {

        }

        private void SaveProperties(object sender, System.EventArgs e)
        {
            this.exam.Properties.Code = txt_code.Text;
            this.exam.Properties.Instructions = txt_instruction.Text;
            this.exam.Properties.Passmark = (int)num_passmark.Value;
            this.exam.Properties.TimeLimit = (int)num_time_limit.Value;
            this.exam.Properties.Title = txt_title.Text;
            this.exam.Properties.Version = int.Parse(lbl_version.Text);
            //
            ExamNode examNode = new ExamNode(exam);
            trv_view_exam.Nodes.Add(examNode);
            //
            trv_view_exam.ExpandAll();
            //
            EnableExamControls();
        }

        private void EnableExamControls()
        {
            saveAsToolStripMenuItem.Enabled = true;
            saveToolStripButton.Enabled = true;
            saveToolStripMenuItem.Enabled = true;
            //
            printPreviewToolStripMenuItem.Enabled = true;
            printToolStripButton.Enabled = true;
            printToolStripMenuItem.Enabled = true;
            //
            undoToolStripMenuItem.Enabled = true;
            redoToolStripMenuItem.Enabled = true;
        }

        private void EnableSectionControls()
        {
            newQuestionToolStripButton.Enabled = true;
            newSectionToolStripButton.Enabled = true;
        }

        private void EnableQuestionControls()
        {
            cutToolStripButton.Enabled = true;
            cutToolStripMenuItem.Enabled = true;
            pasteToolStripButton.Enabled = true;
            pasteToolStripMenuItem.Enabled = true;
            copyToolStripButton.Enabled = true;
            copyToolStripMenuItem.Enabled = true;
        }

        private void DisableAllControls()
        {
            saveAsToolStripMenuItem.Enabled = false;
            saveToolStripButton.Enabled = false;
            saveToolStripMenuItem.Enabled = false;
            //
            printPreviewToolStripMenuItem.Enabled = false;
            printToolStripButton.Enabled = false;
            printToolStripMenuItem.Enabled = false;
            //
            newQuestionToolStripButton.Enabled = false;
            newSectionToolStripButton.Enabled = false;
            //
            undoToolStripMenuItem.Enabled = false;
            redoToolStripMenuItem.Enabled = false;
            //
            cutToolStripButton.Enabled = false;
            cutToolStripMenuItem.Enabled = false;
            pasteToolStripButton.Enabled = false;
            pasteToolStripMenuItem.Enabled = false;
            copyToolStripButton.Enabled = false;
            copyToolStripMenuItem.Enabled = false;
        }

        private void Close(object sender, System.EventArgs e)
        {
            DisableAllControls();
        }
    }
}
