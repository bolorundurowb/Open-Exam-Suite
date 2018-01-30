using System;
using System.IO;
using System.Xml.Serialization;
using MigraDoc.DocumentObjectModel;
using MigraDoc.Rendering;
using Newtonsoft.Json;
using PdfSharp.Pdf;

namespace Shared.Util
{
    public class Writer
    {
        public static bool ToPdf(Exam exam, string filePath)
        {
            try
            {
                Document document = Pdf.CreateDocument(exam);
                document.UseCmykColor = true;
                PdfDocumentRenderer pdfDocumentRenderer = new PdfDocumentRenderer(true, PdfFontEmbedding.Always)
                {
                    Document = document
                };
                pdfDocumentRenderer.RenderDocument();
                pdfDocumentRenderer.PdfDocument.Save(filePath);
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
