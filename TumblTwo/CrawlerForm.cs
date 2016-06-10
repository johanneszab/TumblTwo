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

namespace TumblTwo
{
    public partial class CrawlerForm : Form
    {
        private List<TumblrBlog> activeCrawlList = new List<TumblrBlog>();
        public Task[] taskList = new Task[Properties.Settings.Default.configSimultaneousDownloads];
        public BindingSource bsSmallImage = new BindingSource();
        private List<TumblrBlog> bin = new List<TumblrBlog>();
        private SortableBindingList<TumblrBlog> blogs = new SortableBindingList<TumblrBlog>();
        private BindingList<string> pictureList = new BindingList<string>();

        private CancellationTokenSource crawlBlogsCancellation;
        private PauseTokenSource crawlBlogsPause;

        public CrawlerForm()
        {
            InitializeComponent();

            Shown += new System.EventHandler(OnShown);
            blogs.ListChanged += BlogsListChanged;
        }

        private void OnShown(object sender, EventArgs e)
        {
            LoadGUI();

            BeginInvoke((MethodInvoker)delegate
            {
                try
                {
                    // set databinings for the picturebox and the information label
                    bsSmallImage.DataSource = pictureList;
                    smallImage.DataBindings.Add("ImageLocation", bsSmallImage, "", false, DataSourceUpdateMode.OnPropertyChanged);
                    bsSmallImage.ListChanged += bsSmallImage_ListChanged;
                }
                catch (Exception)
                {
                }

            });
        }

