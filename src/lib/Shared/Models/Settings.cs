using System;
using System.Collections.Generic;
using System.Linq;

namespace Shared.Models
{
    public class Settings
    {
        private List<Question> _questions;

        public string CandidateName { get; set; }

        public List<Section> Sections { get; set; }

        public List<Question> Questions
        {
            get
            {
                if (!MarkedForReviewQuestionsOnly)
                    return _questions;
                else
                    return _questions.Where(q => q.Review).ToList();
            }
            set { _questions = value; }
        }

        public int TimeLimit { get; set; }

        public TimeSpan ElapsedTime { get; set; }

        public int NumberOfCorrectAnswers { get; set; }

        public bool MarkedForReviewQuestionsOnly { get; set; }

        public List<Tuple<string, int, int>> ResultSpread { get; set; }

        public Settings()
        {
            Sections = new List<Section>();
            Questions = new List<Question>();
            CandidateName = string.Empty;
            ResultSpread = new List<Tuple<string, int, int>>();
            TimeLimit = 0;
            MarkedForReviewQuestionsOnly = false;
        }
    }
}
