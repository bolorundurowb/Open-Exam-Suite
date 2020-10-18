using System.Collections.Generic;

namespace oes.Models
{
    public class ExamSection
    {
        public string Title { get; set; }

        public string Instructions { get; set; }

        public List<ExamQuestion> Questions { get; set; }

        public ExamSection()
        {
            Questions = new List<ExamQuestion>();
        }
    }
}