        private void OnFormClosing(object sender, FormClosingEventArgs e)
        {

            try
            {
                if (crawlBlogsCancellation != null)
                {
                    if (crawlBlogsPause != null && crawlBlogsPause.IsPaused)
                        crawlBlogsPause.Resume();
                    crawlBlogsCancellation.Cancel();
                    this.Hide();
                    Task.WaitAll(taskList.ToArray());
                }
            }
            catch (AggregateException)
            {
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

        private void OnLoad(object sender, EventArgs e)
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
            smallImage.Visible = Properties.Settings.Default.configPreviewVisible;
        }

        public async void LoadGUI()
        {
            await LoadLibrary();

            // Delay load of preferences which interfere with library loading
            LoadPreferences();

            // delay online check to reduce startup time
            if (Properties.Settings.Default.configCheckStatusAtStartup)
            {
                foreach (TumblrBlog blog in blogs)
                {
                    Task.Run(() => { CheckIfBlogIsOnline(blog); });
                }
            }
        }

        public Task LoadLibrary()
        {
            return Task.Run(() =>
            {
                // Clean UI
                BeginInvoke((MethodInvoker)delegate
                {
                    blogs.Clear();
                    lblProcess.Text = "";
                });
                if (Directory.Exists(Properties.Settings.Default.configDownloadLocation.ToString() + "Index\\"))
                {
                    string[] files = Directory.GetFiles(Properties.Settings.Default.configDownloadLocation.ToString() + "Index\\", "*.tumblr");

                    BeginInvoke((MethodInvoker)delegate
                    {
                        // load blogs
                        // format the Datagridview and bind the source (list of TumblrBlogs)
                        foreach (string str in files)
                        {
                            blogs.Add(LoadBlog(Path.GetFileNameWithoutExtension(str)));
                        }
                    });
                }
            });
        }

        private void LoadPreferences()
        {
            // Setup Clipboard Monitor Listener
            if (Properties.Settings.Default.CheckClipboard)
            {
                toolCheckClipboard.Checked = true;
                ClipboardMonitor.Start();
                ClipboardMonitor.OnClipboardChange += ClipboardMonitor_OnClipboardChange;
            }
            else
            {
                toolCheckClipboard.Checked = false;
            }
        }

        private void AddBlog(object sender, EventArgs e)
        {
            string str = ExtractBlogname(tBlogUrl.Text);
            if (str != null)
            {
                if (blogs.Select(blog => blog.Name).ToList().Contains(str))
                {
                    MessageBox.Show("The entered URL is already in the library!", Application.ProductName);
                    tBlogUrl.Text = string.Empty;
                    return;
                }
                TumblrBlog newBlog = new TumblrBlog
                {
                    Name = str,
                    Url = ExtractUrl(tBlogUrl.Text),
                    DateAdded = DateTime.Now,
                    DownloadedImages = 0,
                    TotalCount = 0,
                    LastCrawled = System.DateTime.MinValue,
                    FinishedCrawl = false
                };
                SaveBlog(newBlog);
                blogs.Add(newBlog);

                newBlog = null;
                tBlogUrl.Text = "http://";
            } else
            {
                MessageBox.Show("No valid Tumblr URL entered!", Application.ProductName);
            }
        }


        private void RemoveBlog(object sender, EventArgs e)
        {
            if (lvBlog.SelectedRows.Count != 0)
            {
                if (Properties.Settings.Default.configDeleteIndexOnly)
                {
                    if (MessageBox.Show("Should the selected Blog really be deleted from Library (only removes Index Files, no downloaded images)?", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        foreach (DataGridViewRow item in lvBlog.SelectedRows)
                        {
                            string indexPath = Properties.Settings.Default.configDownloadLocation.ToString() + "Index\\" + ((TumblrBlog)item.DataBoundItem).Name + ".tumblr";
                            try
                            {
                                if (System.IO.File.Exists(indexPath))
                                {
                                    System.IO.File.Delete(indexPath);
                                }
                                // Update UI
                                blogs.RemoveAt(item.Index);
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
                        foreach (DataGridViewRow item in lvBlog.SelectedRows)
                        {
                            string indexPath = Properties.Settings.Default.configDownloadLocation.ToString() + "Index\\" + ((TumblrBlog)item.DataBoundItem).Name + ".tumblr";
                            string filePath = Properties.Settings.Default.configDownloadLocation.ToString() + ((TumblrBlog)item.DataBoundItem).Name;
                            try
                            {
                                if (System.IO.File.Exists(indexPath))
                                {
                                    System.IO.File.Delete(indexPath);
                                }
                                Directory.Delete(filePath, true);
                                // Update UI
                                blogs.RemoveAt(item.Index);
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

        private void ClipboardMonitor_OnClipboardChange(ClipboardFormat format, object data)
        {
            if (format.ToString() == "Text")
            {
                string[] Urls = data.ToString().Split(new char[0]);
                foreach (string url in Urls)
                {
                    string str = ExtractBlogname(url);
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
                                Url = ExtractUrl(url),
                                DateAdded = DateTime.Now,
                                DownloadedImages = 0,
                                TotalCount = 0,
                                LastCrawled = System.DateTime.MinValue,
                                FinishedCrawl = false
                            };
                            SaveBlog(newBlog);
                            Invoke((Action)delegate
                            {
                                blogs.Add(newBlog);
                            });
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
                if (!Directory.Exists(path + "\\Index"))
                {
                    Directory.CreateDirectory(path + "\\Index");
                }
                if (!Directory.Exists(path + "\\" + blogname))
                {
                    Directory.CreateDirectory(path + "\\" + blogname);
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        private string ExtractBlogname(string url)
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
            } else if ((source.Count<string>() >= 3) && source[0].StartsWith("https://", true, null)) {
                return source[0].Replace("https://", string.Empty);
            }
            return null;
        }

        private string ExtractUrl(string url)
        {
            return ("http://" + ExtractBlogname(url) + ".tumblr.com/");
        }

        private TumblrBlog LoadBlog(string blogname)
        {
            TumblrBlog blog = new TumblrBlog();
            try
            {
                using (FileStream stream = new FileStream(Properties.Settings.Default.configDownloadLocation.ToString() + "Index\\" + blogname + ".tumblr", FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    IFormatter formatter = new BinaryFormatter();
                    blog = (TumblrBlog)formatter.Deserialize(stream);
                }
            }
            catch (SerializationException SerializationException)
            {
                using (FileStream stream = new FileStream(Properties.Settings.Default.configDownloadLocation.ToString() + "Index\\" + blogname + ".tumblr", FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    IFormatter formatter = new BinaryFormatter();

                    //Set formatters binder property to TumblTwo 1.0.4.0 / 0.0.0.0 Version
                    formatter.Binder = new TumblTwo.Typeconvertor();
                    blog = (TumblrBlog)formatter.Deserialize(stream);
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
            return blog;
        }

        private bool SaveBlog(TumblrBlog newBlog)
        {
            if (newBlog == null)
            {
                return false;
            }
            CreateDataFolder(newBlog.Name);
            try
            {
                using (Stream stream = new FileStream(Properties.Settings.Default.configDownloadLocation.ToString() + "Index\\" + newBlog.Name + ".tumblr", FileMode.Create, FileAccess.Write, FileShare.None))
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

        private void RunCrawler(List<TumblrBlog> bin, CancellationToken ct, PauseToken pt)
        {
            while (true)
            {
                // check if stopped
                if (ct.IsCancellationRequested)
                {
                    throw new OperationCanceledException(ct);
                }
                // check if paused
                if (pt.IsPaused)
                    pt.WaitWhilePausedWithResponseAsyc().Wait();

                System.Threading.Monitor.Enter(bin);
                if (bin.Any())
                {
                    TumblrBlog blog;

                    blog = bin.First<TumblrBlog>();
                    bin.RemoveAt(0);
                    activeCrawlList.Add(blog);

                    var progressHandler = new Progress<DownloadProgress>(value =>
                    {
                        blog.Progress = value.ProgressPercentage;
                        blog.Links.Add(value.Url);
                        blog.DownloadedImages = value.DownloadedImages;
                        blog.Information = value.Information;
                        //blog.TotalCount = value.TotalCount;
                    });
                    var progress = progressHandler as IProgress<DownloadProgress>;

                    BeginInvoke((MethodInvoker)delegate
                    {
                        // Update UI:
                        // Processlabel
                        lblProcess.Text = "Crawling Blogs -- " + String.Join(" - ", activeCrawlList.Select(activeBlog => activeBlog.Name).ToArray());
                        // Queue
                        lvQueue.Items.RemoveAt(0);
                    });
                    System.Threading.Monitor.Exit(bin);
                    if (Properties.Settings.Default.configParallelCrawl)
                        RunParserParallel(blog, progress, ct, pt);
                    else
                        RunParserSerial(blog, progress, ct, pt);
                }
                else
                {
                    System.Threading.Monitor.Exit(bin);
                    Task.Delay(4000, ct).Wait();
                }
            }

        }

        private void RunParserParallel(TumblrBlog blog, IProgress<DownloadProgress> progress, CancellationToken ct, PauseToken pt)
        {
            int totalPostCount = 0;
            int totalImages = 0;
            List<string> crawledImageURLs = new List<string>();
            String ApiUrl = GetApiUrl(blog.Url);

            // make sure we can save files
            CreateDataFolder(blog.Name);

            BeginInvoke((MethodInvoker)delegate
            {
                blog.Information = "Checking blogs status ...";
            });

            // set title and name of the blog
            // check if blogaddress is alive or someone else is using it now

            if (!CheckIfBlogIsOnline(blog))
            {
                // nothing to crawl, cleanup and leave
                CleaupParserAndSaveBlog(blog);
                return;
            }
            // Update image (blog post) count

            totalPostCount = GetPostCount(blog);

            var tuple = GetImageUrls(blog, ct, pt);
            totalImages = tuple.Item1;
            crawledImageURLs = tuple.Item2;

            // Start the crawl process

            BeginInvoke((MethodInvoker)delegate
            {
                blog.Information = "Calculating new image urls ...";
                blog.TotalCount = totalImages;
            });

            if (ct.IsCancellationRequested)
            {
                CleaupParserAndSaveBlog(blog);
                return;
            }
            if (pt.IsPaused)
                pt.WaitWhilePausedWithResponseAsyc().Wait();

            // substract already crawled urls

            if (Properties.Settings.Default.configCheckMirror)
            {
                // 
                //crawledImageURLs = crawledImageURLs.Where(url => blog.Links.Any(Post => !url.Substring(url.LastIndexOf('/') + 1).Contains(Post.Url))).ToList();
                crawledImageURLs = crawledImageURLs.Where(url => !blog.Links.Any(Post => url.Contains(Post.Filename))).ToList();
            }
            else
            {
                // This would work if the hostname would be the same, but when the file is hosted on a different mirror, we load the same file again
                crawledImageURLs = crawledImageURLs.Except(blog.Links.Select(Posts => Posts.Url).ToList()).ToList();
            }

            // start the crawl
            Parallel.ForEach(
                crawledImageURLs,
                new ParallelOptions { MaxDegreeOfParallelism = (Properties.Settings.Default.configParallelImageDownloads / Properties.Settings.Default.configSimultaneousDownloads) },
                (url, state) =>
                {
                    string FileLocation;
                    if (ct.IsCancellationRequested)
                    {
                        state.Break();
                    }
                    if (pt.IsPaused)
                        pt.WaitWhilePausedWithResponseAsyc().Wait();
                    // create filename from url, just skip everything before the first slash (/)
                    string fileName = Path.GetFileName(new Uri(url).LocalPath);
                    // check if we should crawl .gifs
                    if (!Properties.Settings.Default.configChkGIFState || (Path.GetExtension(fileName).ToLower() != ".gif"))
                    {
                        // create path to save the file in the proper folder
                        FileLocation = Properties.Settings.Default.configDownloadLocation.ToString() + blog.Name + "/" + fileName;
                        try
                        {
                            // download file if the file is new
                            if (Download(blog, FileLocation, url, fileName))
                            {
                                var newProgress = new DownloadProgress();
                                newProgress.ProgressPercentage = (int)((double)blog.DownloadedImages / (double)blog.TotalCount * 100);
                                newProgress.Url = new Post(url, fileName);
                                newProgress.DownloadedImages += blog.Links.Count();
                                newProgress.Information = "Downloading " + url;

                                progress.Report(newProgress);

                                pictureList.Add((Path.GetFullPath(FileLocation)));

                            }
                        }
                        catch (Exception)
                        {
                            //NetSocket / IO error --> Connection problem;
                        }
                    }
                });

            BeginInvoke((MethodInvoker)delegate
            {
                blog.Information = "Crawl finished - cleaning up";
            });

            // Finished crawling the blog
            if (!ct.IsCancellationRequested)
            {
                blog.LastCrawled = DateTime.Now;
                blog.FinishedCrawl = true;

                // remove index from listview after completed crawl, if property set
                if (Properties.Settings.Default.configRemoveFinishedBlogs)
                {
                    string path = Properties.Settings.Default.configDownloadLocation.ToString() + "Index\\" + blog.Name + ".tumblr";
                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                    }
                    BeginInvoke((MethodInvoker)delegate
                    {
                        blogs.Remove(blog);
                    });
                }
            }
            CleaupParserAndSaveBlog(blog);
            if (ct.IsCancellationRequested)
            {
                throw new OperationCanceledException(ct);
            }
        }

        private void RunParserSerial(TumblrBlog blog, IProgress<DownloadProgress> progress, CancellationToken ct, PauseToken pt)
        {
            bool readingDataBase = false;
            int numberOfPostsCrawled = 0;
            int numberOfPagesCrawled = 0;
            int totalPostCount = 0;
            List<string> crawledImageURLs = new List<string>();
            String ApiUrl = GetApiUrl(blog.Url);

            // make sure we can save files
            CreateDataFolder(blog.Name);

            BeginInvoke((MethodInvoker)delegate
            {
                blog.Information = "Checking blogs status ...";
            });

            // set title and name of the blog
            // check if blogaddress is alive or someone else is using it now

            if (!CheckIfBlogIsOnline(blog))
            {
                // nothing to crawl, cleanup and leave
                CleaupParserAndSaveBlog(blog);
                return;
            }

            // Update image (blog post) count
            totalPostCount = GetPostCount(blog);

            while (true)
            {
                if (ct.IsCancellationRequested)
                {
                    // Clean up here
                    ct.ThrowIfCancellationRequested();
                }
                if (pt.IsPaused)
                    pt.WaitWhilePausedWithResponseAsyc().Wait();

                blog.TotalCount = totalPostCount;
                IEnumerable<XElement> query;
                XDocument blogDoc = GetBlogDoc(blog, 50, numberOfPagesCrawled);

                try
                {
                    if (string.IsNullOrEmpty(blog.Tags))
                    {
                        query = (from n in blogDoc.Descendants("post")
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
                        List<string> tags = blog.Tags.Split(',').Select(x => x.Trim()).ToList();
                        query = (from n in blogDoc.Descendants("post")

                                     // Identify Posts
                                 where n.Elements("photo-url").Where(x => x.Attribute("max-width").Value == Properties.Settings.Default.configImageSize.ToString()).Any() &&
                                     !n.Elements("photo-url").Where(x => x.Value == "www.tumblr.com").Any() &&
                                     n.Elements("tag").Where(x => tags.Contains(x.Value)).Any() ||

                                     // Identify Photosets
                                     n.Elements("photoset").Where(photoset => photoset.Descendants("photo-url")
                                     .Any(photourl => (string)photourl.Attribute("max-width").Value
                                     == Properties.Settings.Default.configImageSize.ToString() &&
                                     photourl.Value != "www.tumblr.com")).Any() &&
                                     n.Elements("tag").Where(x => tags.Contains(x.Value)).Any()

                                 from m in n.Descendants("photo-url")
                                 where m.Attribute("max-width").Value == Properties.Settings.Default.configImageSize.ToString()
                                 select m);
                    }
                }
                catch (Exception)
                {
                    // no images on the page
                    numberOfPagesCrawled++;
                    numberOfPostsCrawled = 50 * numberOfPagesCrawled;
                    continue;
                }
                // Start the crawl process
                using (IEnumerator<XElement> enumerator = query.GetEnumerator())
                {
                    numberOfPagesCrawled++;
                    numberOfPostsCrawled = 50 * numberOfPagesCrawled;

                    // start the crawl
                    while (enumerator.MoveNext())
                    {
                        XElement p = enumerator.Current;
                        string FileLocation;
                        if (ct.IsCancellationRequested)
                        {
                            // Clean up here
                            CleaupParserAndSaveBlog(blog);
                            ct.ThrowIfCancellationRequested();
                        }
                        if (pt.IsPaused)
                            pt.WaitWhilePausedWithResponseAsyc().Wait();
                        // create filename from url, just skip everything before the first slash (/)
                        string fileName = Path.GetFileName(new Uri(p.Value).LocalPath);
                        // check if we should crawl .gifs
                        if (!Properties.Settings.Default.configChkGIFState || (Path.GetExtension(fileName).ToLower() != ".gif"))
                        {
                            // create path to save the file in the proper folder
                            FileLocation = Properties.Settings.Default.configDownloadLocation.ToString() + blog.Name + "/" + fileName;
                            try
                            {
                                // download file if the file is new
                                if (Download(blog, FileLocation, p.Value, fileName))
                                {
                                    var newProgress = new DownloadProgress();
                                    newProgress.ProgressPercentage = (int)((double)blog.DownloadedImages / (double)blog.TotalCount * 100);
                                    newProgress.Url = new Post(p.Value, fileName);
                                    newProgress.DownloadedImages += blog.Links.Count();
                                    newProgress.Information = "Downloading " + p.Value;

                                    progress.Report(newProgress);
                                    pictureList.Add((Path.GetFullPath(FileLocation)));

                                    readingDataBase = false;
                                }
                                else
                                {
                                    if (!readingDataBase)
                                    {
                                        var newProgress = new DownloadProgress();
                                        newProgress.Information = "Skipping previously downloaded images...";

                                        progress.Report(newProgress);
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

                    BeginInvoke((MethodInvoker)delegate
                    {
                        blog.Information = "Crawl Finished - Cleaning Up";
                    });

                    // Finished crawling the blog
                    blog.LastCrawled = DateTime.Now;
                    blog.FinishedCrawl = true;

                    CleaupParserAndSaveBlog(blog);

                    // remove index from listview after completed crawl, if property set
                    if (Properties.Settings.Default.configRemoveFinishedBlogs)
                    {
                        string path = Properties.Settings.Default.configDownloadLocation.ToString() + "Index\\" + blog.Name + ".tumblr";
                        if (System.IO.File.Exists(path))
                        {
                            System.IO.File.Delete(path);
                        }
                        BeginInvoke((MethodInvoker)delegate
                        {
                            blogs.Remove(blog);
                        });
                    }
                    break;
                }
            }
        }

        void CleaupParserAndSaveBlog(TumblrBlog blog)
        {
            activeCrawlList.Remove(blog);
            blog.Information = "";

            if (activeCrawlList.Count != 0)
            {
                // Update UI
                BeginInvoke((MethodInvoker)delegate
                {
                    // Update current crawling progress label
                    lblProcess.Text = "Crawling Blogs -- " + String.Join(" - ", activeCrawlList.Select(activeBlog => activeBlog.Name).ToArray());
                });
            }
            else
            {
                BeginInvoke((MethodInvoker)delegate
                {
                    // Update current crawling progress label
                    lblProcess.Text = "Queue finished";
                });
            }
            SaveBlog(blog);
        }

        private string GetApiUrl(string url)
        {
            if (url.Last<char>() != '/')
                url += "/api/read?start=";
            else
                url += "api/read?start=";

            return url;
        }

        public int GetPostCount(TumblrBlog blog)
        {
            int count = 0;

            XDocument blogDoc = GetBlogDoc(blog, 0, 0);

            foreach (var type in from data in blogDoc.Descendants("posts") select new { Total = data.Attribute("total").Value })
            {
                count = Convert.ToInt32(type.Total.ToString());
            }

            return count;
        }

        public XDocument GetBlogDoc(TumblrBlog blog, int numPosts, int page)
        {
            XDocument blogDoc = new XDocument();

            try
            {
                blogDoc = XDocument.Load(GetApiUrl(blog.Url) + (numPosts * page) + "&num=" + numPosts);
            }
            catch (Exception ex)
            {
                blog.Online = false;
            }
            return blogDoc;
        }

        private bool Download(TumblrBlog blog, string fileLocation, string url, string filename)
        {
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

        public Tuple<int, List<string>> GetImageUrls(TumblrBlog blog, CancellationToken ct, PauseToken pt)
        {
            int totalPosts = 0;
            int numberOfPostsCrawled = 0;
            int totalImages = 0;
            List<string> urlList = new List<string>();
            List<string> newUrls = new List<string>();

            var blogDoc = GetBlogDoc(blog, 0, 0);

            totalPosts = GetPostCount(blog);

            // Generate URL list of Images
            // the api shows 50 posts at max, determine the number of pages to crawl
            int totalPages = (totalPosts / 50) + 1;

            Parallel.For(
                0,
                totalPages,
                new ParallelOptions { MaxDegreeOfParallelism = (Properties.Settings.Default.configParallelImageDownloads / Properties.Settings.Default.configSimultaneousDownloads) },
                (i, state) =>
                {
                    if (ct.IsCancellationRequested)
                    {
                        state.Break();
                    }
                    if (pt.IsPaused)
                        pt.WaitWhilePausedWithResponseAsyc().Wait();

                    if (string.IsNullOrEmpty(blog.Tags))
                    {
                        try
                        {
                            XDocument document = null;
                            document = XDocument.Load(GetApiUrl(blog.Url) + (i * 50).ToString() + "&num=50");

                            if (Properties.Settings.Default.configDownloadImage)
                            {

                                newUrls = (from n in document.Descendants("post")
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
                            if (Properties.Settings.Default.configDownloadVideo)
                            {
                                foreach (var post in (from data in document.Descendants("post") where data.Attribute("type").Value == "video" select data))
                                {
                                    var videoUrl = post.Descendants("video-player").Where(x => x.Value.Contains("<source src=")).Select(result =>
                                    System.Text.RegularExpressions.Regex.Match(
                                              result.Value, "<source src=\"(.*)\" type=\"video/mp4\">").Groups[1].Value).ToList();

                                    foreach (string video in videoUrl)
                                    {
                                        if (Properties.Settings.Default.configVideoSize == 1080)
                                        {
                                            Monitor.Enter(urlList);
                                            urlList.Add(video.Replace("/480", "") + ".mp4");
                                            Monitor.Exit(urlList);
                                        }
                                        else if (Properties.Settings.Default.configVideoSize == 480)
                                        {
                                            Monitor.Enter(urlList);
                                            urlList.Add("http://vt.tumblr.com/" + video.Replace("/480", "").Split('/').Last() + "_480.mp4");
                                            Monitor.Exit(urlList);
                                        }
                                    }
                                }
                            }

                        }
                        catch (Exception e)
                        {
                            // no images on the page to get
                        }

                    }
                    else
                    {
                        List<string> tags = blog.Tags.Split(',').Select(x => x.Trim()).ToList();
                        try
                        {
                            XDocument document = null;
                            document = XDocument.Load(GetApiUrl(blog.Url) + (i * 50).ToString() + "&num=50");

                            if (Properties.Settings.Default.configDownloadImage)
                            {
                                newUrls = (from n in document.Descendants("post")

                                               // Identify Posts
                                           where n.Elements("photo-url").Where(x => x.Attribute("max-width").Value == Properties.Settings.Default.configImageSize.ToString()).Any() &&
                                                   !n.Elements("photo-url").Where(x => x.Value == "www.tumblr.com").Any() &&
                                                   n.Elements("tag").Where(x => tags.Contains(x.Value)).Any() ||

                                                   // Identify Photosets
                                                   n.Elements("photoset").Where(photoset => photoset.Descendants("photo-url")
                                                   .Any(photourl => (string)photourl.Attribute("max-width").Value
                                                   == Properties.Settings.Default.configImageSize.ToString() &&
                                                   photourl.Value != "www.tumblr.com")).Any() &&
                                                   n.Elements("tag").Where(x => tags.Contains(x.Value)).Any()

                                           from m in n.Descendants("photo-url")
                                           where m.Attribute("max-width").Value == Properties.Settings.Default.configImageSize.ToString()
                                           select (string)m).ToList();
                            }
                            if (Properties.Settings.Default.configDownloadVideo)
                            {
                                foreach (var post in (from data in document.Descendants("post")
                                                      where data.Attribute("type").Value == "video" &&
                                                      data.Descendants("tag").Where(x => tags.Contains(x.Value, StringComparer.OrdinalIgnoreCase)).Any()
                                                      select data))
                                {
                                    var videoUrl = post.Descendants("video-player").Where(x => x.Value.Contains("<source src=")).Select(result =>
                                    System.Text.RegularExpressions.Regex.Match(
                                              result.Value, "<source src=\"(.*)\" type=\"video/mp4\">").Groups[1].Value).ToList();

                                    foreach (string video in videoUrl)
                                    {
                                        if (Properties.Settings.Default.configVideoSize == 1080)
                                        {
                                            Monitor.Enter(urlList);
                                            urlList.Add(video.Replace("/480", "") + ".mp4");
                                            Monitor.Exit(urlList);
                                        }
                                        else if (Properties.Settings.Default.configVideoSize == 480)
                                        {
                                            Monitor.Enter(urlList);
                                            urlList.Add("http://vt.tumblr.com/" + video.Replace("/480", "").Split('/').Last() + "_480.mp4");
                                            Monitor.Exit(urlList);
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            // no images on the page to get
                        }
                    }

                    System.Threading.Monitor.Enter(urlList);
                    urlList.AddRange(newUrls);
                    System.Threading.Monitor.Exit(urlList);
                    newUrls.Clear();
                    {
                        numberOfPostsCrawled += 50;
                        BeginInvoke((MethodInvoker)delegate
                        {
                            blog.Information = "Evaluated " + numberOfPostsCrawled + " tumblr post urls out of " + totalPosts + " total posts.";
                        });
                    }
                });

            // Generate unique url list of urls as we might have fetched duplicates at the end
            urlList = urlList.Distinct().ToList();

            totalImages = urlList.Count;
            return Tuple.Create(totalImages, urlList);
        }

        private bool CheckIfBlogIsOnline(TumblrBlog blog)
        {
            String ApiUrl = blog.Url;

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
            if (blog.Description == null)
            {
                XmlDocument tumblelog = new XmlDocument();

                try
                {
                    tumblelog.Load(ApiUrl.ToString() + "0" + "&num=50");
                    XmlNode tumblblog = tumblelog.DocumentElement.SelectSingleNode("tumblelog");
                    try
                    {
                        blog.Description = tumblblog.Attributes["title"].InnerText;
                        blog.Text = tumblblog.InnerText;
                        blog.Online = true;
                    }
                    catch (NullReferenceException)
                    {
                        // If there is no description, the blog can still be alive
                        blog.Online = true;
                    }
                }
                catch (WebException)
                {
                    // blog dead
                    blog.Online = false;
                    return false;
                }
            }
            // we already have data to compare (description, text) since the blog was
            // already once crawled
            else if (blog.Description != null)
            {
                XmlDocument tumblelog = new XmlDocument();

                try
                {
                    tumblelog.Load(ApiUrl.ToString() + "0" + "&num=50");
                    XmlNode tumblblog = tumblelog.DocumentElement.SelectSingleNode("tumblelog");
                    try
                    {
                        if (!blog.Description.Equals(tumblblog.Attributes["title"].InnerText))
                        {
                            blog.Online = false;
                            return false;
                        }
                        if (!blog.Text.Equals(tumblblog.InnerText))
                        {
                            blog.Online = false;
                            return false;
                        }
                        else
                        {
                            blog.Online = true;
                        }
                    }
                    catch (NullReferenceException)
                    {
                        blog.Online = true;
                    }
                }
                catch (WebException)
                {
                    blog.Online = false;
                    return false;
                }
            }
            return true;
        }

        private void AddBlogtoQueue(List<TumblrBlog> bin)
        {
            try
            {
                if (lvBlog.SelectedRows.Count > 0)
                {
                    foreach (DataGridViewRow item in lvBlog.SelectedRows)
                    {
                        bin.Add((TumblrBlog)item.DataBoundItem);
                        addToQueueUI((TumblrBlog)item.DataBoundItem);
                    }
                }
            }
            catch (OperationCanceledException exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        private void RemoveBlogFromQueue(List<TumblrBlog> bin)
        {
            try
            {
                if (bin.Count != 0)
                {
                    foreach (ListViewItem item in lvQueue.SelectedItems)
                    {
                        bin.RemoveAt(item.Index);
                        lvQueue.Items.Remove(item);
                    }
                }
            }
            catch (OperationCanceledException exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        void bsSmallImage_ListChanged(object sender, ListChangedEventArgs e)
        {
            bsSmallImage.MoveNext();
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
            crawlBlogsPause.PauseWithResponseAsync().Wait();

            lblProcess.Text = "PAUSE! Click on Resume Button to continue...";

            // Enable/Disable Controls
            toolPause.Enabled = false;
            toolResume.Enabled = true;
            toolStop.Enabled = true;

            if (pictureList.Count > 1)
                pictureList.Clear();
        }

        private void toolResume_Click(object sender, EventArgs e)
        {
            crawlBlogsPause.Resume();

            lblProcess.Text = "Crawling Blogs -- " + String.Join(" - ", activeCrawlList.Select(activeBlog => activeBlog.Name).ToArray());


            // Enable/Disable Controls
            toolPause.Enabled = true;
            toolResume.Enabled = false;
            toolStop.Enabled = true;
        }

        private void toolStop_Click(object sender, EventArgs e)
        {
            try
            {
                if (crawlBlogsCancellation != null)
                    crawlBlogsCancellation.Cancel();
            }
            catch (ThreadAbortException)
            {
            }

            panelInfo.Visible = true;
            lblProcess.Text = "Crawling Blogs -- " + String.Join(" - ", activeCrawlList.Select(activeBlog => activeBlog.Name).ToArray()) + " -- has stopped!";

            // Enable/Disable Controls
            toolPause.Enabled = false;
            toolResume.Enabled = false;
            toolStop.Enabled = false;
            toolCrawl.Enabled = true;
            contextBlog.Items[3].Enabled = false;

            foreach (TumblrBlog blog in blogs)
            {
                blog.Information = "";
            }

            if (activeCrawlList.Count != 0)
            {
                foreach (TumblrBlog tumblr in activeCrawlList)
                {
                    SaveBlog(tumblr);
                }
                activeCrawlList.Clear();
            }
            //if (bin.Count != 0)
            //    {
            //        bin.Clear();
            //    }
            //lvQueue.Items.Clear();
            pictureList.Clear();
        }

        private void mnuExit_Click(object sender, EventArgs e)
        {
            base.Close();
        }



        private void mnuShowFilesInExplorer_Click(object sender, EventArgs e)
        {
            if (lvBlog.SelectedRows.Count > 0)
            {
                foreach (DataGridViewRow item in lvBlog.SelectedRows)
                {
                    try
                    {
                        Process.Start("explorer.exe", Properties.Settings.Default.configDownloadLocation + ((TumblrBlog)item.DataBoundItem).Name);
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
            if (lvBlog.SelectedRows.Count > 0)
            {
                foreach (DataGridViewRow item in lvBlog.SelectedRows)
                {
                    try
                    {
                        Process.Start(((TumblrBlog)item.DataBoundItem).Url);
                    }
                    catch (Exception exception)
                    {
                        MessageBox.Show(exception.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    }
                }
            }
        }

        private void toolSettings_Click(object sender, EventArgs e)
        {
            Settings settingsWindow = new Settings(this);
            settingsWindow.Show();
        }

        private void toolAddQueue_Click(object sender, EventArgs e)
        {
            AddBlogtoQueue(bin);
        }

        private void toolRemoveQueue_Click(object sender, EventArgs e)
        {
            RemoveBlogFromQueue(bin);
        }

        private void addToQueueUI(TumblrBlog blog)
        {
            //Update UI
            ListViewItem lvQueueItem = new ListViewItem();
            lvQueueItem.Text = blog.Name;
            lvQueueItem.SubItems.Add("queued");
            lvQueue.Items.Add(lvQueueItem);
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

            BeginInvoke((MethodInvoker)delegate
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

        private void mnuRescanBlog_Click(object sender, EventArgs e)
        {
            var cancellation = new CancellationTokenSource();
            var pause = new PauseTokenSource();
            crawlBlogsCancellation = cancellation;
            crawlBlogsPause = pause;

            // Setup the UI
            lblProcess.Text = "Crawling Blogs -- " + String.Join(" - ", activeCrawlList.Select(activeBlog => activeBlog.Name).ToArray());


            // Start Crawl processes
            for (int i = 0; i < Properties.Settings.Default.configSimultaneousDownloads; i++)
                taskList[i] = Task.Run(() => RunCrawler(bin, cancellation.Token, pause.Token), cancellation.Token).ContinueWith(task =>
                {
                    if (task.Exception != null)
                    {
                        foreach (var innerEx in task.Exception.InnerExceptions)
                        {
                        }
                    }
                });

            // Enable/Disable Controls
            panelInfo.Visible = false;
            toolPause.Enabled = true;
            toolResume.Enabled = false;
            toolStop.Enabled = true;
            toolCrawl.Enabled = false;
            contextBlog.Items[3].Enabled = false;
        }

        private void BlogsListChanged(object sender, ListChangedEventArgs e)
        {
            if (e.ListChangedType == ListChangedType.ItemAdded || e.ListChangedType == ListChangedType.ItemDeleted)
            {
                if (((SortableBindingList<TumblrBlog>)sender).Count == 1)
                    FormatDataSource();
                if (((SortableBindingList<TumblrBlog>)sender).Any())
                {
                    toolShowExplorer.Enabled = true;
                    toolRemoveBlog.Enabled = true;
                    toolCrawl.Enabled = true;
                }
                else
                {
                    toolShowExplorer.Enabled = false;
                    toolRemoveBlog.Enabled = false;
                    toolCrawl.Enabled = false;
                }
            }
        }

        private void toolCheckClipboard_Click(object sender, EventArgs e)
        {
            if (toolCheckClipboard.Checked == true)
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

