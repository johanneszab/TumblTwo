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

        public TumblrBlog()
        {
            this._Name = null;
            this._URL = null;
            this._Discription = null;
            this.Links = new List<Post>();
        }

        public TumblrBlog(string blogname)
        {
            this._Name = blogname;
            this.Links = new List<Post>();
        }
    }
}

