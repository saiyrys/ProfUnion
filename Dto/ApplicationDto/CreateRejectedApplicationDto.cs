namespace Profunion.Dto.RejectedApplication
{
    public class CreateRejectedApplicationDto
    {
        public string id { get; set; }
        public string userId { get; set; }
        public string eventId { get; set; }
        public int ticketsCount { get; set; }
        public DateTime createdAt { get; set; }
    }
}
