using System.Drawing;
using System.Windows.Forms;

namespace TumblOne
{

    public partial class PicturePreviewFullscreen : Form
    {
        Form1 form1;

        public PicturePreviewFullscreen(Form1 form1)
        {
            InitializeComponent();
            this.form1 = form1;
        }

        public Image ShowPreviewImage
        {
            get { return this.pbFullScreen.Image; }
            set { this.pbFullScreen.DataBindings.Add("ImageLocation", form1.bsSmallImage, "Filename", false, DataSourceUpdateMode.OnPropertyChanged); }
        }

        private void pbFullScreen_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }
    }
}
