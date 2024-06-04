namespace Profunion.Dto.NewsDto
{
    public class UpdateNewsDto
    {
        public string title { get; set; }
        public string description { get; set; }
        public List<string> imagesId { get; set; }
        public string content { get; set; }
    }
}
