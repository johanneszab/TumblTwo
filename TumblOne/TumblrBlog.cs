namespace TumblOne
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class TumblrBlog
    {
        public string _Discription;
        public string _Name;
        public string _URL;
        public int _DownloadedImages;
        public int _TotalCount;
        public DateTime _DateAdded;
        public DateTime _LastCrawled;
        public bool _finishedCrawl;
        public List<String> _Tags;
        public List<Post> Links;

        public TumblrBlog()
        {
            this._Discription = null;
            this._Name = null;
            this._URL = null;
            this._DownloadedImages = 0;
            this._TotalCount = 0;
            this._DateAdded = System.DateTime.MinValue;
            this._LastCrawled = System.DateTime.MinValue;
            this._finishedCrawl = false;
            this._Tags = new List<String>();
            this.Links = new List<Post>();
        }

        public TumblrBlog(string blogname)
        {
            this._Name = blogname;
            this._DateAdded = DateTime.Today;
            this._Tags = new List<String>();
            this.Links = new List<Post>();
        }
    }
}

