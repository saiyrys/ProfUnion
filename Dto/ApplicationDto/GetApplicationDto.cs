namespace Profunion.Dto.ApplicationDto
{
    public class GetApplicationDto
    {
        public string id { get; set;  }
        public string userId { get; set; }
        public string eventId { get; set; }
        public User user { get; set; }
        public Event events { get; set; }
        public int ticketsCount { get; set; }
        public string status { get; set; }
        public DateTime createdAt { get; set; }
        public DateTime updatedAt { get; set; }

    }
}
