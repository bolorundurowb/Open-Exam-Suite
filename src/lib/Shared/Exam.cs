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

        public int QuestionsMarkedForReviewCount
        {
            get
            {
                int count = 0;
                foreach (Section section in Sections)
                    count += section.Questions.Count(q => q.Review);

                return count;
            }
        }

        public IEnumerable<Question> AllQuestions
        {
            get
            {
                var questions = new List<Question>();

                foreach (Section section in Sections)
                    questions.AddRange(section.Questions);

                return questions;
            }
        }

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
                section = new Section { Title = sectionName };
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

        public Question FindQuestionById(Guid id)
        {
            foreach (Section section in Sections)
            {
                Question question = section.Questions.FirstOrDefault(q => q.Id.Equals(id));

                if (question != null)
                    return question;
            }

            return null;
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

        public override string ToString()
        {
            return Title;
        }
    }

    [Serializable]
    public class Question
    {
        public Guid Id { get; set; }

        public int No { get; set; }

        public string Text { get; set; }

        public Bitmap Image { get; set; }

        public char Answer { get; set; }

        public bool IsMultipleChoice { get; set; }

        public char[] Answers { get; set; }

        public List<Option> Options { get; set; }

        public string Explanation { get; set; }

        public bool Review { get; set; }

        public Question()
        {
            Options = new List<Option>();
        }
    }

    [Serializable]
    public class Option
    {
        public char Alphabet { get; set; }

        public string Text { get; set; }
    }
}