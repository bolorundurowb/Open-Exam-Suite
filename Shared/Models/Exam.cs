using System;
using System.Collections.Generic;
using System.Linq;

namespace Shared.Models
{
    [Serializable]
    public class Exam
    {
        public int NumberOfQuestions
        {
            get
            {
                return Sections.Sum(section => section.Questions.Count);
            }
        }

        public Properties Properties { get; set; }

        public List<Section> Sections { get; set; }

        public Exam()
        {
            Sections = new List<Section>();
            Properties = new Properties();
        }

        public void AddSection(string sectionName)
        {
            var section = Sections.FirstOrDefault(s => s.Title == sectionName);
            if (section != null) return;
            section = new Section();
            section.Title = sectionName;
            Sections.Add(section);
        }

        public void RemoveSection(string sectionName)
        {
            var section = Sections.FirstOrDefault(s => s.Title == sectionName);
            if (section != null)
                Sections.Remove(section);
        }

        public void AddQuestion(string sectionName, Question question)
        {
            var section = Sections.FirstOrDefault(s => s.Title == sectionName);
            if (section == null)
            {
                section = new Section();
                section.Title = sectionName;
                Sections.Add(section);
                question.No = 1;
                section.Questions.Add(question);
            }
            else
            {
                question.No = 1;
                section.Questions.Add(question);
            }
        }

        public void RemoveQuestion(string sectionName, Question question)
        {
            var section = Sections.FirstOrDefault(s => s.Title == sectionName);
            section?.Questions.Remove(question);
        }
    }
}
