namespace TumblOne
{
    using System;

    [Serializable]
    public class Post: Object
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
        public override bool Equals(Object obj)
        {
            // Check for null values and compare run-time types.
            if (obj == null || GetType() != obj.GetType())
                return false;

            Post p = (Post) obj;
            return (Url == p.Url) && (Filename == p.Filename);
        }
    }
}

