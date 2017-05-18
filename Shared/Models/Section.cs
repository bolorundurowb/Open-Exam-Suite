using System;
using System.Collections.Generic;

namespace Shared.Models
{
    [Serializable]
    public class Section
    {
        public string Title { get; set; }

        public List<Question> Questions { get; set; }

        public Section()
        {
            Questions = new List<Question>();
        }

        public override string ToString()
        {
            return Title;
        }
    }
}