using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Ionic.Zip;
using System.Xml;
using System.Xml.XPath;

namespace Simulator
{
    public partial class Exam_Properties : Form
    {
        string filePath;
        string filename;
        

        public Exam_Properties(string path, string fileName)
        {
            filePath = path;
            filename = fileName;
            InitializeComponent();
            lbl_full_path.Text = filePath;
            FileInfo file = new FileInfo(filePath);
            lbl_file_size.Text = Math.Round(Convert.ToDouble(file.Length / 1024.00), 2).ToString() + " KB";
            lbl_created.Text = file.CreationTime.ToShortDateString();
        }

        private void Exam_Properties_Load(object sender, EventArgs e)
        {
            string xmlPath = GlobalPathVariables.GetXmlFilePath(GlobalPathVariables.GetExamFilesFolder(filename));
            string pathToExtractFiles = GlobalPathVariables.GetExamFilesFolder(filename);
            if (Directory.Exists(pathToExtractFiles))
            {
                Directory.Delete(pathToExtractFiles, true);
                using (ZipFile zip = ZipFile.Read(filePath))
                {
                    foreach (ZipEntry ent in zip)
                    {
                        ent.Extract(pathToExtractFiles, ExtractExistingFileAction.OverwriteSilently);
                    }
                }
            }
            else
            {
                using (ZipFile zip = ZipFile.Read(filePath))
                {
                    foreach (ZipEntry ent in zip)
                    {
                        ent.Extract(pathToExtractFiles, ExtractExistingFileAction.OverwriteSilently);
                    }
                }
            }
            
            try
            {
                XPathDocument doc = new XPathDocument(xmlPath);
                XPathNavigator nav = doc.CreateNavigator();
                XPathExpression expr;
                XPathNodeIterator iterator;
                expr = nav.Compile("//ExamTitle");
                iterator = nav.Select(expr);
                while(iterator.MoveNext())
                {
                    lbl_title.Text = iterator.Current.Value;
                }
                expr = nav.Compile("//FileVersion");
                iterator = nav.Select(expr);
                while(iterator.MoveNext())
                {
                	lbl_file_version.Text = iterator.Current.Value;
                }
                expr = nav.Compile("//TimeAllowed");
                iterator = nav.Select(expr);
                while(iterator.MoveNext())
                {
                	lbl_time_limit.Text = iterator.Current.Value;
                }
                expr = nav.Compile("//PassingScore");
                iterator = nav.Select(expr);
                while (iterator.MoveNext())
                {
                	lbl_passing_score.Text = iterator.Current.Value;
                }
                expr = nav.Compile("//Section");
                iterator = nav.Select(expr);
                lbl_section_number.Text = iterator.Count.ToString();
                expr = nav.Compile("//Question");
                iterator = nav.Select(expr);
                lbl_number_of_questions.Text = iterator.Count.ToString();
            }
            catch (Exception ex)
            {
                GlobalPathVariables.WriteError(ex, this.Name);
            }
            Properties.Settings.Default.Save();
        }
    }
}
