namespace oes.Models
{
    public class ExamSection
    {
        public string Title { get; set; }

        public string Instructions { get; set; }

        public List<Question> Questions { get; set; }

        public ExamSection()
        {
            Questions = new List<Question>();
        }

        public override string ToString()
        {
            return Title;
        }
    }
}