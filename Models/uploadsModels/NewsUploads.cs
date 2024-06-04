namespace Profunion.Models.uploadsModels
{
    public class NewsUploads
    {
        public string newsId { get; set; }
        public News News { get; set; }

        public string fileId { get; set; }
        public Uploads Uploads { get; set; }
    }
}
