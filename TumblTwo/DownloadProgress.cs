using System;

namespace TumblTwo
{
    public class DownloadProgress
    {
        public int ProgressPercentage { get; set; }
        public Post Url { get; set; }
        public int DownloadedImages { get; set; }
        public string Information { get; set; }
    }
}