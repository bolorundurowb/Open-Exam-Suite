using System.Collections.Generic;

namespace Shared
{
    public class Settings
    {
        public string CandidateName { get; set; }

        public List<Section> Sections { get; set; }

        public List<Question> Questions { get; set; }

        public int TimeLimit { get; set; }
    }
}
