using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace Shared.Util
{
    public static class Reader
    {
        public static Exam FromOefFile(string filePath, bool throwOnError = false)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("Empty filepath");
            if (!File.Exists(filePath))
                throw new FileNotFoundException("File specified does not exist");

            IFormatter formatter = new BinaryFormatter();
            formatter.Binder = new DeserializationBinder();
            try
            {
                using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    var exam = (Exam) formatter.Deserialize(stream);
                    return exam;
                }
            }
            catch (Exception ex)
            {
                
                if (throwOnError)
                {
                    throw;
                }

                return null;
            }
        }

        public static Exam FromJsonFile(string filePath)
        {
            Exam exam;
            using (var streamReader = new StreamReader(filePath))
            {
                var jsonString = streamReader.ReadToEnd();
                exam = JsonConvert.DeserializeObject<Exam>(jsonString);
            }

            return exam;
        }

        public static Exam FromXmlFile(string filePath)
        {
            Exam exam;
            var xmlSerializer = new XmlSerializer(typeof(Exam));
            using (var streamReader = new StreamReader(filePath))
            {
                exam = (Exam) xmlSerializer.Deserialize(streamReader);
            }

            return exam;
        }
    }
}