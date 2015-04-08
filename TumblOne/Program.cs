namespace TumblOne
{
    using System;
    using System.Windows.Forms;

    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
            //Form1 mainWindow = new Form1();
            //Application.Run(mainWindow);
        }
    }
}

