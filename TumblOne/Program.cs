using System;
using System.Windows.Forms;

namespace TumblTwo
{

    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            // Increase connection limit for faster url list generation
            System.Net.ServicePointManager.DefaultConnectionLimit = 100;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new CrawlerForm());
        }
    }
}