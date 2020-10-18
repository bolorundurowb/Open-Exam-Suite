using System;
using System.Collections.Generic;
using System.Linq;

namespace oes.Models
{
    public class ExamQuestion
    {
        private const int IntegerRepresentationOfAsciiA = 65;

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

        public QuestionOption AddOption(string text)
        {
            var label = Convert.ToChar(IntegerRepresentationOfAsciiA + Answers.Count);
            var option = new QuestionOption(label, text);
            Options.Add(option);

            return option;
        }

        public void AddAnswer(char label)
        {
            if (Answers.Contains(label))
            {
                return;
            }

            if (Options.All(x => x.Label != label))
            {
                throw new InvalidOperationException($"An option must exsit for the selected answer '{label}'");
            }

            Answers.Add(label);
        }
    }
}