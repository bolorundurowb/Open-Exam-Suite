using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shared
{
    public class Settings
    {
        public string CandidateName { get; set; }

        public List<Section> SelectedSections { get; set; }

        public int NumberOfQuestions { get; set; }

        public int TimeLimit { get; set; }

        public QuestionChoice Choice { get; set; }
    }

    public enum QuestionChoice
    {
        SelectedSections,
        SpecifiedNumber
    }
}
