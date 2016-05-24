using System;
using System.ComponentModel;

namespace TumblTwo
{

    [Serializable]
    public class Post : Object, INotifyPropertyChanged
    {
        private string filename;
        private string url;

        public event PropertyChangedEventHandler PropertyChanged;

        public Post(string url, string filename)
        {
            this.url = url;
            this.filename = filename;
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

        public string Filename
        {
            get
            {
                return this.filename;
            }
            set
            {
                if (this.filename != value)
                {
                    this.filename = value;
                    OnPropertyChanged("Filename");
                }
            }
        }

        public override bool Equals(Object obj)
        {
            // Check for null values and compare run-time types.
            if (obj == null || GetType() != obj.GetType())
                return false;

            Post p = (Post) obj;
            return (url == p.url) && (filename == p.filename);
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

