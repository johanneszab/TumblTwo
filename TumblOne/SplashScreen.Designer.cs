namespace TumblOne
{

    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;

    public partial class SplashScreen
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        /// 
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.timer1 = new Timer(this.components);
            base.SuspendLayout();
            this.timer1.Enabled = true;
            this.timer1.Interval = 0x5dc;
            this.timer1.Tick += new EventHandler(this.SplashScreen_Click);
            base.AutoScaleDimensions = new SizeF(6f, 13f);
//            base.AutoScaleMode = AutoScaleMode.Font;
            this.BackgroundImage = TumblOne.Properties.Resources.TumblrLogo;
            base.ClientSize = new Size(0x18f, 400);
//            base.FormBorderStyle = FormBorderStyle.None;
            base.Name = "SplashScreen";
//            base.SizeGripStyle = SizeGripStyle.Hide;
            base.StartPosition = FormStartPosition.CenterParent;
            this.Text = "SplashScreen";
            base.Click += new EventHandler(this.SplashScreen_Click);
            base.ResumeLayout(false);
        }

        #endregion

        public Timer timer1;

    }
}