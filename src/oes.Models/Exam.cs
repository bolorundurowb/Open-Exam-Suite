using System;
using System.Collections.Generic;
using System.Linq;

namespace oes.Models
{
    public class Exam
    {
        public int NumberOfQuestions => Sections.Sum(section => section.Questions.Count);

        public Properties Properties { get; set; }

        public List<Section> Sections { get; set; }

        public Exam()
        {
            Sections = new List<Section>();
            Properties = new Properties();
        }

        //Methods
        public void AddSection(string sectionName)
        {
            var section = Sections.FirstOrDefault(s => s.Title == sectionName);
            if (section == null)
            {
                section = new Section {Title = sectionName};
                Sections.Add(section);
            }
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

    public class Properties
    {
        public string Title { get; set; }

        public string Code { get; set; }

        public int Version { get; set; }

        public double Passmark { get; set; }

        public int TimeLimit { get; set; }

        public string Instructions { get; set; }
    }

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

    public class Question
    {
        public int No { get; set; }

        public string Text { get; set; }

        public byte[] Image { get; set; }

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