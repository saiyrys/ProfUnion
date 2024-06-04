namespace Profunion.Models.Application
{
    public class Application
    {
        public Application()
        {
            Id = Guid.NewGuid().ToString();
            createdAt = DateTime.Now;
            updatedAt = DateTime.Now;
        }

        public string Id { get; }
        public string EventId { get; set; }
        public string UserId { get; set; }
        public int ticketsCount { get; set; }
        public string status { get; set; }
        public DateTime createdAt { get; set; }
        public DateTime updatedAt { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        [ForeignKey("EventId")]
        public virtual Event Event { get; set; }
    }
    
}
