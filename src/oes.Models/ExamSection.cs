using System.Collections.Generic;

namespace oes.Models
{
    public class ExamSection
    {
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
    }
}