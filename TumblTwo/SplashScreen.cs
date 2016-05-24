using System;
using System.Windows.Forms;

namespace TumblTwo
{

    public partial class SplashScreen : Form
    {
        //private IContainer components;

        public SplashScreen()
        {
            this.InitializeComponent();
        }

        private void SplashScreen_Click(object sender, EventArgs e)
        {
            base.Close();
        }
    }
}