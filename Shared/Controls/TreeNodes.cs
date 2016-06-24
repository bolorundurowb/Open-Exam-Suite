using System.Windows.Forms;

namespace Shared
{
    public class ExamNode : TreeNode
    {
        public Exam Exam { get; set; }

        public ExamNode(Exam exam)
        {
            this.Text = exam.Properties.Code;
            this.Exam = exam;
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
            this.Section.Questions.ForEach(x => this.Nodes.Add(new QuestionNode(x)));
        }
    }

    public class QuestionNode : TreeNode
    {
        public Question Question { get; set; }

        public QuestionNode(Question question)
        {
            this.Text = "Question " + question.No;
            this.Question = question;
        }
    }
}
