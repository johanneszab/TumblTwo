namespace TumblOne
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Runtime.InteropServices;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Threading;
    using System.Windows.Forms;
    using System.Xml.Linq;

    public partial class Form1 : Form
    {
        public TumblrBlog blog;
        public Thread worker;

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
                        MessageBox.Show("Entered Url is always in Library!", Application.ProductName);
                        this.tBlogUrl.Text = string.Empty;
                        return;
                    }
                }
                lvItem = new ListViewItem();
                if ((this.worker != null) && this.worker.IsAlive)
                {
                    TumblrBlog ThreadBlog = new TumblrBlog();
                    base.Invoke((Action)delegate
                    {
                        ThreadBlog._URL = this.ExtractUrl(this.tBlogUrl.Text);
                        ThreadBlog.TOTAL_COUNT = 0;
                        ThreadBlog._Name = this.ExtractBlogname(this.tBlogUrl.Text);
                        lvItem.Text = ThreadBlog._Name;
                        lvItem.SubItems.Add("");
                        lvItem.SubItems.Add(this.tBlogUrl.Text);
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
                        _URL = this.ExtractUrl(this.tBlogUrl.Text)
                    };
                    lvItem.Text = newBlog._Name;
                    lvItem.SubItems.Add("");
                    lvItem.SubItems.Add(newBlog._URL);
                    this.lvBlog.Items.Add(lvItem);
                    this.SaveBlog(newBlog);
                    newBlog = null;
                }
                this.tBlogUrl.Text = "http://";
                if (Directory.Exists("./Blogs/Index/"))
                {
                    if (Directory.GetFiles("./Blogs/Index/", "*.tumblr").Count<string>() > 0)
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

        private bool Download(string filename, string url)
        {
            if (!System.IO.File.Exists(filename))
            {
                try
                {
                    using (WebClient client = new WebClient())
                    {
                        client.DownloadFile(url, filename);
                    }
                }
                catch (Exception)
                {
                    return false;
                }
            }
            return true;
        }

        private string ExtractBlogname(string url)
        {
            if ((url == null) || (url.Length < 0x11))
            {
                MessageBox.Show("Incomplete Url detected!", Application.ProductName);
                return null;
            }
            if (!url.Contains(".tumblr.com"))
            {
                MessageBox.Show("No valid Tumblr Url detected!", Application.ProductName);
                return null;
            }
            string[] source = url.Split(new char[] { '.' });
            if ((source.Count<string>() >= 3) && source[0].StartsWith("http://", true, null))
            {
                return source[0].Replace("http://", string.Empty);
            }
            MessageBox.Show("Invalid Url detected!", Application.ProductName);
            return null;
        }

        private string ExtractUrl(string url)
        {
            return ("http://" + this.ExtractBlogname(url) + ".tumblr.com/");
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if ((this.worker != null) && this.worker.IsAlive)
            {
                try
                {
                    if (this.blog != null)
                    {
                        this.SaveBlog(this.blog);
                    }
                    if (this.wait_handle != null)
                    {
                        this.wait_handle.Close();
                    }
                    this.worker.Abort();
                }
                catch (ThreadAbortException exception)
                {
                    MessageBox.Show("Process stopped by User. " + exception.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                this.worker = null;
                this.wait_handle = null;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            using (SplashScreen screen = new SplashScreen())
            {
                screen.ShowDialog();
            }
        }



        private TumblrBlog LoadBlog(string blogname)
        {
            TumblrBlog blog = new TumblrBlog();
            try
            {
                using (Stream stream = new FileStream("./Blogs/Index/" + blogname + ".tumblr", FileMode.Open, FileAccess.Read, FileShare.Read))
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

        private void LoadLibrary()
        {
            this.lvBlog.Items.Clear();
            this.lblProcess.Text = "";
            this.lblUrl.Text = "";
            if (Directory.Exists("./Blogs/Index/"))
            {
                string[] files = Directory.GetFiles("./Blogs/Index/", "*.tumblr");
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
                        blog.TOTAL_COUNT = Directory.GetFiles("./Blogs/" + blog._Name + "/").Length;
                        ListViewItem item = new ListViewItem
                        {
                            Text = blog._Name
                        };
                        if (blog.TOTAL_COUNT > 0)
                        {
                            item.SubItems.Add(blog.TOTAL_COUNT.ToString());
                        }
                        else
                        {
                            item.SubItems.Add("Not crawled yet!");
                        }
                        item.SubItems.Add(blog._URL);
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
            if ((this.worker != null) && this.worker.IsAlive)
            {
                MessageBox.Show("During a active Crawl Process it is not possible, to crawl another Blog. Stop the current Crawl, by Click on Stop...", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                try
                {
                    if (this.lvBlog.SelectedItems.Count > 0)
                    {
                        this.blog = new TumblrBlog();
                        this.blog._Name = this.ExtractBlogname(this.lvBlog.SelectedItems[0].SubItems[2].Text);
                        this.blog._URL = this.lvBlog.SelectedItems[0].SubItems[2].Text;
                        string text = this.lvBlog.SelectedItems[0].SubItems[2].Text;
                        this.blog.Links.Clear();
                        this.CreateDataFolder(this.blog._Name);
                        if (text.Last<char>() != '/')
                        {
                            text = text + "/api/read?start=";
                        }
                        else
                        {
                            text = text + "api/read?start=";
                        }
                        this.panelInfo.Visible = false;
                        this.lblProcess.Text = "Crawl selected Blog - " + this.blog._Name;
                        this.worker = new Thread(new ParameterizedThreadStart(this.RunParser));
                        this.worker.Name = "TumblOne Thread";
                        this.worker.IsBackground = true;
                        this.wait_handle = new ManualResetEvent(true);
                        this.worker.Start(text);
                        this.toolPause.Enabled = true;
                        this.toolResume.Enabled = false;
                        this.toolStop.Enabled = true;
                        this.toolCrawl.Enabled = false;
                        this.toolRemoveBlog.Enabled = false;
                        this.contextBlog.Enabled = false;
                    }
                }
                catch (Exception exception)
                {
                    MessageBox.Show(exception.Message);
                }
            }
        }

        private void mnuShowFilesInExplorer_Click(object sender, EventArgs e)
        {
            if (this.lvBlog.SelectedItems.Count > 0)
            {
                try
                {
                    Process.Start("explorer.exe", Application.StartupPath + @"\Blogs\" + this.lvBlog.SelectedItems[0].Text);
                }
                catch (Exception exception)
                {
                    MessageBox.Show(exception.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Hand);
                }
            }
        }

        private void mnuVisit_Click(object sender, EventArgs e)
        {
            if (this.lvBlog.SelectedItems.Count >= 0)
            {
                try
                {
                    Process.Start(this.lvBlog.SelectedItems[0].SubItems[2].Text);
                }
                catch (Exception exception)
                {
                    MessageBox.Show(exception.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Hand);
                }
            }
        }

        private void RemoveBlog(object sender, EventArgs e)
        {
            if ((this.worker != null) && this.worker.IsAlive)
            {
                MessageBox.Show("During a active Crawl Process it is not possible to remove a Blog!", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else if ((this.lvBlog.SelectedItems != null) && (this.lvBlog.SelectedItems.Count != 0))
            {
                string path = "./Blogs/Index/" + this.lvBlog.SelectedItems[0].Text + ".tumblr";
                string str2 = "./Blogs/" + this.lvBlog.SelectedItems[0].Text;
                try
                {
                    if (MessageBox.Show("Should the selected Blog really deleted from Library?", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        if (System.IO.File.Exists(path))
                        {
                            System.IO.File.Delete(path);
                        }
                        Directory.Delete(str2, true);
                        this.LoadLibrary();
                    }
                }
                catch (Exception exception)
                {
                    MessageBox.Show(exception.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Hand);
                }
            }
        }

        private void RunParser(object ApiUrl)
        {
            MethodInvoker method = null;
            MethodInvoker invoker3 = null;
            int num = 0;
            int num2 = 0;
            int num3 = 0;
            base.Invoke((Action)delegate
            {
                this.pgBar.Minimum = 0;
                this.pgBar.Maximum = this.blog.TOTAL_COUNT;
            });
            while (true)
            {
                this.wait_handle.WaitOne();
                XDocument document = null;
                try
                {
                    document = XDocument.Load(ApiUrl.ToString() + num.ToString() + "&num=50");
                }
                catch (WebException)
                {
                    this.toolStop_Click(this, null);
                    break;
                }
                if (num == 0)
                {
                    try
                    {
                        foreach (var type in from data in document.Descendants("posts") select new { Total = data.Attribute("total").Value })
                        {
                            this.blog.TOTAL_COUNT = Convert.ToInt32(type.Total.ToString());
                        }
                        if (method == null)
                        {
                            method = delegate
                            {
                                this.pgBar.Minimum = 0;
                                this.pgBar.Maximum = this.blog.TOTAL_COUNT;
                            };
                        }
                        base.Invoke(method);
                    }
                    catch
                    {
                        this.blog.TOTAL_COUNT = 0;
                        this.toolStop_Click(this, null);
                        break;
                    }
                }
                using (IEnumerator<XElement> enumerator2 = (from s in document.Descendants("photo-url")
                                                            where (s.HasAttributes && s.Attribute("max-width").Value.Equals("1280")) && !s.Value.Contains("www.tumblr.com")
                                                            select s).GetEnumerator())
                {
                    while (enumerator2.MoveNext())
                    {
                        XElement p = enumerator2.Current;
                        MethodInvoker invoker = null;
                        string FileLocation;
                        this.wait_handle.WaitOne();
                        string fileName = Path.GetFileName(new Uri(p.Value).LocalPath);
                        if (!this.chkGIF.Checked || (Path.GetExtension(fileName).ToLower() != ".gif"))
                        {
                            FileLocation = "./Blogs/" + this.blog._Name + "/" + fileName;
                            this.blog.Links.Add(new Post(p.Value, fileName));
                            if (invoker3 == null)
                            {
                                invoker3 = delegate
                                {
                                    foreach (ListViewItem item in this.lvBlog.Items)
                                    {
                                        if (item.Text == this.blog._Name)
                                        {
                                            item.SubItems[1].Text = Directory.GetFiles("./Blogs/" + this.blog._Name + "/").Length.ToString();
                                            break;
                                        }
                                    }
                                };
                            }
                            base.Invoke(invoker3);
                            try
                            {
                                if (this.Download(FileLocation, p.Value))
                                {
                                    num2++;
                                    if (invoker == null)
                                    {
                                        invoker = delegate
                                        {
                                            this.lblUrl.Text = p.Value;
                                            this.smallImage.ImageLocation = FileLocation;
                                            if ((this.pgBar.Value + 1) < (this.pgBar.Maximum + 1))
                                            {
                                                this.pgBar.Value++;
                                            }
                                        };
                                    }
                                    base.Invoke(invoker);
                                }
                            }
                            catch (Exception)
                            {
                                continue;
                            }
                            num3++;
                        }
                    }
                }
                if (num3 == 0)
                {
                    this.toolStop_Click(this, null);
                    return;
                }
                num += num3;
                num3 = 0;
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
                using (Stream stream = new FileStream("./Blogs/Index/" + newBlog._Name + ".tumblr", FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    IFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(stream, newBlog);
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Current Blog cannot saved to Disk!\nBe sure, that u have enough Memory and User Permission...", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Hand);
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
            base.Invoke((Action)delegate
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
            base.Invoke((Action)delegate
            {
                this.lblProcess.Text = "Crawl selected Blog - " + this.blog._Name;
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
                        this.lblProcess.Text = "Last Crawl of " + this.blog._Name + " stopped!";
                        this.lblUrl.Text = "";
                        this.smallImage.ImageLocation = "";
                        this.pgBar.Value = 0;
                        this.toolPause.Enabled = false;
                        this.toolResume.Enabled = false;
                        this.toolStop.Enabled = false;
                        this.toolCrawl.Enabled = true;
                        this.toolRemoveBlog.Enabled = true;
                        this.contextBlog.Enabled = true;
                    };
                }
                base.Invoke(method);
            }
            catch (Exception)
            {
            }
            if (this.blog != null)
            {
                this.SaveBlog(this.blog);
            }
            try
            {
                this.worker.Abort();
            }
            catch (ThreadAbortException)
            {
            }
        }
    }
}

