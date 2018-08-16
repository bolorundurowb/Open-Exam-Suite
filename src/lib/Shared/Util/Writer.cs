using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace Shared.Util
{
    public static class Writer
    {
        public static bool ToOef(Exam exam, string filePath, bool throwOnError = false)
        {
            if (exam == null)
                throw new NullReferenceException("The exam to be written cannot be null.");
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("Empty filepath");

            IFormatter formatter = new BinaryFormatter();
            try
            {
                using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    formatter.Serialize(stream, exam);
                    return true;
                }
            }
            catch (Exception)
            {
                if (throwOnError)
                {
                    throw;
                }

                return false;
            }
        }
        
        public static bool ToPdf(Exam exam, string filePath)
        {
            try
            {
                
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool ToJson(Exam exam, string filePath)
        {
            try
            {
                var examJsonString = JsonConvert.SerializeObject(exam, Formatting.Indented);
                File.WriteAllText(filePath, examJsonString);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool ToXml(Exam exam, string filePath)
        {
            try
            {
                var examXmlStringWriter = new StringWriter();
                var serializer = new XmlSerializer(exam.GetType());
                serializer.Serialize(examXmlStringWriter, exam);
                File.WriteAllText(filePath, examXmlStringWriter.ToString());
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
