namespace TumblOne
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
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
    using System.Xml.Linq;

    public partial class Form1 : Form
    {
        public List<TumblrBlog> TumblrActiveList = new List<TumblrBlog>();
        public Task[] tasks = new Task[Properties.Settings.Default.configSimultaneousDownloads];
        string crawlingBlogs = "";
        //public TumblrBlog blog;
        //private Task worker;
        //private BlockingCollection<TumblrBlog> bin = new BlockingCollection<TumblrBlog>();
        //private QueuedTaskScheduler qts = new QueuedTaskScheduler(TaskScheduler.Default, 2); 
        private CancellationTokenSource cts = new CancellationTokenSource();
        private List<TumblrBlog> bin = new List<TumblrBlog>();


        public Form1()
        {
            this.InitializeComponent();
            this.LoadLibrary();
        }

        private void AddBlog(object sender, EventArgs e)
        {
            ListViewItem lvItem;
            string str = this.ExtractBlogname(this.tBlogUrl.Text);
            if (str != null)
            {
                foreach (ListViewItem item in this.lvBlog.Items)
                {
                    if (item.Text.Equals(str))
                    {
                        MessageBox.Show("The entered URL is already in the library!", Application.ProductName);
                        this.tBlogUrl.Text = string.Empty;
                        return;
                    }
                }
                lvItem = new ListViewItem();
                if ((this.tasks[0] != null) && tasks[0].Status == TaskStatus.Running )
                {
                    TumblrBlog ThreadBlog = new TumblrBlog();
                    this.Invoke((Action)delegate
                    {
                        ThreadBlog._URL = this.ExtractUrl(this.tBlogUrl.Text);
                        ThreadBlog._TotalCount = 0;
                        ThreadBlog._DownloadedImages = 0;
                        ThreadBlog._Name = this.ExtractBlogname(this.tBlogUrl.Text);
                        ThreadBlog._DateAdded = DateTime.Now;
                        ThreadBlog._LastCrawled = System.DateTime.MinValue;
                        ThreadBlog._finishedCrawl = false;
                        lvItem.Text = ThreadBlog._Name;
                        lvItem.SubItems.Add("");
                        lvItem.SubItems.Add("");
                        lvItem.SubItems.Add(this.tBlogUrl.Text);
                        lvItem.SubItems.Add(ThreadBlog._DateAdded.ToString("G"));
                        lvItem.SubItems.Add(ThreadBlog._LastCrawled.ToString("G"));
                        lvItem.SubItems.Add(ThreadBlog._finishedCrawl.ToString());
                        this.lvBlog.Items.Add(lvItem);
                    });
                    this.SaveBlog(ThreadBlog);
                    ThreadBlog = null;
                }
                else
                {
                    TumblrBlog newBlog = new TumblrBlog
                    {
                        _Name = str,
                        _URL = this.ExtractUrl(this.tBlogUrl.Text),
                        _DateAdded = DateTime.Now,
                        _DownloadedImages = 0,
                        _TotalCount = 0,
                        _LastCrawled = System.DateTime.MinValue,
                        _finishedCrawl = false
                    };
                    lvItem.Text = newBlog._Name;
                    lvItem.SubItems.Add("");
                    lvItem.SubItems.Add("");
                    lvItem.SubItems.Add(newBlog._URL);
                    lvItem.SubItems.Add(newBlog._DateAdded.ToString("G"));
                    lvItem.SubItems.Add(newBlog._LastCrawled.ToString("G"));
                    lvItem.SubItems.Add(newBlog._finishedCrawl.ToString());
                    this.lvBlog.Items.Add(lvItem);
                    this.SaveBlog(newBlog);
                    newBlog = null;
                }
                this.tBlogUrl.Text = "http://";
                if (Directory.Exists(Properties.Settings.Default.configDownloadLocation.ToString() + "Index/"))
                {
                    if (Directory.GetFiles(Properties.Settings.Default.configDownloadLocation.ToString() + "Index/", "*.tumblr").Count<string>() > 0)
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
        }

        private bool CreateDataFolder(string blogname = null)
        {
            if (blogname == null)
            {
                return false;
            }
            string path = "./Blogs";
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
            if (!blog.Links.Contains(new Post(url, filename)))
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

            // Save Settings
            Properties.Settings.Default.Save();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            using (SplashScreen screen = new SplashScreen())
            {
                screen.ShowDialog();
            }
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
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
            return blog;
        }

        public void LoadLibrary()
        {
            this.lvBlog.Items.Clear();
            this.lblProcess.Text = "";
            this.lblUrl.Text = "";
            if (Directory.Exists(Properties.Settings.Default.configDownloadLocation.ToString() + "Index/"))
            {
                string[] files = Directory.GetFiles(Properties.Settings.Default.configDownloadLocation.ToString() + "Index/", "*.tumblr");
                if (files.Count<string>() > 0)
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
                foreach (string str in files)
                {
                    TumblrBlog blog = this.LoadBlog(Path.GetFileNameWithoutExtension(str));
                    if ((blog != null) && (blog._URL != null))
                    {
                        //blog.TOTAL_COUNT =  Directory.GetFiles(Properties.Settings.Default.configDownloadLocation.ToString() + blog._Name + "/").Length;
                        ListViewItem item = new ListViewItem
                        {
                            Text = blog._Name
                        };
                        if (blog._DownloadedImages > 0)
                        {
                            item.SubItems.Add(blog._DownloadedImages.ToString());
                        }
                        else
                        {
                            item.SubItems.Add("Not yet crawled!");
                        }
                        item.SubItems.Add(blog._TotalCount.ToString());
                        item.SubItems.Add(blog._URL);
                        item.SubItems.Add(blog._DateAdded.ToString("G"));
                        item.SubItems.Add(blog._LastCrawled.ToString("G"));
                        item.SubItems.Add(blog._finishedCrawl.ToString());
                        this.lvBlog.Items.Add(item);
                        blog = null;
                    }
                }
            }
        }

        private void mnuExit_Click(object sender, EventArgs e)
        {
            base.Close();
        }

        private void mnuRescanBlog_Click(object sender, EventArgs e)
        {
            //this.worker = new Thread(new ParameterizedThreadStart(this.RunParser));
            cts = new CancellationTokenSource();
            this.BeginInvoke((MethodInvoker)delegate
            {
                this.smallImage.ImageLocation = "";
                this.crawlingBlogs = "";
                this.lblUrl.Text = "";
                this.lblProcess.Text = "Crawling Blogs - " + this.crawlingBlogs;
            });
            for (int i = 0; i < Properties.Settings.Default.configSimultaneousDownloads; i++ )
                this.tasks[i] = Task.Run(() => runProducer(bin, cts.Token));
            //this.worker.Name = "TumblOne Thread";
            //this.worker.IsBackground = true;
            this.wait_handle = new ManualResetEvent(true);
            //this.worker.Start(text);
            this.panelInfo.Visible = false;
            this.toolPause.Enabled = true;
            this.toolResume.Enabled = false;
            this.toolStop.Enabled = true;
            this.toolCrawl.Enabled = false;
            this.toolRemoveBlog.Enabled = false;
            this.contextBlog.Items[3].Enabled = false;
        }

        private void mnuShowFilesInExplorer_Click(object sender, EventArgs e)
        {
            if (this.lvBlog.SelectedItems.Count > 0)
            {
                foreach (ListViewItem eachItem in this.lvBlog.SelectedItems)
                {
                    try
                    {
                        Process.Start("explorer.exe", Application.StartupPath + @"\Blogs\" + eachItem.Text);
                    }
                    catch (Exception exception)
                    {
                        MessageBox.Show(exception.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    }
                }
            }
        }

        private void mnuVisit_Click(object sender, EventArgs e)
        {
            if (this.lvBlog.SelectedItems.Count >= 0)
            {
                foreach (ListViewItem eachItem in this.lvBlog.SelectedItems)
                {
                    try
                    {
                        Process.Start(eachItem.SubItems[3].Text);
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
            if ((this.tasks[0] != null) && tasks[0].Status == TaskStatus.Running)
            {
                MessageBox.Show("During an active crawl, it's not possible to remove a blog!", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else if ((this.lvBlog.SelectedItems != null) && (this.lvBlog.SelectedItems.Count != 0))
            {
                if (MessageBox.Show("Should the selected Blog really be deleted from Library?", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    foreach (ListViewItem eachItem in this.lvBlog.SelectedItems)
                    {
                        string path = Properties.Settings.Default.configDownloadLocation.ToString() + "Index/" + eachItem.Text + ".tumblr";
                        string str2 = Properties.Settings.Default.configDownloadLocation.ToString() + eachItem.Text;
                        try
                        {
                            if (System.IO.File.Exists(path))
                            {
                                System.IO.File.Delete(path);
                            }
                            Directory.Delete(str2, true);
                            this.LoadLibrary();
                        }
                        catch (Exception exception)
                        {
                            MessageBox.Show(exception.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Hand);
                        }
                    }
                }
            }
        }

        private void RunParser(TumblrBlog _blog)
        {
            MethodInvoker method = null;
            bool readDataBase = false;
            int numberOfPostsCrawled = 0;
            int numberOfPagesCrawled = 0;
            int lastCrawledPageReached = 0;
            //string blogname = ExtractBlogname(ApiUrl.ToString());
            String ApiUrl = _blog._URL ;
            if (ApiUrl.Last<char>() != '/')
            {
                ApiUrl = ApiUrl + "/api/read?start=";
            }
            else
            {
                ApiUrl = ApiUrl + "api/read?start=";
            }
            this.CreateDataFolder(_blog._Name);
            while (true)
            {
                this.wait_handle.WaitOne();
                XDocument document = null;
                try
                {
                    document = XDocument.Load(ApiUrl.ToString() + numberOfPostsCrawled.ToString() + "&num=50");
                }
                catch (WebException)
                {
                    //this.toolStop_Click(this, null);
                    break;
                }
                if (numberOfPostsCrawled == 0)
                {
                    try
                    {
                        foreach (var type in from data in document.Descendants("posts") select new { Total = data.Attribute("total").Value })
                        {
                            _blog._TotalCount = Convert.ToInt32(type.Total.ToString());
                        }
                        if (method == null)
                        {
                            method = delegate
                            {
                                foreach (ListViewItem item in this.lvBlog.Items)
                                {
                                    if (item.Text == _blog._Name)
                                    {
                                        item.SubItems[2].Text = _blog._TotalCount.ToString();
                                        break;
                                    }
                                }
                                this.pgBar.Minimum = 0;
                                this.pgBar.Maximum = _blog._TotalCount;
                                this.pgBar.Value = _blog._DownloadedImages;
                            };
                        }
                        this.BeginInvoke(method);
                    }
                    catch
                    {
                        _blog._TotalCount = 0;
                        //this.toolStop_Click(this, null);
                        break;
                    }
                }

                IEnumerable<XElement> query;
                try
                {
                    if (_blog._Tags.Any())
                    {
                        query = (from n in document.Descendants("post")
                                 where n.Elements("photo-url").Where(x => x.Attribute("max-width").Value == Properties.Settings.Default.configImageSize.ToString()).Any() &&
                                 !n.Elements("photo-url").Where(x => x.Value == "www.tumblr.com").Any() &&
                                 n.Elements("tag").Where(x => _blog._Tags.Contains(x.Value)).Any()
                                 select n.Element("photo-url"));
                    }
                    else
                    {
                        query = (from n in document.Descendants("post")
                                where n.Elements("photo-url").Where(x => x.Attribute("max-width").Value == Properties.Settings.Default.configImageSize.ToString()).Any() &&
                                !n.Elements("photo-url").Where(x => x.Value == "www.tumblr.com").Any()
                                select n.Element("photo-url"));
                    }
                }
                catch (Exception)
                {
                    query = (from n in document.Descendants("post")
                             where n.Elements("photo-url").Where(x => x.Attribute("max-width").Value == Properties.Settings.Default.configImageSize.ToString()).Any() &&
                             !n.Elements("photo-url").Where(x => x.Value == "www.tumblr.com").Any()
                             select n.Element("photo-url"));
                }
                using (IEnumerator<XElement> enumerator = query.GetEnumerator())
                {
                    numberOfPagesCrawled++;
                    numberOfPostsCrawled = 50 * numberOfPagesCrawled;
                    try
                    {
                        while (enumerator.MoveNext())
                        {
                            XElement p = enumerator.Current;
                            MethodInvoker invoker = null;
                            string FileLocation;
                            this.wait_handle.WaitOne();
                            string fileName = Path.GetFileName(new Uri(p.Value).LocalPath);
                            if (!this.chkGIF.Checked || (Path.GetExtension(fileName).ToLower() != ".gif"))
                            {
                                FileLocation = Properties.Settings.Default.configDownloadLocation.ToString() + _blog._Name + "/" + fileName;
                                try
                                {
                                    if (this.Download(_blog, FileLocation, p.Value, fileName))
                                    {
                                        _blog.Links.Add(new Post(p.Value, fileName));
                                        _blog._DownloadedImages = _blog.Links.Count;
                                        if (invoker == null)
                                        {
                                            invoker = delegate
                                            {
                                                foreach (ListViewItem item in this.lvBlog.Items)
                                                {
                                                    if (item.Text == _blog._Name)
                                                    {
                                                        item.SubItems[1].Text = _blog.Links.Count.ToString();
                                                        break;
                                                    }
                                                }
                                                this.lblUrl.Text = p.Value;
                                                this.smallImage.ImageLocation = FileLocation;
                                                if ((this.pgBar.Value + 1) < (this.pgBar.Maximum + 1))
                                                {
                                                    this.pgBar.Value++;
                                                }
                                            };
                                        }
                                        this.BeginInvoke(invoker);
                                    }
                                    else
                                    {
                                        if (!readDataBase)
                                        {
                                            readDataBase = true;
                                            this.BeginInvoke((MethodInvoker)delegate
                                            {
                                                this.lblUrl.Text = "Skipping previously downloaded images - " + _blog._Name;
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
                    catch (Exception)
                    {
                        continue;
                    }
                }
                if (numberOfPostsCrawled >= _blog._TotalCount)
                {
                    //this.toolStop_Click(this, null);
                    // Finished Blog
                    _blog._LastCrawled = DateTime.Now;
                    _blog._finishedCrawl = true;
                    this.SaveBlog(_blog);
                    TumblrActiveList.Remove(_blog);
                    // Update UI
                    this.BeginInvoke((MethodInvoker)delegate
                    {
                        foreach (ListViewItem item in this.lvBlog.Items)
                        {
                            if (item.Text == _blog._Name)
                            {
                                // Update Listview about completed blog
                                item.SubItems[5].Text = DateTime.Now.ToString();
                                item.SubItems[6].Text = "True";
                                // Update current crawling progress label
                                int indexBlogInProgress = crawlingBlogs.IndexOf(_blog._Name);
                                int lengthBlogInProgress = _blog._Name.Length;
                                this.crawlingBlogs = crawlingBlogs.Remove(indexBlogInProgress, (lengthBlogInProgress+1));
                                this.lblProcess.Text = "Crawling Blogs - " + this.crawlingBlogs;
                            }
                        }
                    });

                    if (TumblrActiveList.Count == 0)
                    {
                        this.BeginInvoke((MethodInvoker)delegate
                        {
                            foreach (ListViewItem item in this.lvBlog.Items)
                            {
                                if (item.Text == _blog._Name)
                                {
                                    // Update current crawling progress label
                                    this.smallImage.ImageLocation = "";
                                    this.crawlingBlogs = "";
                                    this.lblUrl.Text = "";
                                    this.lblProcess.Text = "Queue finished";
                                }
                            }
                        });
                    }
                    return;
                }
            }
        }

        private bool SaveBlog(TumblrBlog newBlog)
        {
            if (newBlog == null)
            {
                return false;
            }
            this.CreateDataFolder(newBlog._Name);
            try
            {
                using (Stream stream = new FileStream(Properties.Settings.Default.configDownloadLocation.ToString() + "Index/" + newBlog._Name + ".tumblr", FileMode.Create, FileAccess.Write, FileShare.None))
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
            this.Invoke((Action)delegate
            {
                this.lblProcess.Text = "PAUSE! Click on Resume Button to continue...";
                this.toolPause.Enabled = false;
                this.toolResume.Enabled = true;
                this.toolStop.Enabled = true;
            });
        }

        private void toolResume_Click(object sender, EventArgs e)
        {
            this.wait_handle.Set();
            this.Invoke((Action)delegate
            {
                //Fixme
                this.lblProcess.Text = "Crawling Blogs - " + this.crawlingBlogs;
                this.toolPause.Enabled = true;
                this.toolResume.Enabled = false;
                this.toolStop.Enabled = true;
            });
        }

        private void toolStop_Click(object sender, EventArgs e)
        {
            MethodInvoker method = null;
            try
            {
                this.wait_handle.Reset();
                if (method == null)
                {
                    method = delegate
                    {
                        this.panelInfo.Visible = true;
                        this.lblProcess.Text = "Crawling of " + this.crawlingBlogs + "has stopped!";
                        //FIXME
                        this.lblUrl.Text = "";
                        this.smallImage.ImageLocation = "";
                        this.crawlingBlogs = "";
                        this.toolPause.Enabled = false;
                        this.toolResume.Enabled = false;
                        this.toolStop.Enabled = false;
                        this.toolCrawl.Enabled = true;
                        this.toolRemoveBlog.Enabled = true;
                        this.contextBlog.Items[3].Enabled = false;
                    };
                }
                this.Invoke(method);
            }
            catch (Exception)
            {
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
                this.lvQueue.Rows.Clear();
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
            loadPreferences();
        }

        // Load program preferences

        private void loadPreferences()
        {

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

        private void AddBlogtoQueue(List<TumblrBlog> bin, CancellationToken ct) 
        {
            // Cancellation causes OCE. 
            try
            {
                if (this.lvBlog.SelectedItems.Count > 0)
                {
                    foreach (ListViewItem eachItem in this.lvBlog.SelectedItems)
                    {
                        TumblrBlog blog = this.LoadBlog(this.ExtractBlogname(eachItem.SubItems[3].Text));
                        string text = eachItem.SubItems[3].Text;
                        //blog.Links.Clear();

                        //this.worker = new Thread(new ParameterizedThreadStart(this.RunParser));
                        //success = bin.TryAdd(blog, 2, ct);
                        bin.Add(blog);
                        this.addToQueueUI(blog);
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
                            try
                            {
                                nextBlog._Tags = lvQueue.Rows[0].Cells["chTags"].Value.ToString().Split(',').ToList();
                            }
                            catch (Exception)
                            {
                            }
                            TumblrActiveList.Add(nextBlog);

                            //                        if (TumblrActiveList.Count > Properties.Settings.Default.configSimultaneousDownloads)
                            //                        {
                            //                            TumblrBlog finishedBlog = (TumblrList.Find(x => x.Equals(TumblrActiveList.Take(1))));
                            //                            finishedBlog._LastCrawled = DateTime.Now;
                            //                            finishedBlog._finishedCrawl = true;
                            //                        }
                            //                        else
                            //                        {
                            //                        }

                            this.BeginInvoke((MethodInvoker)delegate
                            {
                                // Update UI:
                                // Processlabel
                                this.crawlingBlogs += this.lvQueue.Rows[0].Cells[0].Value + " ";
                                this.lblProcess.Text = "Crawling Blogs - " + this.crawlingBlogs;
                                // Queue
                                lvQueue.Rows.RemoveAt(0);
                            });
                            System.Threading.Monitor.Exit(bin);
                            this.RunParser(nextBlog);

                            //IEnumerable<TumblrBlog> differenceQuery = this.TumblrList.Except(this.bin);
                            //foreach (TumblrBlog active in differenceQuery)
                            //    foreach (string active._Name  in lvQueue.Items. (active._Name.Equals())
                        }
                        else
                        {
                            System.Threading.Monitor.Exit(bin);
                            Thread.Sleep(4000);
                            //Task.Delay(4000);   
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


            // Slow down consumer just a little to cause 
            // collection to fill up faster, and lead to "AddBlocked"
            // Thread.SpinWait(500000);
        }

        private void RemoveBlogFromQueue(List<TumblrBlog> bin, CancellationToken ct)
        {


            // Cancellation causes OCE. 
            try
            {
                if (bin.Count != 0)
                {
                    foreach (DataGridViewRow eachItem in lvQueue.SelectedRows)
                    {
                        bin.RemoveAt(eachItem.Index);
                        lvQueue.Rows.RemoveAt(eachItem.Index);
                    }

                    List<DataGridViewRow> rowCollection = new List<DataGridViewRow>();
                    foreach(DataGridViewCell cell in lvQueue.SelectedCells)
                    {
                        rowCollection.Add(lvQueue.Rows[cell.RowIndex]);
                    }

                    foreach (DataGridViewRow eachItem in rowCollection)
                    {
                        bin.RemoveAt(eachItem.Index);
                        lvQueue.Rows.RemoveAt(eachItem.Index);
                    }
                    //TumblrBlog nextBlog = null;
                    //this.TumblrList.RemoveAt(TumblrList.Count - 1);
                    //this.lvQueue.Items.RemoveAt(lvQueue.Items.Count - 1);
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
            DataGridViewRow lvQueueItem = new DataGridViewRow();
            this.lvQueue.Rows.Add(lvQueueItem);
            int index = lvQueue.Rows.Count - 1;
            lvQueue.Rows[index] .Cells[0].Value = _blog._Name;
            //lvQueueItem.SubItems.Add("");
            lvQueue.Rows[index].Cells[1].Value = "queued";
            try
            {
                lvQueue.Rows[index].Cells[2].Value = String.Join(",", _blog._Tags);
            }
            catch (Exception)
            {
            }
        }
    }
}