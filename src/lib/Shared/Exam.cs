using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Shared
{
    [Serializable]
    public class Exam
    {
        public int NumberOfQuestions
        {
            get
            {
                var numOfQuestions = 0;
                foreach (var section in this.Sections)
                    numOfQuestions += section.Questions.Count;
                return numOfQuestions;
            }
        }

        public Properties Properties { get; set; }

        public List<Section> Sections { get; set; }

        public Exam()
        {
            Sections = new List<Section>();
            Properties = new Properties();
        }

        public Exam(Exam exam)
        {
            Sections = new List<Section>();
            foreach (Section section in exam.Sections)
            {
                Sections.Add(new Section(section));
            }
            Properties = new Properties(exam.Properties);

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

    [Serializable]
    public class Properties
    {
        public string Title { get; set; }

        public string Code { get; set; }

        public int Version { get; set; }

        public double Passmark { get; set; }

        public int TimeLimit { get; set; }

        public string Instructions { get; set; }

        public Properties()
        {

        }

        public Properties (Properties properties)
        {
            Title = properties.Title;
            Code = properties.Code;
            Version = properties.Version;
            Passmark = properties.Passmark;
            TimeLimit = properties.TimeLimit;
            Instructions = properties.Instructions;
        }
    }

    [Serializable]
    public class Section
    {
        public string Title { get; set; }

        public List<Question> Questions { get; set; }

        public Section()
        {
            Questions = new List<Question>();
        }

        public Section(Section section)
        {
            Title = section.Title;
            Questions = new List<Question>();
            foreach(Question question in section.Questions)
            {
                Questions.Add(new Question(question));
            }
        }

        public override string ToString()
        {
            return Title;
        }
    }

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

        public Question (Question question)
        {
            No = question.No;
            Text = question.Text;
            if (question.Image != null)
            {
                Image = (Bitmap)question.Image.Clone();
            }
            Answer = question.Answer;
            IsMultipleChoice = question.IsMultipleChoice;
            if (question.Answers != null)
            {
                question.Answers.CopyTo(Answers, 0);
            }
            Options = new List<Option>();
            foreach(Option option in question.Options)
            {
                Options.Add(new Option(option));
            }
            Explanation = question.Explanation;

        }
    }

    [Serializable]
    public class Option
    {
        public char Alphabet { get; set; }

        public string Text { get; set; }

        public Option() { }

        public Option (Option option)
        {
            Alphabet = option.Alphabet;
            Text = option.Text;
        }
    }
}