namespace Profunion.Dto.NewsDto
{
    public class GetNewsDto
    {
        public string newsId { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string content { get; set; }
        public List<GetUploadsDto> images { get; set; }
        public string createdAt { get; set; }
        public string updatedAt { get; set; }
        public int views { get; set; }
    }
}
