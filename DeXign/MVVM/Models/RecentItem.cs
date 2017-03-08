using System;

namespace DeXign.Models
{
    public class RecentItem
    {
        public DateTime LastedTime { get; set; }

        public int Id { get; set; }

        public string FileName { get; set; }

        public RecentItem()
        {
        }

        public RecentItem(string fileName)
        {
            this.FileName = fileName;
            this.LastedTime = DateTime.Now;
        }

        public override string ToString()
        {
            return FileName;
        }
    }
}
