using System;
using System.Windows.Forms;
using System.Threading;

namespace Simulator
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            using (Mutex mutex = new Mutex(false, "Global\\" + GetGUID()))
            {
                if (!mutex.WaitOne(0, false))
                {
                    MessageBox.Show("An instance of Open Exam Simulator is already running, select the add button include more exams.","OES Simulator", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                Application.Run(args.Length == 0 ? new UI() : new UI(args[0]));
            }
        }

        static string GetGUID ()
        {
            Guid assemblyGuid = Guid.Empty;
            object[] assemblyObjects = System.Reflection.Assembly.GetEntryAssembly().GetCustomAttributes(typeof(System.Runtime.InteropServices.GuidAttribute), true);
            if (assemblyObjects.Length > 0)
            {
                assemblyGuid = new Guid(((System.Runtime.InteropServices.GuidAttribute)assemblyObjects[0]).Value);
            }
            return assemblyGuid.ToString();
        }
    }
}
