namespace TumblOne
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    //using System.ComponentModel;
    using System.Diagnostics;
    //using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Net;
    //using System.Runtime.InteropServices;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Threading;
    using System.Threading.Tasks;
    //using System.Threading.Tasks.Schedulers;
    using System.Windows.Forms;
    using System.Xml;
    using System.Xml.Linq;

    public partial class Form1 : Form
    {
        public List<TumblrBlog> TumblrActiveList = new List<TumblrBlog>();
        public Task[] tasks = new Task[Properties.Settings.Default.configSimultaneousDownloads];
        string crawlingBlogs = "";
        BindingSource bsSmallImage = new BindingSource();
        BindingSource bslblUrl = new BindingSource();
        private CancellationTokenSource cts = new CancellationTokenSource();
        private List<TumblrBlog> bin = new List<TumblrBlog>();
        private TumblOne.SortableBindingList<TumblrBlog> blogs = new TumblOne.SortableBindingList<TumblrBlog>();

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
                    Url = this.ExtractUrl(str),
                    DateAdded = DateTime.Now,
                    DownloadedImages = 0,
                    TotalCount = 0,
                    LastCrawled = System.DateTime.MinValue,
                    FinishedCrawl = false
                };
                this.SaveBlog(newBlog);
                blogs.Add(newBlog);
                if (blogs.Count == 1)
                    lvBlog.DataSource = blogs;
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
            if (!blog.Links.Any(Post => Post.Url.Contains(url)))
            {
                try
                {
                    using (WebClient client = new WebClient())
                    {
                        client.DownloadFile(url, fileLocation);
                    }
                }
                catch (Exception)
                {
                    return true;
                }
                return true;
            }
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

                    //Set formatters binder property to TumblOne 1.0.4.0 Version
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
                            this.lblUrl.Text = "";
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
            this.smallImage.ImageLocation = "";
            this.crawlingBlogs = "";
            this.lblUrl.Text = "";
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
            numberOfPosts.HeaderText = "Number of Posts";
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

            var dateAdded = new DataGridViewTextBoxColumn();
            dateAdded.HeaderText = "Date Added";
            dateAdded.DataPropertyName = "dateAdded";
            dateAdded.DefaultCellStyle.Format = "dd/MM/yyyy hh:mm:ss";

            var lastCrawl = new DataGridViewTextBoxColumn();
            lastCrawl.HeaderText = "Last Complete Crawl";
            lastCrawl.DataPropertyName = "lastCrawled";

            this.BeginInvoke((MethodInvoker)delegate
            {

                lvBlog.Columns.Add(nameField);
                lvBlog.Columns.Add(downloadedImages);
                lvBlog.Columns.Add(numberOfPosts);
                lvBlog.Columns.Add(urlField);
                lvBlog.Columns.Add(progress);
                lvBlog.Columns.Add(status);
                lvBlog.Columns.Add(dateAdded);
                lvBlog.Columns.Add(lastCrawl);
                lvBlog.DataSource = blogs;


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

                lvBlog.CellBorderStyle = DataGridViewCellBorderStyle.None;
                lvBlog.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
                lvBlog.AutoResizeColumns();
                lvBlog.AllowUserToResizeColumns = true;
                lvBlog.AllowUserToOrderColumns = true;

                // reload saved column positions and sizes
                lvBlog.SetOrder();
            });

            return true;
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
            bool checkedIfBlogIsAlive = false;
            int numberOfPostsCrawled = 0;
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
                    // set title and name of the blog
                    // check if blogaddress is alive or someone else is using it now

                    if (!checkIfBlogIsOnline(_blog))
                    {
                        // nothing to crawl, cleanup and leave
                        cleaupParser(_blog);
                        return;
                    }
                    // Update image (blog post) count
                    // Set progressbar
                    try
                    {
                        XDocument document = null;
                        try
                        {
                            document = XDocument.Load(ApiUrl.ToString() + "0&num=50");
                        }
                        catch (Exception e)
                        {
                            //Console.WriteLine(e.Data);
                        }
                        foreach (var type in from data in document.Descendants("posts") select new { Total = data.Attribute("total").Value })
                        {
                            _blog.TotalCount = Convert.ToInt32(type.Total.ToString());
                        }
                        if (method == null)
                        {
                            if (TumblrActiveList.Count > 1)
                            {
                                method = delegate
                                {
                                    try
                                    {
                                        this.pgBar.Style = ProgressBarStyle.Marquee;
                                        this.pgBar.Minimum = 0;
                                        this.pgBar.Maximum = _blog.TotalCount;
                                        this.pgBar.Value = _blog.DownloadedImages;
                                    }
                                    catch (ArgumentOutOfRangeException)
                                    {
                                        this.pgBar.Style = ProgressBarStyle.Marquee;
                                    }

                                };
                            }
                            else
                            {
                                method = delegate
                                {
                                    try
                                    {
                                        this.pgBar.Style = ProgressBarStyle.Continuous;
                                        this.pgBar.Minimum = 0;
                                        this.pgBar.Maximum = _blog.TotalCount;
                                        this.pgBar.Value = _blog.DownloadedImages;
                                    }
                                    catch (ArgumentOutOfRangeException)
                                    {
                                        this.pgBar.Style = ProgressBarStyle.Marquee;
                                    }

                                };
                            }
                        }
                        this.BeginInvoke(method);
                    }
                    catch
                    {
                        _blog.TotalCount = 0;
                        //this.toolStop_Click(this, null);
                        break;
                    }
                }

                // Generate URL list of Images
                // the api shows 50 posts at max, determine the number of pages to crawl
                int numberOfPagesToCrawl = ((_blog.TotalCount / 50) + 1);
                Parallel.For(0, numberOfPagesToCrawl, i =>
                    {
                        this.wait_handle.WaitOne();
                        if (ct.IsCancellationRequested)
                        {
                            // Clean up here
                            ct.ThrowIfCancellationRequested();
                        }
                        XDocument document2 = null;
                        try
                        {
                            document2 = XDocument.Load(ApiUrl.ToString() + (i * 50).ToString() + "&num=50");
                        }
                        catch (Exception e)
                        {
                            //Console.WriteLine(e.Data);
                        }

                        try
                        {
                            List<string> Urllist = (from n in document2.Descendants("post")
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
                            System.Threading.Monitor.Enter(crawledImageURLs);
                            crawledImageURLs.AddRange(Urllist);
                            System.Threading.Monitor.Exit(crawledImageURLs);
                            Urllist.Clear();
                        }
                        catch (Exception e)
                        {
                            //Console.WriteLine(e.Data);
                        }

                        {
                            //numberOfPagesCrawled = i;
                            numberOfPostsCrawled += 50;
                            this.BeginInvoke((MethodInvoker)delegate
                            {
                                this.lblUrl.Text = "Evaluated " + numberOfPostsCrawled + " Image URLs out of " + _blog.TotalCount + " Posts.";
                            });
                        }
                    });

                // Start the crawl process
                //if (numberOfPostsCrawled >= _blog.TotalCount)
                {
                    // Generate unique and new list of urls, subtract already crawled ones
                    crawledImageURLs = crawledImageURLs.Distinct().ToList();
                    //crawledImageURLs = _blog.Links.Select(Posts => Posts.url).ToList().Intersect(crawledImageURLs).ToList();
                    crawledImageURLs = crawledImageURLs.Except(_blog.Links.Select(Posts => Posts.Url).ToList()).ToList();

                    this.BeginInvoke((MethodInvoker)delegate
                    {
                        try
                        {
                            // set databinings for the picturebox and the information label
                            bsSmallImage.DataSource = _blog.Links;
                            this.smallImage.DataBindings.Add("ImageLocation", bsSmallImage, "Filename", false, DataSourceUpdateMode.OnPropertyChanged);
                            bsSmallImage.ListChanged += bsSmallImage_ListChanged;

                            bslblUrl.DataSource = _blog.Links;
                            this.lblUrl.DataBindings.Add("Text", bslblUrl, "Url", false, DataSourceUpdateMode.OnPropertyChanged);
                            bslblUrl.ListChanged += bslblUrl_ListChanged;
                        }
                        catch (Exception)
                            // two bindings to one source
                        {
                            //continue;
                        }

                    });

                    try
                    {
                        // start the crawl
                        Parallel.ForEach(crawledImageURLs, url =>
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
                                                _blog.Links.Add(new Post(url, FileLocation));
                                                _blog.DownloadedImages = _blog.Links.Count;

                                                // update progress
                                                double progress = (double)_blog.DownloadedImages / (double)_blog.TotalCount * 100;
                                                _blog.Progress = (int) progress;

                                                if ((this.pgBar.Value + 1) < (this.pgBar.Maximum + 1))
                                                {
                                                    this.pgBar.Value++;
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
                                            readingDataBase = true;
                                            this.BeginInvoke((MethodInvoker)delegate
                                            {
                                                this.lblUrl.Text = "Skipping previously downloaded images - " + _blog.Name;
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

            });

            // update UI
            if (TumblrActiveList.Count == 1)
            {
                this.BeginInvoke((MethodInvoker)delegate
                {
                    this.pgBar.Style = ProgressBarStyle.Continuous;
                });
            }

            if (TumblrActiveList.Count == 0)
            {
                this.BeginInvoke((MethodInvoker)delegate
                {
                    foreach (DataGridViewRow item in this.lvBlog.Rows)
                    {

                        // Update current crawling progress label
                        this.smallImage.ImageLocation = "";
                        this.crawlingBlogs = "";
                        this.lblUrl.Text = "";
                        this.lblProcess.Text = "Queue finished";
                    }
                });
            }
        }

        void bsSmallImage_ListChanged(object sender, ListChangedEventArgs e)
        {
            bsSmallImage.MoveNext();
        }

        void bslblUrl_ListChanged(object sender, ListChangedEventArgs e)
        {
            bslblUrl.MoveNext();
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
            this.lblUrl.Text = "";
            this.smallImage.ImageLocation = "";
            this.crawlingBlogs = "";

            // Enable/Disable Controls
            this.toolPause.Enabled = false;
            this.toolResume.Enabled = false;
            this.toolStop.Enabled = false;
            this.toolCrawl.Enabled = true;
            this.contextBlog.Items[3].Enabled = false;

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

        private void chkGIF_CheckedChanged(object sender, EventArgs e)
        {
            if (this.chkGIF.Checked)
            {
                Properties.Settings.Default.configChkGIFState = true;
            }
            else
            {
                Properties.Settings.Default.configChkGIFState = false;
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
    }
}

