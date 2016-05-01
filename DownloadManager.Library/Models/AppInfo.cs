using System;

namespace DownloadManager.Library.Models
{
    public class AppInfo
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Link { get; set; }
        public bool LinkValid { get; set; }
        public DateTime? LastUpdate { get; set; }
        public string WikiLink { get; set; }
        public string WikiVersion { get; set; }
    }
}