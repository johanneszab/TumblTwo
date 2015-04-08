namespace TumblOne
{
    using System;

    [Serializable]
    public class Post
    {
        public string Filename;
        public string Url;

        public Post()
        {
            this.Url = null;
            this.Filename = null;
        }

        public Post(string _Url, string _Filename)
        {
            this.Url = _Url;
            this.Filename = _Filename;
        }
    }
}

