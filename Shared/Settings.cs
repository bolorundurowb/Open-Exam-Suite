using System.Collections.Generic;

namespace Shared
{
    public class Settings
    {
        public string CandidateName { get; set; }

        public List<Section> Sections { get; set; }

        public List<Question> Questions { get; set; }

        public int TimeLimit { get; set; }

        public Settings()
        {
            Sections = new List<Section>();
            Questions = new List<Question>();
            CandidateName = string.Empty;
            TimeLimit = 0;
        }
    }
}
