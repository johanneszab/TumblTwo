using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace TumblOne
{
    [Serializable]
    public class TumblrOneBlog
    {
        public string _Discription;
        public string _Name;
        public string _URL;
        public int _DownloadedImages;
        public int _TotalCount;
        public DateTime _DateAdded;
        public DateTime _LastCrawled;
        public bool _finishedCrawl;
        public List<Post> Links;

        public TumblrOneBlog()
        {
            this._Discription = null;
            this._Name = null;
            this._URL = null;
            this._DownloadedImages = 0;
            this._TotalCount = 0;
            this._DateAdded = System.DateTime.MinValue;
            this._LastCrawled = System.DateTime.MinValue;
            this._finishedCrawl = false;
            this.Links = new List<Post>();
        }

        public TumblrOneBlog(string blogname)
        {
            this._Name = blogname;
            this._DateAdded = DateTime.Today;
            this.Links = new List<Post>();
        }

        public static explicit operator TumblrBlog(TumblrOneBlog obj)
        {
            TumblrBlog output = new TumblrBlog() {
                Description = obj._Discription,
                Name = obj._Name,
                Text = null,
                Url = obj._URL,
                DownloadedImages = obj._DownloadedImages,
                TotalCount = obj._TotalCount,
                Progress = 0,
                DateAdded = obj._DateAdded,
                LastCrawled = obj._LastCrawled,
                FinishedCrawl = obj._finishedCrawl,
                Online = false,
                Information = null,
                Tags = null,
                Urls = new BindingList<string>(),
                Links = new BindingList<Post>(obj.Links)
                };
            return output;
        }
    }
}