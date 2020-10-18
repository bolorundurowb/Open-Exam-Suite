using System.Collections.Generic;

namespace oes.Models
{
    public class ExamQuestion
    {
        public int QuestionNumber { get; private set; }

        public string Text { get; private set; }

        public byte[] Image { get; private set; }

        public List<char> Answers { get; private set; }

        public List<QuestionOption> Options { get; private set; }

        public string AnswerExplanation { get; private set; }

        public bool IsMultipleChoice => Answers?.Count > 1;

        private ExamQuestion()
        {
        }

        public ExamQuestion(int questionNumber, string text, byte[] image = null)
        {
            QuestionNumber = questionNumber;
            Text = text;
            Image = image;
            Options = new List<QuestionOption>();
            Answers = new List<char>();
        }
    }
}