namespace oes.Models
{
    public class ExamMetadata
    {
        public string Title { get; private set; }

        public string Instructions { get; private set; }

        public string Code { get; private set; }

        public int Version { get; private set; }

        public int TimeLimitInMinutes { get; private set; }

        public float MinimumScorePercentage { get; private set; }

        private ExamMetadata()
        {
        }

        public ExamMetadata(string title, string instructions, string code, int timeLimitInMinutes,
            float minimumScorePercentage)
        {
            Title = title;
            Instructions = instructions;
            Code = code;
            TimeLimitInMinutes = timeLimitInMinutes;
            MinimumScorePercentage = minimumScorePercentage;
            Version = 1;
        }

        public void BumpVersion()
        {
            Version += 1;
        }
    }
}