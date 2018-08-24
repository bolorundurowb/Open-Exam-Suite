using System;
using System.Drawing;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Logging;
using Newtonsoft.Json;
using Font = iTextSharp.text.Font;
using Image = iTextSharp.text.Image;

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
            catch (Exception ex)
            {
                Logger.LogException(ex);

                if (throwOnError)
                {
                    throw;
                }

                return false;
            }
        }
        
        public static bool ToPdf(Exam exam, string filePath)
        {
            FileStream fs = null;
            Document doc = null;
            PdfWriter writer = null;

            try
            {
                fs = new FileStream(filePath, FileMode.OpenOrCreate);
                doc = new Document(PageSize.A4, 40, 40, 50, 50);
                writer = PdfWriter.GetInstance(doc, fs);
                
                doc.AddCreationDate();
                doc.AddCreator("Open Exam Suite");
                doc.AddSubject(exam.Properties.Code);
                doc.AddTitle(exam.Properties.Title);
                
                doc.Open();
                var headerFont = new Font(Font.FontFamily.HELVETICA, 13f, Font.BOLD);
                doc.Add(new Chunk("Exam Title: ", headerFont));
                doc.Add(new Chunk(exam.Properties.Title + "" + Environment.NewLine));
                doc.Add(new Paragraph());
                doc.Add(new Chunk("Exam Code: ", headerFont));
                doc.Add(new Chunk(exam.Properties.Code + "" + Environment.NewLine));
                doc.Add(new Chunk("Passmark: ", headerFont));
                doc.Add(new Chunk(exam.Properties.Passmark + " / 1000" + Environment.NewLine));
                doc.Add(new Chunk("Time Limit: ", headerFont));
                doc.Add(new Chunk(exam.Properties.TimeLimit + " (min)" + Environment.NewLine));
                doc.Add(new Chunk("Instructions: ", headerFont));
                doc.Add(new Chunk(exam.Properties.Instructions + "" + Environment.NewLine));

                doc.Add(new Chunk("" + Environment.NewLine));
                foreach (var section in exam.Sections)
                {
                    doc.Add(new Chunk("Section: ", headerFont));
                    doc.Add(new Chunk(section.Title + "" + Environment.NewLine));

                    foreach (var question in section.Questions)
                    {
                        doc.Add(new Paragraph(question.No + ". " + question.Text));

                        if (question.Image != null)
                        {
                            doc.Add(Image.GetInstance(BitmapToByteArray(question.Image)));
                        }

                        foreach (var option in question.Options)
                        {
                            doc.Add(new Paragraph(option.Alphabet + " - " + option.Text));
                        }

                        doc.Add(new Paragraph("Answer: " + question.Answer));

                        if (!string.IsNullOrWhiteSpace(question.Explanation))
                        {
                            doc.Add(new Paragraph("Explanation: " + question.Explanation));
                        }

                        doc.Add(new Chunk("" + Environment.NewLine));
                    }

                    doc.Add(new Chunk("" + Environment.NewLine));
                }

                doc.Close();
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);

                return false;
            }
            finally
            {
                writer?.Close();
                fs?.Close();
            }

            return true;
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
        
        private static byte[] BitmapToByteArray(Bitmap bitmap)
        {
            byte[] result = null;

            if (bitmap != null)
            {
                using (var stream = new MemoryStream())
                {
                    bitmap.Save(stream, bitmap.RawFormat);
                    result = stream.ToArray();
                }
            }
            
            return result;
        }
    }
}
