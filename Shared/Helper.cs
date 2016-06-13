using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Shared
{
    public class Helper
    {
        public static Exam GetExamFromFile (string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("Empty filepath");
            else if (!File.Exists(filePath))
                throw new FileNotFoundException("File specified does not exist");
            else
            {
                Stream stream = null;
                IFormatter formatter = new BinaryFormatter();
                stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.None);
                Exam exam = (Exam)formatter.Deserialize(stream);
                return exam;
            }            
        }

        public static bool WriteExamToFile (Exam exam, string filePath)
        {
            Stream stream = null;
            IFormatter formatter = new BinaryFormatter();
            stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None);
            formatter.Serialize(stream, exam);
            stream.Close();
            return true;
        }
    }
}
