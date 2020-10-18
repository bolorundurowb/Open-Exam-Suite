using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace oes.Models
{
    public class Exam
    {
        public int? NumberOfQuestions => Sections?.Sum(section => section.Questions.Count);

        public ExamMetadata Metadata { get; private set; }

        public List<ExamSection> Sections { get; private set; }

        private Exam()
        {
        }

        public Exam(ExamMetadata metadata)
        {
            Metadata = metadata;
            Sections = new List<ExamSection>();
        }

        // Methods
        public void AddSection(string sectionName)
        {
            var section = Sections.FirstOrDefault(s => s.Title == sectionName);
            if (section == null)
            {
                section = new BitVector32.Section
                {
                    Title = sectionName
                };
                Sections.Add(section);
            }
        }

        public void RemoveSection(string sectionName)
        {
            var section = Sections.FirstOrDefault(s => s.Title == sectionName);
            if (section != null)
                Sections.Remove(section);
        }

        public void AddQuestion(string sectionName, Question question)
        {
            var section = Sections.FirstOrDefault(s => s.Title == sectionName);
            if (section == null)
            {
                section = new BitVector32.Section();
                section.Title = sectionName;
                Sections.Add(section);
                question.No = 1;
                section.Questions.Add(question);
            }
            else
            {
                question.No = 1;
                section.Questions.Add(question);
            }
        }

        public void RemoveQuestion(string sectionName, Question question)
        {
            var section = Sections.FirstOrDefault(s => s.Title == sectionName);
            section?.Questions.Remove(question);
        }
    }
}