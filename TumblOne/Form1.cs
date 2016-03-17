﻿namespace TumblOne
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using System.Xml;
    using System.Xml.Linq;

    public partial class Form1 : Form
    {
        public List<TumblrBlog> TumblrActiveList = new List<TumblrBlog>();
        public Task[] tasks = new Task[Properties.Settings.Default.configSimultaneousDownloads];
        string crawlingBlogs = "";
        public BindingSource bsSmallImage = new BindingSource();
        private CancellationTokenSource cts = new CancellationTokenSource();
        private List<TumblrBlog> bin = new List<TumblrBlog>();
        private TumblOne.SortableBindingList<TumblrBlog> blogs = new TumblOne.SortableBindingList<TumblrBlog>();
        private BindingList<String> pictureList = new BindingList<String>();

        public Form1()
        {
            this.InitializeComponent();

            // Increase connection limit for faster url list generation
            System.Net.ServicePointManager.DefaultConnectionLimit = 100;
            this.Shown += new System.EventHandler(this.Form1_Shown);
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            this.LoadGUI();

            // FIXME
            // bind the data
            this.BeginInvoke((MethodInvoker)delegate
            {
                try
                {
                    // set databinings for the picturebox and the information label
                    bsSmallImage.DataSource = pictureList;
                    this.smallImage.DataBindings.Add("ImageLocation", bsSmallImage, "", false, DataSourceUpdateMode.OnPropertyChanged);
                    bsSmallImage.ListChanged += bsSmallImage_ListChanged;
                }
                catch (Exception)
                // two bindings to one source
                {
                    //continue;
                }

            });
        }

        private void AddBlog(object sender, EventArgs e)
        {
            string str = this.ExtractBlogname(this.tBlogUrl.Text);
            if (str != null)
            {
                if (blogs.Select(blog => blog.Name).ToList().Contains(str))
                {
                    MessageBox.Show("The entered URL is already in the library!", Application.ProductName);
                    this.tBlogUrl.Text = string.Empty;
                    return;
                }
                TumblrBlog newBlog = new TumblrBlog
                {
                    Name = str,
                    Url = this.ExtractUrl(this.tBlogUrl.Text),
                    DateAdded = DateTime.Now,
                    DownloadedImages = 0,
                    TotalCount = 0,
                    LastCrawled = System.DateTime.MinValue,
                    FinishedCrawl = false
                };
                this.SaveBlog(newBlog);
                blogs.Add(newBlog);
                if (blogs.Count == 1)
                    FormatDataSource();
                newBlog = null;
                this.tBlogUrl.Text = "http://";
                if (blogs.Any())
                {
                    this.toolShowExplorer.Enabled = true;
                    this.toolRemoveBlog.Enabled = true;
                    this.toolCrawl.Enabled = true;
                }
                else
                {
                    this.toolShowExplorer.Enabled = false;
                    this.toolRemoveBlog.Enabled = false;
                    this.toolCrawl.Enabled = false;
                }
            }
        }

        private void ClipboardMonitor_OnClipboardChange(ClipboardFormat format, object data)
        {
            if (format.ToString() == "Text")
            {
                string[] URLs = data.ToString().Split(new char[0]);
                foreach (string _url in URLs)
                {
                    string str = this.ExtractBlogname_paste(_url);
                    if (str != null)
                    {
                        if (blogs.Select(blog => blog.Name).ToList().Contains(str))
                        {
                            return;
                        }
                        {
                            TumblrBlog newBlog = new TumblrBlog
                            {
                                Name = str,
                                Url = this.ExtractUrl(_url),
                                DateAdded = DateTime.Now,
                                DownloadedImages = 0,
                                TotalCount = 0,
                                LastCrawled = System.DateTime.MinValue,
                                FinishedCrawl = false
                            };
                            this.SaveBlog(newBlog);
                            this.Invoke((Action)delegate
                            {
                                blogs.Add(newBlog);
                                if (blogs.Count == 1)
                                    FormatDataSource();
                            });
                            newBlog = null;


                            if (blogs.Any())
                            {
                                this.Invoke((Action)delegate
                                {
                                    this.toolShowExplorer.Enabled = true;
                                    this.toolRemoveBlog.Enabled = true;
                                    this.toolCrawl.Enabled = true;
                                });
                            }
                            else
                            {
                                this.Invoke((Action)delegate
                                {
                                    this.toolShowExplorer.Enabled = false;
                                    this.toolRemoveBlog.Enabled = false;
                                    this.toolCrawl.Enabled = false;
                                });
                            }
                        }
                    }
                }
            }
        }

        private bool CreateDataFolder(string blogname = null)
        {
            if (blogname == null)
            {
                return false;
            }
            string path = Properties.Settings.Default.configDownloadLocation.ToString();
            try
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                if (!Directory.Exists(path + "/Index"))
                {
                    Directory.CreateDirectory(path + "/Index");
                }
                if (!Directory.Exists(path + "/" + blogname))
                {
                    Directory.CreateDirectory(path + "/" + blogname);
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        private bool Download(TumblrBlog blog, string fileLocation, string url, string filename)
        {

            // This would work if the hostname would be the same, but when the file is hosted on a different mirror, we load the same file again
            //if (!blog.Links.Any(Post => Post.Filename.Contains(url.Substring(url.LastIndexOf("/") + 1))))
            System.Threading.Monitor.Enter(blog);
            if (!blog.Links.Any(Post => Post.Url.Contains(filename)))
            {
                System.Threading.Monitor.Exit(blog);
                try
                {
                    using (WebClient client = new WebClient())
                    {
                        client.DownloadFile(url, fileLocation);
                    }
                    return true;
                }
                catch (Exception)
                {
                }
                return false;
            }
            System.Threading.Monitor.Exit(blog);
            return false;
        }

        private string ExtractBlogname(string url)
        {
            if ((url == null) || (url.Length < 0x11))
            {
                MessageBox.Show("Incomplete URL entered!", Application.ProductName);
                return null;
            }
            if (!url.Contains(".tumblr.com"))
            {
                MessageBox.Show("No valid Tumblr URL entered!", Application.ProductName);
                return null;
            }
            string[] source = url.Split(new char[] { '.' });
            if ((source.Count<string>() >= 3) && source[0].StartsWith("http://", true, null))
            {
                return source[0].Replace("http://", string.Empty);
            }
            MessageBox.Show("Invalid URL entered!", Application.ProductName);
            return null;
        }

        private string ExtractBlogname_paste(string url)
        {
            if ((url == null) || (url.Length < 0x11))
            {
                return null;
            }
            if (!url.Contains(".tumblr.com"))
            {
                return null;
            }
            string[] source = url.Split(new char[] { '.' });
            if ((source.Count<string>() >= 3) && source[0].StartsWith("http://", true, null))
            {
                return source[0].Replace("http://", string.Empty);
            }
            return null;
        }

        private string ExtractUrl(string url)
        {
            return ("http://" + this.ExtractBlogname(url) + ".tumblr.com/");
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if ((this.tasks[0] != null) && tasks[0].Status == TaskStatus.Running)
            {
                try
                {
                    if (TumblrActiveList.Count != 0)
                    {
                        foreach (TumblrBlog tumblr in TumblrActiveList)
                        {
                            tumblr.Information = "";
                            this.SaveBlog(tumblr);
                        }
                    }
                    TumblrActiveList.Clear();
                    if (this.wait_handle != null)
                    {
                        this.wait_handle.Close();
                    }
                    if (cts != null)
                        cts.Cancel();
                }
                catch (ThreadAbortException exception)
                {
                    MessageBox.Show("Process stopped by the user. " + exception.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                for (int i = 0; i < Properties.Settings.Default.configSimultaneousDownloads; i++)
                    this.tasks[i] = null;
                this.wait_handle = null;
            }
            // Save Window Postion and Size
            if (WindowState == FormWindowState.Maximized)
            {
                Properties.Settings.Default.Location = RestoreBounds.Location;
                Properties.Settings.Default.Size = RestoreBounds.Size;
                Properties.Settings.Default.Maximised = true;
                Properties.Settings.Default.Minimised = false;
            }
            else if (WindowState == FormWindowState.Normal)
            {
                Properties.Settings.Default.Location = Location;
                Properties.Settings.Default.Size = Size;
                Properties.Settings.Default.Maximised = false;
                Properties.Settings.Default.Minimised = false;
            }
            else
            {
                Properties.Settings.Default.Location = RestoreBounds.Location;
                Properties.Settings.Default.Size = RestoreBounds.Size;
                Properties.Settings.Default.Maximised = false;
                Properties.Settings.Default.Minimised = true;
            }

            if (Properties.Settings.Default.CheckClipboard)
            {
                Properties.Settings.Default.CheckClipboard = true;
                // Terminate Clipboard Monitor 
                ClipboardMonitor.Stop();
            }
            else
            {
                Properties.Settings.Default.CheckClipboard = false;
            }

            // Save Settings
            Properties.Settings.Default.Save();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.Maximised)
            {
                WindowState = FormWindowState.Maximized;
                Location = Properties.Settings.Default.Location;
                Size = Properties.Settings.Default.Size;
            }
            else if (Properties.Settings.Default.Minimised)
            {
                WindowState = FormWindowState.Minimized;
                Location = Properties.Settings.Default.Location;
                Size = Properties.Settings.Default.Size;
            }
            else
            {
                Location = Properties.Settings.Default.Location;
                Size = Properties.Settings.Default.Size;
            }
        }

        private TumblrBlog LoadBlog(string blogname)
        {
            TumblrBlog blog = new TumblrBlog();
            try
            {
                using (Stream stream = new FileStream(Properties.Settings.Default.configDownloadLocation.ToString() + "Index/" + blogname + ".tumblr", FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    IFormatter formatter = new BinaryFormatter();
                    blog = (TumblrBlog)formatter.Deserialize(stream);
                }
            }
            catch (SerializationException SerializationException)
            {
                using (Stream stream = new FileStream(Properties.Settings.Default.configDownloadLocation.ToString() + "Index/" + blogname + ".tumblr", FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    IFormatter formatter = new BinaryFormatter();

                    //Set formatters binder property to TumblOne 1.0.4.0 / 0.0.0.0 Version
                    formatter.Binder = new TumblOne.Typeconvertor();
                    blog = (TumblrBlog)formatter.Deserialize(stream);
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
            return blog;
        }

        public async void LoadGUI()
        {
            await LoadLibrary();

            // Delay load of preferences which interfere with library loading
            loadPreferences();

            // delay online check to reduce startup time
            if (Properties.Settings.Default.configCheckStatusAtStartup)
            {
                foreach (TumblrBlog blog in blogs)
                {
                    Task.Run(() => { checkIfBlogIsOnline(blog); });
                }
            }
        }

        public Task LoadLibrary()
        {
            return Task.Run(() =>
                {
                    // Clean UI
                    this.BeginInvoke((MethodInvoker)delegate
                        {
                            blogs.Clear();
                            this.lblProcess.Text = "";
                        });
                    if (Directory.Exists(Properties.Settings.Default.configDownloadLocation.ToString() + "Index/"))
                    {
                        string[] files = Directory.GetFiles(Properties.Settings.Default.configDownloadLocation.ToString() + "Index/", "*.tumblr");
                        
                        // Setup the UI
                        if (files.Count<string>() > 0)
                        {
                            this.BeginInvoke((MethodInvoker)delegate
                            {
                                this.toolShowExplorer.Enabled = true;
                                this.toolRemoveBlog.Enabled = true;
                                this.toolCrawl.Enabled = true;
                            });
                        }
                        else
                        {
                            this.BeginInvoke((MethodInvoker)delegate
                            {
                            this.toolShowExplorer.Enabled = false;
                            this.toolRemoveBlog.Enabled = false;
                            this.toolCrawl.Enabled = false;
                            });
                        }

                        this.BeginInvoke((MethodInvoker)delegate
                        {
                            // load blogs
                            // format the Datagridview and bind the source (list of TumblrBlogs)
                            foreach (string str in files)
                            {
                                blogs.Add(this.LoadBlog(Path.GetFileNameWithoutExtension(str)));
                            }

                            // show UI
                            FormatDataSource();
                        });
                    }
                });
        }

        private void mnuExit_Click(object sender, EventArgs e)
        {
            base.Close();
        }

        private void mnuRescanBlog_Click(object sender, EventArgs e)
        {
            cts = new CancellationTokenSource();

            // Setup the UI
            this.crawlingBlogs = "";
            this.lblProcess.Text = "Crawling Blogs - " + this.crawlingBlogs;

            // Start Crawl processes
            for (int i = 0; i < Properties.Settings.Default.configSimultaneousDownloads; i++ )
                this.tasks[i] = Task.Run(() => runProducer(bin, cts.Token), cts.Token);
            this.wait_handle = new ManualResetEvent(true);

            // Enable/Disable Controls
            this.panelInfo.Visible = false;
            this.toolPause.Enabled = true;
            this.toolResume.Enabled = false;
            this.toolStop.Enabled = true;
            this.toolCrawl.Enabled = false;
            this.contextBlog.Items[3].Enabled = false;
        }

        private void mnuShowFilesInExplorer_Click(object sender, EventArgs e)
        {
            if (this.lvBlog.SelectedRows.Count > 0)
            {
                foreach (DataGridViewRow eachItem in this.lvBlog.SelectedRows)
                {
                    try
                    {
                        Process.Start("explorer.exe", Properties.Settings.Default.configDownloadLocation + ((TumblrBlog)eachItem.DataBoundItem).Name);
                    }
                    catch (Exception exception)
                    {
                        MessageBox.Show(exception.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    }
                }
            }
        }

        private bool FormatDataSource()
        {
            lvBlog.DataSource = null;
            lvBlog.Columns.Clear();

            // format the Datagridview and bind the source (list of TumblrBlogs)
            lvBlog.AutoGenerateColumns = false;

            var nameField = new DataGridViewTextBoxColumn();
            nameField.HeaderText = "Name";
            nameField.DataPropertyName = "Name";

            var downloadedImages = new DataGridViewTextBoxColumn();
            downloadedImages.HeaderText = "Downloaded Images";
            downloadedImages.DataPropertyName = "DownloadedImages";
            downloadedImages.DefaultCellStyle.Format = "#";

            var numberOfPosts = new DataGridViewTextBoxColumn();
            numberOfPosts.HeaderText = "Number of Images";
            numberOfPosts.DataPropertyName = "TotalCount";
            numberOfPosts.DefaultCellStyle.Format = "#";

            var urlField = new DataGridViewTextBoxColumn();
            urlField.HeaderText = "Url";
            urlField.DataPropertyName = "Url";

            var progress = new DataGridViewProgressColumn();
            progress.HeaderText = "Progress";
            progress.DataPropertyName = "Progress";
            progress.DefaultCellStyle.Format = "##";

            var status = new DataGridViewTextBoxColumn();
            status.HeaderText = "Status";
            status.DataPropertyName = "Online";

            var tags = new DataGridViewTextBoxColumn();
            tags.HeaderText = "Tags";
            tags.DataPropertyName = "Tags";

            var dateAdded = new DataGridViewTextBoxColumn();
            dateAdded.HeaderText = "Date Added";
            dateAdded.DataPropertyName = "dateAdded";
            dateAdded.DefaultCellStyle.Format = "dd/MM/yyyy hh:mm:ss";

            var lastCrawl = new DataGridViewTextBoxColumn();
            lastCrawl.HeaderText = "Last Complete Crawl";
            lastCrawl.DataPropertyName = "lastCrawled";

            var information = new DataGridViewTextBoxColumn();
            information.HeaderText = "Current Process";
            information.DataPropertyName = "Information";

            this.BeginInvoke((MethodInvoker)delegate
            {

                lvBlog.Columns.Add(nameField);
                lvBlog.Columns.Add(downloadedImages);
                lvBlog.Columns.Add(numberOfPosts);
                lvBlog.Columns.Add(urlField);
                lvBlog.Columns.Add(progress);
                lvBlog.Columns.Add(status);
                lvBlog.Columns.Add(tags);
                lvBlog.Columns.Add(dateAdded);
                lvBlog.Columns.Add(lastCrawl);
                lvBlog.Columns.Add(information);
                lvBlog.DataSource = blogs;

                lvBlog.ReadOnly = false;
                lvBlog.Columns[0].ReadOnly = true;
                lvBlog.Columns[1].ReadOnly = true;
                lvBlog.Columns[2].ReadOnly = true;
                lvBlog.Columns[3].ReadOnly = true;
                lvBlog.Columns[4].ReadOnly = true;
                lvBlog.Columns[5].ReadOnly = true;
                lvBlog.Columns[6].ReadOnly = false;
                lvBlog.Columns[7].ReadOnly = true;
                lvBlog.Columns[8].ReadOnly = true;
                lvBlog.Columns[9].ReadOnly = true;

                tags.ToolTipText = "Enter comma separated strings, e.g.: great big car, mouse";

                lvBlog.Columns[5].DefaultCellStyle.FormatProvider = new BoolFormatter();
                lvBlog.Columns[5].DefaultCellStyle.Format = "OnlineOffline";

                lvBlog.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.False;
                //lvBlog.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                //lvBlog.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                //lvBlog.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                //lvBlog.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                //lvBlog.Columns[4].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                //lvBlog.Columns[5].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                //lvBlog.Columns[6].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                //lvBlog.Columns[7].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

                lvBlog.Columns[0].SortMode = DataGridViewColumnSortMode.Automatic;
                lvBlog.Columns[1].SortMode = DataGridViewColumnSortMode.Automatic;
                lvBlog.Columns[2].SortMode = DataGridViewColumnSortMode.Automatic;
                lvBlog.Columns[3].SortMode = DataGridViewColumnSortMode.Automatic;
                lvBlog.Columns[4].SortMode = DataGridViewColumnSortMode.Automatic;
                lvBlog.Columns[5].SortMode = DataGridViewColumnSortMode.Automatic;
                lvBlog.Columns[6].SortMode = DataGridViewColumnSortMode.Automatic;
                lvBlog.Columns[7].SortMode = DataGridViewColumnSortMode.Automatic;
                lvBlog.Columns[8].SortMode = DataGridViewColumnSortMode.Automatic;
                lvBlog.Columns[9].SortMode = DataGridViewColumnSortMode.Automatic;

                lvBlog.CellBorderStyle = DataGridViewCellBorderStyle.None;
                lvBlog.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
                lvBlog.AutoResizeColumns();
                lvBlog.AllowUserToResizeColumns = true;
                lvBlog.AllowUserToOrderColumns = true;

                // reload saved column positions and sizes
                try
                {
                    lvBlog.SetOrder();
                }
                catch (Exception e)
                {
                    DataGridViewExtendedSetting.Default.Reset();
                    lvBlog.SetOrder();
                }
            });

            return true;
        }


        void lvBlog_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.CellStyle.FormatProvider is ICustomFormatter)
            {
                e.Value = (e.CellStyle.FormatProvider.GetFormat(typeof(ICustomFormatter)) as ICustomFormatter).Format(e.CellStyle.Format, e.Value, e.CellStyle.FormatProvider);
                e.FormattingApplied = true;
            }
        }

        private void mnuVisit_Click(object sender, EventArgs e)
        {
            if (this.lvBlog.SelectedRows.Count > 0)
            {
                foreach (DataGridViewRow eachItem in this.lvBlog.SelectedRows)
                {
                    try
                    {
                        Process.Start(((TumblrBlog)eachItem.DataBoundItem).Url);
                    }
                    catch (Exception exception)
                    {
                        MessageBox.Show(exception.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    }
                }
            }
        }

        private void RemoveBlog(object sender, EventArgs e)
        {
            if (this.lvBlog.SelectedRows.Count != 0)
            {
                if (Properties.Settings.Default.configDeleteIndexOnly)
                {
                    if (MessageBox.Show("Should the selected Blog really be deleted from Library (only removes Index Files, no downloaded images)?", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        foreach (DataGridViewRow eachItem in this.lvBlog.SelectedRows)
                        {
                            string indexPath = Properties.Settings.Default.configDownloadLocation.ToString() + "Index/" + ((TumblrBlog)eachItem.DataBoundItem).Name + ".tumblr";
                            try
                            {
                                if (System.IO.File.Exists(indexPath))
                                {
                                    System.IO.File.Delete(indexPath);
                                }
                                // Update UI
                                blogs.RemoveAt(eachItem.Index);
                            }
                            catch (Exception exception)
                            {
                                MessageBox.Show(exception.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Hand);
                            }
                        }
                    }
                }
                else
                {
                    if (MessageBox.Show("Should the selected Blog and all images really be deleted from Library?", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        foreach (DataGridViewRow eachItem in this.lvBlog.SelectedRows)
                        {
                            string indexPath = Properties.Settings.Default.configDownloadLocation.ToString() + "Index/" + ((TumblrBlog)eachItem.DataBoundItem).Name + ".tumblr";
                            string filePath = Properties.Settings.Default.configDownloadLocation.ToString() + ((TumblrBlog)eachItem.DataBoundItem).Name;
                            try
                            {
                                if (System.IO.File.Exists(indexPath))
                                {
                                    System.IO.File.Delete(indexPath);
                                }
                                Directory.Delete(filePath, true);
                                // Update UI
                                blogs.RemoveAt(eachItem.Index);
                            }
                            catch (Exception exception)
                            {
                                MessageBox.Show(exception.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Hand);
                            }
                        }
                    }
                }
            }
        }

        private void RunParser(TumblrBlog _blog, CancellationToken ct)
        {
            MethodInvoker method = null;
            bool readingDataBase = false;
            int numberOfPostsCrawled = 0;
            int numberOfPagesCrawled = 0;
            int totalPostCount = 0;
            List<string> crawledImageURLs = new List<string>();
            //string blogname = ExtractBlogname(ApiUrl.ToString());
            String ApiUrl = _blog.Url;

            // Set up and sanitize the url for blog status check
            if (ApiUrl.Last<char>() != '/')
            {
                ApiUrl = ApiUrl + "/api/read?start=";
            }
            else
            {
                ApiUrl = ApiUrl + "api/read?start=";
            }

            // make sure we can save files
            this.CreateDataFolder(_blog.Name);
            while (true)
            {
                this.wait_handle.WaitOne();
                if (ct.IsCancellationRequested)
                {
                    // Clean up here
                    ct.ThrowIfCancellationRequested();
                }

                if (numberOfPostsCrawled == 0)
                {

                    this.BeginInvoke((MethodInvoker)delegate
                    {
                        _blog.Information = "Checking blogs status ...";
                    });

                    // set title and name of the blog
                    // check if blogaddress is alive or someone else is using it now

                    if (!checkIfBlogIsOnline(_blog))
                    {
                        // nothing to crawl, cleanup and leave
                        cleaupParser(_blog);
                        return;
                    }
                    // Update image (blog post) count

                    XDocument document = null;
                    try
                    {
                        document = XDocument.Load(ApiUrl.ToString() + "0&num=50");
                    }
                    catch (Exception e)
                    {
                        _blog.TotalCount = 0;
                        break;
                    }

                    List<string> images = new List<string>();

                    // Get number of photos on blog, excluding duplicate posts
                    foreach (var post in from data in document.Descendants("post") where data.Attribute("type").Value == "photo" select data)
                    {
                        // If it's a photoset, add all urls
                        if (post.Descendants("photoset").Count() > 0)
                            foreach (var photo in from photoData in post.Descendants("photoset").Descendants("photo") select photoData)
                                images.Add(String.Concat(photo.Nodes()));
                        // If it's a single photo, add it to the list
                        else
                            images.Add(String.Concat(post.Nodes()));
                    }

                    // Remove duplicates
                    images = images.Distinct().ToList();

                }

                // Use the parallel crawl path as defined in the settings
                if (Properties.Settings.Default.configParallelCrawl) 
                {

                    // Generate URL list of Images
                    // the api shows 50 posts at max, determine the number of pages to crawl
                    List<string> Urllist = new List<string>();

                    int numberOfPagesToCrawl = ((totalPostCount / 50) + 1);
                    Parallel.For(
                        0,
                        numberOfPagesToCrawl,
                        new ParallelOptions { MaxDegreeOfParallelism = (Properties.Settings.Default.configParallelImageDownloads / Properties.Settings.Default.configSimultaneousDownloads) },
                        i =>
                    {
                        this.wait_handle.WaitOne();
                        if (ct.IsCancellationRequested)
                        {
                            // Clean up here
                            ct.ThrowIfCancellationRequested();
                        }
                        try
                        {
                            if (string.IsNullOrEmpty(_blog.Tags))
                            {
                                XDocument document2 = null;
                                try
                                {
                                    document2 = XDocument.Load(ApiUrl.ToString() + (i * 50).ToString() + "&num=50");
                                    Urllist = (from n in document2.Descendants("post")
                                               where

                                               // Identify Posts
                                               n.Elements("photo-url").Where(x => x.Attribute("max-width").Value == Properties.Settings.Default.configImageSize.ToString()).Any() &&
                                               !n.Elements("photo-url").Where(x => x.Value == "www.tumblr.com").Any() ||

                                               // Identify Photosets
                                               n.Elements("photoset").Where(photoset => photoset.Descendants("photo-url")
                                                   .Any(photourl => (string)photourl.Attribute("max-width").Value
                                                       == Properties.Settings.Default.configImageSize.ToString() &&
                                                       photourl.Value != "www.tumblr.com")).Any()
                                               from m in n.Descendants("photo-url")
                                               where m.Attribute("max-width").Value == Properties.Settings.Default.configImageSize.ToString()
                                               select (string)m).ToList();
                                }
                                catch (Exception e)
                                {
                                    //Console.WriteLine(e.Data);
                                }

                            }
                            else
                            {
                                List<string> tagsToCheckFor = _blog.Tags.Split(',').ToList();
                                XDocument document2 = null;
                                try
                                {
                                    document2 = XDocument.Load(ApiUrl.ToString() + (i * 50).ToString() + "&num=50");
                                    Urllist = (from n in document2.Descendants("post")

                                               // Identify Posts
                                               where n.Elements("photo-url").Where(x => x.Attribute("max-width").Value == Properties.Settings.Default.configImageSize.ToString()).Any() &&
                                               !n.Elements("photo-url").Where(x => x.Value == "www.tumblr.com").Any() &&
                                               n.Elements("tag").Where(x => tagsToCheckFor.Contains(x.Value)).Any() ||

                                               // Identify Photosets
                                               n.Elements("photoset").Where(photoset => photoset.Descendants("photo-url")
                                               .Any(photourl => (string)photourl.Attribute("max-width").Value
                                               == Properties.Settings.Default.configImageSize.ToString() &&
                                               photourl.Value != "www.tumblr.com")).Any() &&
                                               n.Elements("tag").Where(x => tagsToCheckFor.Contains(x.Value)).Any()

                                               from m in n.Descendants("photo-url")
                                               where m.Attribute("max-width").Value == Properties.Settings.Default.configImageSize.ToString()
                                               select (string)m).ToList();
                                }
                                catch (Exception e)
                                {
                                    //Console.WriteLine(e.Data);
                                }

                            }
                        }
                        catch (Exception)
                        {
                            XDocument document2 = null;
                            try
                            {
                                document2 = XDocument.Load(ApiUrl.ToString() + (i * 50).ToString() + "&num=50");

                                Urllist = (from n in document2.Descendants("post")
                                           where

                                           // Identify Posts
                                           n.Elements("photo-url").Where(x => x.Attribute("max-width").Value == Properties.Settings.Default.configImageSize.ToString()).Any() &&
                                           !n.Elements("photo-url").Where(x => x.Value == "www.tumblr.com").Any() ||

                                           // Identify Photosets
                                           n.Elements("photoset").Where(photoset => photoset.Descendants("photo-url")
                                               .Any(photourl => (string)photourl.Attribute("max-width").Value
                                                   == Properties.Settings.Default.configImageSize.ToString() &&
                                                   photourl.Value != "www.tumblr.com")).Any()
                                           from m in n.Descendants("photo-url")
                                           where m.Attribute("max-width").Value == Properties.Settings.Default.configImageSize.ToString()
                                           select (string)m).ToList();
                            }
                            catch (Exception e)
                            {
                                //Console.WriteLine(e.Data);
                            }

                        }
                        System.Threading.Monitor.Enter(crawledImageURLs);
                        crawledImageURLs.AddRange(Urllist);
                        System.Threading.Monitor.Exit(crawledImageURLs);
                        Urllist.Clear();
                        {
                            numberOfPostsCrawled += 50;
                            this.BeginInvoke((MethodInvoker)delegate
                            {
                                _blog.Information = "Evaluated " + numberOfPostsCrawled + " tumblr post urls out of " + totalPostCount + " total posts.";
                            });
                        }
                    });
                    // Start the crawl process
                    //if (numberOfPostsCrawled >= _blog.TotalCount)
                    {
                        // Generate unique url list of urls as we might have fetched duplicates at the end
                        crawledImageURLs = crawledImageURLs.Distinct().ToList();

                        // Save count of images for progress calculation
                        int totalImages = crawledImageURLs.Count;

                        this.BeginInvoke((MethodInvoker)delegate
                        {
                            _blog.Information = "Calculating new image urls ...";
                            _blog.TotalCount = totalImages;
                        });

                        // substract already crawled urls
                        // This would work if the hostname would be the same, but when the file is hosted on a different mirror, we load the same file again
                        // crawledImageURLs = crawledImageURLs.Except(_blog.Links.Select(Posts => Posts.Url).ToList()).ToList();

                        // 
                        //crawledImageURLs = crawledImageURLs.Where(url => _blog.Links.Any(Post => !url.Substring(url.LastIndexOf('/') + 1).Contains(Post.Url))).ToList();
                        crawledImageURLs = crawledImageURLs.Where(url => !_blog.Links.Any(Post => url.Contains(Post.Filename))).ToList();

                        try
                        {
                            // start the crawl
                            Parallel.ForEach(
                                crawledImageURLs,
                                new ParallelOptions { MaxDegreeOfParallelism = (Properties.Settings.Default.configParallelImageDownloads / Properties.Settings.Default.configSimultaneousDownloads) },
                                url =>
                            {
                                MethodInvoker invoker = null;
                                string FileLocation;
                                this.wait_handle.WaitOne();
                                if (ct.IsCancellationRequested)
                                {
                                    // Clean up here
                                    ct.ThrowIfCancellationRequested();
                                }
                                // create filename from url, just skip everything before the first slash (/)
                                string fileName = Path.GetFileName(new Uri(url).LocalPath);
                                // check if we should crawl .gifs
                                if (!Properties.Settings.Default.configChkGIFState || (Path.GetExtension(fileName).ToLower() != ".gif"))
                                {
                                    // create path to save the file in the proper folder
                                    FileLocation = Properties.Settings.Default.configDownloadLocation.ToString() + _blog.Name + "/" + fileName;
                                    try
                                    {
                                        // download file if the file is new
                                        if (this.Download(_blog, FileLocation, url, fileName))
                                        {

                                            if (invoker == null)
                                            {
                                                invoker = delegate
                                                {
                                                    // add file to collection to prevent re-downloads and for statistics
                                                    System.Threading.Monitor.Enter(_blog);
                                                    _blog.Links.Add(new Post(url, fileName));
                                                    System.Threading.Monitor.Exit(_blog);
                                                    _blog.Information = "Downloading " + url;
                                                    _blog.DownloadedImages = _blog.Links.Count;

                                                    pictureList.Add((Path.GetFullPath(FileLocation)));

                                                    // update progress
                                                    double progress = (double)_blog.DownloadedImages / (double)_blog.TotalCount * 100;
                                                    if ((int)_blog.Progress != progress)
                                                    {
                                                        _blog.Progress = (int)progress;
                                                    }
                                                };
                                            }
                                            this.BeginInvoke(invoker);
                                            readingDataBase = false;
                                        }
                                        else
                                        {
                                            if (!readingDataBase)
                                            {
                                                // FIXME
                                                // need to be rewritten as we overwrite the Post.Url value of the last entry since we've bound the label.
                                                readingDataBase = true;
                                                this.BeginInvoke((MethodInvoker)delegate
                                                {
                                                    _blog.Information = "Skipping previously downloaded images ...";
                                                });
                                            }

                                        }
                                    }
                                    catch (Exception)
                                    {
                                        //continue;
                                    }
                                }
                            });
                        }
                        catch (Exception)
                        {
                            continue;
                        }

                        this.BeginInvoke((MethodInvoker)delegate
                        {
                            _blog.Information = "Crawl finished - cleaning up";
                        });

                        // Finished crawling the blog
                        _blog.LastCrawled = DateTime.Now;
                        _blog.FinishedCrawl = true;

                        cleaupParser(_blog);

                        // remove index from listview after completed crawl, if property set
                        if (Properties.Settings.Default.configRemoveFinishedBlogs)
                        {
                            string path = Properties.Settings.Default.configDownloadLocation.ToString() + "Index/" + _blog.Name + ".tumblr";
                            if (System.IO.File.Exists(path))
                            {
                                System.IO.File.Delete(path);
                            }
                            this.BeginInvoke((MethodInvoker)delegate
                            {
                                blogs.Remove(_blog);
                            });
                        }

                        return;
                    }
                }

                // Use the serial crawl path as defined in the settings
                else if (!Properties.Settings.Default.configParallelCrawl)
                {
                    _blog.TotalCount = totalPostCount;
                    IEnumerable<XElement> query;
                    XDocument document3 = null;
                    try
                    {
                        document3 = XDocument.Load(ApiUrl.ToString() + numberOfPostsCrawled.ToString() + "&num=50");
                    }
                    catch (Exception e)
                    {
                        //Console.WriteLine(e.Data);
                    }
                    try
                    {
                        if (string.IsNullOrEmpty(_blog.Tags))
                        {
                            query = (from n in document3.Descendants("post")
                                     where
                                         // Identify Posts
                                        n.Elements("photo-url").Where(x => x.Attribute("max-width").Value == Properties.Settings.Default.configImageSize.ToString()).Any() &&
                                        !n.Elements("photo-url").Where(x => x.Value == "www.tumblr.com").Any() ||

                                        // Identify Photosets
                                        n.Elements("photoset").Where(photoset => photoset.Descendants("photo-url")
                                        .Any(photourl => (string)photourl.Attribute("max-width").Value
                                        == Properties.Settings.Default.configImageSize.ToString() &&
                                        photourl.Value != "www.tumblr.com")).Any()
                                     from m in n.Descendants("photo-url")
                                     where m.Attribute("max-width").Value == Properties.Settings.Default.configImageSize.ToString()
                                     select m);

                        }
                        else
                        {
                            List<string> tagsToCheckFor = _blog.Tags.Split(',').ToList();
                            query = (from n in document3.Descendants("post")

                                     // Identify Posts
                                     where n.Elements("photo-url").Where(x => x.Attribute("max-width").Value == Properties.Settings.Default.configImageSize.ToString()).Any() &&
                                         !n.Elements("photo-url").Where(x => x.Value == "www.tumblr.com").Any() &&
                                         n.Elements("tag").Where(x => tagsToCheckFor.Contains(x.Value)).Any() ||

                                         // Identify Photosets
                                         n.Elements("photoset").Where(photoset => photoset.Descendants("photo-url")
                                         .Any(photourl => (string)photourl.Attribute("max-width").Value
                                         == Properties.Settings.Default.configImageSize.ToString() &&
                                         photourl.Value != "www.tumblr.com")).Any() &&
                                         n.Elements("tag").Where(x => tagsToCheckFor.Contains(x.Value)).Any()

                                     from m in n.Descendants("photo-url")
                                     where m.Attribute("max-width").Value == Properties.Settings.Default.configImageSize.ToString()
                                     select m);
                        }
                    }
                    catch (Exception)
                    {
                        query = (from n in document3.Descendants("post")
                                 where
                                     // Identify Posts
                                     n.Elements("photo-url").Where(x => x.Attribute("max-width").Value == Properties.Settings.Default.configImageSize.ToString()).Any() &&
                                    !n.Elements("photo-url").Where(x => x.Value == "www.tumblr.com").Any() ||

                                    // Identify Photosets
                                    n.Elements("photoset").Where(photoset => photoset.Descendants("photo-url")
                                    .Any(photourl => (string)photourl.Attribute("max-width").Value
                                    == Properties.Settings.Default.configImageSize.ToString() &&
                                    photourl.Value != "www.tumblr.com")).Any()
                                 from m in n.Descendants("photo-url")
                                 where m.Attribute("max-width").Value == Properties.Settings.Default.configImageSize.ToString()
                                 select m);
                    }
                    // Start the crawl process
                    using (IEnumerator<XElement> enumerator = query.GetEnumerator())
                    {
                        numberOfPagesCrawled++;
                        numberOfPostsCrawled = 50 * numberOfPagesCrawled;

                        // start the crawl
                        while (enumerator.MoveNext())
                        {
                            MethodInvoker invoker = null;
                            XElement p = enumerator.Current;
                            string FileLocation;
                            this.wait_handle.WaitOne();
                            if (ct.IsCancellationRequested)
                            {
                                // Clean up here
                                ct.ThrowIfCancellationRequested();
                            }
                            // create filename from url, just skip everything before the first slash (/)
                            string fileName = Path.GetFileName(new Uri(p.Value).LocalPath);
                            // check if we should crawl .gifs
                            if (!Properties.Settings.Default.configChkGIFState || (Path.GetExtension(fileName).ToLower() != ".gif"))
                            {
                                // create path to save the file in the proper folder
                                FileLocation = Properties.Settings.Default.configDownloadLocation.ToString() + _blog.Name + "/" + fileName;
                                try
                                {
                                    // download file if the file is new
                                    if (this.Download(_blog, FileLocation, p.Value, fileName))
                                    {

                                        if (invoker == null)
                                        {
                                            invoker = delegate
                                            {
                                                // add file to collection to prevent re-downloads and for statistics
                                                _blog.Links.Add(new Post(p.Value, fileName));
                                                _blog.Information = "Downloading " + p.Value;
                                                _blog.DownloadedImages = _blog.Links.Count;

                                                pictureList.Add((Path.GetFullPath(FileLocation)));

                                                // update progress
                                                double progress = (double)_blog.DownloadedImages / (double)_blog.TotalCount * 100;
                                                if ((int)_blog.Progress != progress)
                                                {
                                                    _blog.Progress = (int)progress;
                                                }
                                            };
                                        }
                                        this.BeginInvoke(invoker);
                                        readingDataBase = false;
                                    }
                                    else
                                    {
                                        if (!readingDataBase)
                                        {
                                            // FIXME
                                            // need to be rewritten as we overwrite the Post.Url value of the last entry since we've bound the label.
                                            readingDataBase = true;
                                            this.BeginInvoke((MethodInvoker)delegate
                                            {
                                                _blog.Information = "Skipping previously downloaded images ...";
                                            });
                                        }
                                    }
                                }
                                catch (Exception)
                                {
                                    continue;
                                }
                            }
                        }
                    }

                    if (numberOfPostsCrawled >= totalPostCount)
                    {

                        this.BeginInvoke((MethodInvoker)delegate
                        {
                            _blog.Information = "Crawl finished - cleaning up";
                        });

                        // Finished crawling the blog
                        _blog.LastCrawled = DateTime.Now;
                        _blog.FinishedCrawl = true;

                        cleaupParser(_blog);

                        // remove index from listview after completed crawl, if property set
                        if (Properties.Settings.Default.configRemoveFinishedBlogs)
                        {
                            string path = Properties.Settings.Default.configDownloadLocation.ToString() + "Index/" + _blog.Name + ".tumblr";
                            if (System.IO.File.Exists(path))
                            {
                                System.IO.File.Delete(path);
                            }
                            this.BeginInvoke((MethodInvoker)delegate
                            {
                                blogs.Remove(_blog);
                            });
                        }
                        return;
                    }
                }

            }
            cleaupParser(_blog);
            return;
        }

        void cleaupParser(TumblrBlog _blog)
        {
            this.SaveBlog(_blog);
            TumblrActiveList.Remove(_blog);
            // Update UI
            this.BeginInvoke((MethodInvoker)delegate
            {

                // Update current crawling progress label
                int indexBlogInProgress = crawlingBlogs.IndexOf(_blog.Name);
                int lengthBlogInProgress = _blog.Name.Length;
                this.crawlingBlogs = crawlingBlogs.Remove(indexBlogInProgress, (lengthBlogInProgress + 1));
                this.lblProcess.Text = "Crawling Blogs - " + this.crawlingBlogs;
                _blog.Information = "";
            });

            if (TumblrActiveList.Count == 0)
            {
                this.BeginInvoke((MethodInvoker)delegate
                {
                    foreach (DataGridViewRow item in this.lvBlog.Rows)
                    {

                        // Update current crawling progress label
                        this.crawlingBlogs = "";
                        this.lblProcess.Text = "Queue finished";
                    }
                });
            }
        }

        void bsSmallImage_ListChanged(object sender, ListChangedEventArgs e)
        {
            bsSmallImage.MoveNext();
        }

        private bool SaveBlog(TumblrBlog newBlog)
        {
            if (newBlog == null)
            {
                return false;
            }
            this.CreateDataFolder(newBlog.Name);
            try
            {
                using (Stream stream = new FileStream(Properties.Settings.Default.configDownloadLocation.ToString() + "Index/" + newBlog.Name + ".tumblr", FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    IFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(stream, newBlog);
                }
            }
            catch (Exception)
            {
                MessageBox.Show("The blog index file cannot be saved to disk!\nBe sure, that you have enough free space and proper file permissions.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return false;
            }
            return true;
        }

        private void toolAbout_Click(object sender, EventArgs e)
        {
            using (SplashScreen screen = new SplashScreen())
            {
                screen.ShowDialog();
            }
        }

        private void toolPause_Click(object sender, EventArgs e)
        {
            this.wait_handle.Reset();

            this.lblProcess.Text = "PAUSE! Click on Resume Button to continue...";

            // Enable/Disable Controls
            this.toolPause.Enabled = false;
            this.toolResume.Enabled = true;
            this.toolStop.Enabled = true;

            if (pictureList.Count > 1)
                pictureList.Clear();
        }

        private void toolResume_Click(object sender, EventArgs e)
        {
            this.wait_handle.Set();

            this.lblProcess.Text = "Crawling Blogs - " + this.crawlingBlogs;

            // Enable/Disable Controls
            this.toolPause.Enabled = true;
            this.toolResume.Enabled = false;
            this.toolStop.Enabled = true;
        }

        private void toolStop_Click(object sender, EventArgs e)
        {

            this.wait_handle.Reset();
            this.panelInfo.Visible = true;
            this.lblProcess.Text = "Crawling of " + this.crawlingBlogs + "has stopped!";
            this.crawlingBlogs = "";

            // Enable/Disable Controls
            this.toolPause.Enabled = false;
            this.toolResume.Enabled = false;
            this.toolStop.Enabled = false;
            this.toolCrawl.Enabled = true;
            this.contextBlog.Items[3].Enabled = false;

            foreach (TumblrBlog blog in blogs)
            {
                blog.Information = "";
            }

            if (TumblrActiveList.Count != 0)
            {
                foreach (TumblrBlog tumblr in TumblrActiveList)
                {
                    this.SaveBlog(tumblr);
                }
                this.TumblrActiveList.Clear();
                if (bin.Count > 0)
                    {
                        bin.Clear();
                    }
                this.lvQueue.Items.Clear();
                pictureList.Clear();
            }
            try
            {
                if (cts != null)
                    cts.Cancel();
            }
            catch (ThreadAbortException)
            {
            }
        }

        private void optionsSaved()
        {

        }

        // Load program preferences

        private void loadPreferences()
        {
            // Setup Clipboard Monitor Listener
            if (Properties.Settings.Default.CheckClipboard)
            {
                this.toolCheckClipboard.Checked = true;
                ClipboardMonitor.Start();
                ClipboardMonitor.OnClipboardChange += ClipboardMonitor_OnClipboardChange;
            }
            else
            {
                this.toolCheckClipboard.Checked = false;
            }
        }

        private void toolSettings_Click(object sender, EventArgs e)
        {
            Settings settingsWindow = new Settings(this);
            settingsWindow.Show();
            optionsSaved();
        }

        private void toolAddQueue_Click(object sender, EventArgs e)
        {
            AddBlogtoQueue(bin, cts.Token);
        }

        private bool checkIfBlogIsOnline(TumblrBlog _blog)
        {
            String ApiUrl = _blog.Url;

            // sanitize URL
            if (ApiUrl.Last<char>() != '/')
            {
                ApiUrl = ApiUrl + "/api/read?start=";
            }
            else
            {
                ApiUrl = ApiUrl + "api/read?start=";
            }

			// set title and name of the blog
			// check if blogaddress is alive or someone else is using it now

			// first time check -- new blog
			if (_blog.Description == null)
            {
                XmlDocument tumblelog = new XmlDocument();

                try
                {
                    tumblelog.Load(ApiUrl.ToString() + "0" + "&num=50");
					XmlNode tumblblog = tumblelog.DocumentElement.SelectSingleNode("tumblelog");
					try
					{
						_blog.Description = tumblblog.Attributes["title"].InnerText;
						_blog.Text = tumblblog.InnerText;
						_blog.Online = true;
					}
					catch (NullReferenceException)
					{
						// If there is no description, the blog can still be alive
						_blog.Online = true;
					}
                }
                catch (WebException)
                {
					// blog dead
                    _blog.Online = false;
                    return false;
                }
            }
			// we already have data to compare (description, text) since the blog was
			// already once crawled
			else if (_blog.Description != null)
            {
                XmlDocument tumblelog = new XmlDocument();

                try
                {
                    tumblelog.Load(ApiUrl.ToString() + "0" + "&num=50");
					XmlNode tumblblog = tumblelog.DocumentElement.SelectSingleNode("tumblelog");
					try
					{
						if (!_blog.Description.Equals(tumblblog.Attributes["title"].InnerText))
						{
							_blog.Online = false;
                            return false;
						}
						if (!_blog.Text.Equals(tumblblog.InnerText))
						{
							_blog.Online = false;
                            return false;
						}
						else
						{
							_blog.Online = true;
						}
					}
					catch (NullReferenceException)
					{
						_blog.Online = true;
					}
                }
                catch (WebException)
                {
                    _blog.Online = false;
                    return false;
                }
            }
            return true;
        }

        private void AddBlogtoQueue(List<TumblrBlog> bin, CancellationToken ct) 
        {
            // Cancellation causes OCE. 
            try
            {
                if (this.lvBlog.SelectedRows.Count > 0)
                {
                    foreach (DataGridViewRow eachItem in lvBlog.SelectedRows)
                    {
                        bin.Add((TumblrBlog)eachItem.DataBoundItem);
                        this.addToQueueUI((TumblrBlog)eachItem.DataBoundItem);
                    }
                }
            }
            catch (OperationCanceledException exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        private void runProducer(List<TumblrBlog> bin, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();
            try
            {
                while (true) {
                    this.wait_handle.WaitOne();
                    if (ct.IsCancellationRequested)
                    {
                        // Clean up here, then...
                        ct.ThrowIfCancellationRequested();
                    }
                    try
                    {
                        System.Threading.Monitor.Enter(bin);
                        if (bin.Any())
                        {
                            TumblrBlog nextBlog;

                            nextBlog = bin.First<TumblrBlog>();
                            bin.RemoveAt(0);
                            TumblrActiveList.Add(nextBlog);

                            this.BeginInvoke((MethodInvoker)delegate
                            {
                                // Update UI:
                                // Processlabel
                                this.crawlingBlogs += this.lvQueue.Items[0].Text + " ";
                                this.lblProcess.Text = "Crawling Blogs - " + this.crawlingBlogs;
                                // Queue
                                lvQueue.Items.RemoveAt(0);
                            });
                            System.Threading.Monitor.Exit(bin);
                            this.RunParser(nextBlog, ct);
                        }
                        else
                        {
                            System.Threading.Monitor.Exit(bin);
                            Thread.Sleep(4000);
                        }

                    }
                    catch (Exception)
                    {
                        Thread.Sleep(4000);
                    }
                }
                    
            }

            catch (OperationCanceledException)
            {
                Console.WriteLine("Taking canceled.");
            }
        }

        private void RemoveBlogFromQueue(List<TumblrBlog> bin, CancellationToken ct)
        {
            // Cancellation causes OCE. 
            try
            {
                if (bin.Count != 0)
                {
                    foreach (ListViewItem eachItem in lvQueue.SelectedItems)
                    {
                        bin.RemoveAt(eachItem.Index);
                        lvQueue.Items.Remove(eachItem);
                    }
                }
            }
            catch (OperationCanceledException exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        private void toolRemoveQueue_Click(object sender, EventArgs e)
        {
            RemoveBlogFromQueue(bin, cts.Token);
        }

        private void addToQueueUI(TumblrBlog _blog)
        {
            //Update UI
            ListViewItem lvQueueItem = new ListViewItem();
            lvQueueItem.Text = _blog.Name;
            lvQueueItem.SubItems.Add("queued");
            this.lvQueue.Items.Add(lvQueueItem);
        }

        private void toolCheckClipboard_Click(object sender, EventArgs e)
        {
            if (this.toolCheckClipboard.Checked == true)
            {
                Properties.Settings.Default.CheckClipboard = true;
                ClipboardMonitor.Start();
                ClipboardMonitor.OnClipboardChange += ClipboardMonitor_OnClipboardChange;
            }
            else
            {
                Properties.Settings.Default.CheckClipboard = false;
                ClipboardMonitor.Stop();
            }
        }

        private void smallImage_Click(object sender, EventArgs e)
        {
            PicturePreviewFullscreen pictureFullScreen = new PicturePreviewFullscreen(this);
            pictureFullScreen.ShowPreviewImage = smallImage.Image;
            pictureFullScreen.Show();
        }
    }
}

