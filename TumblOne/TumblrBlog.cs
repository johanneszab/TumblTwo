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
        public List<Post> Links;
        public int TOTAL_COUNT;
        public DateTime _DateAdded;
        public DateTime _LastCrawled;
        public bool _finishedCrawl;

        public TumblrBlog()
        {
            this._Name = null;
            this._URL = null;
            this._Discription = null;
            this.Links = new List<Post>();
            //this._DateAdded = null;
            //this._LastCrawled = null;
            this._finishedCrawl = false;
        }

        public TumblrBlog(string blogname)
        {
            this._Name = blogname;
            this.Links = new List<Post>();
            this._DateAdded = DateTime.Today;
        }
    }
}

