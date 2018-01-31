using System.Windows.Forms;
using Shared.Models;

namespace Shared.Controls
{
    public class ExamNode : TreeNode
    {
        public Properties Properties { get; set; }

        public ExamNode(Properties properties)
        {
            this.Text = properties.Title;
            this.Properties = properties;
            this.ImageIndex = 0;
            this.SelectedImageIndex = 0;
        }
    }

    public class SectionNode : TreeNode
    {
        public string Title { get; set; }

        public SectionNode(string title)
        {
            this.Text = title;
            this.Title = title;
            this.ImageIndex = 1;
            this.SelectedImageIndex = 1;
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
