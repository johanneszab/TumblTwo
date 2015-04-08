namespace TumblOne
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;

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