using System.Collections.Generic;

namespace oes.Models
{
    public class ExamQuestion
    {
        public int QuestionNumber { get; set; }

        public string Text { get; set; }

        public byte[] Image { get; set; }

        public bool IsMultipleChoice { get; set; }

        public List<char> Answers { get; set; }

        public List<QuestionOption> Options { get; set; }

        public string AnswerExplanation { get; set; }

        public ExamQuestion()
        {
            Options = new List<QuestionOption>();
            Answers = new List<char>();
        }
    }
}