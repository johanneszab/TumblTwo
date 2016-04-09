using System;

namespace TumblOne
{
    public class DownloadProgress
    {
        public int ProgressPercentage { get; set; }
        public Post Url { get; set; }
        public int DownloadedImages { get; set; }
        public string Information { get; set; }
    }
}