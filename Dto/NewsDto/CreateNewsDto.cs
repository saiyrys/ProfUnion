namespace Profunion.Dto.NewsDto
{
    public class CreateNewsDto
    {
        public string newsId { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string content { get; set; }
        public List<string> imagesId { get; set; }

    }
}
