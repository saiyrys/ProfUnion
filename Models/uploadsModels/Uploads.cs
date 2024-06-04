namespace Profunion.Models.uploadsModels
{
    public class Uploads
    {
        public string id { get; set; }
        public string fileName { get; set; }
        public string filePath { get; set; }

        public virtual ICollection<EventUploads> EventUploads { get; set; }
        public virtual ICollection<NewsUploads> NewsUploads { get; set; }
    }
}
