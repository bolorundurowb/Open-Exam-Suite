using System.Collections.Generic;
using System.Linq;

namespace oes.Models
{
    public class ExamSection
    {
        /// <summary>
        /// Unique and used to specify the order of the sections
        /// </summary>
        public int SectionNumber { get; private set; }

        public string Title { get; private set; }

        public string Instructions { get; private set; }

        public List<ExamQuestion> Questions { get; private set; }

        private ExamSection()
        {
        }

        public ExamSection(int sectionNumber, string title, string instructions)
        {
            SectionNumber = sectionNumber;
            Title = title;
            Instructions = instructions;
            Questions = new List<ExamQuestion>();
        }

        public ExamQuestion AddQuestion(string text, byte[] image = null)
        {
            var questionNumber = Questions.Any() ? Questions.Last().QuestionNumber + 1 : 1;
            var question = new ExamQuestion(questionNumber, text, image);
            Questions.Add(question);

            return question;
        }

        public void RemoveQuestion(int questionNumber)
        {
            var question = Questions
                .FirstOrDefault(s => s.QuestionNumber == questionNumber);

            if (question != null)
            {
                Questions.Remove(question);
            }
        }
    }
}