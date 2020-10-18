namespace oes.Models
{
    public class QuestionOption
    {
        public char Label { get; private set; }

        public string Text { get; private set; }

        private QuestionOption()
        {
        }

        public QuestionOption(char label, string text)
        {
            Label = label;
            Text = text;
        }
    }
}