using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Creator
{
    class WriteXML
    {
        /// <summary>
        /// Gets the questions sorted into sections
        /// </summary>
        /// <param name="sectionTitles">The list  of sections the questions are to be sorted into</param>
        /// <param name="questionList">the complete list of all questions</param>
        /// <returns>A dictionary of the sorted questions ready for writing</returns>
        public static Dictionary<string, List<Question>> ConvertListToFormat(string[] sectionTitles, List<Question> questionList)
        {
            Dictionary<string, List<Question>> result = new Dictionary<string, List<Question>>();
            foreach (string section in sectionTitles)
            {
                var sectionQuestionList = questionList.FindAll(s => s.SectionTitle == section);
                result.Add(section, sectionQuestionList);
            }
            return result;
        }

    }
}
