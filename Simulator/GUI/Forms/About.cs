using System.Diagnostics;
using System.Windows.Forms;

namespace Simulator.GUI.Forms
{
    public partial class About : Form
    {
        public About()
        {
            InitializeComponent();
        }

        private void lnk_web_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo(lnk_web.Text);
            Process.Start(startInfo);
        }

        private void lnk_issues_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo(lnk_issues.Text);
            Process.Start(startInfo);
        }

        private void lnk_wiki_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo(lnk_wiki.Text);
            Process.Start(startInfo);
        }
    }
}
