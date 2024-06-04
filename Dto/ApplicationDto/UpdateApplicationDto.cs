namespace Profunion.Dto.ApplicationDto
{
    public class UpdateApplicationDto
    {
        public string id { get; set;  }
        public string eventId { get; set; }
        public string userId { get; set; }
        public int ticketsCount { get; set; }
        public string status { get; set; }
        public DateTime createdAt { get; set; }
    }
}
