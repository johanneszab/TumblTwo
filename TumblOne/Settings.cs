using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TumblTwo
{
    public partial class Settings : Form
    {
        CrawlerForm crawlerForm;
        bool DownloadLocationChanged = false;

        public Settings(CrawlerForm crawlerForm)
        {
            this.crawlerForm = crawlerForm;
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
                crawlerForm.smallImage.Visible = true;
            }
            else
            {
                Properties.Settings.Default.configPreviewVisible = false;
                crawlerForm.smallImage.Visible = false;
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
            this.nudParallelImageDownloads.Value = Convert.ToDecimal(Properties.Settings.Default.configParallelImageDownloads);
            this.cbImagesize.SelectedItem = Convert.ToString(Properties.Settings.Default.configImageSize);
            this.tbDownloadLocation.Text = Properties.Settings.Default.configDownloadLocation;
            this.cbPicturePreview.Checked = Properties.Settings.Default.configPreviewVisible;
            this.cbRemoveFinished.Checked = Properties.Settings.Default.configRemoveFinishedBlogs;
            this.cbDeleteIndexOnly.Checked = Properties.Settings.Default.configDeleteIndexOnly;
            this.chkGif.Checked = Properties.Settings.Default.configChkGIFState;
            this.cbCheckStatus.Checked = Properties.Settings.Default.configCheckStatusAtStartup;
            this.cbParallelCrawl.Checked = Properties.Settings.Default.configParallelCrawl;
            this.cbCheckMirror.Checked = Properties.Settings.Default.configCheckMirror;
        }

        static void Loaded_PropertyChanged(
            object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Console.WriteLine("Changed {0}", e.PropertyName);
        }

        private void Settings_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Reload Settings and Blogs
            if (crawlerForm.taskList[0] == null && DownloadLocationChanged)
            {
                crawlerForm.LoadGUI();
            }
            if (Convert.ToInt32(this.nudSimultaneousDownloads.Value) > Properties.Settings.Default.configSimultaneousDownloads)
            {
                Array.Resize(ref crawlerForm.taskList, crawlerForm.taskList.Length + (Convert.ToInt32(this.nudSimultaneousDownloads.Value) - Properties.Settings.Default.configSimultaneousDownloads));
            }
            else
            {
                if (crawlerForm.taskList[0] == null)
                {
                    Array.Resize(ref crawlerForm.taskList, Convert.ToInt32(this.nudSimultaneousDownloads.Value));
                }
            }
            // Save Settings
            Properties.Settings.Default.configSimultaneousDownloads = Convert.ToInt32(this.nudSimultaneousDownloads.Value);
            Properties.Settings.Default.configParallelImageDownloads = Convert.ToInt32(this.nudParallelImageDownloads.Value);
            Properties.Settings.Default.configImageSize = Convert.ToInt32(this.cbImagesize.SelectedItem);
            Properties.Settings.Default.Save();
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            // Check Settings_FormClosing
            this.Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void cbDeleteIndexOnly_CheckedChanged(object sender, EventArgs e)
        {
            if (this.cbDeleteIndexOnly.Checked)
            {
                Properties.Settings.Default.configDeleteIndexOnly = true;
            }
            else
            {
                Properties.Settings.Default.configDeleteIndexOnly = false;
            }
        }

        private void chkGif_CheckedChanged(object sender, EventArgs e)
        {
            if (this.chkGif.Checked)
            {
                Properties.Settings.Default.configChkGIFState = true;
            }
            else
            {
                Properties.Settings.Default.configChkGIFState = false;
            }
        }

        private void cbCheckStatus_CheckedChanged(object sender, EventArgs e)
        {
            if (this.cbCheckStatus.Checked)
            {
                Properties.Settings.Default.configCheckStatusAtStartup = true;
            }
            else
            {
                Properties.Settings.Default.configCheckStatusAtStartup = false;
            }
        }

        private void cbParallelCrawl_CheckedChanged(object sender, EventArgs e)
        {
            if (this.cbParallelCrawl.Checked)
            {
                Properties.Settings.Default.configParallelCrawl = true;
            }
            else
            {
                Properties.Settings.Default.configParallelCrawl = false;
            }
        }

        private void cbCheckMirror_CheckedChanged(object sender, EventArgs e)
        {
            if (this.cbParallelCrawl.Checked)
            {
                Properties.Settings.Default.configCheckMirror = true;
            }
            else
            {
                Properties.Settings.Default.configCheckMirror = false;
            }
        }
    }
}
