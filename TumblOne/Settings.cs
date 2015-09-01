using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TumblOne
{
    public partial class Settings : Form
    {
        Form1 _Form1;
        bool DownloadLocationChanged = false;

        public Settings(Form1 _theForm1)
        {
            _Form1 = _theForm1;
            InitializeComponent();
        }

        private void bChooseDownloadLocation_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            DialogResult result = fbd.ShowDialog();

            if (result == DialogResult.OK)
            {
                Properties.Settings.Default.configDownloadLocation = fbd.SelectedPath + "\\";
                this.tbDownloadLocation.Text = fbd.SelectedPath + "\\";
                DownloadLocationChanged = true;
            }

//            string[] files = Directory.GetFiles(fbd.SelectedPath);
//            System.Windows.Forms.MessageBox.Show("Files found: " + files.Length.ToString(), "Message");
        }

        private void tbDownloadLocation_TextChanged(object sender, EventArgs e)
        {
            if (!this.tbDownloadLocation.Text.EndsWith("\\"))
            {
                this.tbDownloadLocation.Text = this.tbDownloadLocation.Text + "\\";
            }
            Properties.Settings.Default.configDownloadLocation = this.tbDownloadLocation.Text;
            DownloadLocationChanged = true;
        }

        private void cbPicturePreview_CheckedChanged(object sender, EventArgs e)
        {
            if (this.cbPicturePreview.Checked)
            {
                Properties.Settings.Default.configPreviewVisible = true;
                _Form1.smallImage.Visible = true;
            }
            else
            {
                Properties.Settings.Default.configPreviewVisible = false;
                _Form1.smallImage.Visible = false;
            }
        }

        private void cbRemoveFinished_CheckedChanged(object sender, EventArgs e)
        {
            if (this.cbRemoveFinished.Checked)
            {
                Properties.Settings.Default.configRemoveFinishedBlogs = true;
            }
            else
            {
                Properties.Settings.Default.configRemoveFinishedBlogs = false;
            }
        }

        private void Settings_Load(object sender, EventArgs e)
        {
            this.nudSimultaneousDownloads.Value = Convert.ToDecimal(Properties.Settings.Default.configSimultaneousDownloads);
            this.cbImagesize.SelectedItem = Convert.ToString(Properties.Settings.Default.configImageSize);
            this.tbDownloadLocation.Text = Properties.Settings.Default.configDownloadLocation;
            this.cbPicturePreview.Checked = Properties.Settings.Default.configPreviewVisible;
            this.cbRemoveFinished.Checked = Properties.Settings.Default.configRemoveFinishedBlogs;
        }

        static void Loaded_PropertyChanged(
            object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Console.WriteLine("Changed {0}", e.PropertyName);
        }

        private void Settings_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Reload Settings and Blogs
            if (_Form1.tasks[0] == null && DownloadLocationChanged)
            {
                _Form1.LoadLibrary();
            }
            if (Convert.ToInt32(this.nudSimultaneousDownloads.Value) > Properties.Settings.Default.configSimultaneousDownloads)
            {
                Array.Resize(ref _Form1.tasks, _Form1.tasks.Length + (Convert.ToInt32(this.nudSimultaneousDownloads.Value) - Properties.Settings.Default.configSimultaneousDownloads));
            }
            else
            {
                if (_Form1.tasks[0] == null)
                {
                    Array.Resize(ref _Form1.tasks, Convert.ToInt32(this.nudSimultaneousDownloads.Value));
                }
            }
            // Save Settings
            Properties.Settings.Default.configSimultaneousDownloads = Convert.ToInt32(this.nudSimultaneousDownloads.Value);
            Properties.Settings.Default.configImageSize = Convert.ToInt32(this.cbImagesize.SelectedItem);
            Properties.Settings.Default.Save();
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            // Reload Settings and Blogs
            if (_Form1.tasks[0] == null && DownloadLocationChanged)
            {
                _Form1.LoadLibrary();
            }
            if (Convert.ToInt32(this.nudSimultaneousDownloads.Value) > Properties.Settings.Default.configSimultaneousDownloads)
            {
                Array.Resize(ref _Form1.tasks, _Form1.tasks.Length + (Convert.ToInt32(this.nudSimultaneousDownloads.Value) - Properties.Settings.Default.configSimultaneousDownloads));
            }
            else
            {
                if (_Form1.tasks[0] == null)
                {
                    Array.Resize(ref _Form1.tasks, Convert.ToInt32(this.nudSimultaneousDownloads.Value));
                }
            }
            // Save Settings
            Properties.Settings.Default.configSimultaneousDownloads = Convert.ToInt32(this.nudSimultaneousDownloads.Value);
            Properties.Settings.Default.configImageSize = Convert.ToInt32(this.cbImagesize.SelectedItem);
            Properties.Settings.Default.Save();
            this.Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

    }
}
