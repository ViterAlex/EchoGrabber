using System;
using System.Windows.Forms;

namespace EchoGrabber.GUI
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            var splash = new Splash();
            var result = splash.ShowDialog();
            if (result != DialogResult.OK)
            {
                MessageBox.Show("Ошибка");
                return;
            }
            Application.Run(new MainForm());
        }
    }
}
