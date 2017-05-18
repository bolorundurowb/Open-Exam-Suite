using System;
using System.Collections.Generic;

namespace Shared.Models
{
    public class Settings
    {
        public string CandidateName { get; set; }

        public List<Section> Sections { get; set; }

        public List<Question> Questions { get; set; }

        public int TimeLimit { get; set; }

        public TimeSpan ElapsedTime { get; set; }

        public int NumberOfCorrectAnswers { get; set; }

        public List<Tuple<string,int,int>> ResultSpread { get; set; }

        public Settings()
        {
            Sections = new List<Section>();
            Questions = new List<Question>();
            CandidateName = string.Empty;
            ResultSpread = new List<Tuple<string, int, int>>();
            TimeLimit = 0;
        }
    }
}
