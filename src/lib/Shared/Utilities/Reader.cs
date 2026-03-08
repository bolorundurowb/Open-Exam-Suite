using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using Logging;
using Newtonsoft.Json;

namespace Shared.Utilities;

public static class Reader
{
    public static Exam? FromOefFile(string filePath, bool throwOnError = false)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("Empty filepath");

        if (!File.Exists(filePath))
            throw new FileNotFoundException("File specified does not exist");

        IFormatter formatter = new BinaryFormatter();
        formatter.Binder = new DeserializationBinder();
        try
        {
            using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.None);
            var exam = (Exam) formatter.Deserialize(stream);
            return exam;
        }
        catch (Exception ex)
        {
            Logger.LogException(ex);

            if (throwOnError)
            {
                throw;
            }

            return null;
        }
    }

    public static Exam? FromJsonFile(string filePath)
    {
        var jsonString = File.ReadAllText(filePath);
        return JsonConvert.DeserializeObject<Exam>(jsonString);
    }

    public static Exam? FromXmlFile(string filePath)
    {
        var xmlSerializer = new XmlSerializer(typeof(Exam));
        using var streamReader = new StreamReader(filePath);
        return xmlSerializer.Deserialize(streamReader) as Exam;
    }
}