using System.Windows.Forms;
using System.Diagnostics;

namespace Simulator
{
    public partial class About : Form
    {
        public About()
        {
            InitializeComponent();
        }

        private void lnk_web_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ProcessStartInfo sInfo = new ProcessStartInfo(lnk_web.Text);
            Process.Start(sInfo);
        }

        private void lnk_issues_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ProcessStartInfo sInfo = new ProcessStartInfo(lnk_issues.Text);
            Process.Start(sInfo);
        }

        private void lnk_wiki_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ProcessStartInfo sInfo = new ProcessStartInfo(lnk_wiki.Text);
            Process.Start(sInfo);
        }
    }
}
