using System.Collections.Generic;
using System.Linq;

namespace oes.Models
{
    public class Exam
    {
        public int NumberOfQuestions => Sections.Sum(x => x.Questions.Count);

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
        public ExamSection AddSection(string title, string instructions)
        {
            var sectionNumber = Sections.Any() ? Sections.Last().SectionNumber + 1 : 1;
            var section = new ExamSection(sectionNumber, title, instructions);
            Sections.Add(section);

            return section;
        }

        public void RemoveSection(int sectionNumber)
        {
            var section = Sections
                .FirstOrDefault(s => s.SectionNumber == sectionNumber);

            if (section != null)
            {
                Sections.Remove(section);
            }
        }
    }
}