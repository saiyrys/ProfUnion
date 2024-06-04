namespace Profunion.Dto.EventDto
{
    public class UpdateEventDto
    {
        public string? title { get; set; }
        public string? description { get; set; }
        public string organizer { get; set; }
        public DateTime eventDate { get; set; }
        public List<string> imagesId { get; set; }
        public string link { get; set; }
        public List<string> categoriesId { get; set; }
        public int totalTickets { get; set; }

    }
}
