namespace Profunion.Models.NewsModel
{
    public class News
    {
        public News()
        {
            createdAt = DateTime.Now;
            updatedAt = DateTime.Now;
        }
        public string newsId { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string content { get; set; }
        public DateTime createdAt { get; set; }
        public DateTime updatedAt { get; set; }
        public int views { get; set; }

        public virtual ICollection<Uploads> Uploads { get; set; }
        public virtual ICollection<NewsUploads> NewsUploads { get; set; }

    }
}
