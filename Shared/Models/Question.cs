using System;
using System.Collections.Generic;
using System.Drawing;

namespace Shared.Models
{
    [Serializable]
    public class Question
    {
        public int No { get; set; }

        public string Text { get; set; }

        public Bitmap Image { get; set; }

        public char Answer { get; set; }

        public bool IsMultipleChoice { get; set; }

        public char[] Answers { get; set; }

        public List<Option> Options { get; set; }

        public string Explanation { get; set; }

        public Question()
        {
            Options = new List<Option>();
        }
    }
}