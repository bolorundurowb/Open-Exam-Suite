using System.Windows.Forms;

namespace Shared.Controls
{
    public class ExamNode : TreeNode
    {
        public Exam Exam { get; set; }

        public ExamNode(Exam exam)
        {
            this.Text = exam.Properties.Title;
            this.Exam = exam;
            this.ImageIndex = 0;
            this.SelectedImageIndex = 0;
            this.Exam.Sections.ForEach(x => this.Nodes.Add(new SectionNode(x)));
        }
    }

    public class SectionNode : TreeNode
    {
        public Section Section { get; set; }

        public SectionNode(Section section)
        {
            this.Text = section.Title;
            this.Section = section;
            this.ImageIndex = 1;
            this.SelectedImageIndex = 1;
            this.Section.Questions.ForEach(x => this.Nodes.Add(new QuestionNode(x)));
        }
    }

    public class QuestionNode : TreeNode
    {
        public Question Question { get; set; }

        public QuestionNode(Question question)
        {
            this.Text = "Question " + question.No;

            this.ImageIndex = 2;
            this.SelectedImageIndex = 2;
            this.Question = question;
        }
    }
}
