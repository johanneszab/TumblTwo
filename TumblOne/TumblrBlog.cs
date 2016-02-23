namespace TumblOne
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;

    [Serializable]
    public class TumblrBlog : INotifyPropertyChanged
    {
        private string description;
        private string name;
        private string text;
        private string url;
        private int downloadedImages;
        private int totalCount;
        private double progress;
        private DateTime dateAdded;
        private DateTime lastCrawled;
        private bool finishedCrawl;
        private bool online;
        private BindingList<string> urls;
        private BindingList<Post> links;

        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        public TumblrBlog()
        {
            this.description = null;
            this.name = null;
            this.text = null;
            this.url = null;
            this.downloadedImages = 0;
            this.totalCount = 0;
            this.progress = 0.0;
            this.dateAdded = System.DateTime.MinValue;
            this.lastCrawled = System.DateTime.MinValue;
            this.finishedCrawl = false;
            this.online = false;
            this.urls = new BindingList<string>();
            this.links = new BindingList<Post>();
        }

        public TumblrBlog(string blogname)
        {
            this.name = blogname;
            this.dateAdded = DateTime.Today;
            this.links = new BindingList<Post>();
        }

        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                if (this.name != value)
                {
                    this.name = value;
                    OnPropertyChanged("Name");
                }
            }
        }

        public string Description
        {
            get
            {
                return this.description;
            }
            set
            {
                if (this.description != value)
                {
                    this.description = value;
                    OnPropertyChanged("Description");
                }
            }
        }

        public string Text
        {
            get
            {
                return this.text;
            }
            set
            {
                if (this.text != value)
                {
                    this.text = value;
                    OnPropertyChanged("Text");
                }
            }
        }

        public string Url
        {
            get
            {
                return this.url;
            }
            set
            {
                if (this.url != value)
                {
                    this.url = value;
                    OnPropertyChanged("Url");
                }
            }
        }

        public int DownloadedImages
        {
            get
            {
                return this.downloadedImages;
            }
            set
            {
                if (this.downloadedImages != value)
                {
                    this.downloadedImages = value;
                    OnPropertyChanged("DownloadedImages");
                }
            }
        }

        public double Progress
        {
            get
            {
                return this.progress;
            }
            set
            {
                if (this.progress != value)
                {
                    this.progress = value;
                    OnPropertyChanged("Progress");
                }
            }
        }

        public int TotalCount
        {
            get
            {
                return this.totalCount;
            }
            set
            {
                if (this.totalCount != value)
                {
                    this.totalCount = value;
                    OnPropertyChanged("TotalCount");
                }
            }
        }

        public DateTime DateAdded
        {
            get
            {
                return this.dateAdded;
            }
            set
            {
                if (this.dateAdded != value)
                {
                    this.dateAdded = value;
                    OnPropertyChanged("DateAdded");
                }
            }
        }

        public DateTime LastCrawled
        {
            get
            {
                return this.lastCrawled;
            }
            set
            {
                if (this.lastCrawled != value)
                {
                    this.lastCrawled = value;
                    OnPropertyChanged("LastCrawled");
                }
            }
        }

        public bool FinishedCrawl
        {
            get
            {
                return this.finishedCrawl;
            }
            set
            {
                if (this.finishedCrawl != value)
                {
                    this.finishedCrawl = value;
                    OnPropertyChanged("FinishedCrawl");
                }
            }
        }

        public bool Online
        {
            get
            {
                return this.online;
            }
            set
            {
                if (this.online != value)
                {
                    this.online = value;
                    OnPropertyChanged("Online");
                }
            }
        }

        public BindingList<Post> Links
        {
            get { return links; }
            set
            {
                if (links != value)
                {
                    links = value;
                    OnPropertyChanged("Links");
                }
            }
        }

        public BindingList<string> Urls
        {
            get { return urls; }
            set
            {
                if (urls != value)
                {
                    urls = value;
                    OnPropertyChanged("Urls");
                }
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}

